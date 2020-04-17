using System;
using System.Collections.Generic;
using System.Text;
using Uno.Collections;
using Uno.Compiler.API;
using Uno.Compiler.API.Backends;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Backends.CPlusPlus
{
    public sealed class CppBackend : SourceBackend
    {
        public override string Name => "CPlusPlus";

        internal const string RootNamespace = "g"; // Collision fix
        internal IncludeResolver IncludeResolver { get; private set; }

        // Configuration
        public bool EnableReflection { get; private set; }
        public bool EnableDebugDumps { get; private set; }
        public bool EnableStackTrace { get; private set; }
        public string HeaderDirectory { get; private set; }
        public string SourceDirectory { get; private set; }
        readonly BackendExtension _extension;
        HashSet<string> _keywords;

        public CppBackend(ShaderBackend shaderBackend, BackendExtension extension = null)
            : base(shaderBackend)
        {
            _extension = extension ?? BackendExtension.Null;

            FunctionOptions =
                FunctionOptions.DecodeDelegateOps |
                FunctionOptions.DecodeEnumOps |
                FunctionOptions.DecodeNullOps |
                FunctionOptions.DecodeSwizzles |
                FunctionOptions.DecodeSetChains |
                FunctionOptions.MakeNativeCode |
                FunctionOptions.Analyze |
                FunctionOptions.ClosureConvert;
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
            Decompiler = new CppDecompiler(this);
            IncludeResolver = new IncludeResolver(this, Environment);
            Scheduler.AddTransform(new CppFinallyTransform(this));
            Scheduler.AddTransform(new CppTransform(this));
            _extension.Begin(compiler);
        }

        public override void Configure()
        {
            _keywords = Environment.GetWords("Keywords");
            EnableReflection = Environment.IsDefined("REFLECTION");
            EnableDebugDumps = Environment.IsDefined("DEBUG_DUMPS");
            EnableStackTrace = Environment.IsDefined("STACKTRACE") || Environment.IsDefined("CPPSTACKTRACE");
            HeaderDirectory = Environment.GetOutputPath("HeaderDirectory");
            SourceDirectory = Environment.GetOutputPath("SourceDirectory");

            // On Android, truncate lengths of filenames to avoid problem with Gradle on Windows.
            if (Environment.IsDefined("ANDROID"))
                MaxExportNameLength = 30;
        }

        public override bool CanLink(Function f)
        {
            return Environment.GetBool(f, "IsIntrinsic");
        }

        public override BackendResult Build()
        {
            ExportBundle(Environment.GetString("BundleDirectory"));
            new CppPrecalc(this).Run();
            ExportNamespace(Data.IL);

            foreach (var e in IncludeResolver.GetIncludes(Data.Entrypoint))
                Environment.Require("Main.Include", e);

            Environment.Set("Main.Body", Decompiler.GetScope(Data.StartupCode, Data.Entrypoint));
            return null;
        }

        protected override void ExportNamespace(Namespace ns)
        {
            var joinedTypes = new ListDictionary<string, DataType>();
            var lonelyTypes = new List<DataType>();

            foreach (var dt in ns.Types)
                ClassifyTypes(ns, dt, joinedTypes, lonelyTypes);

            foreach (var list in joinedTypes)
                CppGenerator.ExportTypes(Environment, Essentials, this, ns, list.Key, list.Value);
            foreach (var item in lonelyTypes)
                CppGenerator.ExportType(Environment, Essentials, this, item);

            foreach (var child in ns.Namespaces)
                ExportNamespace(child);
        }

        void ClassifyTypes(Namespace ns, DataType dt, ListDictionary<string, DataType> joinedTypes, List<DataType> lonelyTypes)
        {
            foreach (var it in dt.NestedTypes)
                ClassifyTypes(ns, it, joinedTypes, lonelyTypes);

            // Classes and structs in user projects gets their own source file for better incremental build support.
            // Other types are grouped into shared source files (per namespace) for better build performance.
            switch (dt.TypeType)
            {
                case TypeType.Delegate:
                case TypeType.Enum:
                case TypeType.Interface:
                    joinedTypes.Add(GetKey(ns), dt);
                    break;
                default:
                    if (dt.Source.Package.IsCached)
                        joinedTypes.Add(GetKey(ns), dt);
                    else if (dt.IsNestedType || dt.NestedTypes.Count > 0)
                        joinedTypes.Add(GetKey(dt), dt);
                    else
                        lonelyTypes.Add(dt);
                    break;
            }
        }

        string GetKey(DataType dt)
        {
            return dt.IsNestedType
                ? GetKey(dt.ParentType)
                : GetExportName(dt) + ".g";
        }

        string GetKey(Namespace ns)
        {
            return GetExportName(ns) + ".g";
        }

        public int GetIndex(Field f)
        {
            var index = GetType(f.DeclaringType).FlattenedFields.IndexOf(f.MasterDefinition);

            if (index == -1)
                Log.Error("C++: Failed to get index of " + f.Quote());

            return index;
        }

        public CppFunction GetFunction(Function f)
        {
            return f.Tag as CppFunction ?? (CppFunction) (
                    f.Tag = f.IsMasterDefinition
                        ? new CppFunction()
                        : GetFunction(f.MasterDefinition)
                );
        }

        public CppType GetType(DataType dt)
        {
            var data = TypeData.Get(dt);
            return data.Type ?? (
                    data.Type = dt.IsMasterDefinition
                        ? new CppType(Environment, this, dt)
                        : GetType(dt.MasterDefinition)
                );
        }

        public HashSet<IEntity> GetDependencies(Function f)
        {
            var data = GetFunction(f);
            return data.CachedDependencies ?? (data.CachedDependencies = Utilities.FindDependencies(f));
        }

        public string GetStaticName(Namescope ns, Namescope parent = null)
        {
            if (ns.IsRoot)
                return "::" + RootNamespace;

            if (ns.MasterDefinition == parent)
            {
                var dt = ns as DataType;
                if (dt != null)
                {
                    var name = dt.Name;
                    for (dt = dt.ParentType; dt != null; dt = dt.ParentType)
                        name = dt.Name + "__" + name;
                    return name;
                }

                return ns.Name;
            }

            return
                GetStaticName(ns.Parent, parent) +
                (ns.IsNestedType ? "__" : "::") +
                ns.Name;
        }

        public string GetTypeName(DataType dt, Namescope parent = null, bool inline = false)
        {
            switch (dt.TypeType)
            {
                case TypeType.Void:
                    return "void";
                case TypeType.RefArray:
                    return "uArray*";
                case TypeType.Delegate:
                    return "uDelegate*";
                case TypeType.GenericParameter:
                    return dt.IsReferenceType
                            ? GetTypeName(dt.Base, parent, inline) :
                        inline
                            ? dt.Name
                            : "void*";

                case TypeType.Enum:
                case TypeType.Interface:
                    return GetTypeName(dt.Base, parent, inline);

                default:
                    SourceValue result;
                    if (Environment.TryGetValue(dt, "TypeName", out result))
                        return result.String;
                    if (!inline && IsConstrained(dt) ||
                        dt.HasAttribute(Essentials.TargetSpecificTypeAttribute))
                        return "void*";

                    var name = GetStaticName(dt, parent) + GetTemplateSuffix(dt, parent);
                    return dt.IsReferenceType ? name + "*" : name;
            }
        }

        public string GetTypeDef(DataType dt, Namescope parent = null, TypeFlags flags = 0)
        {
            if (flags == TypeFlags.ThisByRefDeclaration && dt.TypeType == TypeType.Interface ||
                flags == TypeFlags.ThisInlineDeclaration && dt.TypeType == TypeType.Interface)
                return "const uInterface&";
            if (flags == TypeFlags.ReturnByRef && IsConstrained(dt) ||
                flags == TypeFlags.ParameterByRef && IsConstrained(dt) ||
                flags.HasFlag(TypeFlags.ThisByRef) && IsConstrained(dt))
                return "uTRef";

            var name = GetTypeName(dt, parent, flags.HasFlag(TypeFlags.Inline));
            return RequiresIndirection(dt, flags)
                ? name + "*"
                : name;
        }

        public bool RequiresIndirection(DataType dt, TypeFlags flags = 0)
        {
            if (flags == TypeFlags.ThisByRefDeclaration && dt.TypeType == TypeType.Interface ||
                flags == TypeFlags.ThisInlineDeclaration && dt.TypeType == TypeType.Interface)
                return false;
            if (flags == TypeFlags.ReturnByRef && IsConstrained(dt) ||
                flags == TypeFlags.ParameterByRef && IsConstrained(dt) ||
                flags.HasFlag(TypeFlags.ThisByRef) && IsConstrained(dt))
                return false;

            return flags.HasFlag(TypeFlags.ByRef) &&
                !IsConstrained(dt) && !(
                    dt.IsReferenceType &&
                    (flags & TypeFlags.TypeMask) != TypeFlags.Return
                ) || (
                    dt.IsValueType &&
                    !flags.HasFlag(TypeFlags.Inline) &&
                    (flags & TypeFlags.TypeMask) == TypeFlags.Parameter &&
                    !IsConstrained(dt)
                ) || (
                    dt.IsStruct &&
                    !flags.HasFlag(TypeFlags.Inline) &&
                    flags.HasFlag(TypeFlags.This) &&
                    !IsConstrained(dt)
                ) ||
                flags == TypeFlags.ParameterByRef ||
                flags == (TypeFlags.ParameterByRef | TypeFlags.Inline);
        }

        public string GetFieldType(Field f, Namescope parent)
        {
            switch (GetReferenceType(f))
            {
                case ReferenceType.Strong:
                    return (f.IsStatic
                                ? "uSStrong"
                                : "uStrong"
                        ) + GetTemplateString(f.ReturnType, parent);
                case ReferenceType.Weak:
                    return (f.IsStatic
                                ? "uSWeak"
                                : "uWeak"
                        ) + GetTemplateString(f.ReturnType, parent);
                default:
                    return GetTypeName(f.ReturnType, parent, true);
            }
        }

        public string GetBaseType(DataType dt, Namescope parent)
        {
            SourceValue result;
            return Environment.TryGetValue(dt, "BaseType", out result)
                    ? result.String
                    : GetStaticName(dt, parent);
        }

        public string GetTypeOfType(DataType dt, Namescope parent)
        {
            if (dt == null)
                return "uClassType";

            switch (dt.TypeType)
            {
                case TypeType.Delegate:
                    return "uDelegateType";
                case TypeType.Enum:
                    return "uEnumType";
                case TypeType.Interface:
                    return "uInterfaceType";
                default:
                    SourceValue result;
                    return Environment.TryGetValue(dt, "TypeOfType", out result)
                            ? result.String :
                        GetType(dt).EmitTypeStruct
                            ? GetStaticName(dt, parent) + "_type" :
                        dt.IsStruct
                            ? "uStructType" :
                        dt.Base != null
                            ? GetTypeOfType(dt.Base, parent)
                            : "uClassType";
            }
        }

        public string GetTypeOf(Parameter p, Namescope parent, string type, TypeCache? cache = null)
        {
            return GetTypeOf(p.Type, parent, type, null, cache) + (p.IsReference ? "->ByRef()" : null);
        }

        public string GetTypeOf(DataType dt, Namescope parent, Function func, TypeCache? cache = null, BodyFlags flags = 0)
        {
            return func == null || parent != func.DeclaringType || (func.IsStatic || func.DeclaringType.IsStruct) && !HasTypeParameter(func)
                    ? GetTypeOf(dt, parent, null, null, cache) :
                !flags.HasFlag(BodyFlags.Extension) && (
                        HasTypeParameter(func) || !PassByRef(func) || flags.HasFlag(BodyFlags.ClassMember))
                    ? GetTypeOf(dt, parent, "__type", func, cache)
                    : GetTypeOf(dt, parent, "__this->__type", func, cache);
        }

        public string GetTypeOf(DataType dt, Namescope parent, string type = null, Function func = null, TypeCache? cache = null)
        {
            var r = GetTypeOf_internal(dt, parent, type, func, cache);
            
            if (!dt.IsGenericMethodType &&
                ((func as Method)?.IsGenericDefinition ?? false) &&
                r == "__type")
                return r + "->Base";
            
            return r;
        }

        string GetTypeOf_internal(DataType dt, Namescope parent, string type = null, Function func = null, TypeCache? cache = null)
        {
            if (cache != null && !Compare(dt, parent))
            {
                int index;
                if (cache.Value.Scope != null && cache.Value.Scope.TryGetValue(dt, out index))
                    return "__types[" + index + "]";
                if (cache.Value.Global != null && cache.Value.Global.TryGetValue(dt, out index))
                    return "::TYPES[" + index + "/*" + dt + "*/]";
                if (type != null && cache.Value.Precalc != null && cache.Value.Precalc.TryGetValue(dt, out index))
                    return type + (HasVirtualType(func)
                                ? "->GetBase(" + GetTypeOf((DataType) parent, parent, null, func, cache) + ")"
                                : null
                        ) + "->Precalced(" + index + "/*" + dt + "*/)";
            }

            var isBuildFunction = type == "type" ||
                type != null && type.StartsWith("type->");

            switch (dt.TypeType)
            {
                case TypeType.Void:
                    return "uVoid_typeof()";
                case TypeType.RefArray:
                    return GetTypeOf(dt.ElementType, parent, type, func, cache) + "->Array()";
                case TypeType.Enum:
                    return GetStaticName(dt, parent) + "_typeof()";

                case TypeType.GenericParameter:
                {
                    if (!dt.ParentType.IsGenericMethodType && isBuildFunction)
                        type = "type";

                    var index = dt.GenericIndex;
                    if (index != -1 && type != null)
                        return type + (
                                dt.ParentType.IsGenericMethodType
                                    ? "->U("
                                    : (!isBuildFunction && HasVirtualType(func)
                                            ? "->GetBase(" + GetTypeOf((DataType) parent, parent, null, func, cache) + ")"
                                            : null
                                        ) + "->T("
                            ) + index + ")";

                    Log.Error("C++: Failed to get typeof(" + dt.VerboseName + ") in " + ((object) func ?? parent).Quote());
                    return "nullptr";
                }
                default:
                {
                    if (type != null &&
                        (dt.IsFlattenedDefinition || dt.IsMasterDefinition) && (
                            dt == parent && (
                                isBuildFunction ||
                                !HasVirtualType(func)
                            ) || (
                                dt.IsGenericMethodType &&
                                Compare(dt.Parent, parent)
                        )   ))
                        return type;

                    if (dt.IsFlattenedParameterization)
                    {
                        if (dt.IsGenericMethodType)
                        {
                            var index = GetType(dt).MethodIndex;
                            if (index == -1)
                                Log.Error("C++: Failed to get index of " + dt.Quote() + " (generic method)");

                            var baseType = GetTypeOf(dt.ParentType, parent, type, func, cache);
                            if (baseType.StartsWith("type->MethodTypes["))
                                baseType = "type";

                            var sb = new StringBuilder(baseType +
                                "->MakeMethod(" + index + "/*" + dt.UnoName + dt.GenericSuffix + "*/");

                            foreach (var a in dt.GenericArguments)
                                sb.Append(", " + GetTypeOf(a, parent, type, func, cache));

                            sb.Append(", nullptr)");
                            return sb.ToString();
                        }
                        else
                        {
                            if (type != null &&
                                Compare(dt, parent))
                                return type + (!isBuildFunction && HasVirtualType(func)
                                            ? "->GetBase(" + GetTypeOf((DataType) parent, parent, null, func, cache) + ")"
                                            : null);

                            var comma = false;
                            var sb = new StringBuilder(
                                GetTypeOf(dt.MasterDefinition, parent, isBuildFunction ? "type" : type, func, cache) +
                                "->MakeType(");

                            foreach (var a in dt.FlattenedArguments)
                            {
                                sb.CommaWhen(comma);
                                sb.Append(GetTypeOf(a, parent, type, func, cache));
                                comma = true;
                            }

                            sb.Append(", nullptr)");
                            return sb.ToString();
                        }
                    }

                    SourceValue result;
                    return Environment.TryGetValue(dt, "TypeOfFunction", out result)
                        ? result.String + "()"
                        : GetStaticName(dt, parent) + "_typeof()";
                }
            }
        }

        public static bool Compare(Namescope dt, Namescope parent)
        {
            if (dt == parent)
                return true;
            if (dt.MasterDefinition != parent)
                return false;

            var at = (DataType)dt;
            var pt = (DataType)parent;
            if (at.IsFlattenedParameterization && pt.IsFlattenedDefinition)
            {
                var args = at.FlattenedArguments;
                var generics = pt.FlattenedParameters;

                for (int i = 0; i < args.Length; i++)
                    if (args[i] != generics[i])
                        return false;

                return true;
            }

            return false;
        }

        bool HasVirtualType(Function func)
        {
            return !func.IsStatic &&
                !func.DeclaringType.IsSealed &&
                !func.DeclaringType.IsStatic &&
                !func.DeclaringType.IsStruct &&
                !HasTypeParameter(func);
        }

        public bool IsStructMethod(Function f)
        {
            return f != null && f.IsStatic && f.DeclaringType.IsStruct && HasStructParameters(f);
        }

        bool HasStructParameters(Function f)
        {
            foreach (var p in f.Parameters)
                if (!p.IsReference && p.Type.IsStruct && !IsOpaque(p.Type))
                    return true;

            return f.ReturnType.IsStruct && !IsOpaque(f.ReturnType);
        }

        public bool IsExtensionMethod(Function f)
        {
            return !f.IsStatic && (
                    f.DeclaringType.IsInterface ||
                    f.DeclaringType.HasAttribute(Essentials.TargetSpecificTypeAttribute) || (
                        !Environment.GetBool(f, "IsIntrinsic") &&
                        Environment.HasProperty(f.DeclaringType, "TypeName") && (
                            !f.DeclaringType.IsReferenceType ||
                            !f.IsVirtualOverride)));
        }

        public bool IsStackFrame(Function f)
        {
            return EnableStackTrace && 
                f.Stats.HasFlag(EntityStats.ThrowsException) && !(
                    !f.HasBody ||
                    f.Body.Statements.Count == 0 || (
                        f.Prototype.IsConstructor && (
                            f.IsStatic ||
                            f.DeclaringType.IsSubclassOfOrEqual(Essentials.Exception)
                            )) ||
                    f.UnoName.EndsWith("_boxed"));
        }

        public string GetTypeOfPointer(DataType dt, Namescope parent = null)
        {
            return "(uType*(*)())&" + GetStaticName(dt, parent) + "_typeof";
        }

        public string GetTypeOfDeclaration(DataType dt)
        {
            var type = GetType(dt);
            var emit = type.EmitTypeStruct && !dt.IsInterface;
            var result = dt.Name;
            var p = dt.Parent;

            for (; p is DataType; p = p.Parent)
                result = p.Name + "__" + result;

            if (emit)
                result = "struct " + result + "_type; " + result + "_type* " + result + "_typeof();";
            else
            {
                result = GetTypeOfType(dt, null) + "* " + result + "_typeof();";

                for (var bt = dt.Base; bt != null; bt = bt.Base)
                {
                    if (GetType(bt).EmitTypeStruct)
                    {
                        var decl = bt.Name;
                        var p2 = bt.Parent;

                        for (; p2 is DataType; p2 = p2.Parent)
                            decl = p2.Name + "__" + decl;

                        for (; p != null && !p.IsRoot && p != p2; p = p.Parent)
                            result = "namespace " + p.Name + "{" + result + "}";

                        decl = "struct " + decl + "_type;";

                        if (p == p2)
                        {
                            decl += " " + result;
                            result = null;
                            p = null;
                        }

                        while (p2 != null && !p2.IsRoot)
                        {
                            decl = "namespace " + p2.Name + "{" + decl + "}";
                            p2 = p2.Parent;
                        }

                        result = result != null
                            ? decl + " " + result
                            : decl;
                        break;
                    }
                }
            }

            for (; p != null && !p.IsRoot; p = p.Parent)
                result = "namespace " + p.Name + "{" + result + "}";

            return "namespace " + RootNamespace + "{" + result + "}";
        }

        public string GetDefaultValue(DataType dt, Namescope parent, Function func, TypeCache? cache, ExpressionUsage u)
        {
            if (dt.IsEnum)
                dt = dt.Base;

            SourceValue result;
            return Environment.TryGetValue(dt, "DefaultValue", out result)
                    ? result.String :
                dt.IsReferenceType
                    ? "nullptr" :
                IsConstrained(dt)
                    ? (u == ExpressionUsage.VarArg ? "(void*)" : null) +
                        "uT(" + GetTypeOf(dt, parent, func, cache) +
                        ", alloca(" + GetTypeOf(dt, parent, func, cache) + "->ValueSize))"
                    : "uDefault" + GetTemplateString(dt, parent) + "()";
        }

        public string GetIncludeFilename(DataType dt)
        {
            var data = TypeData.Get(dt);
            return data.Include ?? (
                    data.Include = !dt.IsMasterDefinition
                        ? GetIncludeFilename(dt.MasterDefinition)
                        : GetExportName(dt) + ".h"
                );
        }

        public bool HasForwardDeclaration(DataType dt)
        {
            switch (dt.TypeType)
            {
                case TypeType.Struct:
                    return !IsTemplate(dt);

                case TypeType.Class:
                case TypeType.Delegate:
                case TypeType.Interface:
                    return true;

                default:
                    return false;
            }
        }

        public string GetForwardDeclaration(DataType dt)
        {
            var data = TypeData.Get(dt);

            if (data.Declaration != null)
                return data.Declaration;

            string result = null;
            if (!dt.IsMasterDefinition)
                result = GetForwardDeclaration(dt.MasterDefinition);
            else if (IsOpaque(dt))
            {
                result = Environment.GetString(dt, "ForwardDeclaration");

                if (string.IsNullOrEmpty(result))
                {
                    result = Environment.GetString(dt, "Include");

                    if (!string.IsNullOrEmpty(result))
                        result = "#include <" + result + ">";
                }
            }
            else
            {
                var p = dt.Parent;

                while (p is DataType)
                {
                    result = p.Name + "__" + result;
                    p = p.Parent;
                }

                result = GetTemplateHeader(dt) + "struct " + result + dt.Name + ";";

                while (p != null && !p.IsRoot)
                {
                    result = "namespace " + p.Name + "{" + result + "}";
                    p = p.Parent;
                }

                result = "namespace " + RootNamespace + "{" + result + "}";
            }

            return data.Declaration = result;
        }

        public string GetFileExtension(DataType dt)
        {
            return (Environment.GetString(dt, "FileExtension") ?? "cpp").TrimStart('.').ToLower();
        }

        public string GetFunctionPointer(Function f, Namescope parent = null, string suffix = "_fn")
        {
            return GetStaticName(f.DeclaringType, parent) + "__" + f.Name + suffix;
        }

        public string GetParameterType(Parameter p, DataType parent = null, bool inline = false)
        {
            return GetTypeDef(p.Type, parent, (
                    p.IsReference
                        ? TypeFlags.ParameterByRef
                        : TypeFlags.Parameter
                ) | (inline
                        ? TypeFlags.Inline
                        : 0
                ));
        }

        public bool HasTypeParameter(Function f)
        {
            return f != null && (
                    f.IsConstructor || f.IsGenericMethod || f.DeclaringType.MasterDefinition.IsFlattenedDefinition && (
                        f.IsStatic || f.DeclaringType.IsStruct
                    ) || f.DeclaringType.IsStruct && f.IsVirtual ||
                    f is Method && HasTypeParameter(((Method)f).ImplementedMethod)
                );
        }

        public string GetStaticName(Field f, DataType parent = null)
        {
            return GetStaticName(f.DeclaringType, parent) + "::" + GetMemberName(f);
        }

        public string GetStaticName(Member f, DataType parent = null)
        {
            return GetStaticName(f.DeclaringType, parent) + (
                    IsStructMethod(f as Function)
                        ? "__"
                        : "::"
                ) + GetMemberName(f);
        }

        public string GetMemberName(Member m, Function f, Namescope parent = null)
        {
            return IsConstrained(f) ? f.Name + "_ex" : GetMemberName(m) + GetTemplateSuffix(f, parent);
        }

        public string GetMemberName(Member f)
        {
            var m = f as Method;
            return m?.DeclaringMember is Property ? m.DeclaringMember.Name : f.Name;
        }

        public string GetTemplateSuffix(DataType dt, Namescope parent)
        {
            return dt.IsStruct ? GetTemplateString(GetTemplateArguments(dt), parent) : null;
        }

        public string GetTemplateSuffix(Function f, Namescope parent)
        {
            return GetTemplateString(GetTemplateArguments(f), parent);
        }

        public string GetTemplateString(List<Tuple<DataType, bool>> args, Namescope parent)
        {
            if (args == null)
                return null;

            var sb = new StringBuilder("<");

            for (int i = 0; i < args.Count; i++)
            {
                var arg = args[i].Item2 && GetReferenceType(args[i].Item1) == ReferenceType.Strong
                        ? "uStrong" + GetTemplateString(args[i].Item1, parent)
                        : GetTypeName(args[i].Item1, parent, true);

                sb.AppendWhen(i == 0 && arg.StartsWith("::", StringComparison.InvariantCulture), ' ');
                sb.CommaWhen(i > 0);
                sb.Append(arg);
            }

            sb.AppendWhen(sb.Length > 0 && sb[sb.Length - 1] == '>', ' ');
            sb.Append(">");
            return sb.ToString();
        }

        public string GetTemplateString(DataType dt, Namescope parent)
        {
            return GetTemplateString(GetTypeName(dt, parent, true));
        }

        public string GetTemplateString(string arg)
        {
            return (arg.StartsWith(':') ? "< " : "<") +
                arg + (arg.EndsWith('>') ? " >" : ">");
        }

        public string GetTemplateHeader(DataType dt)
        {
            return GetTemplateHeader(GetTemplateParameters(dt));
        }

        public string GetTemplateHeader(List<Tuple<string, bool>> parameters)
        {
            if (parameters == null)
                return null;

            var sb = new StringBuilder("template<");

            for (int i = 0; i < parameters.Count; i++)
            {
                sb.CommaWhen(i > 0);
                sb.Append("class " + parameters[i].Item1);
            }

            sb.Append('>');
            return sb.ToString();
        }

        public List<Tuple<DataType, bool>> GetTemplateArguments(Function f)
        {
            var parameters = GetTemplateParameters(f.MasterDefinition);

            if (parameters == null)
                return null;

            var m = f as Method;
            var dt = f.DeclaringType;
            var map = new Dictionary<string, DataType>();
            var result = new List<Tuple<DataType, bool>>();

            if (dt.IsFlattenedParameterization)
            {
                for (int i = 0; i < dt.FlattenedArguments.Length; i++)
                {
                    var at = dt.FlattenedArguments[i];
                    var gt = dt.MasterDefinition.FlattenedParameters[i];
                    map[gt.Name] = at;
                }
            }

            if (m != null && m.IsGenericParameterization)
            {
                for (int i = 0; i < m.GenericArguments.Length; i++)
                {
                    var at = m.GenericArguments[i];
                    var gt = m.GenericDefinition.GenericParameters[i];
                    map[gt.Name] = at;
                }
            }

            if (map.Count == 0)
                return null;

            foreach (var p in parameters)
                result.Add(Tuple.Create(map[p.Item1], p.Item2));

            return result;
        }

        public List<Tuple<DataType, bool>> GetTemplateArguments(DataType dt)
        {
            if (!dt.IsFlattenedParameterization)
                return null;

            var parameters = GetTemplateParameters(dt.MasterDefinition);

            if (parameters == null)
                return null;

            var map = new Dictionary<string, DataType>();
            var result = new List<Tuple<DataType, bool>>();

            for (int i = 0; i < dt.FlattenedArguments.Length; i++)
            {
                var at = dt.FlattenedArguments[i];
                var gt = dt.MasterDefinition.FlattenedParameters[i];
                map[gt.Name] = at;
            }

            foreach (var p in parameters)
                result.Add(Tuple.Create(map[p.Item1], p.Item2));

            return result;
        }

        public List<Tuple<string, bool>> GetTemplateParameters(DataType dt)
        {
            if (!dt.IsStruct || !dt.IsFlattenedDefinition)
                return null;

            var types = new Dictionary<string, bool>();

            foreach (var f in dt.Fields)
                if (!f.IsStatic)
                    AddTemplateParameter(types, f.ReturnType, true);

            if (types.Count == 0)
                return null;

            var result = new List<Tuple<string, bool>>();

            if (dt.IsFlattenedDefinition)
                AddTemplateParameters(types, dt.FlattenedParameters, result);

            return result;
        }

        public List<Tuple<string, bool>> GetTemplateParameters(Function f)
        {
            var m = f as Method;
            var dt = f.DeclaringType;

            if (!dt.IsFlattenedDefinition &&
                (m == null || !m.IsGenericDefinition))
                return null;

            var types = new Dictionary<string, bool>();
            AddTemplateParameter(types, f.ReturnType);

            foreach (var p in f.Parameters)
                AddTemplateParameter(types, p.Type);

            if (dt.IsStruct && !IsStructMethod(f))
            {
                var tp = GetTemplateParameters(dt);
                if (tp != null)
                    foreach (var e in tp)
                        types.Remove(e.Item1);
            }

            if (types.Count == 0)
                return null;

            var result = new List<Tuple<string, bool>>();

            if (dt.IsFlattenedDefinition)
                AddTemplateParameters(types, dt.FlattenedParameters, result);

            if (m != null && m.IsGenericDefinition)
                AddTemplateParameters(types, m.GenericParameters, result);

            return result;
        }

        void AddTemplateParameters(
            Dictionary<string, bool> types,
            IEnumerable<GenericParameterType> parameters,
            List<Tuple<string, bool>> result)
        {
            foreach (var p in parameters)
            {
                bool strong;
                if (types.TryGetValue(p.Name, out strong))
                {
                    result.Add(Tuple.Create(p.Name, strong));
                    types.Remove(p.Name);
                }
            }
        }

        void AddTemplateParameter(Dictionary<string, bool> types, DataType dt, bool strong = false)
        {
            if (dt.IsGenericParameter && !dt.IsReferenceType)
                types[dt.Name] = strong;
            else if (dt.IsStruct && dt.IsFlattenedParameterization)
                foreach (var f in dt.Fields)
                    AddTemplateParameter(types, f.ReturnType, true);
        }

        public bool IsTemplate(DataType dt)
        {
            switch (dt.TypeType)
            {
                case TypeType.GenericParameter:
                    return !dt.IsReferenceType;
                default:
                    return GetTemplateParameters(dt.MasterDefinition) != null;
            }
        }

        public bool IsTemplate(Function f)
        {
            return GetTemplateParameters(f.MasterDefinition) != null;
        }

        public bool IsConstrained(DataType dt)
        {
            return !dt.IsClosed && IsTemplate(dt);
        }

        public bool IsConstrained(Field f)
        {
            return IsTemplate(f.ReturnType) || !f.IsStatic && IsConstrained(f.DeclaringType);
        }

        public override bool IsConstrained(Function f)
        {
            if (f.DeclaringType.TypeType == TypeType.Delegate)
                return f.ReturnType.IsValueType || IsConstrained(f.ReturnType);

            foreach (var p in f.Parameters)
                if (IsConstrained(p.Type))
                    return true;

            return IsTemplate(f) || !f.IsStatic && IsConstrained(f.DeclaringType) || IsConstrained(f.ReturnType);
        }

        public bool IsOpaque(DataType dt)
        {
            switch (dt.TypeType)
            {
                case TypeType.Enum:
                case TypeType.Delegate:
                case TypeType.Interface:
                    return true;
                default:
                    return Environment.HasProperty(dt, "TypeName") ||
                        dt.HasAttribute(Essentials.TargetSpecificTypeAttribute);
            }
        }

        public override bool PassByRef(Function f)
        {
            return f.IsVirtual || f.IsConstructor || f.IsFinalizer ||
                f.IsMethod && ((Method)f).ImplementedMethod != null ||
                IsConstrained(f);
        }

        public ReferenceType GetReferenceType(Field f)
        {
            var result = GetReferenceType(f.ReturnType);
            return result == ReferenceType.Strong &&
                f.HasAttribute(Essentials.WeakReferenceAttribute)
                    ? ReferenceType.Weak
                    : result;
        }

        public ReferenceType GetReferenceType(DataType dt)
        {
            var data = TypeData.Get(dt);

            if (data.Reference.HasValue)
                return data.Reference.Value;

            ReferenceType result = 0;
            if (dt.IsReferenceType)
            {
                if (!dt.HasAttribute(Essentials.TargetSpecificTypeAttribute))
                    result = ReferenceType.Strong;
            }
            else if (dt.IsStruct)
            {
                data.Reference = 0;

                foreach (var f in dt.EnumerateFields())
                    switch (GetReferenceType(f))
                    {
                        case ReferenceType.Weak:
                        case ReferenceType.Strong:
                        case ReferenceType.Struct:
                            result = ReferenceType.Struct;
                            goto RETURN;
                    }
            }
        RETURN:
            data.Reference = result;
            return result;
        }

        public override bool IsReserved(string id)
        {
            // Patterns reserved for exclusive use by non-generated code:
            //     1. u(A-Z)*!(0-9)
            //     2. U_*!(0-9)
            //     3. __*!(0-9)
            return _keywords.Contains(id) ||
                id.Length > 1 && !char.IsNumber(id[id.Length - 1]) && (
                    id[0] == 'u' && char.IsUpper(id[1]) ||
                    id[0] == 'U' && id[1] == '_' ||
                    id[0] == '_' && id[1] == '_'
                );
        }
    }
}
