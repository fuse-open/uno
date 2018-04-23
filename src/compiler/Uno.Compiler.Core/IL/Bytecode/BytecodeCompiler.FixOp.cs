using System;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.Bytecode;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Logging;

namespace Uno.Compiler.Core.IL.Bytecode
{
    public partial class BytecodeCompiler
    {
        void LoadOne(DataType dt)
        {
            switch (dt.BuiltinType)
            {
                case BuiltinType.Byte:
                    Emit(Opcodes.Constant, (byte) 1);
                    break;
                case BuiltinType.Char:
                    Emit(Opcodes.Constant, (char) 1);
                    break;
                case BuiltinType.Double:
                    Emit(Opcodes.Constant, (double) 1);
                    break;
                case BuiltinType.Int:
                    Emit(Opcodes.Constant, 1);
                    break;
                case BuiltinType.Float:
                    Emit(Opcodes.Constant, (float) 1);
                    break;
                case BuiltinType.Long:
                    Emit(Opcodes.Constant, (long) 1);
                    break;
                case BuiltinType.SByte:
                    Emit(Opcodes.Constant, (sbyte) 1);
                    break;
                case BuiltinType.Short:
                    Emit(Opcodes.Constant, (short) 1);
                    break;
                case BuiltinType.UInt:
                    Emit(Opcodes.Constant, (uint) 1);
                    break;
                case BuiltinType.ULong:
                    Emit(Opcodes.Constant, (ulong) 1);
                    break;
                case BuiltinType.UShort:
                    Emit(Opcodes.Constant, (ushort) 1);
                    break;
                default:
                    throw new InvalidOperationException("Not a constant: " + dt);
            }
        }

        void Fix(FixOp fo, Action store, bool pop)
        {
            switch (fo.Operator)
            {
                case FixOpType.IncreaseBefore:
                case FixOpType.DecreaseBefore:
                    {
                        LoadOne(fo.Operand.ReturnType); // obj value 1
                        Emit(fo.Operator == FixOpType.IncreaseBefore 
                                ? Opcodes.Add 
                                : Opcodes.Sub);
                        var temp = StoreTemp(fo.Operand.ReturnType, pop); // obj (value+-1)
                        store();
                        LoadTemp(temp); // (value+-1)
                    }
                    break;
                case FixOpType.IncreaseAfter:
                case FixOpType.DecreaseAfter:
                    {
                        var temp = StoreTemp(fo.Operand.ReturnType, pop); // obj value
                        LoadOne(fo.Operand.ReturnType); // obj value 1
                        Emit(fo.Operator == FixOpType.IncreaseAfter
                                ? Opcodes.Add
                                : Opcodes.Sub);
                        store();
                        LoadTemp(temp); // value
                    }
                    break;
            }
        }

        void CompileFixOp(FixOp fo, bool pop, bool addressMode)
        {
            switch (fo.Operand.ExpressionType)
            {
                case ExpressionType.LoadLocal:
                    {
                        var ll = fo.Operand as LoadLocal;

                        Emit(Opcodes.LoadLocal, ll.Variable);
                        Fix(fo, () => Emit(Opcodes.StoreLocal, ll.Variable), pop);
                    }
                    break;

                case ExpressionType.LoadField:
                    {
                        var lf = fo.Operand as LoadField;

                        if (lf.Object != null)
                        {
                            CompileExpression(lf.Object); // obj
                            Emit(Opcodes.Dup); // obj obj
                            Emit(Opcodes.LoadField, lf.Field); // obj value
                            Fix(fo, () => Emit(Opcodes.StoreField, lf.Field), pop);
                        }
                        else
                        {
                            Emit(Opcodes.LoadStaticfield, lf.Field); // value
                            Fix(fo, () => Emit(Opcodes.StoreStaticField, lf.Field), pop);
                        }
                    }
                    break;

                case ExpressionType.LoadElement:
                    {
                        var lae = fo.Operand as LoadElement;
                        var elmType = lae.Array.ReturnType.ElementType;

                        CompileExpression(lae.Array); // array
                        Emit(Opcodes.Dup); // array array

                        CompileExpression(lae.Index); // array array index
                        var tempIndex = StoreTemp(lae.Index.ReturnType, false);

                        Emit(Opcodes.LoadArrayElement, elmType); // array value

                        Fix(fo, () =>
                            {
                                // array (value+-1)
                                var tempValue = StoreTempDontDup(elmType); // array
                                LoadTemp(tempIndex); // array index
                                LoadTemp(tempValue); // array index value
                                Emit(Opcodes.StoreArrayElement, elmType); //
                            }, pop);
                    }
                    break;

                case ExpressionType.LoadArgument:
                    {
                        var lp = fo.Operand as LoadArgument;

                        var paramRef = lp.Parameter.IsReference;

                        if (!paramRef)
                        {
                            Emit(Opcodes.LoadArg, lp.Index); // value
                            Fix(fo, () => Emit(Opcodes.StoreArg, lp.Index), pop);
                        }
                        else
                        {
                            Emit(Opcodes.LoadArg, lp.Index); // argptr
                            Emit(Opcodes.LoadArg, lp.Index); // argptr argptr
                            Emit(Opcodes.LoadObj, lp.Parameter.Type); // argpr value
                            Fix(fo, () => Emit(Opcodes.StoreObj, lp.Parameter.Type), pop);
                        }
                    }
                    break;

                default:
                    throw new FatalException(fo.Source, ErrorCode.I0027, "Unhandled fix-operator");
            }

            if (!pop && addressMode)
                CreateIndirection(fo.Operand.ReturnType);
        }
    }
}
