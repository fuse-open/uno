using System;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.Bytecode;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Core.IL.Bytecode
{
    public partial class BytecodeCompiler
    {
        void CreateIndirection(DataType dt)
        {
            var v = new Variable(Function.Source, Function, "#ind" + (_tempVariableCounter++), dt);
            Locals.Add(v);
            Emit(Opcodes.StoreLocal, v);
            Emit(Opcodes.LoadLocalAddress, v);
        }

        Variable StoreTempDontDup(DataType dt)
        {
            var v = new Variable(Function.Source, Function, "#temp" + (_tempVariableCounter++), dt);
            Locals.Add(v);
            Emit(Opcodes.StoreLocal, v);
            return v;
        }

        Variable StoreTemp(DataType dt, bool pop)
        {
            if (pop) return null;
            var v = new Variable(Function.Source, Function, "#temp" + (_tempVariableCounter++), dt);
            Locals.Add(v);
            Emit(Opcodes.Dup);
            Emit(Opcodes.StoreLocal, v);
            return v;
        }

        void LoadTemp(Variable v)
        {
            if (v != null)
                Emit(Opcodes.LoadLocal, v);
        }

        public enum ConditionSequence
        {
            NoneFollows,
            TrueFollows,
            FalseFollows,
        }

        public class Condition
        {
            public Label TrueLabel, FalseLabel;
            public bool Handled;
            public ConditionSequence Sequence;

            public bool CanSkipTrue;
            public bool CanSkipFalse;

            public Condition(ConditionSequence sequence, Label trueLabel, Label falseLabel)
            {
                Sequence = sequence;
                TrueLabel = trueLabel;
                FalseLabel = falseLabel;
                Handled = false;
            }
        }

        public void CompileExpression(Expression e, bool pop = false, bool addressMode = false, Condition cond = null)
        {
            switch (e.ExpressionType)
            {
                case ExpressionType.NoOp:
                    break;

                case ExpressionType.AddressOf:
                    CompileExpression((e as AddressOf).Operand, pop, true);
                    break;

                case ExpressionType.Constant:
                    {
                        if (cond != null && !cond.Handled)
                        {
                            if ((bool) e.ConstantValue)
                            {
                                cond.CanSkipFalse = true;

                                if (cond.Sequence != ConditionSequence.TrueFollows)
                                    Branch(Opcodes.Br, cond.TrueLabel ?? (cond.TrueLabel = NewLabel()));
                            }
                            else
                            {
                                cond.CanSkipTrue = true;

                                if (cond.Sequence != ConditionSequence.FalseFollows)
                                    Branch(Opcodes.Br, cond.FalseLabel ?? (cond.FalseLabel = NewLabel()));
                            }

                            cond.Handled = true;
                        }
                        else if (!pop)
                        {
                            if (e.ConstantValue != null)
                                Emit(Opcodes.Constant, e.ConstantValue);
                            else if (!e.ReturnType.IsReferenceType || e.ReturnType.IsGenericType)
                                Emit(Opcodes.DefaultInit, e.ReturnType);
                            else
                                Emit(Opcodes.Null);

                            if (addressMode)
                                CreateIndirection(e.ReturnType);
                        }
                    }
                    break;

                case ExpressionType.Default:
                    {
                        if (cond != null && !cond.Handled)
                        {
                            Branch(Opcodes.Br, cond.FalseLabel);
                            cond.Handled = true;
                        }
                        else if (!pop)
                        {
                            Emit(Opcodes.DefaultInit, e.ReturnType);

                            if (addressMode)
                                CreateIndirection(e.ReturnType);
                        }
                    }
                    break;

                case ExpressionType.TypeOf:
                    {
                        if (!pop)
                            Emit(Opcodes.TypeOf, (e as TypeOf).Type);
                    }
                    break;

                case ExpressionType.This:
                    {
                        if (pop) return;
                        Emit(Opcodes.This);

                        if (!addressMode && Function.DeclaringType.IsValueType)
                            Emit(Opcodes.LoadObj, Function.DeclaringType);
                    }
                    break;

                case ExpressionType.Base:
                    {
                        if (pop) return;
                        Emit(Opcodes.This);

                        if (!Function.DeclaringType.IsValueType)
                            return;

                        Emit(Opcodes.LoadObj, Function.DeclaringType);
                        Emit(Opcodes.Box, Function.DeclaringType);
                    }
                    break;

                case ExpressionType.SequenceOp:
                    {
                        var s = e as SequenceOp;
                        CompileExpression(s.Left, !s.Left.ReturnType.IsVoid);
                        CompileExpression(s.Right, pop, addressMode);
                    }
                    break;

                case ExpressionType.IsOp:
                    {
                        var s = e as IsOp;
                        CompileExpression(s.Operand, pop);

                        if (pop)
                            break;
                        if (s.Operand.ReturnType.IsGenericParameter)
                            Emit(Opcodes.Box, s.Operand.ReturnType);

                        Emit(Opcodes.AsClass, s.TestType);
                        Emit(Opcodes.Null);
                        Emit(Opcodes.Neq);

                        if (addressMode)
                            CreateIndirection(s.ReturnType);
                    }
                    break;

                case ExpressionType.AsOp:
                    {
                        var s = e as AsOp;
                        CompileExpression(s.Operand, pop);

                        if (pop)
                            break;
                        if (s.Operand.ReturnType.IsGenericParameter)
                            Emit(Opcodes.Box, s.Operand.ReturnType);

                        Emit(Opcodes.AsClass, s.ReturnType);

                        if (s.ReturnType.IsGenericParameter)
                            Emit(Opcodes.UnboxAny, s.ReturnType);
                        if (addressMode)
                            CreateIndirection(s.ReturnType);
                    }
                    break;

                case ExpressionType.LoadLocal:
                    {
                        var s = e as LoadLocal;
                        if (pop) break;

                        Emit(addressMode 
                                ? Opcodes.LoadLocalAddress
                                : Opcodes.LoadLocal,
                            s.Variable);
                    }
                    break;

                case ExpressionType.LoadField:
                    {
                        var s = e as LoadField;

                        if (s.Object != null)
                        {
                            CompileExpression(s.Object, false, addressMode && s.Object is This);

                            if (pop)
                                Pop();
                            else
                                Emit(addressMode
                                        ? Opcodes.LoadFieldAddress
                                        : Opcodes.LoadField,
                                    s.Field);
                        }
                        else
                        {
                            if (!pop)
                                Emit(addressMode 
                                        ? Opcodes.LoadStaticFieldAddress 
                                        : Opcodes.LoadStaticfield, 
                                    s.Field);
                        }
                    }
                    break;

                case ExpressionType.LoadElement:
                    {
                        var s = e as LoadElement;
                        CompileExpression(s.Array, pop);
                        CompileExpression(s.Index, pop);

                        if (pop) return;

                        Emit(addressMode 
                                ? Opcodes.LoadArrayElementAddress 
                                : Opcodes.LoadArrayElement,
                            s.Array.ReturnType.ElementType);
                    }
                    break;

                case ExpressionType.LoadArgument:
                    {
                        if (pop) return;
                        var s = e as LoadArgument;
                        var paramRef = s.Parameter.IsReference;

                        if (paramRef)
                        {
                            if (addressMode)
                                Emit(Opcodes.LoadArg, s.Index);
                            else
                            {
                                Emit(Opcodes.LoadArg, s.Index);
                                Emit(Opcodes.LoadObj, s.Parameter.Type);
                            }
                        }
                        else
                            Emit(addressMode 
                                    ? Opcodes.LoadArgAddress 
                                    : Opcodes.LoadArg, 
                                s.Index);
                    }
                    break;

                case ExpressionType.StoreLocal:
                    {
                        var s = e as StoreLocal;

                        // TODO: This is a workaround for bug in sub ctor calls, remove this later
                        if (!Locals.Contains(s.Variable))
                            Locals.Add(s.Variable);

                        CompileExpression(s.Value);
                        if (!pop) Emit(Opcodes.Dup);
                        Emit(Opcodes.StoreLocal, s.Variable);
                    }
                    break;

                case ExpressionType.StoreArgument:
                    {
                        var s = e as StoreArgument;

                        if (s.Parameter.IsReference)
                        {
                            Emit(Opcodes.LoadArg, s.Index); // Loads the pointer from a out/ref argument
                            CompileExpression(s.Value);
                            var temp = StoreTemp(s.Value.ReturnType, pop);
                            Emit(Opcodes.StoreObj, s.Value.ReturnType);
                            LoadTemp(temp);
                        }
                        else
                        {
                            CompileExpression(s.Value);
                            var temp = StoreTemp(s.Value.ReturnType, pop);
                            Emit(Opcodes.StoreArg, s.Index);
                            LoadTemp(temp);
                        }

                        if (!pop && addressMode)
                            CreateIndirection(s.ReturnType);
                    }
                    break;

                case ExpressionType.StoreField:
                    {
                        var s = e as StoreField;

                        if (s.Object != null)
                        {
                            CompileObject(s.Object, s.Field);
                            CompileExpression(s.Value);

                            var temp = StoreTemp(s.Value.ReturnType, pop);
                            Emit(Opcodes.StoreField, s.Field);
                            LoadTemp(temp);
                        }
                        else
                        {
                            CompileExpression(s.Value);

                            var temp = StoreTemp(s.Value.ReturnType, pop);
                            Emit(Opcodes.StoreStaticField, s.Field);
                            LoadTemp(temp);
                        }

                        if (!pop & addressMode)
                            CreateIndirection(s.ReturnType);
                    }
                    break;

                case ExpressionType.StoreThis:
                    {
                        var s = e as StoreThis;

                        Emit(Opcodes.This);

                        if (!pop & addressMode)
                            Emit(Opcodes.Dup);

                        CompileExpression(s.Value);
                        Emit(Opcodes.StoreObj, s.Value.ReturnType);
                    }
                    break;

                case ExpressionType.StoreElement:
                    {
                        var s = e as StoreElement;

                        CompileExpression(s.Array);
                        CompileExpression(s.Index);
                        CompileExpression(s.Value);

                        var temp = StoreTemp(s.Value.ReturnType, pop);
                        Emit(Opcodes.StoreArrayElement, s.Value.ReturnType);
                        LoadTemp(temp);

                        if (!pop & addressMode)
                            CreateIndirection(s.ReturnType);
                    }
                    break;

                case ExpressionType.SetProperty:
                    {
                        var s = e as SetProperty;
                        CompileObject(s.Object, s.Property);

                        foreach (var arg in s.Arguments)
                            CompileExpression(arg);

                        CompileExpression(s.Value);

                        var temp = StoreTemp(s.Value.ReturnType, pop);

                        Call(s.Object, s.Property.SetMethod);

                        LoadTemp(temp);

                        if (!pop & addressMode)
                            CreateIndirection(s.ReturnType);
                    }
                    break;

                case ExpressionType.GetProperty:
                    {
                        var s = e as GetProperty;
                        CompileObject(s.Object, s.Property);

                        foreach (var arg in s.Arguments)
                            CompileExpression(arg);

                        if (s.Property.DeclaringType.IsArray && s.Property.Parameters.Length == 0 && s.Property.UnoName == "Length")
                            Emit(Opcodes.LoadArrayLength);
                        else
                            Call(s.Object, s.Property.GetMethod);

                        if (pop)
                            Pop();
                        else if (addressMode)
                            CreateIndirection(s.ReturnType);
                    }
                    break;

                case ExpressionType.AddListener:
                    {
                        var s = e as AddListener;
                        CompileObject(s.Object, s.Event);
                        CompileExpression(s.Listener);
                        Call(s.Object, s.Event.AddMethod);
                    }
                    break;

                case ExpressionType.RemoveListener:
                    {
                        var s = e as RemoveListener;
                        CompileObject(s.Object, s.Event);
                        CompileExpression(s.Listener);
                        Call(s.Object, s.Event.RemoveMethod);
                    }
                    break;

                case ExpressionType.ReferenceOp:
                    {
                        var s = e as ReferenceOp;

                        CompileExpression(s.Left);

                        if (s.Left.ReturnType.IsGenericParameter)
                            Emit(Opcodes.Box, s.Left.ReturnType);

                        CompileExpression(s.Right);

                        if (s.Right.ReturnType.IsGenericParameter)
                            Emit(Opcodes.Box, s.Right.ReturnType);

                        if (cond != null && !cond.Handled)
                        {
                            if (s.EqualityType == EqualityType.NotEqual)
                            {
                                Branch(Opcodes.BrNeq, cond.TrueLabel ?? (cond.TrueLabel = NewLabel()));
                                Branch(Opcodes.Br, cond.FalseLabel ?? (cond.FalseLabel = NewLabel()));
                            }
                            else
                            {
                                Branch(Opcodes.BrEq, cond.TrueLabel ?? (cond.TrueLabel = NewLabel()));
                                Branch(Opcodes.Br, cond.FalseLabel ?? (cond.FalseLabel = NewLabel()));
                            }

                            cond.Handled = true;
                            return;
                        }

                        Emit(s.EqualityType == EqualityType.NotEqual 
                                ? Opcodes.Neq 
                                : Opcodes.Eq);

                        if (pop)
                            Pop();
                        else if (addressMode)
                            CreateIndirection(e.ReturnType);
                    }
                    break;

                case ExpressionType.BranchOp:
                    {
                        var s = e as BranchOp;

                        if (s.BranchType == BranchType.And)
                        {
                            if (cond != null && !cond.Handled)
                            {
                                var brtrue = NewLabel();
                                CompileCondition(s.Left, ConditionSequence.TrueFollows,  brtrue, cond.FalseLabel ?? (cond.FalseLabel = NewLabel()));

                                MarkLabel(brtrue);
                                CompileCondition(s.Right, ConditionSequence.NoneFollows, cond.TrueLabel ?? (cond.TrueLabel = NewLabel()), cond.FalseLabel ?? (cond.FalseLabel = NewLabel()));

                                cond.Handled = true;
                            }
                            else
                            {
                                var c = CompileCondition(s.Left, ConditionSequence.TrueFollows);

                                var brafter = NewLabel();

                                MarkLabel(c.TrueLabel);
                                CompileExpression(s.Right);
                                Branch(Opcodes.Br, brafter);

                                MarkLabel(c.FalseLabel);
                                Emit(Opcodes.Constant, false);

                                MarkLabel(brafter);
                            }
                        }
                        else
                        {
                            if (cond != null && !cond.Handled)
                            {
                                var brfalse = NewLabel();
                                CompileCondition(s.Left, ConditionSequence.FalseFollows, cond.TrueLabel ?? (cond.TrueLabel = NewLabel()), brfalse);

                                MarkLabel(brfalse);
                                CompileCondition(s.Right, ConditionSequence.NoneFollows, cond.TrueLabel ?? (cond.TrueLabel = NewLabel()), cond.FalseLabel ?? (cond.FalseLabel = NewLabel()));

                                cond.Handled = true;
                            }
                            else
                            {
                                var c = CompileCondition(s.Left, ConditionSequence.TrueFollows);

                                var brafter = NewLabel();

                                MarkLabel(c.TrueLabel);
                                Emit(Opcodes.Constant, true);
                                Branch(Opcodes.Br, brafter);

                                MarkLabel(c.FalseLabel);
                                CompileExpression(s.Right);

                                MarkLabel(brafter);
                            }
                        }
                    }
                    break;

                case ExpressionType.ConditionalOp:
                    {
                        var s = e as ConditionalOp;
                        var brtrue = NewLabel();
                        var brfalse = NewLabel();
                        var brafter = NewLabel();
                        var cc = CompileCondition(s.Condition, ConditionSequence.TrueFollows, brtrue, brfalse);

                        if (!cc.CanSkipTrue)
                        {
                            MarkLabel(brtrue);
                            CompileExpression(s.True, pop, addressMode);
                            Branch(Opcodes.Br, brafter);
                        }

                        if (!cc.CanSkipFalse)
                        {
                            MarkLabel(brfalse);
                            CompileExpression(s.False, pop, addressMode);
                        }

                        MarkLabel(brafter);
                    }
                    break;

                case ExpressionType.NullOp:
                    {
                        var s = e as NullOp;
                        var brnull = NewLabel();
                        var brafter = NewLabel();

                        CompileExpression(s.Left);
                        Emit(Opcodes.Dup);

                        if (s.Left.ReturnType.IsGenericParameter)
                            Emit(Opcodes.Box, s.Left.ReturnType);

                        Branch(Opcodes.BrNotNull, brafter);

                        MarkLabel(brnull);
                        Emit(Opcodes.Pop);
                        CompileExpression(s.Right);

                        MarkLabel(brafter);

                        if (pop) Pop();
                    }
                    break;

                case ExpressionType.CallCast:
                    {
                        var s = e as CallCast;
                        CompileExpression(s.Operand);

                        // TODO: Many casts are NOOP - i.e. byte -> char, char -> int
                        switch (s.Operand.ReturnType.BuiltinType)
                        {
                            case BuiltinType.Char:
                            case BuiltinType.Byte:
                            case BuiltinType.SByte:
                            case BuiltinType.UShort:
                            case BuiltinType.Short:
                            case BuiltinType.UInt:
                            case BuiltinType.Int:
                            case BuiltinType.ULong:
                            case BuiltinType.Long:
                            case BuiltinType.Float:
                            case BuiltinType.Double:
                                switch (s.ReturnType.BuiltinType)
                                {
                                    case BuiltinType.Char: Emit(Opcodes.ConvChar); break;
                                    case BuiltinType.Byte: Emit(Opcodes.ConvByte); break;
                                    case BuiltinType.SByte: Emit(Opcodes.ConvSByte); break;
                                    case BuiltinType.UShort: Emit(Opcodes.ConvUShort); break;
                                    case BuiltinType.Short: Emit(Opcodes.ConvShort); break;
                                    case BuiltinType.UInt: Emit(Opcodes.ConvUInt); break;
                                    case BuiltinType.Int: Emit(Opcodes.ConvInt); break;
                                    case BuiltinType.ULong: Emit(Opcodes.ConvULong); break;
                                    case BuiltinType.Long: Emit(s.Operand.ReturnType.IsUnsignedType ? Opcodes.ConvULong : Opcodes.ConvLong); break;
                                    case BuiltinType.Float: Emit(Opcodes.ConvFloat); break;
                                    case BuiltinType.Double: Emit(Opcodes.ConvDouble); break;
                                    default: Call(null, s.Cast); break;
                                }

                                break;

                            default:
                                Call(null, s.Cast);
                                break;
                        }

                        if (pop)
                            Pop();
                        else if (addressMode)
                            CreateIndirection(s.ReturnType);
                    }
                    break;

                case ExpressionType.CallConstructor:
                    {
                        var s = e as CallConstructor;

                        Emit(Opcodes.This);

                        for (int i = 0; i < s.Arguments.Length; i++)
                            CompileExpression(s.Arguments[i]);

                        Call(null, s.Constructor);
                    }
                    break;

                case ExpressionType.CallMethod:
                    {
                        var s = e as CallMethod;
                        CompileObject(s.Object, s.Method);

                        for (int i = 0; i < s.Arguments.Length; i++)
                            CompileExpression(s.Arguments[i]);

                        Call(s.Object, s.Method);

                        if (!s.ReturnType.IsVoid)
                        {
                            if (pop)
                                Pop();
                            else if (addressMode)
                                CreateIndirection(s.ReturnType);
                        }
                    }
                    break;

                case ExpressionType.CallDelegate:
                    {
                        var s = e as CallDelegate;
                        CompileExpression(s.Object);

                        foreach (var arg in s.Arguments)
                            CompileExpression(arg);

                        Emit(Opcodes.CallDelegate, s.Object.ReturnType);

                        if (!s.ReturnType.IsVoid)
                        {
                            if (pop)
                                Pop();
                            else if (addressMode)
                                CreateIndirection(s.ReturnType);
                        }
                    }
                    break;

                case ExpressionType.CallBinOp:
                    {
                        CompileBinOp(e as CallBinOp, pop, cond);
                        if (addressMode) CreateIndirection(e.ReturnType);
                    }
                    break;

                case ExpressionType.CallUnOp:
                    {
                        CompileUnOp(e as CallUnOp, pop);
                        if (addressMode) CreateIndirection(e.ReturnType);
                    }
                    break;

                case ExpressionType.CastOp:
                    {
                        var s = e as CastOp;
                        CompileExpression(s.Operand);

                        if (s.ReturnType.IsReferenceType && !s.ReturnType.IsGenericParameter)
                        {
                            if (s.Operand.ReturnType.IsValueType || s.Operand.ReturnType.IsGenericParameter)
                                Emit(Opcodes.Box, s.Operand.ReturnType);
                            else
                                Emit(Opcodes.CastClass, s.ReturnType);

                            if (pop) Pop();
                        }
                        else if (s.Operand.ReturnType.IsReferenceType && !s.Operand.ReturnType.IsGenericParameter)
                        {
                            if (s.ReturnType.IsValueType)
                            {
                                Emit(Opcodes.Unbox, s.ReturnType);
                                Emit(Opcodes.LoadObj, s.ReturnType);

                                if (pop)
                                    Pop();
                                else if (addressMode)
                                    CreateIndirection(e.ReturnType);
                            }
                            else if (s.ReturnType.IsGenericParameter)
                            {
                                Emit(Opcodes.UnboxAny, s.ReturnType);

                                if (pop)
                                    Pop();
                                else if (addressMode)
                                    CreateIndirection(e.ReturnType);
                            }
                            else
                            {
                                Emit(Opcodes.CastClass, s.ReturnType);
                                if (pop) Pop();
                            }
                        }
                        else
                        {
                            if (addressMode && !pop)
                                CreateIndirection(s.ReturnType);
                        }
                    }
                    break;

                case ExpressionType.NewObject:
                    {
                        var s = e as NewObject;
                        foreach (var arg in s.Arguments)
                            CompileExpression(arg);

                        Emit(Opcodes.NewObject, s.Constructor);

                        if (pop)
                            Pop();
                        else if (s.ReturnType.IsValueType && addressMode)
                            CreateIndirection(e.ReturnType);
                    }
                    break;

                case ExpressionType.NewDelegate:
                    {
                        var s = e as NewDelegate;

                        if (s.Object != null)
                            CompileExpression(s.Object);
                        else
                            Emit(Opcodes.Null);

                        if (s.Object == null ||
                            s.Object is Base)
                        {
                            Emit(Opcodes.LoadFunction, s.Method);
                        }
                        else
                        {
                            Emit(Opcodes.Dup);
                            Emit(Opcodes.LoadFunctionVirtual, s.Method);
                        }

                        Emit(Opcodes.NewDelegate, s.ReturnType);
                    }
                    break;

                case ExpressionType.NewArray:
                    {
                        var s = e as NewArray;

                        if (s.Size != null)
                            CompileExpression(s.Size);
                        else
                            Emit(Opcodes.Constant, s.Initializers.Length);

                        Emit(Opcodes.NewArray, ((RefArrayType)s.ReturnType).ElementType);

                        if (s.Initializers != null)
                        {
                            for (int i = 0; i < s.Initializers.Length; i++)
                            {
                                Emit(Opcodes.Dup);
                                Emit(Opcodes.Constant, i);
                                CompileExpression(s.Initializers[i]);
                                Emit(Opcodes.StoreArrayElement, s.Initializers[i].ReturnType);
                            }
                        }

                        if (pop) Pop();
                    }
                    break;

                case ExpressionType.FixOp:
                    CompileFixOp(e as FixOp, pop, addressMode);
                    break;

                case ExpressionType.Swizzle:
                    {
                        var sw = e as Swizzle;

                        CompileExpression(sw.Object);
                        var temp = StoreTemp(sw.Object.ReturnType, false);

                        foreach (var f in sw.Fields)
                        {
                            LoadTemp(temp);
                            Emit(Opcodes.LoadField, f);
                        }

                        Emit(Opcodes.NewObject, sw.Constructor);

                        if (pop)
                            Pop();
                        else if (sw.ReturnType.IsValueType && addressMode)
                            CreateIndirection(e.ReturnType);
                    }
                    break;

                // Shader expression types must be handled to be able to use the bytecode backend for control flow validation of shaders,
                // as well as generating bytecode/assembly shaders
                case ExpressionType.RuntimeConst:
                    {
                        var s = e as RuntimeConst;
                        Emit(Opcodes.GetShaderConst, s.State.RuntimeConstants[s.Index]);
                    }
                    break;

                case ExpressionType.LoadUniform:
                    {
                        var s = e as LoadUniform;
                        Emit(Opcodes.LoadShaderUniform, s.State.Uniforms[s.Index]);
                    }
                    break;

                case ExpressionType.LoadPixelSampler:
                    {
                        var s = e as LoadPixelSampler;
                        Emit(Opcodes.LoadShaderPixelSampler, s.State.PixelSamplers[s.Index]);
                    }
                    break;

                case ExpressionType.LoadVarying:
                    {
                        var s = e as LoadVarying;
                        Emit(Opcodes.LoadShaderVarying, s.State.Varyings[s.Index]);
                    }
                    break;

                case ExpressionType.LoadVertexAttrib:
                    {
                        var s = e as LoadVertexAttrib;
                        Emit(Opcodes.LoadShaderVarying, s.State.VertexAttributes[s.Index]);
                    }
                    break;

                case ExpressionType.CallShader:
                    {
                        var s = e as CallShader;

                        for (int i = 0; i < s.Arguments.Length; i++)
                            CompileExpression(s.Arguments[i]);

                        Call(null, s.Function);

                        if (!s.ReturnType.IsVoid)
                        {
                            if (pop)
                                Pop();
                            else if (addressMode)
                                CreateIndirection(s.ReturnType);
                        }
                    }
                    break;

                case ExpressionType.LoadPtr:
                    {
                        var s = e as LoadPtr;
                        CompileExpression(s.Argument, pop, addressMode, cond);
                    }
                    break;

                default:
                    throw new Exception("<" + e.ExpressionType + "> is not supported in bytecode backend - at " + e.Source);
            }
        }

        void CompileObject(Expression e, Member member)
        {
            if (e == null)
                return;

            CompileExpression(e);

            if (e.ReturnType.IsGenericParameter && 
                member.DeclaringType.IsReferenceType && (
                    member.DeclaringType.BuiltinType != BuiltinType.Object ||
                    !member.IsVirtual))
                Emit(Opcodes.Box, e.ReturnType);
        }
    }
}
