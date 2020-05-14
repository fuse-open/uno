using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.API.Backends.Decompilers
{
    public partial class SourceWriter
    {
        protected void UnsupportedExpression(Expression e)
        {
            Log.Error(e.Source, ErrorCode.I0000, "<" + e.ExpressionType + "> is not supported by this backend");
            Write("<error>");
        }

        public void WriteExpression(Expression e, ExpressionUsage u = ExpressionUsage.Argument)
        {
            switch (e.ExpressionType)
            {
                case ExpressionType.Invalid: Write("<invalid>"); break;
                case ExpressionType.NoOp: WriteNoOp(e as NoOp, u); break;
                case ExpressionType.ExternOp: WriteExternOp(e as ExternOp, u); break;
                case ExpressionType.Constant: WriteConstant(e as Constant, u); break;
                case ExpressionType.AddressOf: WriteAddressOf(e as AddressOf, u); break;
                case ExpressionType.LoadLocal: WriteLoadLocal(e as LoadLocal, u); break;
                case ExpressionType.LoadField: WriteLoadField(e as LoadField, u); break;
                case ExpressionType.LoadArgument: WriteLoadArgument(e as LoadArgument, u); break;
                case ExpressionType.LoadElement: WriteLoadElement(e as LoadElement, u); break;
                case ExpressionType.FixOp: WriteFixOp(e as FixOp, u); break;
                case ExpressionType.SequenceOp: WriteSequenceOp(e as SequenceOp, u); break;
                case ExpressionType.BranchOp: WriteBranchOp(e as BranchOp, u); break;
                case ExpressionType.ReferenceOp: WriteReferenceOp(e as ReferenceOp, u); break;
                case ExpressionType.This: WriteThis(e as This, u); break;
                case ExpressionType.Base: WriteBase(e as Base, u); break;
                case ExpressionType.AllocObject: WriteAllocObject(e as AllocObject, u); break;
                case ExpressionType.NewObject: WriteNewObject(e as NewObject, u); break;
                case ExpressionType.NewDelegate: WriteNewDelegate(e as NewDelegate, u); break;
                case ExpressionType.Swizzle: WriteSwizzle(e as Swizzle, u); break;
                case ExpressionType.CallCast: WriteCallCast(e as CallCast, u); break;
                case ExpressionType.CallBinOp: WriteCallBinOp(e as CallBinOp, u); break;
                case ExpressionType.CallUnOp: WriteCallUnOp(e as CallUnOp, u); break;
                case ExpressionType.CallMethod: WriteCallMethod(e as CallMethod, u); break;
                case ExpressionType.CallDelegate: WriteCallDelegate(e as CallDelegate, u); break;
                case ExpressionType.RuntimeConst: WriteRuntimeConst(e as RuntimeConst, u); break;
                case ExpressionType.LoadUniform: WriteLoadUniform(e as LoadUniform, u); break;
                case ExpressionType.LoadVarying: WriteLoadVarying(e as LoadVarying, u); break;
                case ExpressionType.LoadVertexAttrib: WriteLoadVertexAttrib(e as LoadVertexAttrib, u); break;
                case ExpressionType.LoadPixelSampler: WriteLoadPixelSampler(e as LoadPixelSampler, u); break;
                case ExpressionType.CallShader: WriteCallShader(e as CallShader, u); break;
                case ExpressionType.Default: WriteDefault(e as Default, u); break;
                case ExpressionType.TypeOf: WriteTypeOf(e as TypeOf, u); break;
                case ExpressionType.StoreThis: WriteStoreThis(e as StoreThis, u); break;
                case ExpressionType.StoreLocal: WriteStoreLocal(e as StoreLocal, u); break;
                case ExpressionType.StoreArgument: WriteStoreArgument(e as StoreArgument, u); break;
                case ExpressionType.StoreField: WriteStoreField(e as StoreField, u); break;
                case ExpressionType.StoreElement: WriteStoreElement(e as StoreElement, u); break;
                case ExpressionType.SetProperty: WriteSetProperty(e as SetProperty, u); break;
                case ExpressionType.GetProperty: WriteGetProperty(e as GetProperty, u); break;
                case ExpressionType.AddListener: WriteAddListener(e as AddListener, u); break;
                case ExpressionType.RemoveListener: WriteRemoveListener(e as RemoveListener, u); break;
                case ExpressionType.NewArray: WriteNewArray(e as NewArray, u); break;
                case ExpressionType.CastOp: WriteCastOp(e as CastOp, u); break;
                case ExpressionType.IsOp: WriteIsOp(e as IsOp, u); break;
                case ExpressionType.AsOp: WriteAsOp(e as AsOp, u); break;
                case ExpressionType.NullOp: WriteNullOp(e as NullOp, u); break;
                case ExpressionType.ConditionalOp: WriteConditionalOp(e as ConditionalOp, u); break;
                case ExpressionType.LoadPtr: WriteLoadPtr(e as LoadPtr, u); break;
                default:
                    if (e is StringExpression)
                        Write(((StringExpression) e).String);
                    else
                        UnsupportedExpression(e);
                    break;
            }
        }

        public virtual void WriteNoOp(NoOp s, ExpressionUsage u)
        {
        }

        public virtual void WriteExternOp(ExternOp s, ExpressionUsage u)
        {
            WriteExternString(s.Source, s.String, s.Object, s.Arguments, s.Usings);
        }

        public virtual void WriteFixOp(FixOp s, ExpressionUsage u)
        {
            Begin(u);

            switch (s.Operator)
            {
                case FixOpType.IncreaseBefore: Write("++"); break;
                case FixOpType.DecreaseBefore: Write("--"); break;
            }

            WriteExpression(s.Operand, ExpressionUsage.Object);

            switch (s.Operator)
            {
                case FixOpType.IncreaseAfter: Write("++"); break;
                case FixOpType.DecreaseAfter: Write("--"); break;
            }

            End(u);
        }

        public virtual void WriteLoadVertexAttrib(LoadVertexAttrib s, ExpressionUsage u)
        {
            Write(s.State.VertexAttributes[s.Index].Name);
        }

        public virtual void WriteRuntimeConst(RuntimeConst s, ExpressionUsage u)
        {
            Write(s.State.RuntimeConstants[s.Index].Name);
        }

        public virtual void WriteLoadUniform(LoadUniform s, ExpressionUsage u)
        {
            Write(s.State.Uniforms[s.Index].Name);
        }

        public virtual void WriteLoadPixelSampler(LoadPixelSampler s, ExpressionUsage u)
        {
            Write(s.State.PixelSamplers[s.Index].Name);
        }

        public virtual void WriteLoadVarying(LoadVarying s, ExpressionUsage u)
        {
            Write(s.State.Varyings[s.Index].Name);

            if (s.Fields.Length > 0)
            {
                Write(".");

                foreach (var f in s.Fields)
                    WriteName(s.State.Varyings[s.Index].Type.Fields[f]);
            }
        }

        public virtual void WriteLoadLocal(LoadLocal s, ExpressionUsage u)
        {
            Write(s.Variable.Name);
        }

        public virtual void WriteAddressOf(AddressOf s, ExpressionUsage u)
        {
            switch (s.AddressType)
            {
                case AddressType.Const:
                case AddressType.Out:
                case AddressType.Ref:
                    Write(s.AddressType.ToString().ToLower() + " ");
                    break;
            }

            WriteExpression(s.Operand, u);
        }

        public virtual void WriteBranchOp(BranchOp s, ExpressionUsage u)
        {
            Begin(u);
            WriteExpression(s.Left, ExpressionUsage.Operand);

            switch (s.BranchType)
            {
                case BranchType.And:
                    Write(And);
                    break;
                case BranchType.Or:
                    Write(Or);
                    break;
                default:
                    throw new SourceException(s.Source, "Invalid BranchOpType: " + s.BranchType);
            }

            WriteExpression(s.Right, ExpressionUsage.Operand);
            End(u);
        }

        public virtual void WriteReferenceOp(ReferenceOp s, ExpressionUsage u)
        {
            Begin(u);
            WriteExpression(s.Left, ExpressionUsage.Operand);

            switch (s.EqualityType)
            {
                case EqualityType.Equal:
                    Write(Equals);
                    break;
                case EqualityType.NotEqual:
                    Write(NotEquals);
                    break;
                default:
                    throw new SourceException(s.Source, "Invalid EqualityType: " + s.EqualityType);
            }

            WriteExpression(s.Right, ExpressionUsage.Operand);
            End(u);
        }

        void WriteSequenceOp(SequenceOp s, ExpressionUsage u, bool parentIsSequenceOp)
        {
            Begin(!parentIsSequenceOp);

            if (s.Left is SequenceOp)
                WriteSequenceOp(s.Left as SequenceOp, ExpressionUsage.Statement, true);
            else
                WriteExpression(s.Left, ExpressionUsage.Statement);

            Write(Comma);

            if (s.Right is SequenceOp)
                WriteSequenceOp(s.Right as SequenceOp, u, true);
            else
                WriteExpression(s.Right, u);

            End(!parentIsSequenceOp);
        }

        public virtual void WriteSequenceOp(SequenceOp s, ExpressionUsage u)
        {
            WriteSequenceOp(s, u, u < ExpressionUsage.Argument);
        }

        public virtual void WriteNullOp(NullOp s, ExpressionUsage u)
        {
            Begin(u.IsObject());
            WriteExpression(s.Left, ExpressionUsage.Operand);
            Write(NullOp);
            WriteExpression(s.Right, ExpressionUsage.Operand);
            End(u.IsObject());
        }

        public virtual void WriteConditionalOp(ConditionalOp s, ExpressionUsage u)
        {
            Begin(u);
            WriteExpression(s.Condition, ExpressionUsage.Operand);
            Write(QuestionMark);
            WriteExpression(s.True, ExpressionUsage.Operand);
            Write(Colon);
            WriteExpression(s.False, ExpressionUsage.Operand);
            End(u);
        }

        public virtual void WriteIsOp(IsOp s, ExpressionUsage u)
        {
            Begin(u.IsObject());
            WriteExpression(s.Operand, ExpressionUsage.Operand);
            Write(" is ");
            WriteType(s.Source, s.TestType);
            End(u.IsObject());
        }

        public virtual void WriteAsOp(AsOp s, ExpressionUsage u)
        {
            Begin(u.IsObject());
            WriteExpression(s.Operand, ExpressionUsage.Operand);
            Write(" as ");
            WriteType(s.Source, s.ReturnType);
            End(u.IsObject());
        }

        public virtual void WriteStoreThis(StoreThis s, ExpressionUsage u)
        {
            Begin(u);
            Write("this");
            Write(Assign);
            WriteExpression(s.Value);
            End(u);
        }

        public virtual void WriteStoreLocal(StoreLocal s, ExpressionUsage u)
        {
            Begin(u);
            Write(s.Variable.Name + Assign);
            WriteExpression(s.Value);
            End(u);
        }

        public virtual void WriteStoreArgument(StoreArgument s, ExpressionUsage u)
        {
            Begin(u);
            Write(s.Parameter.Name + Assign);
            WriteExpression(s.Value);
            End(u);
        }

        public virtual void WriteStoreField(Source src, Field field, Expression obj, Expression value, ExpressionUsage u = ExpressionUsage.Argument)
        {
            Begin(u);
            WriteMemberBase(src, field.DeclaringType, obj);
            WriteName(field);
            Write(Assign);
            WriteExpression(value);
            End(u);
        }

        public void WriteStoreField(StoreField s, ExpressionUsage u)
        {
            WriteStoreField(s.Source, s.Field, s.Object, s.Value, u);
        }

        public virtual void WriteStoreElement(Source src, DataType elementType, Expression array, Expression index, Expression value, ExpressionUsage u = ExpressionUsage.Argument)
        {
            Begin(u);
            WriteExpression(array, ExpressionUsage.Object);
            Write("[");
            WriteExpression(index);
            Write("]" + Assign);
            WriteExpression(value);
            End(u);
        }

        public void WriteStoreElement(StoreElement s, ExpressionUsage u)
        {
            WriteStoreElement(s.Source, s.ReturnType, s.Array, s.Index, s.Value, u);
        }

        public virtual void WriteSetProperty(Source src, Property property, Expression obj, Expression[] args, Expression value, Variable ret = null, ExpressionUsage u = ExpressionUsage.Argument)
        {
            Begin(u);

            if (property.Parameters.Length == 0)
                WriteMemberBase(src, property.DeclaringType, obj, property.Name);
            else
            {
                WriteExpression(obj, ExpressionUsage.Object);
                WriteArguments(property, args, true, '[', ']');
            }

            Write(Assign);
            WriteExpression(value);
            End(u);
        }

        public void WriteSetProperty(SetProperty s, ExpressionUsage u)
        {
            WriteSetProperty(s.Source, s.Property, s.Object, s.Arguments, s.Value, s.Storage, u);
        }

        public virtual void WriteAddListener(AddListener s, ExpressionUsage u)
        {
            Begin(u);

            if (s.Object == null)
                WriteStaticType(s.Source, s.Event.DeclaringType);
            else
                WriteExpression(s.Object, ExpressionUsage.Object);

            Write("." + s.Event.Name + " += ");
            WriteExpression(s.Listener);
            End(u);
        }

        public virtual void WriteRemoveListener(RemoveListener s, ExpressionUsage u)
        {
            Begin(u);

            if (s.Object == null)
                WriteStaticType(s.Source, s.Event.DeclaringType);
            else
                WriteExpression(s.Object, ExpressionUsage.Object);

            Write("." + s.Event.Name + " -= ");
            WriteExpression(s.Listener);
            End(u);
        }

        public virtual void WriteNewArray(Source src, ArrayType type, Expression size, ExpressionUsage u = ExpressionUsage.Argument)
        {
            Write("new ");

            int arrayDepth = 0;
            var elementType = type.ElementType;
            while (elementType is ArrayType)
            {
                arrayDepth++;
                elementType = elementType.ElementType;
            }

            WriteType(src, elementType);
            Write("[");
            WriteExpression(size);
            Write("]");

            for (int i = 0; i < arrayDepth; i++)
                Write("[]");
        }

        public virtual void WriteNewArray(Source src, ArrayType type, Expression[] elements, ExpressionUsage u = ExpressionUsage.Argument)
        {
            Write("new ");
            WriteType(src, type);
            Write(" { ");
            bool first = true;

            foreach (var i in elements)
            {
                CommaWhen(!first);
                WriteExpression(i);
                first = false;
            }

            Write("}");
        }

        public void WriteNewArray(NewArray s, ExpressionUsage u)
        {
            if (s.Initializers != null)
                WriteNewArray(s.Source, s.ArrayType, s.Initializers, u);
            else
                WriteNewArray(s.Source, s.ArrayType, s.Size, u);
        }

        public void WriteDefault(Default s, ExpressionUsage u)
        {
            WriteDefault(s.Source, s.ReturnType, u);
        }

        public void WriteTypeOf(TypeOf s, ExpressionUsage u)
        {
            WriteTypeOf(s.Source, s.Type, u);
        }

        public virtual void WriteLoadArgument(LoadArgument s, ExpressionUsage u)
        {
            Write(s.Parameter.Name);
        }

        public virtual void WriteNewDelegate(NewDelegate s, ExpressionUsage u)
        {
            Write("new ");
            WriteStaticType(s.Source, s.ReturnType);
            Write("(");
            WriteMemberBase(s.Source, s.Method.DeclaringType, s.Object, s.Method.Name);

            if (s.Method.IsGenericParameterization)
                WriteGenericList(s.Source, s.Method.GenericArguments);

            Write(")");
        }

        public virtual void WriteCallDelegate(Source src, Expression obj, Expression[] args, Variable ret = null, ExpressionUsage u = ExpressionUsage.Argument)
        {
            WriteExpression(obj, ExpressionUsage.Object);
            WriteArguments(obj.ReturnType, args, true);
        }

        public void WriteCallDelegate(CallDelegate s, ExpressionUsage u)
        {
            WriteCallDelegate(s.Source, s.Object, s.Arguments, s.Storage, u);
        }

        public virtual void WriteLoadField(Source src, Field field, Expression obj, ExpressionUsage u = ExpressionUsage.Argument)
        {
            WriteMemberBase(src, field.DeclaringType, obj);
            WriteName(field);
        }

        public void WriteLoadField(LoadField s, ExpressionUsage u)
        {
            WriteLoadField(s.Source, s.Field, s.Object, u);
        }

        public virtual void WriteCallBinOp(CallBinOp s, ExpressionUsage u)
        {
            Begin(u);
            WriteExpression(s.Left, ExpressionUsage.Operand);
            Write(Space + s.Operator.Symbol + Space);
            WriteExpression(s.Right, ExpressionUsage.Operand);
            End(u);
        }

        public virtual void WriteCallUnOp(CallUnOp s, ExpressionUsage u)
        {
            Begin(u.IsObject());
            Write(s.Operator.Symbol);
            WriteExpression(s.Operand, ExpressionUsage.Object);
            End(u.IsObject());
        }

        public virtual void WriteCallCast(CallCast s, ExpressionUsage u)
        {
            WriteCast(s.Source, s.ReturnType, s.Operand, u);
        }

        public void WriteCastOp(CastOp s, ExpressionUsage u)
        {
            switch (s.CastType)
            {
                default:
                    WriteCast(s.Source, s.ReturnType, s.Operand, u);
                    break;
                case CastType.Up:
                    WriteUpCast(s.Source, s.ReturnType, s.Operand, u);
                    break;
                case CastType.Down:
                    WriteDownCast(s.Source, s.ReturnType, s.Operand, u);
                    break;
                case CastType.Box:
                    WriteBox(s.Source, s.ReturnType, s.Operand, false, u);
                    break;
                case CastType.Unbox:
                    WriteUnbox(s.Source, s.ReturnType, s.Operand, u);
                    break;
            }
        }

        public virtual void WriteCallMethod(Source src, Method method, Expression obj, Expression[] args, Variable ret = null, ExpressionUsage u = ExpressionUsage.Argument)
        {
            WriteMemberBase(src, method.DeclaringType, obj, method.Name);

            if (method.IsGenericParameterization)
                WriteGenericList(src, method.GenericArguments);

            WriteArguments(method, args, true);
        }

        public void WriteCallMethod(CallMethod s, ExpressionUsage u)
        {
            WriteCallMethod(s.Source, s.Method, s.Object, s.Arguments, s.Storage, u);
        }

        public void WriteAllocObject(AllocObject s, ExpressionUsage u)
        {
            WriteNewObject(s.Source, s.ReturnType, u);
        }

        public virtual void WriteNewObject(Source src, Constructor ctor, Expression[] args, ExpressionUsage u = ExpressionUsage.Argument)
        {
            Write("new ");
            WriteStaticType(src, ctor.DeclaringType);
            WriteArguments(ctor, args, true);
        }

        public void WriteNewObject(NewObject s, ExpressionUsage u)
        {
            WriteNewObject(s.Source, s.Constructor, s.Arguments, u);
        }

        public virtual void WriteCallShader(CallShader s, ExpressionUsage u)
        {
            Write(s.Function.Name);
            WriteArguments(s.Function, s.Arguments, true);
        }

        public virtual void WriteLoadElement(Source src, DataType elementType, Expression array, Expression index, ExpressionUsage u = ExpressionUsage.Argument)
        {
            WriteExpression(array, ExpressionUsage.Object);
            Write("[");
            WriteExpression(index);
            Write("]");
        }

        public void WriteLoadElement(LoadElement s, ExpressionUsage u)
        {
            WriteLoadElement(s.Source, s.ReturnType, s.Array, s.Index, u);
        }

        public virtual void WriteThis(This s, ExpressionUsage u)
        {
            Write("this");
        }

        public virtual void WriteBase(Base s, ExpressionUsage u)
        {
            Write("base");
        }

        public virtual void WriteGetProperty(Source src, Property property, Expression obj, Expression[] args, Variable ret = null, ExpressionUsage u = ExpressionUsage.Argument)
        {
            if (property.Parameters.Length == 0)
                WriteMemberBase(src, property.DeclaringType, obj, property.Name);
            else
            {
                WriteExpression(obj, ExpressionUsage.Object);
                WriteArguments(property, args, true, '[', ']');
            }
        }

        public virtual void WriteGetProperty(GetProperty s, ExpressionUsage u)
        {
            WriteGetProperty(s.Source, s.Property, s.Object, s.Arguments, s.Storage, u);
        }

        public virtual void WriteSwizzle(Swizzle s, ExpressionUsage u)
        {
            WriteMemberBase(s.Object);

            foreach (var f in s.Fields)
                WriteName(f);
        }

        public virtual void WriteLoadPtr(LoadPtr s, ExpressionUsage u)
        {
            UnsupportedExpression(s);
        }
    }
}
