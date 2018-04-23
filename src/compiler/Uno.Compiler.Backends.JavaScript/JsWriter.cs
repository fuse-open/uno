using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Uno.Compiler.API.Backends.Decompilers;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Backends.JavaScript
{
    public class JsWriter : SourceWriter
    {
        public static void ExportClass(JsBackend backend, DataType dt)
        {
            var filename = backend.GetExportName(dt, Path.DirectorySeparatorChar) + ".js";
            backend.SourceFiles.Add(filename);

            using (var w = new JsWriter(backend, Path.Combine(backend.SourceDirectory, filename.UnixToNative())))
                w.WriteClass(dt);
        }

        readonly JsBackend Backend;
        readonly List<string> This = new List<string>() { "this" };

        public JsWriter(JsBackend backend, string filename)
            : base(backend, filename)
        {
            Backend = backend;
            HasFloatSuffix = false;
        }

        public JsWriter(JsBackend backend, StringBuilder sb, Function context)
            : base(backend, sb, context)
        {
            Backend = backend;
            HasFloatSuffix = false;
        }

        public override void WriteThis(This s, ExpressionUsage u)
        {
            Write(This.Last());
        }

        public override void WriteBase(Base s, ExpressionUsage u)
        {
            Write(This.Last());
        }

        public override void WriteName(Field f)
        {
            Write(Backend.GetMember(f));
        }

        public override void WriteParameter(Source src, DataType dt, ParameterModifier m, string name)
        {
            Write(name);
        }

        public override void WriteType(Source src, DataType dt)
        {
            Write("var");
        }

        public override void WriteCast(Source src, DataType dt, Expression s, ExpressionUsage u = ExpressionUsage.Argument)
        {
            Begin(u.IsObject());
            WriteExpression(s, ExpressionUsage.Operand);
            End(u.IsObject());
        }

        public override void WriteStaticType(Source src, DataType dt)
        {
            Write(Backend.GetGlobal(dt));
        }

        public override void WriteTypeOf(Source src, DataType dt, ExpressionUsage u)
        {
            Write(Backend.GetTypeId(src, Function, dt));
        }

        public override void WriteConstant(Constant s, ExpressionUsage u)
        {
            if (s.Value is char)
                Write((int)((char)s.Value));
            else
                base.WriteConstant(s, u);
        }

        public override void WriteLoadField(Source src, Field field, Expression obj, ExpressionUsage u)
        {
            if (obj == null)
                Write(Backend.GetGlobal(field));
            else
                WriteMemberBase(obj, field.DeclaringType, Backend.GetMember(field));
        }

        public override void WriteStoreField(Source src, Field field, Expression obj, Expression value, ExpressionUsage u)
        {
            Begin(u);

            if (obj == null)
                Write(Backend.GetGlobal(field));
            else
                WriteMemberBase(obj, field.DeclaringType, Backend.GetMember(field));

            Write(Assign);
            WriteExpression(value);
            End(u);
        }

        public override void WriteCallCast(CallCast s, ExpressionUsage u)
        {
            if (Environment.GetBool(s.Operand.ReturnType, "IsIntrinsic"))
            {
                // Cast to integer
                if (s.ReturnType.IsIntegralType && s.Operand.ReturnType.IsFloatingPointType)
                {
                    if (u >= ExpressionUsage.Operand)
                        Write("(");

                    WriteExpression(s.Operand, ExpressionUsage.Operand);
                    Write(" | 0");

                    if (u >= ExpressionUsage.Operand)
                        Write(")");

                    return;
                }

                WriteExpression(s.Operand, ExpressionUsage.Object);

                if (s.ReturnType == Essentials.SByte && s.Operand.ReturnType == Essentials.Byte)
                    Write(".ByteToSByte()");
                else if (s.ReturnType == Essentials.Byte && s.Operand.ReturnType == Essentials.SByte)
                    Write(".SByteToByte()");

                return;
            }

            base.WriteCallCast(s, u);
        }

        public override void WriteCallBinOp(CallBinOp s, ExpressionUsage u)
        {
            // Integer division
            if (s.Left.ReturnType.IsIntegralType && s.Operator.Type == OperatorType.Division)
            {
                Begin(u);
                base.WriteCallBinOp(s, ExpressionUsage.Operand);
                Write(" | 0");
                End(u);
                return;
            }

            base.WriteCallBinOp(s, u);
        }

        public override void WriteGetProperty(Source src, Property property, Expression obj, Expression[] args, Variable ret, ExpressionUsage u)
        {
            if (obj != null)
            {
                if (obj.ReturnType.IsRefArray || obj.ReturnType == Essentials.String)
                {
                    WriteMemberBase(obj);

                    switch (property.UnoName)
                    {
                        case "Item":
                            Write("charCodeAt");
                            break;

                        default:
                            Write(property.UnoName.ToLower());
                            break;
                    }

                    if (args.Length > 0)
                        WriteArguments(property.GetMethod, args, true);

                    return;
                }
                else if (obj is Base)
                {
                    Write(Backend.GetGlobal(property.DeclaringType) + ".prototype." + Backend.GetMember(property) + ".call(this");
                    CommaWhen(args.Length > 0);
                }
                else if (property.DeclaringType.IsInterface)
                {
                    var name = Backend.GetGlobal(property);

                    if (name.IsValidJsIdentifier())
                        WriteMemberBase(obj, property.DeclaringType, name);
                    else
                    {
                        WriteExpression(obj, ExpressionUsage.Object);
                        Write("[" + name.ToLiteral() + "]");
                    }

                    Write("(");
                }
                else
                {
                    WriteMemberBase(obj, property.DeclaringType, Backend.GetMember(property));
                    Write("(");
                }
            }
            else
            {
                Write(Backend.GetGlobal(property) + "(");
            }

            WriteArguments(property, args);
            Write(")");
        }

        public override void WriteSetProperty(Source src, Property property, Expression obj, Expression[] args, Expression value, Variable ret, ExpressionUsage u)
        {
            if (obj != null)
            {
                if (obj is Base)
                    Write(Backend.GetGlobal(property.DeclaringType) + ".prototype." + Backend.GetMember(property) + ".call(this, ");
                else if (property.DeclaringType.IsInterface)
                {
                    var name = Backend.GetGlobal(property);

                    if (name.IsValidJsIdentifier())
                        WriteMemberBase(obj, property.DeclaringType, name);
                    else
                    {
                        WriteExpression(obj, ExpressionUsage.Object);
                        Write("[" + name.ToLiteral() + "]");
                    }

                    Write("(");
                }
                else
                {
                    WriteMemberBase(obj, property.DeclaringType, Backend.GetMember(property));
                    Write("(");
                }
            }
            else
                Write(Backend.GetGlobal(property) + "(");

            if (property.Parameters.Length > 0)
            {
                WriteArguments(property, args);
                Write(", ");
            }

            WriteExpression(value);
            Write(")");
        }

        public override void WriteNewArray(Source src, ArrayType type, Expression[] initializers, ExpressionUsage u)
        {
            Write("Array.Init([");
            Indent();

            for (int i = 0; i < initializers.Length; i++)
            {
                CommaWhen(i > 0);

                if (i > 0 && (i % 64 == 0))
                {
                    EndLine();
                    BeginLine();
                }

                WriteExpression(initializers[i]);
            }

            Write("], " + Backend.GetTypeId(src, Function, type.ElementType) + ")");
            Unindent();
        }

        public override void WriteNewArray(Source src, ArrayType type, Expression size, ExpressionUsage u)
        {
            var dt = type.ElementType;

            if (Environment.GetString(dt, "DefaultValue") == "0")
            {
                Write("Array.Zeros(");
                WriteExpression(size);
                Write(", " + Backend.GetTypeId(src, Function, dt) + ")");
            }
            else
            {
                switch (dt.TypeType)
                {
                    case TypeType.Enum:
                        Write("Array.Zeros(");
                        WriteExpression(size);
                        Write(", " + Backend.GetTypeId(src, Function, dt) + ")");
                        break;
                    case TypeType.Struct:
                        Write("Array.Structs(");
                        WriteExpression(size);
                        Write(
                            ", " + Backend.GetGlobal(dt) +
                            ", " + Backend.GetTypeId(src, Function, dt) + (
                                dt.IsFlattenedParameterization
                                    ? ", " + Backend.GetTypeIds(src, Function, dt.FlattenedArguments)
                                    : null
                            ) +
                            ")");
                        break;
                    default:
                        Write("Array.Sized(");
                        WriteExpression(size);
                        Write(", " + Backend.GetTypeId(src, Function, dt) + ")");
                        break;
                }
            }
        }

        public override void WriteCallMethod(Source src, Method method, Expression obj, Expression[] args, Variable ret, ExpressionUsage u)
        {
            if (obj != null)
            {
                if (obj is Base)
                {
                    Write(Backend.GetGlobal(method.DeclaringType) + ".prototype." + Backend.GetMember(method) + ".call(this");
                    CommaWhen(args.Length > 0);
                }
                else if (method.DeclaringType.IsInterface)
                {
                    var name = Backend.GetGlobal(method);

                    if (name.IsValidJsIdentifier())
                        WriteMemberBase(obj, method.DeclaringType, name);
                    else
                    {
                        WriteExpression(obj, ExpressionUsage.Object);
                        Write("[" + name.ToLiteral() + "]");
                    }

                    Write("(");
                }
                else
                {
                    WriteMemberBase(obj, method.DeclaringType, Backend.GetMember(method));
                    Write("(");
                }
            }
            else
            {
                Write(Backend.GetGlobal(method) + "(");
            }

            WriteArguments(method, args);
            bool needsComma = args.Length > 0;

            if (method.IsStatic && method.DeclaringType.IsFlattenedParameterization)
            {
                if (needsComma) Write(", ");
                Write(Backend.GetTypeIds(src, Function, method.DeclaringType.FlattenedArguments));
                needsComma = true;
            }

            if (method.IsGenericParameterization)
            {
                if (needsComma) Write(", ");
                Write(Backend.GetTypeIds(src, Function, method.GenericArguments));
                needsComma = true;
            }

            if (obj != null && (
                    obj.ReturnType.IsGenericParameter ||
                    obj.ReturnType == Essentials.Char
                ) && (
                    method.UnoName == "ToString" ||
                    method.UnoName == "Equals"
                ))
            {
                if (needsComma) Write(", ");
                Write(Backend.GetTypeId(src, Function, obj.ReturnType));
                needsComma = true;
            }

            Write(")");
        }

        public override void WriteArgument(Parameter p, Expression a)
        {
            if (p.IsReference)
            {
                if (a.ActualValue is LoadArgument)
                {
                    var parameter = (a.ActualValue as LoadArgument).Parameter;
                    if (parameter.IsReference)
                    {
                        Write(parameter.Name);
                        return;
                    }
                }

                This.Add("this.$");
                Write("$CreateRef(function(){return ");
                WriteExpression(a);
                Write("}, function($){");
                WriteExpression(a, ExpressionUsage.Operand);
                Write("=$}, this)");
                This.RemoveLast();
            }
            else
                WriteExpression(a);
        }

        public override void WriteThrow(Throw s)
        {
            if (s.IsRethrow)
                WriteLine("throw $js_exception");
            else
            {
                BeginLine("throw new $Error(");
                WriteExpression(s.Exception);
                EndLine(");");
            }
        }

        public override void WriteTryCatchFinally(TryCatchFinally s)
        {
            WriteLine("try");
            WriteScope(s.TryBody);

            if (s.CatchBlocks.Length > 0)
            {
                WriteLine("catch ($js_exception)");
                BeginScope();

                WriteLine("$uno_exception = $ConvertNativeException($js_exception);");

                bool first = true;
                foreach (var c in s.CatchBlocks)
                {
                    BeginLine(!first ? "else " : "");
                    EndLine("if ($uno_exception instanceof " + Backend.GetGlobal(c.Exception.ValueType) + ")");
                    BeginScope();
                    WriteLine("var " + c.Exception.Name + " = $uno_exception;");
                    WriteScope(c.Body, false, false);
                    EndScope();
                    first = false;
                }

                WriteLine("else");
                BeginScope();
                WriteLine("throw $js_exception");
                EndScope();
                EndScope();
            }

            if (s.OptionalFinallyBody != null)
            {
                WriteLine("finally");
                WriteScope(s.OptionalFinallyBody);
            }
        }

        public override void WriteStoreArgument(StoreArgument s, ExpressionUsage u)
        {
            if (s.Parameter.IsReference)
            {
                Write(s.Parameter.Name + "(");
                WriteExpression(s.Value);
                Write(")");
            }
            else
                base.WriteStoreArgument(s, u);
        }

        public override void WriteLoadArgument(LoadArgument s, ExpressionUsage u)
        {
            if (s.Parameter.IsReference)
            {
                base.WriteLoadArgument(s, ExpressionUsage.Object);
                Write("()");
            }
            else
                base.WriteLoadArgument(s, u);
        }

        public override void WriteMemberBase(Expression s, DataType declType = null, string member = null)
        {
            if (s is LoadArgument && (s as LoadArgument).Parameter.IsReference)
            {
                base.WriteLoadArgument(s as LoadArgument, ExpressionUsage.Object);
                Write("()." + member);
            }
            else if (s.ActualValue is Constant)
            {
                Write("(");
                WriteExpression(s);
                Write(")." + member);
            }
            else
                base.WriteMemberBase(s, declType, member);
        }

        bool IsBoxedType(DataType dt)
        {
            return
                dt.IsEnum ||
                dt.IsGenericParameter ||
                Environment.GetBool(dt, "IsIntrinsic");
        }

        public override void WriteDownCast(Source src, DataType dt, Expression s, ExpressionUsage u)
        {
            Write("$DownCast(");
            WriteExpression(s);
            Write(", " + Backend.GetTypeId(src, Function, dt) + ")");
        }

        public override void WriteBox(Source src, DataType dt, Expression s, bool stack, ExpressionUsage u)
        {
            if (IsBoxedType(s.ReturnType))
            {
                Write("$CreateBox(");
                WriteExpression(s);
                Write(", " + Backend.GetTypeId(src, Function, s.ReturnType) + ")");
            }
            else
            {
                Write("$CopyStruct(");
                WriteExpression(s);
                Write(")");
            }
        }

        public override void WriteUnbox(Source src, DataType dt, Expression s, ExpressionUsage u = ExpressionUsage.Argument)
        {
            WriteDownCast(src, dt, s, u);
        }

        public override void WriteIsOp(IsOp s, ExpressionUsage u)
        {
            Write("$IsOp(");
            WriteExpression(s.Operand);
            Write(", " + Backend.GetTypeId(s.Source, Function, s.TestType));

            if (IsBoxedType(s.Operand.ReturnType))
                Write(", " + Backend.GetTypeId(s.Source, Function, s.Operand.ReturnType));

            Write(")");
        }

        public override void WriteAsOp(AsOp s, ExpressionUsage u)
        {
            Write("$AsOp(");
            WriteExpression(s.Operand);
            Write(", " + Backend.GetTypeId(s.Source, Function, s.ReturnType));

            if (IsBoxedType(s.Operand.ReturnType))
                Write(", " + Backend.GetTypeId(s.Source, Function, s.Operand.ReturnType));

            Write(")");
        }

        public override void WriteAddressOf(AddressOf s, ExpressionUsage u)
        {
            WriteExpression(s.ActualValue, u);
        }

        public override void WriteNewObject(Source src, DataType dt, ExpressionUsage u)
        {
            Write("new " + Backend.GetGlobal(dt));

            if (dt.IsFlattenedParameterization)
                Write("(" + Backend.GetTypeIds(dt.Source, Function, dt.FlattenedArguments) + ")");
        }

        public override void WriteDefault(Source src, DataType dt, ExpressionUsage u = ExpressionUsage.Argument)
        {
            string defVal = Environment.GetString(dt, "DefaultValue");

            if (defVal != null)
                Write(defVal);
            else
            {
                switch (dt.TypeType)
                {
                    case TypeType.Enum:
                        Write("0");
                        break;
                    case TypeType.Struct:
                        WriteNewObject(src, dt, u);
                        break;
                    default:
                        Write("null");
                        break;
                }
            }
        }

        public void WriteDefaultInitGenericStruct(DataType dt, DataType declaringType)
        {
            Write("new " + Backend.GetGlobal(dt) + "(this.$typeArgIds" + Backend.CountBaseClasses(declaringType) + ")");
        }

        public override void WriteNewDelegate(NewDelegate s, ExpressionUsage u)
        {
            Write("$CreateDelegate(");

            if (s.Object != null)
            {
                WriteExpression(s.Object);
                Write(", ");

                if (s.Method.DeclaringType.IsInterface)
                {
                    // FIXME: Potential side effect
                    WriteExpression(s.Object, ExpressionUsage.Object);

                    var name = Backend.GetGlobal(s.Method);
                    Write(name.IsValidJsIdentifier()
                        ? "." + name
                        : "[" + name.ToLiteral() + "]");
                }
                else if (s.Method.IsVirtual)
                {
                    // FIXME: Potential side effect
                    WriteExpression(s.Object, ExpressionUsage.Object);
                    Write("." + Backend.GetMember(s.Method));
                }
                else
                    Write(Backend.GetGlobal(s.Method.DeclaringType) + ".prototype." + Backend.GetMember(s.Method));
            }
            else
                Write("null, " + Backend.GetGlobal(s.Method));

            Write(", " + Backend.GetTypeId(s.Source, Function, s.ReturnType));

            if (s.ReturnType.IsGenericParameterization)
                Write(", " + Backend.GetTypeIds(s.Source, Function, s.ReturnType.FlattenedArguments));

            Write(")");
        }

        public override void WriteCallDelegate(Source src, Expression obj, Expression[] args, Variable ret, ExpressionUsage u)
        {
            WriteMemberBase(obj, null, "Invoke");
            WriteArguments(obj.ReturnType, args, true);
        }

        void WriteInterfaceImplementedMember(Member decl, Member impl)
        {
            var name = Backend.GetGlobal(decl);

            if (impl.IsVirtual)
            {
                var ret = impl.ReturnType.IsVoid ? "" : "return ";

                if (name.IsValidJsIdentifier())
                    WriteLine("I." + name + " = function() { " + ret + "this." + Backend.GetMember(impl) + ".apply(this, arguments); };");
                else
                    WriteLine("I[" + name.ToLiteral() + "] = function() { " + ret + "this." + Backend.GetMember(impl) + ".apply(this, arguments); };");

                // TODO: This can also be done without a wrapper function.
            }
            else
            {
                if (name.IsValidJsIdentifier())
                    WriteLine("I." + name + " = I." + Backend.GetMember(impl) + ";");
                else
                    WriteLine("I[" + name.ToLiteral() + "] = I." + Backend.GetMember(impl) + ";");
            }
        }

        public void WriteClass(DataType dt)
        {
            if (dt.IsStatic)
            {
                if (!Backend.Obfuscate)
                    BeginLine(Backend.GetGlobal(dt) + " = ");
                else
                    BeginLine();

                EndLine("$CreateClass(");
                Indent();
                WriteLine("function() {");
                WriteLine("},");
                WriteLine("function(S) {");
                Indent();
            }
            else
            {
                WriteLine(Backend.GetGlobal(dt) + " = $CreateClass(");
                Indent();
                BeginLine("function(");

                // Anders' hack: this used to be 'dt is StructType', but it seems
                // to be needed for classes as well, as they may have generic methods and
                // need '$typeArgIds too
                var needsTypeArgIds = dt is StructType || dt is ClassType;

                var c = Backend.CountBaseClasses(dt);

                if (needsTypeArgIds && dt.IsFlattenedDefinition)
                    Write(dt.FlattenedParameters.Length == 1 ? ("$typeArgId" + c) : ("$typeArgIds" + c));

                EndLine(") {");
                Indent();

                if (dt.Base != null && dt.Base != Essentials.Object)
                {
                    BeginLine(Backend.GetGlobal(dt.Base) + ".call(this");

                    if (needsTypeArgIds && dt.IsFlattenedDefinition)
                        Write(", " + (dt.FlattenedParameters.Length == 1 ? ("$typeArgId" + c) : ("$typeArgIds" + c)));

                    EndLine(");");
                }

                if (dt is StructType)
                    WriteLine("this.$struct = true;");

                if (needsTypeArgIds && dt.IsFlattenedDefinition)
                    WriteLine(dt.FlattenedParameters.Length == 1 ? "this.$typeArgId" + c + " = $typeArgId" + c + ";" : "this.$typeArgIds" + c + " = $typeArgIds" + c + ";");

                foreach (var f in dt.Fields)
                {
                    if (f.IsStatic)
                        continue;

                    BeginLine("this." + Backend.GetMember(f) + " = ");
                    if (f.ReturnType.IsStruct && f.ReturnType.IsFlattenedParameterization)
                        WriteDefaultInitGenericStruct(f.ReturnType, dt);
                    else
                        WriteDefault(f.Source, f.ReturnType);
                    EndLine(";");
                }

                Unindent();

                WriteLine("},");
                WriteLine("function(S) {");
                Indent();

                var targetTypeName = Environment.GetString(dt, "TargetTypeName");

                if (targetTypeName != null)
                {
                    WriteLine("var I = " + targetTypeName + ".prototype;");
                }
                else
                {
                    BeginLine("var I = S.prototype");

                    if (dt.Base != null && dt.Base != Essentials.Object)
                        Write(" = new " + Backend.GetGlobal(dt.Base));

                    EndLine(";");
                }

                Skip();
            }

            // Static fields
            foreach (var f in dt.Fields.Where(f => f.IsStatic))
            {
                BeginLine(Backend.GetGlobal(f) + " = ");
                WriteDefault(f.Source, f.ReturnType);
                EndLine(";");
            }

            Skip();

            if (!dt.IsStatic)
            {
                WriteLine("I.GetType = function()");
                BeginScope();
                WriteLine("return " + Backend.GetTypeId(dt.Source, dt) + ";");
                EndScopeSemicolon();

                if (dt.Interfaces.Length > 0)
                {
                    WriteLine("I.$II = function(id)");
                    BeginScope();
                    BeginLine("return ");

                    // TODO: This can probably be done in a more clever way

                    Write("[");

                    HashSet<int> writtenTypes = new HashSet<int>();
                    var bt = dt;

                    var first = true;

                    do
                    {
                        for (int i = 0; i < bt.Interfaces.Length; i++)
                        {
                            var id = Backend.GetTypeId(bt.Source, bt.Interfaces[i]) ^ (1 << 15);

                            if (writtenTypes.Contains(id))
                                continue;

                            writtenTypes.Add(id);

                            if (!first)
                                Write(", ");
                            else
                                first = false;

                            Write(id.ToString());
                        }

                        bt = bt.Base;
                    }
                    while (bt != null);

                    EndLine("].indexOf(id) != -1;");

                    EndScopeSemicolon();
                }
            }

            foreach (var p in dt.Properties)
            {
                if (p.IsAbstract ||
                    p.GetMethod != null && Environment.GetBool(p.GetMethod, "IsIntrinsic"))
                    continue;

                if (p.ImplementedProperty != null)
                {
                    var name = Backend.GetGlobal(p.ImplementedProperty);

                    if (name.IsValidJsIdentifier())
                        BeginLine("I." + name + " = function(");
                    else
                        BeginLine("I[" + name.ToLiteral() + "] = function(");
                }
                else
                {
                    BeginLine((p.IsStatic ? Backend.GetGlobal(p) : "I." + Backend.GetMember(p)) + " = function(");
                }

                if (p.IsStatic && dt.IsFlattenedDefinition)
                {
                    Write(dt.FlattenedParameters.Length == 1 
                            ? "$staticArgId"
                            : "$staticArgIds");

                    if (p.SetMethod != null)
                        Write(", ");
                }

                for (int i = 0; i < p.Parameters.Length; i++)
                {
                    if (i > 0)
                        Write(", ");

                    Write(p.Parameters[i].Name);
                }

                if (p.SetMethod != null)
                {
                    if (p.Parameters.Length > 0)
                        Write(", ");

                    Write("value");
                }

                EndLine(")");
                BeginScope();

                if (p.SetMethod != null)
                {
                    WriteLine("if (value !== undefined)");
                    BeginScope();
                    WriteFunctionBody(p.SetMethod, false);
                    EndScope();
                }

                if (p.GetMethod != null)
                {
                    if (p.SetMethod != null)
                    {
                        DisableSkip();
                        WriteLine("else");
                        BeginScope();
                    }

                    WriteFunctionBody(p.GetMethod, false);

                    if (p.SetMethod != null)
                        EndScope();
                }

                EndScopeSemicolon();
            }

            foreach (var m in dt.Methods)
            {
                if (m.IsAbstract ||
                    Environment.GetBool(m, "IsIntrinsic"))
                    continue;

                if (m.ImplementedMethod != null)
                {
                    var name = Backend.GetGlobal(m.ImplementedMethod);

                    if (name.IsValidJsIdentifier())
                        BeginLine("I." + name + " = function(");
                    else
                        BeginLine("I[" + name.ToLiteral() + "] = function(");
                }
                else
                {
                    BeginLine((m.IsStatic ? Backend.GetGlobal(m) : "I." + Backend.GetMember(m)) + " = function(");
                }

                WriteParameters(m);

                bool needsComma = m.Parameters.Length > 0;

                if (m.IsStatic && dt.IsFlattenedDefinition)
                {
                    if (needsComma)
                        Write(", ");

                    Write(dt.FlattenedParameters.Length == 1 
                        ? "$staticArgId" 
                        : "$staticArgIds");
                    needsComma = true;
                }

                if (m.IsGenericDefinition)
                {
                    if (needsComma)
                        Write(", ");

                    Write(m.GenericParameters.Length == 1 
                            ? "$methodArgId" 
                            : "$methodArgIds");
                    needsComma = true;
                }

                EndLine(")");

                BeginScope();
                WriteFunctionBody(m, false);
                EndScopeSemicolon();
            }

            if (dt.InterfaceMethods.Count > 0)
            {
                var p = new HashSet<Property>();

                foreach (var m in dt.InterfaceMethods)
                {
                    var pp = m.Value.DeclaringMember as Property;
                    var ip = m.Key.DeclaringMember as Property;

                    if (pp != null && ip != null)
                    {
                        if (pp.ImplementedProperty == null && !p.Contains(pp))
                        {
                            WriteInterfaceImplementedMember(ip, pp);
                            p.Add(pp);
                        }
                    }
                    else
                    {
                        if (m.Value.ImplementedMethod == null)
                            WriteInterfaceImplementedMember(m.Key, m.Value);
                    }
                }

                Skip();
            }

            Unindent();
            WriteLine("});");
            Unindent();
        }
    }
}
