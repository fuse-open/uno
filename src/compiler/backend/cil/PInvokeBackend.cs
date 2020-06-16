using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IKVM.Reflection;
using IKVM.Reflection.Emit;
using System.Runtime.InteropServices;
using Uno.Compiler.API;
using Uno.Compiler.API.Backends;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;
using Type = IKVM.Reflection.Type;

namespace Uno.Compiler.Backends.CIL
{
    public class PInvokeBackend : SourceBackend
    {
        public override string Name => "PInvoke";
        public string SourceDirectory;
        public string HeaderDirectory;

        Dictionary<DataType, List<Function>> _pinvokeTypes = new Dictionary<DataType, List<Function>>();
        public readonly Dictionary<DataType, string> Types = new Dictionary<DataType, string>();

        public PInvokeBackend(ShaderBackend shaderBackend)
            : base(shaderBackend)
        {
            FunctionOptions =
                FunctionOptions.MakeNativeCode;
            TypeOptions =
                TypeOptions.FlattenConstructors |
                TypeOptions.FlattenOperators |
                TypeOptions.FlattenCasts |
                TypeOptions.IgnoreAttributes |
                TypeOptions.IgnoreProtection |
                TypeOptions.MakeUniqueNames;
        }

        public override void Begin(ICompiler compiler)
        {
            base.Begin(compiler);
            Decompiler = new PInvokeDecompiler(this);
        }

        public override void Configure()
        {
            SourceDirectory = Environment.GetOutputPath("SourceDirectory");
            HeaderDirectory = Environment.GetOutputPath("HeaderDirectory");
            Types.Add(Essentials.Bool, "bool");
            Types.Add(Essentials.Byte, "uint8_t");
            Types.Add(Essentials.Char, "char16_t");
            Types.Add(Essentials.Double, "double");
            Types.Add(Essentials.Float, "float");
            Types.Add(Essentials.Int, "int");
            Types.Add(Essentials.Long, "int64_t");
            Types.Add(Essentials.SByte, "int8_t");
            Types.Add(Essentials.Short, "int16_t");
            Types.Add(Essentials.UInt, "unsigned int");
            Types.Add(Essentials.ULong, "uint64_t");
            Types.Add(Essentials.UShort, "uint16_t");
            Types.Add(ILFactory.GetType("global::Uno.IntPtr"), "void*");
        }

        public override BackendResult Build()
        {
            ExportNamespace(Data.IL);
            var sourceFiles = new List<string>();
            var headerFiles = new List<string>();
            foreach (var kv in _pinvokeTypes)
            {
                sourceFiles.Add(GenerateFile(kv.Key, kv.Value));
            }

            AddExternalSourceFiles(sourceFiles, headerFiles);

            foreach (var sourceFile in sourceFiles)
            {
                Environment.Require("SourceFile", sourceFile);
            }
            foreach (var headerFile in headerFiles)
            {
                Environment.Require("HeaderFile", headerFile);
            }

            return null;
        }

        protected override void ExportType(DataType dt)
        {
            foreach (var f in dt.Methods)
            {
                if (f.IsPInvokable(Essentials, Log))
                {
                    List<Function> l;
                    if (_pinvokeTypes.TryGetValue(dt, out l))
                    {
                        l.Add(f);
                    }
                    else
                    {
                        l = new List<Function> { f };
                        _pinvokeTypes[dt] = l;
                    }
                }
            }
            foreach (var nt in dt.NestedTypes)
            {
                ExportType(nt);
            }
        }

        string GenerateFile(DataType dt, List<Function> functions)
        {
            var outputFile = CName(dt) + ".cpp";

            using (var w = new PInvokeWriter(this, Path.Combine(SourceDirectory, outputFile)))
            {
                w.WriteLine("#include <stdint.h>");

                var declarations = new List<string>();

                foreach (var e in Environment.GetSet(dt, "Source.Declaration"))
                    declarations.Add(e.Trim());

                foreach (var e in Environment.GetSet(dt, "Source.Include"))
                    declarations.Add("#include <" + e.Trim() + ">");

                declarations.Sort();

                foreach (var d in declarations)
                    w.WriteLine(d);

                w.WriteLine("#ifdef _WIN32");
                w.WriteLine("#  define UNO_DLLEXPORT __declspec(dllexport)");
                w.WriteLine("#  define UNO_CDECL __cdecl");
                w.WriteLine("#  define UNO_STDCALL __stdcall");
                w.WriteLine("#else");
                w.WriteLine("#  define UNO_DLLEXPORT");
                w.WriteLine("#  define UNO_CDECL");
                w.WriteLine("#  define UNO_STDCALL");
                w.WriteLine("#endif");
                w.WriteLine("template <class ReturnType, class... Params>");
                w.WriteLine("using StdCallFun = ReturnType (UNO_STDCALL *) (Params...);");
                w.WriteLine("static_assert(sizeof(bool) == 1, \"The PInvoke backend requires 1-byte bools\");");
                w.Skip();
                foreach (var f in functions)
                {
                    w.BeginLine();
                    w.Write("extern \"C\" UNO_DLLEXPORT ");
                    w.WriteReturnType(f.Source, f.ReturnType);
                    w.Write(" UNO_CDECL " + CName(f));
                    w.WriteParameters(f.Source, f.Parameters, true);
                    w.EndLine();
                    w.WriteFunctionBody(f);
                    w.Skip();
                }
            }

            return outputFile;
        }

        void AddExternalSourceFiles(List<string> sourceFiles, List<string> headerFiles)
        {
            foreach (var package in Data.IL.Packages)
            {
                foreach (var f in package.ForeignSourceFiles)
                {
                    if (Environment.Test(Source.Unknown, f.Condition))
                    {
                        var path = f.UnixPath.UnixToNative();
                        switch (f.SourceKind)
                        {
                            case ForeignItem.Kind.CSource:
                            {
                                var sourcePath = Path.Combine(SourceDirectory, path);
                                Disk.CopyFile(Path.Combine(package.SourceDirectory, path), sourcePath);
                                sourceFiles.Add(f.UnixPath);
                                break;
                            }
                            case ForeignItem.Kind.CHeader:
                            {
                                var sourcePath = Path.Combine(HeaderDirectory, path);
                                Disk.CopyFile(Path.Combine(package.SourceDirectory, path), sourcePath);
                                headerFiles.Add(f.UnixPath);
                                break;
                            }
                            default:
                                break;
                        }
                    }
                }
            }
        }

        static string CName(Namescope s)
        {
            var parts = new List<string>();
            while (s != null && !s.IsRoot)
            {
                parts.Add(s.Name);
                s = s.Parent;
            }
            parts.Reverse();
            return string.Join("_", parts);
        }

        public static string CName(Function f) => CName(f.DeclaringType) + "__" + f.Name;

        public override bool CanLink(Function f) => Environment.IsGeneratingCode && !f.IsPInvokable(Essentials, Log);
        public override bool CanLink(DataType dt) => false;

        public string GetForeignParamType(Source src, Parameter p)
        {
            if (p.IsReference)
                return GetForeignType(src, p.Type) + "*";
            return GetForeignParamType(src, p.Type);
        }

        public string GetForeignParamType(Source src, DataType dt)
        {
            if (dt.IsArray)
                return GetForeignType(src, dt.ElementType) + "*";

            if (dt == Essentials.String)
                return "const char16_t*";

            return GetForeignType(src, dt);
        }

        public string GetForeignType(Source src, DataType dt)
        {
            if (dt.IsVoid)
                return "void";

            if (dt.IsEnum)
                return GetForeignType(src, dt.Base);

            if (dt.IsDelegate)
                return GetDelegateType(src, (DelegateType)dt);

            string result;
            if (Types.TryGetValue(dt, out result))
                return result;

            Log.Error(src, ErrorCode.E0000, "Foreign CPlusPlus: No corresponding type for " + dt);
            return null;
        }

        public string GetDelegateType(Source src, DelegateType dt)
        {
            return "StdCallFun<" + GetForeignType(src, dt.ReturnType) +
                   string.Concat(dt.Parameters.Select(p => ", " + GetForeignParamType(src, p))) + ">";
        }

        // -------------------------------------------------------------------------------------------------------------
        // CIL implementation
        public static MethodBuilder CreateCilPInvokeMethod(
            Universe universe,
            IEssentials essentials,
            TypeBuilder builder,
            Method m,
            MethodAttributes methodAttributes,
            Type resolvedReturnType,
            Type[] resolvedParameterTypes)
        {

            var mb = builder.DefinePInvokeMethod(
                m.Name,
                m.Source.Package.Name + "-PInvoke.dll",
                PInvokeBackend.CName(m),
                methodAttributes | MethodAttributes.PinvokeImpl,
                CallingConventions.Standard,
                resolvedReturnType,
                null,
                null,
                resolvedParameterTypes,
                GetRequiredModifiers(universe, m.Parameters),
                null,
                CallingConvention.Cdecl,
                CharSet.Unicode);

            mb.SetImplementationFlags(MethodImplAttributes.PreserveSig);

            var rb = mb.DefineParameter(0, ParameterAttributes.None, null);
            var customReturnAttribute = GetCustomAttribute(universe, essentials, m.ReturnType);
            if (customReturnAttribute != null)
                rb.SetCustomAttribute(customReturnAttribute);

            int i = 1;
            foreach (var param in m.Parameters)
            {

                var pb = mb.DefineParameter(i, GetParameterTypeAttributes(param.Type), param.Name);
                var customAttribute = GetCustomAttribute(universe, essentials, param.Type);
                if (customAttribute != null)
                    pb.SetCustomAttribute(customAttribute);
                ++i;
            }

            return mb;
        }

        static Type[][] GetRequiredModifiers(Universe universe, Parameter[] parameters)
        {
            return parameters.Select(p => GetRequiredModifiers(universe, p.Type)).ToArray();
        }

        static Type[] GetRequiredModifiers(Universe universe, DataType dt)
        {
            if (dt.IsArray)
            {
                return new[]
                {
                    universe.Import(typeof(InAttribute)),
                    universe.Import(typeof(OutAttribute))
                };
            }
            return null;
        }

        public static ParameterAttributes GetParameterTypeAttributes(DataType dt)
        {
            if (dt.IsArray)
            {
                return ParameterAttributes.In | ParameterAttributes.Out;
            }
            return ParameterAttributes.None;
        }

        static CustomAttributeBuilder GetBoolCustomAttribute(Universe universe)
        {
            var argTypes = new[] { universe.Import(typeof(UnmanagedType)) };
            var ci = universe.Import(typeof(MarshalAsAttribute)).GetConstructor(argTypes);
            // Portability note: This assumes that the C++ compiler has sizeof(bool) = 1,
            // which is implementation-defined, but true for our current PInvoke-enabled targets.
            // The generated code includes a static_assert that tests this.
            object[] args = { UnmanagedType.I1 };

            return new CustomAttributeBuilder(ci, args);
        }

        static CustomAttributeBuilder GetCustomAttribute(Universe universe, IEssentials essentials, DataType dt)
        {
            if (dt == essentials.Bool)
                return GetBoolCustomAttribute(universe);
            if (dt.IsDelegate)
            {
                var argTypes = new[] { universe.Import(typeof(UnmanagedType)) };
                var ci = universe.Import(typeof(MarshalAsAttribute)).GetConstructor(argTypes);
                object[] args = { UnmanagedType.FunctionPtr };

                return new CustomAttributeBuilder(ci, args);
            }
            return null;
        }

        public static ParameterBuilder DefineCilDelegateParameter(
            Universe universe,
            IEssentials essentials,
            MethodBuilder mb,
            Parameter p,
            int paramIndex)
        {
            var pb = mb.DefineParameter(paramIndex + 1, GetParameterTypeAttributes(p.Type), p.Name);
            var cab = GetDelegateParameterCustomAttribute(universe, essentials, p, paramIndex);
            if (cab != null)
                pb.SetCustomAttribute(cab);
            return pb;
        }

        static CustomAttributeBuilder GetDelegateParameterCustomAttribute(
            Universe universe,
            IEssentials essentials,
            Parameter p,
            int paramIndex)
        {
            var dt = p.Type;
            if (dt == essentials.Bool)
                return GetBoolCustomAttribute(universe);
            if (dt.IsArray)
            {
                var argTypes = new[] { universe.Import(typeof(UnmanagedType)) };
                var attrType = universe.Import(typeof(MarshalAsAttribute));
                var ci = attrType.GetConstructor(argTypes);
                var args = new object[] { UnmanagedType.LPArray };
                var fields = new[] { attrType.GetField("SizeParamIndex") };
                var fieldValues = new object[] { (short)(paramIndex + 1) };

                return new CustomAttributeBuilder(ci, args, fields, fieldValues);
            }
            if (dt == essentials.String)
            {
                var argTypes = new[] { universe.Import(typeof(UnmanagedType)) };
                var attrType = universe.Import(typeof(MarshalAsAttribute));
                var ci = attrType.GetConstructor(argTypes);
                var args = new object[] { UnmanagedType.LPWStr };

                return new CustomAttributeBuilder(ci, args);
            }
            return null;
        }
    }
}
