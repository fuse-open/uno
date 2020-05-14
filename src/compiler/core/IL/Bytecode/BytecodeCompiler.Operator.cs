using System;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.Bytecode;
using Uno.Compiler.API.Domain.IL.Expressions;

namespace Uno.Compiler.Core.IL.Bytecode
{
    public partial class BytecodeCompiler
    {
        Opcodes InvertOp(Opcodes op)
        {
            switch (op)
            {
                case Opcodes.Lt: return Opcodes.Gte;
                case Opcodes.Lte: return Opcodes.Gt;
                case Opcodes.Gt: return Opcodes.Lte;
                case Opcodes.Gte: return Opcodes.Lt;
                case Opcodes.Eq: return Opcodes.Neq;
                case Opcodes.Neq: return Opcodes.Eq;
                case Opcodes.Lt_Unsigned: return Opcodes.Gte_Unsigned;
                case Opcodes.Lte_Unsigned: return Opcodes.Gt_Unsigned;
                case Opcodes.Gt_Unsigned: return Opcodes.Lte_Unsigned;
                case Opcodes.Gte_Unsigned: return Opcodes.Lt_Unsigned;

                case Opcodes.BrLt: return Opcodes.BrGte;
                case Opcodes.BrLte: return Opcodes.BrGt;
                case Opcodes.BrGt: return Opcodes.BrLte;
                case Opcodes.BrGte: return Opcodes.BrLt;
                case Opcodes.BrEq: return Opcodes.BrNeq;
                case Opcodes.BrNeq: return Opcodes.BrEq;
                case Opcodes.BrLt_Unsigned: return Opcodes.BrGte_Unsigned;
                case Opcodes.BrLte_Unsigned: return Opcodes.BrGt_Unsigned;
                case Opcodes.BrGt_Unsigned: return Opcodes.BrLte_Unsigned;
                case Opcodes.BrGte_Unsigned: return Opcodes.BrLt_Unsigned;
                case Opcodes.BrNotNull: return Opcodes.BrNull;
                case Opcodes.BrNull: return Opcodes.BrNotNull;
                case Opcodes.BrTrue: return Opcodes.BrFalse;
                case Opcodes.BrFalse: return Opcodes.BrFalse;
                default: throw new Exception();
            }
        }

        Opcodes ToBranchOp(Opcodes op)
        {
            switch (op)
            {
                case Opcodes.Lt: return Opcodes.BrLt;
                case Opcodes.Lte: return Opcodes.BrLte;
                case Opcodes.Gt: return Opcodes.BrGt;
                case Opcodes.Gte: return Opcodes.BrGte;
                case Opcodes.Eq: return Opcodes.BrEq;
                case Opcodes.Neq: return Opcodes.BrNeq;
                case Opcodes.Lt_Unsigned: return Opcodes.BrLt_Unsigned;
                case Opcodes.Lte_Unsigned: return Opcodes.BrLte_Unsigned;
                case Opcodes.Gt_Unsigned: return Opcodes.BrGt_Unsigned;
                case Opcodes.Gte_Unsigned: return Opcodes.BrGte_Unsigned;
                default: throw new Exception();
            }
        }

        public void CompileBinOp(CallBinOp s, bool pop, Condition cond)
        {
            Opcodes builtInOp = Opcodes.Null;
            bool isIntrinsic = false;

            switch (s.Operator.DeclaringType.BuiltinType)
            {
                case BuiltinType.Bool:
                case BuiltinType.Char:
                case BuiltinType.Byte:
                case BuiltinType.SByte:
                case BuiltinType.UShort:
                case BuiltinType.Short:
                case BuiltinType.ULong:
                case BuiltinType.Long:
                case BuiltinType.UInt:
                case BuiltinType.Int:
                case BuiltinType.Float:
                case BuiltinType.Double:
                case BuiltinType.Object:
                    {
                        isIntrinsic = true;
                        switch (s.Operator.Type)
                        {
                            case OperatorType.Addition: builtInOp = Opcodes.Add; break;
                            case OperatorType.Subtraction: builtInOp = Opcodes.Sub; break;
                            case OperatorType.Multiply: builtInOp = Opcodes.Mul; break;
                            case OperatorType.Division:
                                if (s.Left.ReturnType.IsUnsignedType)
                                    builtInOp = Opcodes.Div_Un;
                                else
                                    builtInOp = Opcodes.Div;
                                break;
                            case OperatorType.Modulus:
                                if (s.Left.ReturnType.IsUnsignedType)
                                    builtInOp = Opcodes.Rem_Un;
                                else
                                    builtInOp = Opcodes.Rem;
                                break;
                            case OperatorType.BitwiseAnd: builtInOp = Opcodes.And; break;
                            case OperatorType.BitwiseOr: builtInOp = Opcodes.Or; break;
                            case OperatorType.ExclusiveOr: builtInOp = Opcodes.Xor; break;
                            case OperatorType.LeftShift: builtInOp = Opcodes.Shl; break;
                            case OperatorType.RightShift:
                                if (s.Left.ReturnType.IsUnsignedType)
                                    builtInOp = Opcodes.Shr_Un;
                                else
                                    builtInOp = Opcodes.Shr;
                                break;

                        }
                    }
                    break;
            }

            if (builtInOp != Opcodes.Null)
            {
                CompileExpression(s.Left);
                CompileExpression(s.Right);
                Emit(builtInOp);
                return;
            }

            if (isIntrinsic)
            {
                switch (s.Operator.Type)
                {
                    case OperatorType.LessThan: builtInOp = Opcodes.Lt; break;
                    case OperatorType.LessThanOrEqual: builtInOp = Opcodes.Lte; break;
                    case OperatorType.GreaterThan: builtInOp = Opcodes.Gt; break;
                    case OperatorType.GreaterThanOrEqual: builtInOp = Opcodes.Gte; break;
                    case OperatorType.Equality: builtInOp = Opcodes.Eq; break;
                    case OperatorType.Inequality: builtInOp = Opcodes.Neq; break;
                }

                if (s.Left.ReturnType.IsUnsignedType)
                {
                    switch (s.Operator.Type)
                    {
                        case OperatorType.LessThan: builtInOp = Opcodes.Lt_Unsigned; break;
                        case OperatorType.LessThanOrEqual: builtInOp = Opcodes.Lte_Unsigned; break;
                        case OperatorType.GreaterThan: builtInOp = Opcodes.Gt_Unsigned; break;
                        case OperatorType.GreaterThanOrEqual: builtInOp = Opcodes.Gte_Unsigned; break;
                    }
                }

                if (builtInOp != Opcodes.Null)
                {
                    CompileExpression(s.Left);
                    CompileExpression(s.Right);

                    if (cond != null && !cond.Handled)
                    {
                        if (cond.Sequence == ConditionSequence.TrueFollows)
                        {
                            builtInOp = InvertOp(builtInOp);
                            builtInOp = ToBranchOp(builtInOp);
                            Branch(builtInOp, cond.FalseLabel ?? (cond.FalseLabel = NewLabel()));
                        }
                        else if (cond.Sequence == ConditionSequence.FalseFollows)
                        {
                            builtInOp = ToBranchOp(builtInOp);
                            Branch(builtInOp, cond.TrueLabel ?? (cond.TrueLabel = NewLabel()));
                        }
                        else
                        {
                            builtInOp = ToBranchOp(builtInOp);
                            Branch(builtInOp, cond.TrueLabel ?? (cond.TrueLabel = NewLabel()));
                            Branch(Opcodes.Br, (cond.FalseLabel ?? (cond.FalseLabel = NewLabel())));
                        }

                        cond.Handled = true;
                        return;
                    }

                    Emit(builtInOp);
                    return;
                }
            }

            CompileExpression(s.Left);
            CompileExpression(s.Right);
            Call(null, s.Operator);

            if (pop && !s.Operator.ReturnType.IsVoid)
                Pop();
        }

        public void CompileUnOp(CallUnOp s, bool pop)
        {
            var builtInOp = Opcodes.Null;

            switch (s.Operator.DeclaringType.BuiltinType)
            {
                case BuiltinType.Bool:
                case BuiltinType.Char:
                case BuiltinType.Byte:
                case BuiltinType.SByte:
                case BuiltinType.UShort:
                case BuiltinType.Short:
                case BuiltinType.ULong:
                case BuiltinType.Long:
                case BuiltinType.UInt:
                case BuiltinType.Int:
                case BuiltinType.Float:
                case BuiltinType.Double:
                    switch (s.Operator.Type)
                    {
                        case OperatorType.OnesComplement: builtInOp = Opcodes.BitwiseNot; break;
                        case OperatorType.LogicalNot: builtInOp = Opcodes.LogNot; break;
                        case OperatorType.UnaryNegation: builtInOp = Opcodes.Neg; break;
                    }
                    break;
            }

            if (builtInOp != Opcodes.Null)
            {
                CompileExpression(s.Operand);
                Emit(builtInOp);
                return;
            }

            CompileExpression(s.Operand);
            Call(null, s.Operator);

            if (pop && !s.Operator.ReturnType.IsVoid)
                Pop();
        }

    }
}
