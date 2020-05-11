using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using Uno.Compiler.API;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Compiler.Backends.CPlusPlus;
using Uno.Compiler.Foreign.CPlusPlus;
using Uno.IO;

namespace Uno.Compiler.Foreign
{
    enum ErrorCode
    {
        E0000,
    }

    namespace CPlusPlus
    {
        struct IdentityConversion
        {
            public readonly DataType UnoType;
            public readonly string ForeignType;

            public IdentityConversion(DataType unoType, string foreignType)
            {
                UnoType = unoType;
                ForeignType = foreignType;
            }
        }

        struct TypeConversion
        {
            public readonly DataType UnoType;
            public readonly string ForeignType;
            public readonly Func<string, string> FromUno;
            public readonly bool IsIdentity;

            public TypeConversion(DataType unoType, string foreignType, Func<string, string> fromUno, bool isIdentity = false)
            {
                UnoType = unoType;
                ForeignType = foreignType;
                FromUno = fromUno;
                IsIdentity = isIdentity;
            }

            public static TypeConversion Identity(IdentityConversion conversion)
            {
                return new TypeConversion(conversion.UnoType, conversion.ForeignType, x => x, true);
            }
        }
    }

    class ForeignCPlusPlusPass : ForeignPass
    {
        readonly Dictionary<DataType, string> IdentityTypes = new Dictionary<DataType, string>();

        public ForeignCPlusPlusPass(CppBackend backend)
            : base(backend)
        {
        }

        protected override string Language => "CPlusPlus";
        bool _initialized;

        void EnsureInitialized()
        {
            if (!_initialized)
            {
                _initialized = true;
                IdentityTypes.Add(Essentials.Bool, "bool");
                IdentityTypes.Add(Essentials.Byte, "uint8_t");
                IdentityTypes.Add(Essentials.Char, "char16_t");
                IdentityTypes.Add(Essentials.Double, "double");
                IdentityTypes.Add(Essentials.Float, "float");
                IdentityTypes.Add(Essentials.Int, "int");
                IdentityTypes.Add(Essentials.Long, "int64_t");
                IdentityTypes.Add(Essentials.SByte, "int8_t");
                IdentityTypes.Add(Essentials.Short, "int16_t");
                IdentityTypes.Add(Essentials.UInt, "unsigned int");
                IdentityTypes.Add(Essentials.ULong, "uint64_t");
                IdentityTypes.Add(Essentials.UShort, "uint16_t");
                IdentityTypes.Add(ILFactory.GetType("global::Uno.IntPtr"), "void*");
            }
        }

        protected override void OnForeignFunction(Function f, List<string> annotations)
        {
            EnsureInitialized();

            var wrappedBody = WrapBody(f, Helpers.GetForeignCodeFromFunction(f));
            var code = Preprocess(f.Source, wrappedBody, f, Helpers.GetExternFunctionScopes(f));
            Helpers.ReplaceBody(f, code);
        }

        public string Preprocess(Source source, string str, Function context = null, params Namescope[] usings)
        {
            EnsureInitialized();
            return Environment.Expand(
                source,
                new ExpandInterceptor(
                    (m, e, c, mc) => ExpandEntity(source, e, c, mc)),
                str,
                false,
                context,
                usings);
        }

        string ExpandEntity(Source source, string expansionResult, object context, Macros.MacroCall call)
        {
            if (call.Method == null && context is DataType)
                return GetParamConversion(source, (DataType)context).ForeignType;

            if (call.Method == "Call" && context is DelegateType)
                return GetDelegateCall(source, (DelegateType)context, call.Arguments);

            return expansionResult;
        }

        string WrapBody(Function f, string body)
        {
            if (!f.IsStatic)
            {
                Log.Error(f.Source, ErrorCode.E0000, "Foreign CPlusPlus functions must be static");
            }
            var paramConversions = f.Parameters.Select(GetParamConversion).ToArray();
            var paramNames = f.Parameters.Select((x, i) => "@IL$" + i).ToArray();
            var foreignParamNames = f.Parameters.Select(x => x.UnoName).ToArray();
            var returnConversion = GetConversion(f.Source, f.ReturnType);
            var arguments = paramConversions.Select((p, i) => p.FromUno(paramNames[i])).ToArray();

            if (paramConversions.All(x => x.IsIdentity))
            {
                return body;
            }
            else
            {
                using (var sw = new StringWriter())
                {
                    using (var tf = new TextFormatter(sw))
                    {
                        tf.BeginLine();
                        tf.Write("[] ");
                        tf.Write(ForeignTypeParamList(paramConversions, foreignParamNames));
                        tf.Write(" -> ");
                        tf.Write(returnConversion.ForeignType);
                        tf.EndLine();
                        tf.Indent("{");
                        tf.WriteLines(body);
                        tf.Unindent();
                        tf.Write("} (");
                        tf.Write(string.Join(", ", arguments));
                        tf.Write(")");
                        return MakeReturnStatement(f.ReturnType, sw.ToString());
                    }
                }
            }
        }

        string ForeignTypeParamList(IEnumerable<TypeConversion> conversions, string[] parameters)
        {
            return "(" + string.Join(", ", conversions.Select((x, i) => x.ForeignType + " " + parameters[i])) + ")";
        }

        string MakeReturnStatement(DataType dt, string x)
        {
            return (dt.IsVoid ? x : "return " + x) + ";";
        }

        string GetDataPtr(string x) => "(" + x + " == nullptr ? nullptr : " + x + "->Ptr())";

        TypeConversion GetParamConversion(Parameter p)
        {
            if (p.IsReference)
            {
                var ic = GetConversion(p.Source, p.Type);
                return TypeConversion.Identity(new IdentityConversion(p.Type, ic.ForeignType + "*"));
            }
            return GetParamConversion(p.Source, p.Type);
        }

        TypeConversion GetParamConversion(Source src, DataType dt)
        {
            if (dt.IsArray)
            {
                var ic = GetConversion(src, dt.ElementType);
                return new TypeConversion(dt, ic.ForeignType + "*", x => "(" + ic.ForeignType + "*)" + GetDataPtr(x));
            }
            if (dt == Essentials.String)
                return new TypeConversion(dt, "const char16_t*", x => "(char16_t*)" + GetDataPtr(x));

            if (dt.IsDelegate)
                return TypeConversion.Identity(new IdentityConversion(dt, Helpers.CompiledType(dt)));

            return TypeConversion.Identity(GetConversion(src, dt));
        }

        IdentityConversion GetConversion(Source src, DataType dt)
        {
            if (dt.IsVoid)
                return new IdentityConversion(dt, "void");

            if (dt.IsEnum)
                return GetConversion(src, dt.Base);

            if (dt.IsDelegate)
                return new IdentityConversion(dt, "::uDelegate*");

            string result;
            if (IdentityTypes.TryGetValue(dt, out result))
                return new IdentityConversion(dt, result);

            Log.Error(src, ErrorCode.E0000, "Foreign CPlusPlus error: No known foreign type for " + dt.FullName + ".");
            return default(IdentityConversion);
        }

        string GetDelegateCall(Source src, DelegateType dt, List<string> arguments)
        {
            var parameters = dt.Parameters;
            // + 1 for the delegate itself
            if (parameters.Length + 1 != arguments.Count)
                Log.Error(src, ErrorCode.E0000,
                    "Foreign CPlusPlus error: Wrong number of arguments for macro call to delegate " + dt.FullName + ".");

            string[] foreignParamTypes;
            string[] unoArgs;
            ConvertDelegateArguments(src, parameters, out foreignParamTypes, out unoArgs);
            var returnConversion = GetConversion(src, dt.ReturnType);

            using (var sw = new StringWriter())
            using (var tf = new TextFormatter(sw))
            {
                tf.BeginLine();
                tf.Write("[] (" + Helpers.CompiledType(dt) + " __unoDelegate");
                for (var i = 0; i < parameters.Length; ++i)
                {
                    var p = parameters[i];
                    tf.Write(", " + foreignParamTypes[i] + " " + p.Name);
                }
                tf.Write(") -> " + returnConversion.ForeignType);
                tf.EndLine();
                tf.Indent("{");

                var call = Helpers.CallDelegate(
                    Helpers.StringExpr(dt, "__unoDelegate"),
                    parameters.Select((p, i) => Helpers.StringExpr(p.Type, unoArgs[i])).ToArray());
                tf.WriteLine((dt.ReturnType.IsVoid ? call : "return " + call) + ";");

                tf.Unindent();
                tf.Write("} (" + string.Join(", ", arguments) + ")");
                return sw.ToString();
            }
        }

        // Uses the names of the parameters for convertedArgs
        void ConvertDelegateArguments(
            Source src,
            Parameter[] parameters,
            out string[] foreignTypes, out string[] convertedArgs)
        {
            foreignTypes = new string[parameters.Length];
            convertedArgs = new string[parameters.Length];

            for (int i = 0; i < parameters.Length; ++i)
            {
                var p = parameters[i];
                var lengthp = i < parameters.Length - 1 ? parameters[i + 1] : null;
                ConvertDelegateArgument(src, p, lengthp, out foreignTypes[i], out convertedArgs[i]);
            }
        }

        void ConvertDelegateArgument(
            Source src,
            Parameter p, Parameter lengthp,
            out string foreignType, out string convertedArg)
        {
            foreignType = convertedArg = null;
            var dt = p.Type;
            if (dt.IsArray)
            {
                if (p.IsReference)
                    Log.Error(src, ErrorCode.E0000,
                        "Foreign CPlusPlus: Unsupported reference parameter type in delegate.");
                if (lengthp == null || lengthp.Type != Essentials.Int || lengthp.IsReference)
                    Log.Error(src, ErrorCode.E0000,
                        "Foreign CPlusPlus: A delegate array parameter has to be followed by an 'int' parameter describing its length");

                foreignType = GetParamConversion(src, dt).ForeignType;
                convertedArg = "::uArray::New(" + Helpers.TypeOf(dt) + ", " + lengthp.Name + ", " + p.Name + ")";
                return;
            }
            if (dt == Essentials.String)
            {
                if (p.IsReference)
                    Log.Error(src, ErrorCode.E0000,
                        "Foreign CPlusPlus: Unsupported reference parameter type in delegate.");

                foreignType = GetParamConversion(src, dt).ForeignType;
                var char16_t = Helpers.CompiledType(Essentials.Char);
                convertedArg = "::uString::Utf16((const " + char16_t + "*)" + p.Name + ")";
                return;
            }
            if (dt.IsDelegate)
            {
                Log.Error(src, ErrorCode.E0000,
                    "Foreign CPlusPlus: Unsupported type in delegate: " + dt.FullName + " ");
                return;
            }
            var ft = GetConversion(src, p.Type).ForeignType;
            foreignType = p.IsReference ? ft + "*" : ft;
            convertedArg = p.Name;
        }
    }
}
