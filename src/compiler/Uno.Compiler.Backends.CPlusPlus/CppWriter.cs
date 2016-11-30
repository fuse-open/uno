using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Uno.Compiler.API.Backends.Decompilers;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Backends.CPlusPlus
{
    public class CppWriter : SourceWriter
    {
        readonly Dictionary<string, int> _strings;
        readonly Dictionary<DataType, int> _fileTypes;
        readonly Dictionary<DataType, int> _funcTypes = new Dictionary<DataType, int>();
        readonly Dictionary<DataType, int> _typeTypes = new Dictionary<DataType, int>();
        readonly CppBackend _backend;
        BodyFlags _flags;
        CppType _type;

        int _finallyCount;

        bool IsClassMember => _flags.HasFlag(BodyFlags.ClassMember);
        bool IsExtension => _flags.HasFlag(BodyFlags.Extension);
        bool IsInline => _flags.HasFlag(BodyFlags.Inline);
        bool IsReturnByRef => _flags.HasFlag(BodyFlags.ReturnByRef);

        public CppWriter(CppBackend backend, string filename
            , Dictionary<string, int> strings
            , Dictionary<DataType, int> types)
            : base(backend, filename)
        {
            _backend = backend;
            _strings = strings;
            _fileTypes = types;
        }

        public CppWriter(CppBackend backend, StringBuilder sb, Function context)
            : base(backend, sb, context)
        {
            _backend = backend;
        }

        public void DeclareGlobals()
        {
            if (_strings != null && _strings.Count > 0)
                WriteLine("static uString* STRINGS[" + _strings.Count + "];");
            if (_fileTypes != null && _fileTypes.Count > 0)
                WriteLine("static uType* TYPES[" + _fileTypes.Count + "];");
        }

        public void BeginNamespace(IReadOnlyList<string> namespaces)
        {
            Skip();
            WriteLine("namespace " + CppBackend.RootNamespace + "{");

            foreach (var ns in namespaces)
                WriteLine("namespace " + ns + "{");

            Skip();
        }

        public void EndNamespace(IReadOnlyList<string> namespaces)
        {
            Skip();
            WriteLine(new string('}', namespaces.Count + 1) + " // ::" + CppBackend.RootNamespace + (
                    namespaces.Count > 0
                        ? "::" + string.Join("::", namespaces)
                        : null
                ));
            Skip();
        }

        public void BeginType(CppType type, DataType dt)
        {
            _type = type;
            WriteComment(dt);
            WriteLine("// {");
            DisableSkip();
        }

        public void EndType()
        {
            _type = null;
            DisableSkip();
            WriteLine("// }");
            Skip();
        }

        void BeginMethod(Function f, BodyFlags flags)
        {
            _finallyCount = 0;
            _flags = flags;
            SetContext(f);
            Write(" { ");

            if (!f.ReturnType.IsVoid && !flags.HasFlag(BodyFlags.ReturnByRef))
                Write(_backend.GetTypeName(f.ReturnType, f.DeclaringType, true) +
                      " __retval; return ");
        }

        void EndMethod(Function f)
        {
            var comma = !f.IsStatic;

            if (HasTypeParameter(f))
            {
                CommaWhen(comma);
                Write("__type");
                comma = true;
            }

            foreach (var p in f.Parameters)
            {
                CommaWhen(comma);
                Write(IsReturnByRef
                        ? p.Name :
                    IsConstrained(p.Type)
                        ? "uConstrain(" + GetTypeOf(p.Type) + ", " + p.Name + ")" :
                    !p.IsReference && !p.Type.IsReferenceType
                        ? "&" + p.Name
                        : p.Name);
                comma = true;
            }

            if (!f.ReturnType.IsVoid)
            {
                CommaWhen(f.Parameters.Length > 0 || !f.IsStatic);
                Write(IsReturnByRef
                    ? "__retval)"
                    : "&__retval), __retval");
            }
            else
                Write(")");

            EndLine("; }");
            _flags = 0;
        }

        public override void BeginReturn(Expression value = null)
        {
            Write("return");

            if (Function.ReturnType.IsVoid)
                return;

            Write(IsClassMember
                    ? " " :
                IsConstrained(Function.ReturnType)
                    ? value.HasStorage()
                        ? " __retval.Store("
                        : " __retval.Store(" + GetTypeOf(Function.ReturnType) + ", "
                    : " *__retval = ");
        }

        public override void EndReturn(Expression value = null)
        {
            if (Function.ReturnType.IsVoid || IsClassMember)
                return;

            End(IsConstrained(Function.ReturnType));
            Write(", void()");
        }

        public void WriteComment(DataType dt)
        {
            Skip();
            BeginLine("// " + dt.Modifiers.ToLiteral() + " " + dt.TypeType.ToString().ToLower() + " ");

            if (dt.IsDelegate)
                Write(dt.ReturnType + " ");

            WriteUnoName(dt);

            if (dt.IsDelegate)
                Write(dt.Parameters.BuildString("(", ")", true));

            EndLine();
        }

        public void WriteComment(Function f, string text = null)
        {
            Skip();
            BeginLine("// " + f.Prototype.Modifiers.ToLiteral(true) + (
                    f.IsFinalizer
                        ? "~" + f.Prototype.DeclaringType.UnoName :
                    f.Prototype.IsOperator
                        ? "operator " + f.Prototype.OperatorString :
                    f.Prototype.IsCast
                        ? "operator " + f.Prototype.ReturnType :
                    f.Prototype.IsConstructor
                        ? (f.ReturnType.IsVoid
                            ? f.Prototype.DeclaringType.UnoName
                            : f.Prototype.DeclaringType.UnoName + " New")
                        : f.Prototype.ReturnType + " " + f.Prototype.UnoName
                ));

            if (f.IsGenericMethod)
            {
                Write("<");
                var first = true;
                foreach (var p in ((Method) f).GenericParameters)
                {
                    CommaWhen(!first);
                    Write(p.UnoName);
                    first = false;
                }
                Write(">");
            }

            EndLine(f.Prototype.Parameters.BuildString("(", ")", true) +
                (text != null ? " [" + text + "]" : null));
        }

        public void WriteUnoName(DataType dt)
        {
            if (dt.IsNestedType)
            {
                WriteUnoName(dt.ParentType);
                Write(".");
            }

            Write(dt.UnoName);

            if (dt.IsGenericDefinition)
            {
                Write("<");
                var first = true;
                foreach (var p in dt.GenericParameters)
                {
                    CommaWhen(!first);
                    Write(p.UnoName);
                    first = false;
                }
                Write(">");
            }
        }

        public void WriteTemplate(DataType dt)
        {
            var gp = _backend.GetTemplateParameters(dt);
            if (gp != null)
                WriteLine(_backend.GetTemplateHeader(gp));
        }

        public bool WriteTemplate(Function f)
        {
            var gp = _backend.GetTemplateParameters(f);
            if (gp != null)
            {
                WriteLine(_backend.GetTemplateHeader(gp));
                return true;
            }
            return false;
        }

        public void WriteInstanceMethodBody(Function f, BodyFlags flags = 0)
        {
            if (flags.HasFlag(BodyFlags.Inline))
            {
                BeginMethod(f, flags | (
                        _type.IsOpaque
                            ? BodyFlags.Extension
                            : BodyFlags.ClassMember
                    ));
                Write((f.IsVirtual && !f.DeclaringType.IsValueType
                            ? "(" + (f.DeclaringType != Essentials.Object
                                        ? "((" + _backend.GetTypeOfType(f.DeclaringType, f.DeclaringType) + "*)"
                                        : null
                                ) + (
                                    _type.IsOpaque
                                        ? "__this->" :
                                    HasTypeParameter(f)
                                        ? "this->"
                                        : null
                                ) +
                                "__type" +
                                (f.DeclaringType != Essentials.Object ? ")" : null) +
                                "->fp_" + f.Name + ")("
                            : _backend.GetFunctionPointer(f, f.DeclaringType) + "("
                    ) + (f.IsStatic
                            ? null :
                        _type.IsOpaque
                            ? (f.DeclaringType.IsValueType
                                    ? "&__this"
                                    : "__this"
                                )
                            : "this"
                    ));
                EndMethod(f);
            }
            else
                WriteMethodBody(f, BodyFlags.ClassMember);
        }

        public void WriteStaticMethodBody(Function f, BodyFlags flags = 0)
        {
            if (flags.HasFlag(BodyFlags.Inline))
            {
                BeginMethod(f, flags);
                Write(_backend.GetFunctionPointer(f, f.DeclaringType) + "(" + (
                        f.IsStatic
                            ? null :
                        f.DeclaringType.IsValueType
                            ? "&__this"
                            : "__this"
                    ));
                EndMethod(f);
            }
            else
                WriteMethodBody(f, BodyFlags.ClassMember);
        }

        public void WriteInterfaceMethodBody(Function f, BodyFlags flags = 0)
        {
            BeginMethod(f, flags);
            Write("__this.VTable" +
                  GetTemplateString(_backend.GetStaticName(f.DeclaringType, f.DeclaringType)) +
                  "()->fp_" + f.Name + "(__this");
            EndMethod(f);
        }

        public void WriteMethodBody(Function f, BodyFlags flags = 0)
        {
            _flags = flags;
            if (flags.HasFlag(BodyFlags.ClassMember) && _backend.IsExtensionMethod(f))
                _flags |= BodyFlags.Extension;

            SetContext(f);
            BeginScope();

            if (f.IsGenericMethod && (f.IsVirtual || f.Stats.HasFlag(EntityStats.ImplementsInterface)))
                WriteLine("__type = " +
                    _backend.GetTypeOf(f.DeclaringType, Type, null,
                        new TypeCache(_fileTypes),
                        flags) + "->GetVirtual(" + _backend.GetType(f.GenericType).MethodIndex + ", __type);");

            var func = _backend.GetFunction(f);
            if (func.PrecalcedTypes.Count > 0)
            {
                if (f.IsGenericMethod)
                {
                    var gtype = _backend.GetType(f.GenericType);
                    for (int i = 0; i < gtype.PrecalcedTypes.Count; i++)
                        _typeTypes[gtype.PrecalcedTypes[i]] = i;
                }
                else
                    for (int i = 0; i < _type.PrecalcedTypes.Count; i++)
                        _typeTypes[_type.PrecalcedTypes[i]] = i;

                WriteLine("uType* __types[] = {");
                Indent();

                foreach (var e in func.PrecalcedTypes)
                {
                    WriteLine(_backend.GetTypeOf(e, Type, Function,
                            new TypeCache(_fileTypes, null, _typeTypes),
                            flags) + ",");
                    _funcTypes[e] = _funcTypes.Count;
                }

                Unindent();
                WriteLine("};");
            }

            if (_backend.IsStackFrame(f))
                WriteLine("uStackFrame __(\"" + f.DeclaringType + "\", \"" + f.Prototype.NameAndParameterList + "\");");

            if (f == f.DeclaringType.Initializer)
                foreach (var dt in func.Dependencies)
                    WriteLine(GetTypeOf(dt) + "->Init();");
            else if (f.IsStatic && !f.Prototype.IsConstructor && f.DeclaringType.HasInitializer)
                WriteLine(GetTypeOf(f.DeclaringType) + "->Init();");

            foreach (var v in func.Constrained)
                WriteLine("uT " + v.Name + "(" + GetTypeOf(v.ValueType) + ", alloca(" + GetTypeOf(v.ValueType) + "->ValueSize));");

            WriteFunctionBody(f, false);
            EndScope();

            _typeTypes.Clear();
            _funcTypes.Clear();
            _flags = 0;
        }

        public override bool HasVariable(DataType dt, Expression optionalValue)
        {
            return !IsConstrained(dt) || optionalValue != null;
        }

        public override void WriteVariable(Variable var)
        {
            var def = GetTypeDef(var.ValueType);

            var isConstrained = IsConstrained(var.ValueType);

            if (!isConstrained)
                Write(def + " " + var.Name);
            else if (var.OptionalValue != null)
                Write(var.Name);

            if (var.OptionalValue != null)
            {
                Write(Assign);
                WriteExpression(var.OptionalValue);
            }

            for (var = var.Next; var != null; var = var.Next)
            {
                if (!HasVariable(var.ValueType, var.OptionalValue))
                    continue;

                Write(Comma +
                    (def.EndsWith('*') && !isConstrained
                        ? "*"
                        : null) +
                    var.Name);

                if (var.OptionalValue != null)
                {
                    Write(Assign);
                    WriteExpression(var.OptionalValue);
                }
            }
        }

        string EscapeByte(byte b)
        {
            switch ((char)b)
            {
                case '\"': return "\\\"";
                case '\\': return "\\\\";
                case '\0': return "\\300\\200"; // C0 80, MUTF-8 encoded U+0000
                case '\a': return "\\a";
                case '\b': return "\\b";
                case '\f': return "\\f";
                case '\n': return "\\n\"\n" + IndentString + "    \"";
                case '\r': return "\\r";
                case '\t': return "\\t";
                case '\v': return "\\v";
                default: return
                    b >= 128 || char.IsControl((char)b)
                        ? "\\" + Convert.ToString(b, 8).PadLeft(3, '0')
                        : ((char)b).ToString(CultureInfo.InvariantCulture);
            }
        }

        public string EscapeString(string s)
        {
            var ret = new StringBuilder();
            foreach (var b in Encoding.UTF8.GetBytes(s))
                ret.Append(EscapeByte(b));
            return ret.ToString();
        }

        static string StringComment(string s)
        {
            if (s == null)
                return "/*null*/";

            s = s.ToLiteral();

            if (s.Length > 15)
                s = s.Substring(0, 12) + "...";

            return "/*" + s.Replace("*/", "*\\/") + "*/";
        }

        public override void WriteConstant(Constant s, ExpressionUsage u)
        {
            if (s.Value == null)
                WriteDefault(s.Source, s.ReturnType, u);
            else switch (s.ReturnType.BuiltinType)
            {
                case BuiltinType.String:
                {
                    int index;
                    var v = (string) s.Value;
                    if (_strings != null && _strings.TryGetValue(v, out index))
                        Write("::STRINGS[" + index + StringComment(v) + "]");
                    else
                        Write("uString::Const(\"" + EscapeString(v) + "\")");
                    break;
                }
                case BuiltinType.Char:
                {
                    var v = (char) s.Value;
                    if (v < 32 || v > 126)
                        Write(((int) v).ToLiteral());
                    else
                        Write(v.ToLiteral());
                    break;
                }
                case BuiltinType.Float:
                {
                    var v = (float) s.Value;
                    if (float.IsNaN(v))
                        Write("FLT_NAN");
                    else if (float.IsPositiveInfinity(v))
                        Write("FLT_INF");
                    else if (float.IsNegativeInfinity(v))
                        Write("-FLT_INF");
                    else
                        base.WriteConstant(s, u);
                    break;
                }
                case BuiltinType.Double:
                {
                    var v = (double) s.Value;
                    if (double.IsNaN(v))
                        Write("DBL_NAN");
                    else if (double.IsPositiveInfinity(v))
                        Write("DBL_INF");
                    else if (double.IsNegativeInfinity(v))
                        Write("-DBL_INF");
                    else
                        base.WriteConstant(s, u);
                    break;
                }
                case BuiltinType.Int:
                {
                    var v = (int) s.Value;
                    if (v == int.MinValue)
                        Write("(-" + int.MaxValue + " - 1)");
                    else
                        base.WriteConstant(s, u);
                    break;
                }
                case BuiltinType.UInt:
                {
                    Write(s.Value + "U");
                    break;
                }
                case BuiltinType.Long:
                {
                    var v = (long) s.Value;
                    if (v == long.MinValue)
                        Write("(-" + long.MaxValue + "LL - 1)");
                    else
                        Write(s.Value + "LL");
                    break;
                }
                case BuiltinType.ULong:
                {
                    Write(s.Value + "ULL");
                    break;
                }
                default:
                {
                    base.WriteConstant(s, u);
                    break;
                }
            }
        }

        void WriteThis(DataType dt, ExpressionUsage u)
        {
            if (IsExtension)
                Write("__this");
            else
            {
                var isRefType = dt.IsReferenceType;
                var isObject = u.IsObject();
                Begin(!isRefType && isObject);
                WriteWhen(!isRefType, "*");
                Write(IsClassMember ? "this" : "__this");
                End(!isRefType && isObject);
            }
        }

        public override void WriteThis(This s, ExpressionUsage u)
        {
            WriteThis(s.ReturnType, u);
        }

        public override void WriteBase(Base s, ExpressionUsage u)
        {
            if (Function != null && Type.IsValueType)
                WriteBox(s.Source, s.ReturnType, new This(s.Source, Type), true, u);
            else
                WriteThis(s.ReturnType, u);
        }

        public override void WriteStaticBase(Source src, DataType t, string member)
        {
            WriteStaticType(src, t);
            Write("::" + member);
        }

        public override void WriteMemberBase(Expression s, DataType declType = null, string member = null)
        {
            if ((s.ActualValue is This || s.ActualValue is Base) &&
                (declType == null || s.ReturnType.Name != member))
            {
                WriteWhen(!IsClassMember || IsExtension, "__this->");
                WriteWhen(member != null, member);
            }
            else if (s is AddressOf && !s.ReturnType.IsReferenceType)
            {
                WriteExpression(s.ActualValue, ExpressionUsage.Object);
                Write("." + member);
            }
            else if (declType != null && s.ReturnType.Name == member)
            {
                Write("((");
                WriteType(s.Source, declType);
                Write(")");
                WriteExpression(s);
                Write(")->" + member);
            }
            else if (s.ReturnType.IsValueType && declType != null && declType.IsReferenceType)
            {
                WriteBox(s.Source, declType, s.ActualValue, true, ExpressionUsage.Object);
                Write("->" + member);
            }
            else
            {
                WriteExpression(s, ExpressionUsage.Object);
                Write("->" + member);
            }
        }

        void WriteObject(Expression obj, DataType declType = null, bool isExtension = false)
        {
            if (declType != null && declType.IsInterface)
            {
                Write("uInterface(");
                if (!obj.ReturnType.IsReferenceType)
                    WriteBox(obj.ActualValue);
                else
                    WriteExpression(obj.ActualValue);
                Write(", " + GetTypeOf(declType) + ")");
            }
            else if (declType == Essentials.Object && !obj.ReturnType.IsReferenceType)
                WriteBox(obj.ActualValue);
            else
                WriteExpression(isExtension ? obj.ActualValue : obj);
        }

        void WriteArgument(Expression arg, Parameter p, Parameter master = null, bool isConstrained = false, ExpressionUsage u = ExpressionUsage.Argument)
        {
            if (isConstrained && !p.IsReference && p.Type.IsValueType && !IsConstrained(p.Type))
            {
                if (arg is LoadArgument && _backend.PassByRef(Function) &&
                    !IsClassMember && !IsExtension)
                    WriteExpression(new AddressOf(arg));
                else
                {
                    Write("uCRef" + (
                            arg.ReturnType.IsIntegralType
                                ? GetTemplateString(arg.ReturnType)
                                : null
                        ) + "(");
                    WriteExpression(arg);
                    Write(")");
                }
            }
            else if (master != null && p.IsReference && p.Type.IsReferenceType &&
                !p.Type.IsGenericParameter && master.Type.IsGenericParameter)
            {
                Write("(" + GetTypeName(master.Type) + "*)");
                WriteExpression(arg, ExpressionUsage.Operand);
            }
            else
                WriteExpression(arg, u);
        }

        void WriteArguments(Function func, Expression obj, Expression[] args, Variable storage, ExpressionUsage u, CallFlags flags = 0)
        {
            var comma = false;
            Write("(");

            if (obj != null)
            {
                WriteObject(obj, func.DeclaringType, flags.HasFlag(CallFlags.Extension));
                comma = true;
            }

            if (func.IsGenericMethod)
            {
                CommaWhen(comma);
                Write(GetTypeOf(func.GenericType));
                comma = true;
            }
            else if (HasTypeParameter(func))
            {
                CommaWhen(comma);
                Write(GetTypeOf(func.DeclaringType));
                comma = true;
            }

            for (int i = 0; i < args.Length; i++)
            {
                CommaWhen(comma);
                WriteArgument(args[i], func.Parameters[i], func.MasterDefinition.Parameters[i], flags.HasFlag(CallFlags.Constrained));
                comma = true;
            }

            if (storage != null)
            {
                CommaWhen(comma);
                Write("&" + storage.Name + ")");
                WriteWhen(u > 0, ", " + (
                        u == ExpressionUsage.VarArg &&
                        IsConstrained(storage.ValueType)
                            ? "(void*)"
                            : null
                    ) + storage.Name + ")");
            }
            else
                Write(")");
        }

        public void WriteCall(Source src, Function func, Expression obj, Expression[] args, Variable storage = null, ExpressionUsage u = ExpressionUsage.Argument)
        {
            Begin(storage != null && u != 0);
            var cast = storage == null &&
                       u >= ExpressionUsage.Argument &&
                       func.MasterDefinition.ReturnType.IsGenericType &&
                       func.MasterDefinition.ReturnType.IsReferenceType &&
                       func.ReturnType != func.MasterDefinition.ReturnType;

            if (!cast && storage == null && u == ExpressionUsage.VarArg)
                switch (GetReferenceType(func.ReturnType))
                {
                    case ReferenceType.Strong:
                    case ReferenceType.Weak:
                        cast = true;
                        break;
                    default:
                        if (func.MasterDefinition.ReturnType.IsGenericParameter && func.ReturnType.IsReferenceType)
                        {
                            var p = _backend.GetTemplateParameters(func.DeclaringType);
                            if (p != null)
                                foreach (var e in p)
                                    if (e.Item1 == func.ReturnType.Name)
                                    { cast = true; break; }
                        }
                        break;
                }

            if (cast)
            {
                Begin(u);
                Write("(" + GetTypeName(func.ReturnType) + ")");
            }

            SourceValue intrinsicName;
            if (Environment.TryGetValue(func, "FunctionName", out intrinsicName))
            {
                Write(intrinsicName.String);
                WriteArguments(func, obj, args, storage, u);
            }
            else if (obj is Base && func.IsVirtual && (storage != null || func.ReturnType.IsVoid))
            {
                Write(GetFunctionPointer(func));
                WriteArguments(func, obj, args, storage, u, CallFlags.Constrained);
            }
            else if (IsConstrained(func))
            {
                if (_backend.IsExtensionMethod(func))
                {
                    Write(GetStaticName(func.DeclaringType) + "::" +
                          GetMemberName(func, func));
                    WriteArguments(func, obj, args, storage, u, CallFlags.Constrained | CallFlags.Extension);
                }
                else if (func.IsVirtual)
                {
                    WriteMemberBase(src, func.DeclaringType, obj, GetMemberName(func, func));
                    WriteArguments(func, null, args, storage, u, CallFlags.Constrained);
                }
                else
                {
                    Write(GetFunctionPointer(func));
                    WriteArguments(func, obj, args, storage, u, CallFlags.Constrained);
                }
            }
            else if (storage != null)
            {
                Write(GetFunctionPointer(func));
                WriteArguments(func, obj, args, storage, u, CallFlags.Constrained);
            }
            else if (_backend.IsExtensionMethod(func))
            {
                Write(GetStaticName(func.DeclaringType) + "::" +
                        GetMemberName(func, func));
                WriteArguments(func, obj, args, null, u, CallFlags.Extension);
            }
            else if (_backend.IsStructMethod(func))
            {
                WriteStaticType(src, func.DeclaringType);
                Write("__" + GetMemberName(func, func));
                WriteArguments(func, null, args, null, u);
            }
            else
            {
                WriteMemberBase(src, func.DeclaringType, obj, GetMemberName(func, func));
                WriteArguments(func, null, args, null, u);
            }

            EndWhen(cast, u);
        }

        public override void WriteCallMethod(Source src, Method method, Expression obj, Expression[] args, Variable ret, ExpressionUsage u)
        {
            WriteCall(src, method, obj, args, ret, u);
        }

        public override void WriteGetProperty(Source src, Property property, Expression obj, Expression[] args, Variable ret, ExpressionUsage u)
        {
            WriteCall(src, property.GetMethod, obj, args, ret, u);
        }

        public override void WriteSetProperty(Source src, Property property, Expression obj, Expression[] args, Expression value, Variable ret, ExpressionUsage u)
        {
            WriteCall(src, property.SetMethod, obj, args.Concat(value), ret, u);
        }

        public override void WriteAddListener(AddListener s, ExpressionUsage u)
        {
            WriteCall(s.Source, s.Event.AddMethod, s.Object, new[] { s.Listener }, s.Storage, u);
        }

        public override void WriteRemoveListener(RemoveListener s, ExpressionUsage u)
        {
            WriteCall(s.Source, s.Event.RemoveMethod, s.Object, new[] { s.Listener }, s.Storage, u);
        }

        public override void WriteTypeOf(Source src, DataType dt, ExpressionUsage u = ExpressionUsage.Argument)
        {
            WriteWhen(u.IsOperand(), "(uType*)");
            Write(GetTypeOf(dt));
        }

        public override void WriteConditionalOp(ConditionalOp s, ExpressionUsage u)
        {
            // Find out why we use ExpressionUsage.VarArg by looking at WriteLoadField() and WriteLoadElement()
            Begin(u);
            WriteExpression(s.Condition, ExpressionUsage.Operand);
            Write(QuestionMark);
            WriteExpression(s.True, ExpressionUsage.VarArg);
            Write(Colon);
            WriteExpression(s.False, ExpressionUsage.VarArg);
            End(u);
        }

        public override void WriteDefault(Source src, DataType dt, ExpressionUsage u)
        {
            Write(_backend.GetDefaultValue(dt, Type, Function, !IsInline ? (TypeCache?)new TypeCache(_fileTypes, _funcTypes, _typeTypes) : null, u));
        }

        public override void WriteType(Source src, DataType dt)
        {
            Write(GetTypeDef(dt));
        }

        public override void WriteStaticType(Source src, DataType dt)
        {
            Write(GetStaticName(dt) + (
                    _backend.IsTemplate(dt)
                        ? GetTemplateSuffix(dt)
                        : null
                ));
        }

        public override void WriteNewObject(Source src, DataType dt, ExpressionUsage u)
        {
            Write("(" + GetTypeName(dt) + ")uNew(" + GetTypeOf(dt) + ")");
        }

        public override void WriteNewObject(Source src, Constructor ctor, Expression[] args, ExpressionUsage u = ExpressionUsage.Argument)
        {
            if (!ctor.DeclaringType.IsGenericParameter || args.Length > 0)
                Log.Error(src, ErrorCode.I0000, "Unexpected NewObject expression " + ctor.Quote());

            Write("(" + GetTypeName(ctor.DeclaringType.Base) + ")" + GetTypeOf(ctor.DeclaringType) + "->New()");
        }

        public override void WriteNewDelegate(NewDelegate s, ExpressionUsage u)
        {
            Write("uDelegate::New(" + GetTypeOf(s.ReturnType));

            if (!s.Method.IsVirtual || s.Object is Base)
            {
                Write(", (void*)" + GetFunctionPointer(s.Method));

                if (s.Object != null || HasTypeParameter(s.Method))
                {
                    Write(", ");
                    WriteObject(s.Object ?? new Constant(s.Source, Essentials.Object, null));
                }
            }
            else if (s.Method.DeclaringType.IsInterface)
            {
                Write(", ");
                WriteObject(s.Object, s.Method.DeclaringType, true);
                Write(", offsetof(" + GetStaticName(s.Method.DeclaringType) + ", fp_" + s.Method.Name + ")");
            }
            else
            {
                Write(", ");
                WriteExpression(s.Object);
                Write(", offsetof(" + GetTypeOfType(s.Method.DeclaringType) + ", fp_" + s.Method.Name + ")");
            }

            if (HasTypeParameter(s.Method))
                Write(", " + GetTypeOf(s.Method.GenericType ?? s.Method.DeclaringType));

            Write(")");
        }

        public override void WriteCallDelegate(Source src, Expression obj, Expression[] args, Variable ret, ExpressionUsage u)
        {
            var dt = (DelegateType)obj.ReturnType;

            if (dt.ReturnType.IsVoid && args.Length <= 1)
            {
                WriteExpression(obj, ExpressionUsage.Object);
                Write("->InvokeVoid(");

                if (args.Length == 1)
                    WriteArgument(args[0], dt.Parameters[0], null, true);

                Write(")");
            }
            else
            {
                var p = u != ExpressionUsage.Statement;

                if (ret == null)
                {
                    p = p && dt.ReturnType.IsValueType;

                    if (p)
                        Write("uUnbox" + GetTemplateString(dt.ReturnType) + "(");
                    else if (dt.ReturnType.IsReferenceType && dt.ReturnType != Essentials.Object)
                        Write("(" + GetTypeDef(dt.ReturnType) + ")");
                }
                else
                    WriteWhen(p, "(");

                WriteExpression(obj, ExpressionUsage.Object);
                Write("->Invoke(");

                if (ret != null)
                {
                    Write("&" + ret.Name);
                    WriteWhen(args.Length > 0, ", ");
                }

                WriteWhen(args.Length > 0, args.Length);

                for (int i = 0; i < args.Length; i++)
                {
                    Write(", ");
                    WriteArgument(args[i], dt.Parameters[i], null, true, ExpressionUsage.VarArg);
                }

                Write(")");

                if (p && ret != null)
                    Write(", " + (
                            u == ExpressionUsage.VarArg &&
                            IsConstrained(ret.ValueType)
                                ? "(void*)"
                                : null
                        ) + ret.Name);

                WriteWhen(p, ")");
            }
        }

        public override void WriteIsOp(IsOp s, ExpressionUsage u)
        {
            Write("uIs(");
            if (!s.Operand.ReturnType.IsReferenceType)
                Write(GetTypeOf(s.Operand.ReturnType) + ", ");
            WriteExpression(s.Operand, ExpressionUsage.VarArg);
            Write(", " + GetTypeOf(s.TestType) + ")");
        }

        public override void WriteAsOp(AsOp s, ExpressionUsage u)
        {
            Write("uAs" + GetTemplateString(s.ReturnType) + "(");
            WriteExpression(s.Operand);
            Write(", " + GetTypeOf(s.TestType) + ")");
        }

        string GetTypeOf(DataType dt)
        {
            return _backend.GetTypeOf(dt,
                Type,
                Function,
                !IsInline
                    ? (TypeCache?) new TypeCache(_fileTypes, _funcTypes, _typeTypes)
                    : null,
                _flags);
        }

        public override void WriteNewArray(Source src, ArrayType type, Expression size, ExpressionUsage u)
        {
            Write("uArray::New(" + GetTypeOf(type) + ", ");
            WriteExpression(size);
            Write(")");
        }

        public override void WriteNewArray(Source src, ArrayType type, Expression[] initializers, ExpressionUsage u)
        {
            Write("uArray::Init" + (
                  type.ElementType.IsGenericParameter
                    ? "T"
                    : GetTemplateString(GetVarArgType(type.ElementType))) +
                  "(" + GetTypeOf(type) + ", " + initializers.Length);
            Indent();

            for (int i = 0; i < initializers.Length; i++)
            {
                Write(", ");

                if (i > 0 && (i % 64 == 0))
                {
                    EndLine();
                    BeginLine();
                }

                WriteExpression(initializers[i], ExpressionUsage.VarArg);
            }

            Write(")");
            Unindent();
        }

        DataType GetVarArgType(DataType dt)
        {
            switch (dt.BuiltinType)
            {
                case BuiltinType.Bool:
                case BuiltinType.Char:
                case BuiltinType.Byte:
                case BuiltinType.SByte:
                case BuiltinType.Short:
                case BuiltinType.UShort:
                    // Promote small ints to 'int' when passing through '...'
                    return Essentials.Int;
                case BuiltinType.Float:
                    // Promote floats to 'double' when passing through '...'
                    return Essentials.Double;
                default:
                    return dt;
            }
        }

        void WriteConstrained(Expression s)
        {
            if (s.ReturnType.IsValueType && !_backend.IsTemplate(s.ReturnType))
            {
                Write("uCRef" + (
                        s.ReturnType.IsIntegralType
                            ? GetTemplateString(s.ReturnType)
                            : null
                    ) + "(");
                WriteExpression(s);
                Write(")");
            }
            else
                WriteExpression(s);
        }

        public override void WriteLoadLocal(LoadLocal s, ExpressionUsage u)
        {
            if (u == ExpressionUsage.VarArg && IsConstrained(s.ReturnType))
                Write("(void*)");

            base.WriteLoadLocal(s, u);
        }

        public override void WriteLoadField(Source src, Field field, Expression obj, ExpressionUsage u)
        {
            var cast = u >= ExpressionUsage.Argument &&
                       field.MasterDefinition.ReturnType.IsGenericType &&
                       field.MasterDefinition.ReturnType.IsReferenceType &&
                       field.ReturnType != field.MasterDefinition.ReturnType;

            if (!cast && u == ExpressionUsage.VarArg)
                switch (GetReferenceType(field))
                {
                    case ReferenceType.Strong:
                    case ReferenceType.Weak:
                        cast = true;
                        break;
                    default:
                        cast = IsConstrained(field) && !IsConstrained(field.ReturnType) ||
                            !_backend.IsTemplate(field.DeclaringType) && IsConstrained(field.MasterDefinition.ReturnType);
                        break;
                }

            if (cast)
            {
                Begin(u);
                Write("(" + GetTypeName(field.ReturnType) + ")");
            }
            else if (u == ExpressionUsage.VarArg && IsConstrained(field.ReturnType))
                Write("(void*)");

            if (IsConstrained(field))
            {
                if (obj is AddressOf &&
                        IsConstrained(obj.ReturnType) &&
                        obj.ActualValue.HasStorage())
                {
                    WriteExpression(obj.ActualValue, ExpressionUsage.Object);
                    Write("[" + _backend.GetIndex(field) + "/*" + field.UnoName + "*/]");
                }
                else
                {
                    if (!field.IsStatic && field.DeclaringType.IsReferenceType)
                    {
                        WriteMemberBase(obj, field.DeclaringType, field.Name);
                        Write("()");
                    }
                    else
                    {
                        Write(GetTypeOf(field.DeclaringType) + "->Field(");

                        if (!field.IsStatic)
                        {
                            WriteConstrained(obj);
                            CommaWhen(true);
                        }

                        Write(_backend.GetIndex(field) + "/*" + field.UnoName + "*/)");
                    }
                }

                if (!IsConstrained(field.ReturnType))
                    Write("." + (
                            GetReferenceType(field.ReturnType) == ReferenceType.Strong
                                ? "Strong"
                                : "Value"
                        ) + GetTemplateString(field.ReturnType) + "()");
            }
            else
            {
                if (field.IsStatic)
                    Write(GetStaticName(field));
                else
                    base.WriteLoadField(src, field, obj, u);

                if (!_backend.IsTemplate(field.DeclaringType) && IsConstrained(field.MasterDefinition.ReturnType) ||
                    field.IsStatic && !field.DeclaringType.MasterDefinition.IsClosed)
                    Write("(" + (
                            field.IsStatic
                                ? GetTypeOf(field.DeclaringType)
                                : null
                        ) + ")." + (
                            GetReferenceType(field.ReturnType) == ReferenceType.Strong
                                ? "Strong"
                                : "Value"
                        ) +
                        GetTemplateString(field.ReturnType) + "()");
                else if (field.IsStatic)
                    Write(field.CanUsePointerDirectly() ||
                        field.DeclaringType == Function.DeclaringType
                            ? "_"
                            : "()");
            }

            EndWhen(cast, u);
        }

        public override void WriteStoreField(Source src, Field field, Expression obj, Expression value, ExpressionUsage u)
        {
            if (u == ExpressionUsage.VarArg)
            {
                if (IsConstrained(field.ReturnType))
                {
                    Write("(void*)");
                    u = ExpressionUsage.Object; // for parens
                }
                else switch (GetReferenceType(field))
                {
                    case ReferenceType.Strong:
                    case ReferenceType.Weak:
                        Write("(" + GetTypeName(field.ReturnType) + ")");
                        u = ExpressionUsage.Object; // for parens
                        break;
                }
            }

            if (IsConstrained(value.ReturnType) && value is Default)
            {
                WriteLoadField(src, field, obj, 0);
                Write(".Default()");
            }
            else
            {
                Begin(u);
                WriteLoadField(src, field, obj, 0);
                Write(Assign);
                WriteExpression(value, ExpressionUsage.Operand);
                End(u);
            }
        }

        public override void WriteLoadElement(Source src, DataType elementType, Expression array, Expression index, ExpressionUsage u)
        {
            if (u == ExpressionUsage.VarArg)
                Write(IsConstrained(elementType)
                        ? "(void*)" :
                    GetReferenceType(elementType) == ReferenceType.Strong
                        ? "(" + GetTypeName(elementType) + ")"
                        : null);

            WriteMemberBase(array);
            Write(IsConstrained(elementType)
                    ? "TItem(" :
                GetReferenceType(elementType) == ReferenceType.Strong
                    ? "Strong" + GetTemplateString(elementType) + "("
                    : "Item" + GetTemplateString(elementType) + "("
                );
            WriteExpression(index);
            Write(")");
        }

        public override void WriteStoreElement(Source src, DataType elementType, Expression array, Expression index, Expression value, ExpressionUsage u)
        {
            if (IsConstrained(value.ReturnType) && value is Default)
            {
                WriteLoadElement(src, elementType, array, index, 0);
                Write(".Default()");
            }
            else
            {
                if (u == ExpressionUsage.VarArg)
                {
                    Write(IsConstrained(elementType)
                            ? "(void*)" :
                        GetReferenceType(elementType) == ReferenceType.Strong
                            ? "(" + GetTypeName(elementType) + ")"
                            : null);
                    u = ExpressionUsage.Object; // for parens
                }

                Begin(u);
                WriteLoadElement(src, elementType, array, index, 0);
                Write(Assign);
                WriteExpression(value);
                End(u);
            }
        }

        public override void WriteLoadArgument(LoadArgument s, ExpressionUsage u)
        {
            if (!s.Parameter.IsReference && (
                s.Parameter.Type.IsReferenceType ||
                IsClassMember ||
                IsConstrained(s.Parameter.Type)
                ))
            {
                bool load = u == ExpressionUsage.VarArg && s.Parameter.IsReference &&
                            IsConstrained(s.Parameter.Type);
                WriteWhen(load, "(void*)");
                Write(s.Parameter.Name + (load
                    ? ".Load(" + GetTypeOf(s.Parameter.Type) + ")"
                    : null
                    ));
            }
            else
            {
                var isObject = u.IsObject();
                Begin(isObject);
                Write("*");
                Write(s.Parameter.Name);
                End(isObject);
            }
        }

        public override void WriteStoreArgument(StoreArgument s, ExpressionUsage u)
        {
            if (s.Parameter.IsReference &&
                    IsConstrained(s.Parameter.Type) &&
                    !IsClassMember)
            {
                if (s.Value is Default)
                    Write(s.Parameter.Name + ".Default(" +
                        GetTypeOf(s.Parameter.Type) + ")");
                else
                {
                    Write(s.Parameter.Name + ".Store(");

                    if (!s.Value.HasStorage())
                        Write(GetTypeOf(s.Parameter.Type) + ", ");

                    WriteExpression(s.Value);
                    Write(")");
                }
            }
            else
            {
                Begin(u);
                WriteWhen(!IsClassMember && !s.Parameter.Type.IsReferenceType || s.Parameter.IsReference, "*");
                Write(s.Parameter.Name + Assign);
                WriteExpression(s.Value);
                End(u);
            }
        }

        public override void WriteStoreLocal(StoreLocal s, ExpressionUsage u)
        {
            if (IsConstrained(s.ReturnType) && s.Value is Default)
                Write(s.Variable.Name + ".Default()");
            else
                base.WriteStoreLocal(s, u);
        }

        public override void WriteThrow(Throw s)
        {
            if (s.IsRethrow)
                WriteLine("throw __t;");
            else
            {
                BeginLine("U_THROW(");
                WriteExpression(s.Exception);
                EndLine(");");
            }
        }

        public override void WriteTryCatchFinally(TryCatchFinally s)
        {
            Skip();

            if (s.OptionalFinallyBody != null)
                BeginScope();

            WriteLine("try");
            BeginScope();
            WriteScope(s.TryBody, true, false);
            EndScope();

            WriteLine("catch (const uThrowable& __t)");
            BeginScope();

            if (s.OptionalFinallyBody != null)
                WriteScope(s.OptionalFinallyBody, true, false);

            if (s.CatchBlocks.Length == 1 && s.CatchBlocks[0].Exception.ValueType == Essentials.Exception)
            {
                var c = s.CatchBlocks[0];
                var et = GetTypeDef(c.Exception.ValueType);
                WriteLine(et + " " + c.Exception.Name + " = __t.Exception;");
                WriteScope(c.Body, false, false);
            }
            else
            {
                bool first = true;
                foreach (var c in s.CatchBlocks)
                {
                    var et = GetTypeDef(c.Exception.ValueType);
                    DisableSkip();
                    BeginLine(first ? null : "else ");
                    EndLine("if (uIs(__t.Exception, " + GetTypeOf(c.Exception.ValueType) + "))");
                    BeginScope();
                    WriteLine(et + " " + c.Exception.Name + " = (" + et + ")__t.Exception;");
                    WriteScope(c.Body, false, false);
                    EndScope();
                    first = false;
                }

                DisableSkip();

                BeginLine(first ? null : "else ");
                WriteLine("throw __t;");
            }

            if (s.OptionalFinallyBody != null)
            {
                var afterFinallyLabel = "__after_finally_" + _finallyCount;
                ++_finallyCount;

                WriteLine("goto " + afterFinallyLabel + ";");
                EndScope();
                WriteScope(s.OptionalFinallyBody, true, false);
                WriteLine(afterFinallyLabel + ":;");
                EndScope();
            }
            else
            {
                EndScope();
            }
        }

        public override void WriteCast(Source src, DataType dt, Expression s, ExpressionUsage u)
        {
            if (dt.IsEnum || s.ReturnType.IsEnum)
                WriteExpression(s, u);
            else
                base.WriteCast(src, dt, s, u);
        }

        public override void WriteDownCast(Source src, DataType dt, Expression s, ExpressionUsage u)
        {
            if (dt.IsInterface)
            {
                if (s.ReturnType.IsInterface)
                    WriteExpression(s, u);
                else
                {
                    Begin(u.IsObject());
                    Write("(uObject*)");
                    WriteExpression(s, ExpressionUsage.Operand);
                    End(u.IsObject());
                }
            }
            else
            {
                Write("uCast" + GetTemplateString(dt) + "(");
                WriteExpression(s);
                Write(", " + GetTypeOf(dt) + ")");
            }
        }

        public override void WriteBox(Source src, DataType dt, Expression s, bool stack, ExpressionUsage u)
        {
            SourceValue result;
            if (Environment.TryGetValue(s.ReturnType, "BoxFunction", out result))
                Write(result.String + "(");
            else if (IsConstrained(s.ReturnType))
                Write("uBoxPtr(" + GetTypeOf(s.ReturnType) + ", ");
            else
                Write("uBox" + (
                        s.ReturnType.IsIntegralType
                            ? GetTemplateString(s.ReturnType)
                            : null
                    ) + "(" + GetTypeOf(s.ReturnType) + ", ");

            WriteExpression(s);

            if (stack)
                Write(", alloca(" + GetTypeOf(s.ReturnType) + "->ObjectSize)");

            Write(")");
        }

        public override void WriteUnbox(Source src, DataType dt, Expression s, ExpressionUsage u)
        {
            if (IsConstrained(dt))
                Write((u == ExpressionUsage.VarArg
                        ? "(void*)"
                        : null
                    ) + "uUnboxAny(" + GetTypeOf(dt) + ", ");
            else
                Write("uUnbox" + GetTemplateString(dt) + "(" + GetTypeOf(dt) + ", ");

            WriteExpression(s);
            Write(")");
        }

        public override void WriteStoreThis(StoreThis s, ExpressionUsage u)
        {
            Begin(u);
            Write(IsClassMember && !IsExtension ? "*this = " : "*__this = ");
            WriteExpression(s.Value);
            End(u);
        }

        public override void WriteLoadPtr(LoadPtr s, ExpressionUsage u)
        {
            Write("uPtr" + (
                    s.Argument.ActualValue is Constant
                        ? GetTemplateString(s.Argument.ReturnType)
                        : null
                ) + "(");
            WriteExpression(s.Argument);
            Write(")");
        }

        public override void WriteAddressOf(AddressOf s, ExpressionUsage u)
        {
            switch (s.Operand.ExpressionType)
            {
                case ExpressionType.This:
                {
                    if (IsExtension)
                        break;
                    Write(IsClassMember ? "this" : "__this");
                    return;
                }
                case ExpressionType.LoadArgument:
                {
                    var v = (LoadArgument)s.Operand;
                    if (IsClassMember && !v.Parameter.IsReference)
                        break;
                    Write(v.Parameter.Name);
                    return;
                }
            }

            Begin(u);
            Write("&");
            WriteExpression(s.Operand, ExpressionUsage.Object);
            End(u);
        }

        string GetFunctionPointer(Function func)
        {
            return _backend.GetFunctionPointer(func, Type);
        }

        string GetMemberName(Member m, Function f)
        {
            return _backend.GetMemberName(m, f, Type);
        }

        string GetTemplateString(string arg)
        {
            return _backend.GetTemplateString(arg);
        }

        string GetTemplateString(DataType dt)
        {
            return _backend.GetTemplateString(dt, Type);
        }

        string GetTemplateSuffix(DataType dt)
        {
            return _backend.GetTemplateSuffix(dt, Type);
        }

        string GetTypeDef(DataType dt)
        {
            return _backend.GetTypeDef(dt, Type);
        }

        string GetTypeName(DataType dt)
        {
            return _backend.GetTypeName(dt, Type);
        }

        string GetTypeOfType(DataType dt)
        {
            return _backend.GetTypeOfType(dt, Type);
        }

        string GetStaticName(DataType dt)
        {
            return _backend.GetStaticName(dt, Type);
        }

        string GetStaticName(Field field)
        {
            return _backend.GetStaticName(field, Type);
        }

        bool IsConstrained(DataType dt)
        {
            return _backend.IsConstrained(dt);
        }

        bool IsConstrained(Field field)
        {
            return _backend.IsConstrained(field);
        }

        bool IsConstrained(Function function)
        {
            return _backend.IsConstrained(function);
        }

        ReferenceType GetReferenceType(DataType dt)
        {
            return _backend.GetReferenceType(dt);
        }

        ReferenceType GetReferenceType(Field field)
        {
            return _backend.GetReferenceType(field);
        }

        bool HasTypeParameter(Function f)
        {
            return _backend.HasTypeParameter(f);
        }
    }
}
