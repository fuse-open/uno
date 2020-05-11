using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Uno.Compiler.API;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.Extensions;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Compiler.Backends.CPlusPlus;
using Uno.IO;

namespace Uno.Compiler.Foreign.ObjC
{
    class ForeignObjCPass : ForeignPass
    {
        public ForeignObjCPass(CppBackend backend)
            : base(backend)
        {
        }

        protected override string Language => "ObjC";

        bool _initialized;
        DataType _objCObject;
        DataType _objCID;
        DataType _intPtr;
        Method _getHandle;
        Method _newObjCObject;

        void EnsureInitialized(Source source, Function f)
        {
			Helpers.CacheContext(f, source);
            if (!_initialized)
            {
                _initialized = true;
                _objCObject = ILFactory.GetType("global::ObjC.Object");
                _objCID = ILFactory.GetType("global::ObjC.ID");
                _intPtr = ILFactory.GetType("global::Uno.IntPtr");
                _getHandle = (Method)ILFactory.GetEntity("global::ObjC.Object.GetHandle(" + Helpers.FullGlobalName(_objCObject) + ")");
                _newObjCObject = (Method)ILFactory.GetEntity(Helpers.FullGlobalName(_objCObject) + ".Create(" + Helpers.FullGlobalName(_objCID) + ")");
            }
        }

        protected override void OnForeignFunction(Function f, List<string> annotations)
        {
            EnsureInitialized(f.Source, f);

            Helpers.TryAddProperty(
                f.DeclaringType,
                "FileExtension",
                new Element(f.Source, "mm"));

            Environment.Require(
                f.DeclaringType,
                "Source.Include",
                f.Source, 
                "uObjC.Foreign.h");

            _unique = 0;
            var wrappedBody = WrapBody(f, Helpers.GetForeignCodeFromFunction(f));
            var code = Preprocess(f.Source, wrappedBody, f, Helpers.GetExternFunctionScopes(f));
            Helpers.ReplaceBody(f, code);
            HandleForeignImports(f.DeclaringType);
        }

        int _unique;
        string UniqueName(string suggested)
        {
            return suggested + "__" + _unique++;
        }

        public string Preprocess(Source source, string str, Function context = null, params Namescope[] usings)
        {
            EnsureInitialized(source, context);
            return Environment.Expand(
                source,
                new ExpandInterceptor(
                    (m, e, c, mc) => ExpandType(source, e, c, mc),
                    (x, o) => GetConversion(source, o as DataType).ToUno(x),
                    (x, o) =>
                    {
                        var tc = GetConversion(source, o as DataType);
                        return WithForeignPool(tc, MakeReturnStatement(tc.UnoType, tc.FromUno(x)));
                    }),
                str,
                false,
                context,
                usings);
        }

        string ExpandType(Source source, string expansionResult, object context, Macros.MacroCall call)
        {
            if (context is DataType && call.Method == null)
                return GetConversion(source, (DataType)context).ForeignType;
            else
                return expansionResult;
        }

        string WrapBody(Function f, string body)
        {
            var paramConversions = new List<ParamTypeConversion>();
            var paramNames = new List<string>();
            var foreignParamNames = new List<string>();

            if (!f.IsStatic && body.Contains("_this"))
            {
                paramConversions.Add(
                    GetParamConversion(
                        new Parameter(
                            f.Source,
                            new NewObject[0],
                            ParameterModifier.This,
                            f.DeclaringType,
                            "@IL$$",
                            null)));
                paramNames.Add("@IL$$");
                foreignParamNames.Add("_this");
            }

            paramConversions.AddRange(f.Parameters.Select(GetParamConversion));
            paramNames.AddRange(f.Parameters.Select((x, i) => "@IL$" + i));
            foreignParamNames.AddRange(f.Parameters.Select(x => x.UnoName));

            var returnConversion = GetConversion(f.Source, f.ReturnType);

            var convertedArguments = paramConversions.Select((p, i) => p.FromUno(paramNames[i])).ToList();
            var preStatements = convertedArguments.SelectMany(x => x.PreStatements).ToList();
            var arguments = convertedArguments.Select(x => x.Expression).ToList();
            var postStatements = convertedArguments.SelectMany(x => x.PostStatements).ToList();

            if (paramConversions.All(x => x.IsIdentity) &&
                returnConversion.IsIdentity &&
                preStatements.Count == 0 &&
                postStatements.Count == 0 &&
                f.Parameters.All(p => p.Name == p.UnoName))
            {
                return WithObjCAutoreleasePool(body);
            }
            else
            {
                using (var tw = new StringWriter())
                {
                    using (var ftw = new TextFormatter(tw))
                    {
                        ftw.BeginLine();
                        ftw.Write("[] ");
                        ftw.Write(ForeignTypeParamList(paramConversions, foreignParamNames.ToArray()));
                        ftw.Write(" -> ");
                        ftw.Write(returnConversion.ForeignType);
                        ftw.EndLine();
                        ftw.Indent("{");
                        ftw.WriteLines(body);
                        ftw.Unindent();
                        ftw.Write("} (");
                        ftw.Write(string.Join(", ", arguments));
                        ftw.Write(")");
                        return WithObjCAutoreleasePool(ReturnWithPrePostStatements(f.ReturnType, preStatements, returnConversion.ToUno(tw.ToString()), postStatements));
                    }
                }
            }
        }

        string ReturnWithPrePostStatements(DataType returnType, List<string> preStatements, string expression, List<string> postStatements)
        {
            using (var tw = new StringWriter())
            {
                using (var ftw = new TextFormatter(tw))
                {
                    preStatements.ForEach(ftw.WriteLine);

                    if (postStatements.Count == 0)
                    {
                        ftw.WriteLine(MakeReturnStatement(returnType, expression));
                    }
                    else
                    {
                        var resultName = UniqueName("return");
                        ftw.WriteLines(returnType.IsVoid
                            ? expression + ";"
                            : Helpers.CompiledType(returnType) + " " + resultName + " = " + expression + ";");
                        if (!returnType.IsVoid)
                            ftw.WriteLine(MakeReturnStatement(returnType, resultName));
                        postStatements.ForEach(ftw.WriteLine);
                    }
                    return tw.ToString();
                }
            }
        }

        string ForeignTypeParamList(IEnumerable<ParamTypeConversion> conversions, string[] parameters)
        {
            return "(" + string.Join(", ", conversions.Select((x, i) => x.ForeignType + " " + parameters[i])) + ")";
        }

        string ForeignTypes(IEnumerable<ParamTypeConversion> conversions)
        {
            return string.Join(", ", conversions.Select(x => x.ForeignType));
        }

        string MakeReturnStatement(DataType dt, string x)
        {
            return (dt.IsVoid ? x : "return " + x) + ";";
        }

        ParamTypeConversion GetParamConversion(Parameter p)
        {
            var tc = GetConversion(p.Source, p.Type);
            if (p.IsReference)
            {
                var foreignType = tc.ForeignType + (tc.IsBoxed ? " __autoreleasing*" : "*");
                if (tc.IsIdentity)
                {
                    return new ParamTypeConversion(
                        p,
                        foreignType,
                        x => new FatConversion(tc.FromUno(x)),
                        x => new FatConversion(tc.ToUno(x)),
                        ConversionFlags.Identity);
                }
                else
                {
                    return new ParamTypeConversion(
                        p,
                        foreignType,
                        x =>
                        {
                            var name = UniqueName(x + "__foreign");
                            return new FatConversion(
                                new string[] { tc.ForeignType + " " + name + (p.Modifier == ParameterModifier.Out ? ";" : " = " + tc.FromUno("*" + x) + ";") },
                                "&" + name,
                                new string[] { "*" + x + " = " + tc.ToUno(name) + ";" });
                        },
                        x =>
                        {
                            var name = UniqueName(x + "__uno");
                            return new FatConversion(
                                new string[] { Helpers.CompiledType(tc.UnoType) + " " + name + " = " + tc.ToUno("*" + x) + ";" },
                                "&" + name,
                                new string[] { "*" + x + " = " + tc.FromUno(name) + ";" });
                        },
                        ConversionFlags.None);
                }
            }
            else
            {
                return new ParamTypeConversion(
                    p,
                    tc.ForeignType,
                    x => new FatConversion(tc.FromUno(x)),
                    x => new FatConversion(tc.ToUno(x)),
                    tc.ConversionFlags);
            }
        }

        TypeConversion GetConversion(Source src, DataType dt)
        {
            if (dt == _objCID || dt.IsVoid)
            {
                return TypeConversion.Identity(dt, Helpers.CompiledType(dt), x => x, x => x);
            }
            if (Helpers.IsPrimitive(dt))
            {
                var type = Helpers.CompiledType(dt);
                return TypeConversion.Identity(
                    dt,
                    type,
                    x => "::uObjC::Box<" + type + ">(" + x + ")",
                    x => "::uObjC::Unbox<" + type + ">(" + x + ")");
            }
            if (dt.IsEnum)
            {
                return GetConversion(src, dt.Base);
            }
            if (dt == Essentials.String)
            {
                return TypeConversion.Boxed(
                    dt,
                    "::NSString*",
                    x => "::uObjC::NativeString(" + x + ")",
                    x => "::uObjC::UnoString(" + x + ")");
            }
            if (dt == _objCObject)
            {
                return TypeConversion.Boxed(
                    dt,
                    "::id",
                    x => Helpers.CallStatic(_getHandle, Helpers.StringExpr(_objCObject, x)),
                    x => Helpers.CallStatic(_newObjCObject, Helpers.StringExpr(_objCID, x)));
            }
            {
                string typeName = dt.TryGetAttributeString(Essentials.ForeignTypeNameAttribute);
                if (typeName != null)
                {
                    return GetTypeNameConversion(src, typeName, dt);
                }
            }
            if (dt == _intPtr || dt.IsStruct)
            {
                return TypeConversion.Identity(dt, Helpers.CompiledType(dt), x => x, x => x);
            }
            if (dt.IsArray)
            {
                return GetArrayConversion(src, (ArrayType)dt);
            }
            if (dt.IsClass || dt.IsInterface)
            {
                return TypeConversion.Boxed(
                    dt,
                    "id<UnoObject>",
                    x => "[::StrongUnoObject strongUnoObjectWithUnoObject: " + x + "]",
                    x => Helpers.CastTo(dt, "(" + x + ").unoObject"));
            }
            if (dt.IsDelegate)
            {
                return GetDelegateConversion(src, (DelegateType)dt);
            }

            throw new Exception(src + ": Uno->ObjC error. No known ObjC type for " + dt.FullName + ".");
        }

        string GetDelegateParameterType(ParamTypeConversion pc)
        {
            return Backend.GetTypeDef(pc.UnoParameter.Type, Namescope, pc.UnoParameter.IsReference ? TypeFlags.ParameterByRef : TypeFlags.Parameter);
        }

        string GetDelegateArgumentString(Parameter p)
        {
            return ((!p.IsReference && Backend.RequiresIndirection(p.Type, TypeFlags.Parameter)) ? "*" : "") + p.Name;
        }

        string GetDelegateReturnType(TypeConversion tc)
        {
            return Backend.GetTypeDef(tc.UnoType, Namescope, TypeFlags.ReturnByRef);
        }

        string GetDelegateReturnString(TypeConversion tc, string s)
        {
            return (Backend.RequiresIndirection(tc.UnoType, TypeFlags.ReturnByRef) ? "*" : "") + s;
        }

        TypeConversion GetTypeNameConversion(Source src, string typeName, DataType dt)
        {
            if (!dt.IsSubclassOf(_objCObject))
                throw new Exception(src + ": Uno->ObjC error. " + dt.FullName +
                    " must be a subclass of " + _objCObject.FullName +
                    " to be usable with " + Essentials.ForeignTypeNameAttribute.Name + ".");
            if ( dt.TryGetDefaultConstructor() == null)
                throw new Exception(src + ": Uno->ObjC error. " + dt.FullName +
                    " must have a default constructor to be usable with " +
                    Essentials.ForeignTypeNameAttribute.Name + ".");

            var ct = Helpers.CompiledType(dt);
            var fullName = "global::" + dt.FullName;

            return TypeConversion.Boxed(
                dt,
                typeName,
                x => "(" + typeName + ")" + Helpers.CallStatic(_getHandle, Helpers.StringExpr(_objCObject, x)),
                x => "[](" + typeName + " x) -> " + ct + " { if (x == nil) return nullptr; " +
                    // TODO How to not have to touch :New in a comment to get it not to strip?
                    ct + " r = \\@{" + fullName + "():New()}; /* @{" + fullName + "():New()} */ \\@{" + fullName +
                    ":Of(r).Handle:Set(x)}; return r; }(" + x + ")");
        }

        TypeConversion GetArrayConversion(Source src, ArrayType at)
        {
            var elementConversion = GetConversion(src, at.ElementType);
            var getAt = "^ id (::uArray* arr, int i) { return " +
                elementConversion.Box(elementConversion.FromUno(Helpers.ArrayGet(Helpers.StringExpr(at, "arr"), Helpers.StringExpr(Essentials.Int, "i")))) +
                "; }";
            var setAt = "^ (::uArray* arr, int i, id obj) { " +
                Helpers.ArraySet(
                    Helpers.StringExpr(at, "arr"),
                    Helpers.StringExpr(Essentials.Int, "i"),
                    Helpers.StringExpr(at.ElementType, elementConversion.ToUno(elementConversion.Unbox("obj")))) +
                "; }";

            var ret = TypeConversion.Boxed(
                at,
                "id<UnoArray>",
                x => "[::StrongUnoArray strongUnoArrayWithUnoArray: " + x + " getAt: " + getAt + " setAt: " + setAt + "]",
                x => Helpers.CastTo(at, "(" + x + ").unoArray"));

            return ret;
        }

        TypeConversion GetDelegateConversion(Source src, DelegateType dt)
        {
            var paramConversions = dt.Parameters.Select(GetParamConversion).ToArray();
            var returnConversion = GetConversion(src, dt.ReturnType);
            var compiledType = Helpers.CompiledType(dt);

            var foreignType = "::uObjC::Function<" +
                     returnConversion.ForeignType +
                     ((paramConversions.Length > 0) ? ", " : "") +
                     ForeignTypes(paramConversions) + ">";

            Func<string, string> fromUno = (string x) =>
            {
                var convertedArguments = paramConversions.Select((p, i) => p.ToUno(dt.Parameters[i].Name)).ToList();
                var preStatements = convertedArguments.SelectMany(a => a.PreStatements).ToList();
                var arguments = convertedArguments.Select((arg, i) => Helpers.StringExpr(dt.Parameters[i].Type, arg.Expression)).ToArray();
                var postStatements = convertedArguments.SelectMany(a => a.PostStatements).ToList();

                using (var tw = new StringWriter())
                {
                    using (var ftw = new TextFormatter(tw))
                    {
                        ftw.WriteLine("[] (id<UnoObject> __delegateRef) -> " + foreignType);
                        ftw.Indent("{");
                        ftw.WriteLine("return __delegateRef == nil ? (" + foreignType + ")nil : (^ " + returnConversion.ForeignType + " " +
                                ForeignTypeParamList(paramConversions, dt.Parameters.Select(p => p.Name).ToArray()));
                        ftw.Indent("{");
                        ftw.WriteLine("::uForeignPool __foreignPool;");
                        ftw.WriteLine(compiledType + " __unoDelegate = (" + compiledType + ")__delegateRef.unoObject;");
                        ftw.WriteLines(ReturnWithPrePostStatements(
                                dt.ReturnType,
                                preStatements,
                                returnConversion.FromUno(Helpers.CallDelegate(Helpers.StringExpr(dt, "__unoDelegate"), arguments)),
                                postStatements));
                        ftw.Unindent("});");
                        ftw.Unindent();
                        ftw.Write("} ([::StrongUnoObject strongUnoObjectWithUnoObject: " + x + "])");
                        return tw.ToString();
                    }
                }
            };

            Func<string, string> toUno = (string x) =>
            {
                var convertedArguments = paramConversions.Select((p, i) => p.FromUno(GetDelegateArgumentString(dt.Parameters[i]))).ToList();
                var preStatements = convertedArguments.SelectMany(a => a.PreStatements).ToList();
                var arguments = convertedArguments.Select(a => a.Expression).ToList();
                var postStatements = convertedArguments.SelectMany(a => a.PostStatements).ToList();

                using (var tw = new StringWriter())
                {
                    using (var ftw = new TextFormatter(tw))
                    {
                        ftw.Write("::uObjC::NewUnoDelegate(" + Helpers.TypeOf(dt) + ", ");
                        string fptrType = "::uObjC::RawFunction<void, " + Helpers.CompiledType(Essentials.Object) +
                            ((paramConversions.Length) > 0 ? ", " : "") +
                            string.Join(", ", paramConversions.Select(GetDelegateParameterType)) +
                            (dt.ReturnType.IsVoid ? "" : (", " + Helpers.CompiledType(returnConversion.UnoType) + "*")) +
                            ">";
                        ftw.Write("(void*) (" + fptrType + ") [] (");
                        ftw.Write(Helpers.CompiledType(Essentials.Object) + " __this" +
                            ((paramConversions.Length > 0) ? ", " : "") +
                            string.Join(", ", paramConversions.Select((p, i) => GetDelegateParameterType(p) + " " + dt.Parameters[i].Name)) +
                            (dt.ReturnType.IsVoid ? "" : (", " + GetDelegateReturnType(returnConversion) + " __ret")));
                        ftw.Write(")");
                        ftw.EndLine();
                        ftw.Indent("{");
                        ftw.WriteLine(Helpers.CompiledType(_objCObject) + " __thisObj = (" + Helpers.CompiledType(_objCObject) + ")__this;");
                        preStatements.ForEach(ftw.WriteLine);
                        ftw.BeginLine();
                        if (!dt.ReturnType.IsVoid)
                        {
                            ftw.Write(GetDelegateReturnString(returnConversion, "__ret") + " = ");
                        }
                        ftw.Write(returnConversion.ToUno("((" + foreignType + ")" + Helpers.CallStatic(_getHandle, Helpers.StringExpr(_objCObject, "__thisObj")) +
                            ")(" + string.Join(", ", arguments) + ")") + ";");
                        ftw.EndLine();
                        postStatements.ForEach(ftw.WriteLine);
                        ftw.Unindent();
                        ftw.Write("}, " + Helpers.CallStatic(_newObjCObject, Helpers.StringExpr(_objCID, x)) + ")");
                        return tw.ToString();
                    }
                }
            };

            return TypeConversion.Boxed(
                dt,
                foreignType,
                fromUno,
                toUno);
        }

        string WithForeignPool(TypeConversion tc, string x)
        {
            return "[&]() -> " + tc.ForeignType + " { ::uForeignPool __foreignPool; " + x + " }()";
        }

        string WithObjCAutoreleasePool(string x)
        {
            using (var tw = new StringWriter())
            {
                using (var ftw = new TextFormatter(tw))
                {
                    ftw.WriteLine("@autoreleasepool");
                    ftw.Indent("{");
                    ftw.WriteLines(x);
                    ftw.Unindent("}");
                    return tw.ToString();
                }
            }
        }

        HashSet<DataType> visitedTypes = new HashSet<DataType>();
        void HandleForeignImports(DataType dt)
        {
            if (!visitedTypes.Contains(dt))
            {
                visitedTypes.Add(dt);
                var includes = Helpers.GetForeignIncludes(dt, "ObjC", Environment);
                if (includes.Count > 0)
                    Helpers.SourceInclude(includes, dt);
            }
        }
    }
}
