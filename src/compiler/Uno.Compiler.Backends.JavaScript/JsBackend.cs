using System.Collections.Generic;
using System.IO;
using System.Text;
using Uno.Compiler.API;
using Uno.Compiler.API.Backends;
using Uno.Compiler.API.Domain.Extensions;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;
using System.Linq;

namespace Uno.Compiler.Backends.JavaScript
{
    public class JsBackend : SourceBackend, IJsBackend
    {
        public override string Name => "JavaScript";

        public bool Minify => !Environment.Debug;
        public bool Obfuscate => !Environment.Debug;
        internal string SourceDirectory { get; private set; }
        public List<string> SourceFiles { get; } = new List<string>();

        internal readonly Dictionary<IEntity, string> Globals = new Dictionary<IEntity, string>();
        internal readonly Dictionary<IEntity, string> Members = new Dictionary<IEntity, string>();
        internal readonly List<Namespace> Namespaces = new List<Namespace>();
        internal readonly List<int> BaseTypeIds = new List<int>();
        internal readonly Dictionary<DataType, int> TypeIds = new Dictionary<DataType, int>();
        readonly HashSet<DataType> ExportedTypes = new HashSet<DataType>();
        HashSet<string> _keywords;

        public JsBackend(ShaderBackend shaderBackend)
            : base(shaderBackend)
        {
            Options =
                BackendOptions.ExportFiles |
                BackendOptions.ExportMergedBlob;
            TypeOptions =
                TypeOptions.MakeUniqueNames |
                TypeOptions.IgnoreAttributes |
                TypeOptions.IgnoreProtection |
                TypeOptions.FlattenConstructors |
                TypeOptions.FlattenEvents |
                TypeOptions.FlattenOperators |
                TypeOptions.FlattenCasts |
                TypeOptions.CopyStructs;
            FunctionOptions =
                FunctionOptions.DecodeEnumOps |
                FunctionOptions.DecodeNullOps |
                FunctionOptions.DecodeDelegateOps |
                FunctionOptions.DecodeSwizzles |
                FunctionOptions.DecodeSetChains;
        }

        public override void Begin(ICompiler compiler)
        {
            base.Begin(compiler);
            Scheduler.AddTransform(new JsObfuscator(this));
            Decompiler = new JsDecompiler(this);
        }

        public override void Configure()
        {
            _keywords = Environment.GetWords("Keywords");
        }

        bool AllTypesEquals(DataType[] a, DataType[] b)
        {
            if (a.Length != b.Length)
                return false;

            for (int i = 0; i < a.Length; i++)
                if (!a[i].Equals(b[i]))
                    return false;

            return true;
        }

        internal string GetGlobal(IEntity entity)
        {
            string result;
            return Globals.TryGetValue(entity.MasterDefinition, out result)
                 ? result
                 : "$";
        }

        internal string GetMember(IEntity entity)
        {
            string result;
            return Members.TryGetValue(entity.MasterDefinition, out result) ? result
                 : Globals.TryGetValue(entity.MasterDefinition, out result) ? result
                 : "$";
        }

        internal int CountBaseClasses(DataType dt, int i = 0)
        {
            if (dt.Base != null)
                return CountBaseClasses(dt.Base, i + 1);

            return i;
        }

        public int GetTypeId(Source src, DataType dt)
        {
            int result;
            if (TypeIds.TryGetValue(dt, out result))
                return dt is InterfaceType ? result | (1 << 15) : result;

            if (!dt.IsMasterDefinition)
                return GetTypeId(src, dt.MasterDefinition);

            if (dt.IsArray)
            {
                var i = GetTypeId(src, dt.ElementType);
                return i + (1 << 16);
            }

            Log.Warning("Unable to get type id for " + dt.Quote());
            return 0;
        }

        public string GetTypeId(Source src, Function context, DataType dt)
        {
            if (dt.IsArray)
            {
                var id = GetTypeId(src, context, dt.ElementType);

                int i;
                if (int.TryParse(id, out i))
                    return (i + (1 << 16)).ToString();

                return "Array.CreateId(" + id + ")";
            }

            if (dt.IsGenericParameter)
            {
                if (context != null)
                {
                    if (context.DeclaringType.IsFlattenedDefinition)
                    {
                        for (int i = 0; i < context.DeclaringType.FlattenedParameters.Length; i++)
                        {
                            if (dt.Equals(context.DeclaringType.FlattenedParameters[i]))
                            {
                                if (context.IsStatic)
                                    return context.DeclaringType.FlattenedParameters.Length == 1 ? "$staticArgId" : ("$staticArgIds[" + i + "]");

                                int c = CountBaseClasses(context.DeclaringType);
                                return (context.IsConstructor ? "$this." : "this.") +
                                    (context.DeclaringType.FlattenedParameters.Length == 1 ? ("$typeArgId" + c) : ("$typeArgIds" + c + "[" + i + "]"));
                            }
                        }
                    }

                    if (context is Method && (context as Method).IsGenericDefinition)
                        for (int i = 0; i < (context as Method).GenericParameters.Length; i++)
                            if (dt.Equals((context as Method).GenericParameters[i]))
                                return (context as Method).GenericParameters.Length == 1 ? "$methodArgId" : ("$methodArgIds[" + i + "]");
                }
            }

            return GetTypeId(src, dt).ToString();
        }

        public string GetTypeIds(Source src, Function context, DataType[] types)
        {
            if (types.Length == 1)
                return GetTypeId(src, context, types[0]);

            if (context != null)
            {
                if (context.DeclaringType.IsFlattenedDefinition && AllTypesEquals(context.DeclaringType.FlattenedParameters, types))
                    return context.IsStatic ? "$staticArgIds" :
                        (context is Constructor ? "$this." : "this.") + "$typeArgIds" + CountBaseClasses(context.DeclaringType);

                if (context is Method && (context as Method).IsGenericDefinition && AllTypesEquals((context as Method).GenericParameters, types))
                    return "$methodArgIds";
            }

            var str = "[";

            for (int i = 0; i < types.Length; i++)
            {
                if (i > 0) str += ", ";
                str += GetTypeId(src, context, types[i]);
            }

            return str + "]";
        }

        void FindTypes(Namespace root, List<DataType> result)
        {
            foreach (var ns in root.Namespaces)
                FindTypes(ns, result);

            foreach (var dt in root.Types)
                if (dt.Stats.HasFlag(EntityStats.RefCount))
                    result.Add(dt);
        }

        void GenerateTypeIds()
        {
            var types = new List<DataType>();
            FindTypes(Data.IL, types);

            //types.Sort((a, b) => b.RefCount - a.RefCount);

            for (int i = 0; i < types.Count; i++)
                TypeIds[types[i]] = i + 1;

            BaseTypeIds.Add(0);

            for (int i = 0; i < types.Count; i++)
            {
                var bt = types[i].Base;
                BaseTypeIds.Add(bt != null ? GetTypeId(Source.Unknown, bt) : 0);
            }
        }

        public override bool IgnoreType(DataType dt)
        {
            switch (dt.TypeType)
            {
                case TypeType.Class:
                case TypeType.Struct:
                    return false;
                default:
                    return true;
            }
        }

        protected override void ExportType(DataType dt)
        {
            if (ExportedTypes.Contains(dt))
                return;

            ExportedTypes.Add(dt);

            if (dt.Base != null && !IgnoreType(dt.Base.MasterDefinition))
                ExportType(dt.Base.MasterDefinition);

            JsWriter.ExportClass(this, dt);
        }

        public override bool CanLink(Function f)
        {
            return Environment.GetBool(f, "IsIntrinsic");
        }

        public override BackendResult Build()
        {
            SourceDirectory = Minify
                ? Path.Combine(Environment.CacheDirectory, "src")
                : Environment.Combine("src");

            GenerateTypeIds();
            Environment.Set("BaseTypeIds", string.Join(", ", BaseTypeIds));
            Environment.Set("SourceDirectory", SourceDirectory.NativeToUnix());
            Environment.Set("Main.Namespaces", BuildNamespaceString());

            SourceFiles.AddRange(Environment.GetSet("PreSourceFile", true));

            ExportBundle("data");
            ExportNamespace(Data.IL);

            SourceFiles.AddRange(Environment.GetSet("PostSourceFile", true));

            Environment.Set("Main.StartupCode", Decompiler.GetScope(Data.StartupCode, Data.Entrypoint, 3).Trim());
            Environment.Set("Main.BundleImages", BuildBundleString(ContentType.Texture2D, ContentType.TextureCube));
            Environment.Set("Main.BundleSounds", BuildBundleString(ContentType.Sound));
            Environment.Set("Main.BundleFonts", BuildBundleString(ContentType.FontFace));
            Environment.Set("Main.BundleBuffers", BuildBundleString(ContentType.Blob, ContentType.Buffer, ContentType.BundleFile));
            return null;
        }

        string BuildNamespaceString()
        {
            if (Obfuscate)
                return "";

            var sb = new StringBuilder();

            foreach (var ns in Namespaces)
                sb.AppendLine(ns + " = {};");

            return sb.ToString();
        }

        string BuildBundleString(params ContentType[] types)
        {
            var sb = new StringBuilder();

            foreach (var f in Data.Extensions.BundleFiles)
                if (types.Contains(f.ContentType))
                    sb.AppendLine("            " + f.TargetName.ToLiteral() + ",");

            return sb.ToString().Trim();
        }

        public override void End()
        {
            if (!Minify)
                return;

            var sb = new StringBuilder();

            foreach (var script in SourceFiles)
            {
                sb.AppendWhen(sb.Length > 0 && sb[sb.Length - 1] != ';', ';');
                sb.Append(File.ReadAllText(Path.Combine(SourceDirectory, script)));
            }

            using (var f = Disk.CreateText(Environment.Combine("app.js")))
                f.Write(sb.ToString());
        }

        public override bool IsReserved(string id)
        {
            return _keywords.Contains(id);
        }
    }
}
