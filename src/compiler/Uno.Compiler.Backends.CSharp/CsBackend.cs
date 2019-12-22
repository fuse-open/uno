using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Uno.Compiler.API.Backends;
using Uno.Compiler.API.Domain.Extensions;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Compiler.Backends.OpenGL;

namespace Uno.Compiler.Backends.CSharp
{
    public partial class CsBackend : SourceBackend
    {
        public override string Name => "CSharp";

        static readonly Dictionary<string, string> Keywords = new Dictionary<string, string>()
        {
            { "System.Boolean", "bool" },
            { "System.Byte", "byte" },
            { "System.SByte", "sbyte" },
            { "System.Int16", "short" },
            { "System.Int32", "int" },
            { "System.Int64", "long" },
            { "System.UInt16", "ushort" },
            { "System.UInt32", "uint" },
            { "System.UInt64", "ulong" },
            { "System.Single", "float" },
            { "System.Double", "double" },
            { "System.String", "string" },
            { "System.Object", "object" },
            { "System.Void", "void" }
        };

        readonly List<string> SourceFiles = new List<string>();

        public CsBackend()
            : base(new GLBackend())
        {
            FunctionOptions |= FunctionOptions.DecodeSwizzles;
        }

        public override bool CanLink(Function f)
        {
            FunctionExtension ext;
            return !(Environment.TryGetExtension(f, out ext) && ext.HasImplementation) && f.IsExtern && !f.HasBody;
        }

        public override BackendResult Build()
        {
            SourceFiles.AddRange(Environment.GetSet("SourceFile"));
            ExportNamespace(Data.IL);
            GenerateProject();
            return null;
        }

        public override bool IgnoreType(DataType dt)
        {
            if (dt.HasAttribute(Essentials.TargetSpecificTypeAttribute))
                return true;

            var dotNetName = dt.TryGetAttributeString(Essentials.DotNetTypeAttribute) ?? (
                    dt.HasAttribute(Essentials.DotNetTypeAttribute)
                        ? dt.MasterDefinition.FullName
                        : null
                );

            if (dotNetName == null)
                return true;

            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                var t = a.GetType(dotNetName, false);
                if (t != null)
                    return true;
            }

            return false;
        }

        protected override void ExportType(DataType dt)
        {
            var filename = GetExportName(dt, Path.DirectorySeparatorChar) + ".cs";
            SourceFiles.Add(filename);

            using (var w = new CsWriter(this, Environment.Combine(filename)))
            {
                w.WriteOrigin(dt.Source);

                if (!dt.Parent.IsRoot)
                {
                    w.WriteLine("namespace " + string.Join(".", GetNamespaceNames(dt)));
                    w.BeginScope();
                }

                w.WriteType(dt);

                if (!dt.Parent.IsRoot)
                    w.EndScope();
            }
        }

        internal string GetDotNetTypeName(Type netType)
        {
            string keyword;
            if (netType.FullName != null && Keywords.TryGetValue(netType.FullName, out keyword))
                return keyword;

            if (netType.FullName == null ||
                netType.FullName.IndexOf('`') != -1)
            {
                var str = netType.ToString();
                if (netType.IsGenericParameter ||
                    netType.IsArray && netType.GetElementType().IsGenericParameter ||
                    str.IndexOf('`') < 0)
                    // Generic parameter, or an array of those, usually
                    return str;

                var name = netType.Namespace + "." + netType.Name.Substring(0, netType.Name.IndexOf('`')) + "<";
                var args = netType.GetGenericArguments();

                for (int i = 0; i < args.Length; i++)
                {
                    if (i > 0)
                        name += ", ";

                    name += GetDotNetTypeName(args[i]);
                }

                return "global::" + name + ">";
            }

            return "global::" + netType.FullName;
        }

        internal string GetStaticTypeName(Namescope ns, Namescope parent)
        {
            var dt = ns as DataType;

            if (ns is Namespace && ns.IsRoot)
                return "";

            if (ns is VoidType)
                return "void";

            if (ns is ArrayType)
                return GetStaticTypeName((ns as ArrayType).ElementType, parent) + "[]";

            if (ns is GenericParameterType)
                return (ns as GenericParameterType).MasterDefinition.Name;

            var result = new StringBuilder();
            var dotNetName = dt?.TryGetAttributeString(Essentials.DotNetTypeAttribute);

            if (dotNetName != null)
            {
                string keyword;
                if (Keywords.TryGetValue(dotNetName, out keyword))
                    return keyword;

                // TODO: Simple removal of generic name mangling (assumes to never encounter mangled inner types)
                int i = dotNetName.IndexOf('`');
                result.Append("global::" + (i != -1 ? dotNetName.Substring(0, i) : dotNetName));
            }
            else
            {
                if (ns.Parent != null &&
                    ns.Parent != parent &&
                    ns.Parent != parent.Parent &&
                    !ns.Parent.IsRoot)
                    result.Append(GetStaticTypeName(ns.Parent, parent) + ".");
                else if (ns.Parent == null ||
                    ns.Parent.IsRoot)
                    result.Append("global::");

                result.Append(ns.Name);
            }

            if (dt != null)
            {
                if (dt.IsGenericDefinition)
                {
                    result.Append("<");

                    for (int i = 0; i < dt.GenericParameters.Length; i++)
                    {
                        result.CommaWhen(i > 0);
                        result.Append(dt.GenericParameters[i].Name);
                    }

                    result.Append(">");
                }
                else if (dt.IsGenericParameterization)
                {
                    result.Append("<");

                    for (int i = 0; i < dt.GenericArguments.Length; i++)
                    {
                        var t = GetStaticTypeName(dt.GenericArguments[i], parent);
                        result.CommaWhen(i > 0);
                        result.Append(t);
                    }

                    result.Append(">");
                }
            }

            return result.ToString();
        }
    }
}
