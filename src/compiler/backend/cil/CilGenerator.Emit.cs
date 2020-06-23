using System;
using System.Collections.Generic;
using IKVM.Reflection.Emit;
using Uno.Compiler.API;
using Uno.Compiler.API.Backends;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.Bytecode;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Backends.CIL
{
    partial class CilGenerator
    {
        void EmitFunction(ILGenerator cil, Function f)
        {
            var document = GetDocument(f.Source.FullPath);

            if (!f.HasBody)
            {
                if (!f.IsAbstract && !f.IsPInvokable(_essentials, Log))
                    Log.Error(f.Source, ErrorCode.E0093, f.Quote() + " does not provide an implementation");

                return;
            }

            var bc = f.CreateBytecodeCompiler();
            bc.Compile();

            var bytecode = bc.Code;
            var locals = bc.Locals;
            var localBuilders = new LocalBuilder[locals.Count];
            var labels = bc.Labels;
            var cilLabels = new Dictionary<API.Domain.Bytecode.Label, IKVM.Reflection.Emit.Label>();

            foreach (var l in labels)
            {
                if (l.Offset != -1)
                {
                    var x = cil.DefineLabel();
                    cilLabels.Add(l, x);
                }
            }

            // Create locals
            for (int i = 0; i < locals.Count; i++)
            {
                var t = _linker.GetType(locals[i].ValueType);
                var localBuilder = cil.DeclareLocal(t);
                localBuilder.SetLocalSymInfo(locals[i].Name);
                localBuilders[i] = localBuilder;
            }

            cil.BeginScope();

            // Translate bytecode
            for (int i = 0; i < bytecode.Count; i++)
            {
                var b = bytecode[i];

                switch (b.Opcode)
                {
                    case Opcodes.Nop:
                        break;

                    case Opcodes.MarkSource:
                        {
                            var s = (Source) b.Argument;

                            if (s.Line > 0)
                            {
                                var line = s.Line;
                                var column = s.Column != 0 ? s.Column : 1;
                                cil.MarkSequencePoint(document, line, column, s.EndLine != 0 ? s.EndLine : line, s.EndColumn != 0 ? s.EndColumn : column);
                                AddLocation(cil.ILOffset, s.FullPath, s.Line, s.Column);
                            }
                        }
                        break;

                    case Opcodes.MarkLabel:
                        {
                            var label = b.Argument as API.Domain.Bytecode.Label;
                            cil.MarkLabel(cilLabels[label]);
                        }
                        break;

                    case Opcodes.Constrained:
                        cil.Emit(OpCodes.Constrained, _linker.GetType((DataType) b.Argument));
                        break;
                    case Opcodes.BeginExceptionBlock:
                        cil.BeginExceptionBlock();
                        break;
                    case Opcodes.EndExceptionBlock:
                        cil.EndExceptionBlock();
                        break;
                    case Opcodes.BeginCatchBlock:
                        cil.BeginCatchBlock(_linker.GetType((DataType) b.Argument));
                        break;
                    case Opcodes.BeginFinallyBlock:
                        cil.BeginFinallyBlock();
                        break;
                    case Opcodes.Null:
                        cil.Emit(OpCodes.Ldnull);
                        break;
                    case Opcodes.NewObject:
                        {
                            var ctor = (Constructor) b.Argument;
                            if (ctor.DeclaringType.IsGenericParameter)
                                cil.Emit(OpCodes.Call, _linker.System_Activator_CreateInstance.MakeGenericMethod(_linker.GetType(ctor.DeclaringType)));
                            else
                                cil.Emit(OpCodes.Newobj, _linker.GetConstructor((Constructor) b.Argument));
                        }
                        break;
                    case Opcodes.NewArray:
                        cil.Emit(OpCodes.Newarr, _linker.GetType((DataType) b.Argument));
                        break;
                    case Opcodes.NewDelegate:
                        cil.Emit(OpCodes.Newobj, _linker.GetDelegateConstructor((DelegateType) b.Argument));
                        break;
                    case Opcodes.Box:
                        cil.Emit(OpCodes.Box, _linker.GetType((DataType) b.Argument));
                        break;
                    case Opcodes.Unbox:
                        cil.Emit(OpCodes.Unbox, _linker.GetType((DataType) b.Argument));
                        break;
                    case Opcodes.UnboxAny:
                        cil.Emit(OpCodes.Unbox_Any, _linker.GetType((DataType) b.Argument));
                        break;
                    case Opcodes.Constant:
                        EmitConstant(cil, b.Argument);
                        break;
                    case Opcodes.DefaultInit:
                        {
                            var type = _linker.GetType((DataType) b.Argument);
                            var temp = cil.DeclareLocal(type);
                            EmitLoadLocal(cil, temp, true);
                            cil.Emit(OpCodes.Initobj, type);
                            EmitLoadLocal(cil, temp, false);
                        }
                        break;
                    case Opcodes.TypeOf:
                        {
                            var type = _linker.GetType((DataType) b.Argument);
                            cil.Emit(OpCodes.Ldtoken, type);
                            cil.Emit(OpCodes.Call, _linker.System_Type_GetTypeFromHandle);
                        }
                        break;
                    case Opcodes.This:
                        cil.Emit(OpCodes.Ldarg_0);
                        break;
                    case Opcodes.Call:
                        if (b.Argument is Constructor)
                            cil.Emit(OpCodes.Call, _linker.GetConstructor((Constructor) b.Argument));
                        else
                            cil.Emit(OpCodes.Call, _linker.GetMethod((Function) b.Argument));
                        break;
                    case Opcodes.CallVirtual:
                        cil.Emit(OpCodes.Callvirt, _linker.GetMethod((Function) b.Argument));
                        break;
                    case Opcodes.CallDelegate:
                        cil.Emit(OpCodes.Callvirt, _linker.GetDelegateInvokeMethod((DelegateType) b.Argument));
                        break;
                    case Opcodes.Pop:
                        cil.Emit(OpCodes.Pop);
                        break;
                    case Opcodes.Dup:
                        cil.Emit(OpCodes.Dup);
                        break;
                    case Opcodes.LoadObj:
                        cil.Emit(OpCodes.Ldobj, _linker.GetType((DataType) b.Argument));
                        break;
                    case Opcodes.StoreObj:
                        cil.Emit(OpCodes.Stobj, _linker.GetType((DataType) b.Argument));
                        break;
                    case Opcodes.LoadFunction:
                        cil.Emit(OpCodes.Ldftn, _linker.GetMethod((Function) b.Argument));
                        break;
                    case Opcodes.LoadFunctionVirtual:
                        cil.Emit(OpCodes.Ldvirtftn, _linker.GetMethod((Function) b.Argument));
                        break;
                    case Opcodes.LoadArg:
                        EmitLoadArgument(cil, f, (int) b.Argument, false);
                        break;
                    case Opcodes.LoadArgAddress:
                        EmitLoadArgument(cil, f, (int) b.Argument, true);
                        break;
                    case Opcodes.StoreArg:
                        {
                            var argIndex = (int) b.Argument;
                            if (!f.IsStatic) argIndex++;

                            if (argIndex < 256) cil.Emit(OpCodes.Starg_S, (byte)argIndex);
                            else cil.Emit(OpCodes.Starg, (ushort)argIndex);
                        }
                        break;
                    case Opcodes.LoadLocal:
                        EmitLoadLocal(cil, localBuilders[locals.IndexOf((Variable) b.Argument)], false);
                        break;
                    case Opcodes.LoadLocalAddress:
                        EmitLoadLocal(cil, localBuilders[locals.IndexOf((Variable) b.Argument)], true);
                        break;
                    case Opcodes.StoreLocal:
                        EmitStoreLocal(cil, localBuilders[locals.IndexOf((Variable) b.Argument)]);
                        break;
                    case Opcodes.LoadField:
                        cil.Emit(OpCodes.Ldfld, _linker.GetField((Field) b.Argument));
                        break;
                    case Opcodes.LoadFieldAddress:
                        cil.Emit(OpCodes.Ldflda, _linker.GetField((Field) b.Argument));
                        break;
                    case Opcodes.StoreField:
                        cil.Emit(OpCodes.Stfld, _linker.GetField((Field) b.Argument));
                        break;
                    case Opcodes.LoadStaticfield:
                        cil.Emit(OpCodes.Ldsfld, _linker.GetField((Field) b.Argument));
                        break;
                    case Opcodes.LoadStaticFieldAddress:
                        cil.Emit(OpCodes.Ldsflda, _linker.GetField((Field) b.Argument));
                        break;
                    case Opcodes.StoreStaticField:
                        cil.Emit(OpCodes.Stsfld, _linker.GetField((Field) b.Argument));
                        break;
                    case Opcodes.LoadArrayElement:
                    case Opcodes.LoadArrayElementAddress:
                        {
                            bool addressMode = b.Opcode == Opcodes.LoadArrayElementAddress;

                            var elmType = b.Argument as DataType;
                            if (addressMode) cil.Emit(OpCodes.Ldelema, _linker.GetType(elmType));
                            else
                            {
                                switch (elmType.BuiltinType)
                                {
                                    case BuiltinType.Long: cil.Emit(OpCodes.Ldelem_I8); break;
                                    case BuiltinType.ULong: cil.Emit(OpCodes.Ldelem_I8); break; // There is no U8 instruction
                                    case BuiltinType.Int: cil.Emit(OpCodes.Ldelem_I4); break;
                                    case BuiltinType.UInt: cil.Emit(OpCodes.Ldelem_U4); break;
                                    case BuiltinType.Short: cil.Emit(OpCodes.Ldelem_I2); break;
                                    case BuiltinType.UShort: cil.Emit(OpCodes.Ldelem_U2); break;
                                    case BuiltinType.Byte: cil.Emit(OpCodes.Ldelem_U1); break;
                                    case BuiltinType.SByte: cil.Emit(OpCodes.Ldelem_I1); break;
                                    case BuiltinType.Float: cil.Emit(OpCodes.Ldelem_R4); break;
                                    case BuiltinType.Double: cil.Emit(OpCodes.Ldelem_R8); break;
                                    default:
                                        if (!elmType.IsValueType && !elmType.IsGenericParameter) cil.Emit(OpCodes.Ldelem_Ref);
                                        else cil.Emit(OpCodes.Ldelem, _linker.GetType(elmType));
                                        break;
                                }
                            }
                        }
                        break;
                    case Opcodes.StoreArrayElement:
                        {
                            var elmType = b.Argument as DataType;
                            switch (elmType.BuiltinType)
                            {
                                case BuiltinType.Long:
                                case BuiltinType.ULong: cil.Emit(OpCodes.Stelem_I8); break; // There is no U8 instruction
                                case BuiltinType.Int:
                                case BuiltinType.UInt: cil.Emit(OpCodes.Stelem_I4); break;
                                case BuiltinType.Short:
                                case BuiltinType.UShort: cil.Emit(OpCodes.Stelem_I2); break;
                                case BuiltinType.Byte:
                                case BuiltinType.SByte: cil.Emit(OpCodes.Stelem_I1); break;
                                case BuiltinType.Float: cil.Emit(OpCodes.Stelem_R4); break;
                                case BuiltinType.Double: cil.Emit(OpCodes.Stelem_R8); break;
                                default:
                                    if (elmType is ClassType || elmType is RefArrayType) cil.Emit(OpCodes.Stelem_Ref);
                                    else cil.Emit(OpCodes.Stelem, _linker.GetType(elmType));
                                    break;
                            }
                        }
                        break;
                    case Opcodes.LoadArrayLength:
                        cil.Emit(OpCodes.Ldlen);
                        break;

                    case Opcodes.ConvByte: cil.Emit(OpCodes.Conv_U1); break;
                    case Opcodes.ConvSByte: cil.Emit(OpCodes.Conv_I1); break;
                    case Opcodes.ConvChar: cil.Emit(OpCodes.Conv_U2); break;
                    case Opcodes.ConvUShort: cil.Emit(OpCodes.Conv_U2); break;
                    case Opcodes.ConvShort: cil.Emit(OpCodes.Conv_I2); break;
                    case Opcodes.ConvUInt: cil.Emit(OpCodes.Conv_U4); break;
                    case Opcodes.ConvInt: cil.Emit(OpCodes.Conv_I4); break;
                    case Opcodes.ConvULong: cil.Emit(OpCodes.Conv_U8); break;
                    case Opcodes.ConvLong: cil.Emit(OpCodes.Conv_I8); break;
                    case Opcodes.ConvFloat: cil.Emit(OpCodes.Conv_R4); break;
                    case Opcodes.ConvDouble: cil.Emit(OpCodes.Conv_R8); break;
                    case Opcodes.AsClass: cil.Emit(OpCodes.Isinst, _linker.GetType(b.Argument as DataType)); break;
                    case Opcodes.CastClass: cil.Emit(OpCodes.Castclass, _linker.GetType(b.Argument as DataType)); break;
                    case Opcodes.Eq:
                        cil.Emit(OpCodes.Ceq);
                        break;
                    case Opcodes.Neq:
                        cil.Emit(OpCodes.Ceq);
                        cil.Emit(OpCodes.Ldc_I4_0);
                        cil.Emit(OpCodes.Ceq);
                        break;
                    case Opcodes.Lt: cil.Emit(OpCodes.Clt); break;
                    case Opcodes.Lt_Unsigned: cil.Emit(OpCodes.Clt_Un); break;
                    case Opcodes.Lte:
                        cil.Emit(OpCodes.Cgt);
                        cil.Emit(OpCodes.Ldc_I4_0);
                        cil.Emit(OpCodes.Ceq);
                        break;
                    case Opcodes.Lte_Unsigned:
                        cil.Emit(OpCodes.Cgt_Un);
                        cil.Emit(OpCodes.Ldc_I4_0);
                        cil.Emit(OpCodes.Ceq);
                        break;
                    case Opcodes.Gt:
                        cil.Emit(OpCodes.Cgt);
                        break;
                    case Opcodes.Gt_Unsigned:
                        cil.Emit(OpCodes.Cgt_Un);
                        break;
                    case Opcodes.Gte:
                        cil.Emit(OpCodes.Clt);
                        cil.Emit(OpCodes.Ldc_I4_0);
                        cil.Emit(OpCodes.Ceq);
                        break;
                    case Opcodes.Gte_Unsigned:
                        cil.Emit(OpCodes.Clt_Un);
                        cil.Emit(OpCodes.Ldc_I4_0);
                        cil.Emit(OpCodes.Ceq);
                        break;

                    case Opcodes.Add:
                        cil.Emit(OpCodes.Add);
                        break;
                    case Opcodes.Sub:
                        cil.Emit(OpCodes.Sub);
                        break;
                    case Opcodes.Mul:
                        cil.Emit(OpCodes.Mul);
                        break;
                    case Opcodes.Div:
                        cil.Emit(OpCodes.Div);
                        break;
                    case Opcodes.Div_Un:
                        cil.Emit(OpCodes.Div_Un);
                        break;
                    case Opcodes.Rem:
                        cil.Emit(OpCodes.Rem);
                        break;
                    case Opcodes.Rem_Un:
                        cil.Emit(OpCodes.Rem_Un);
                        break;
                    case Opcodes.And:
                        cil.Emit(OpCodes.And);
                        break;
                    case Opcodes.Or:
                        cil.Emit(OpCodes.Or);
                        break;
                    case Opcodes.Xor:
                        cil.Emit(OpCodes.Xor);
                        break;
                    case Opcodes.Shl:
                        cil.Emit(OpCodes.Shl);
                        break;
                    case Opcodes.Shr:
                        cil.Emit(OpCodes.Shr);
                        break;
                    case Opcodes.Shr_Un:
                        cil.Emit(OpCodes.Shr_Un);
                        break;
                    case Opcodes.BitwiseNot:
                        cil.Emit(OpCodes.Not);
                        break;

                    case Opcodes.Leave:
                        cil.Emit(OpCodes.Leave, cilLabels[(API.Domain.Bytecode.Label) b.Argument]); break;

                    case Opcodes.LogNot: cil.Emit(OpCodes.Ldc_I4_0); cil.Emit(OpCodes.Ceq); break;
                    case Opcodes.Neg: cil.Emit(OpCodes.Neg); break;
                    case Opcodes.Br: cil.Emit(OpCodes.Br, cilLabels[(API.Domain.Bytecode.Label) b.Argument]); break;
                    case Opcodes.BrEq: cil.Emit(OpCodes.Beq, cilLabels[(API.Domain.Bytecode.Label) b.Argument]); break;
                    case Opcodes.BrNeq:
                        cil.Emit(OpCodes.Ceq);
                        cil.Emit(OpCodes.Brfalse, cilLabels[(API.Domain.Bytecode.Label) b.Argument]);
                        break;
                    case Opcodes.BrTrue: cil.Emit(OpCodes.Brtrue, cilLabels[(API.Domain.Bytecode.Label) b.Argument]); break;
                    case Opcodes.BrFalse: cil.Emit(OpCodes.Brfalse, cilLabels[(API.Domain.Bytecode.Label) b.Argument]); break;
                    case Opcodes.BrLt: cil.Emit(OpCodes.Blt, cilLabels[(API.Domain.Bytecode.Label) b.Argument]); break;
                    case Opcodes.BrLte: cil.Emit(OpCodes.Ble, cilLabels[(API.Domain.Bytecode.Label) b.Argument]); break;
                    case Opcodes.BrGt: cil.Emit(OpCodes.Bgt, cilLabels[(API.Domain.Bytecode.Label) b.Argument]); break;
                    case Opcodes.BrGte: cil.Emit(OpCodes.Bge, cilLabels[(API.Domain.Bytecode.Label) b.Argument]); break;
                    case Opcodes.BrLt_Unsigned: cil.Emit(OpCodes.Blt_Un, cilLabels[(API.Domain.Bytecode.Label) b.Argument]); break;
                    case Opcodes.BrLte_Unsigned: cil.Emit(OpCodes.Ble_Un, cilLabels[(API.Domain.Bytecode.Label) b.Argument]); break;
                    case Opcodes.BrGt_Unsigned: cil.Emit(OpCodes.Bgt_Un, cilLabels[(API.Domain.Bytecode.Label) b.Argument]); break;
                    case Opcodes.BrGte_Unsigned: cil.Emit(OpCodes.Bge_Un, cilLabels[(API.Domain.Bytecode.Label) b.Argument]); break;
                    case Opcodes.BrNull:
                        cil.Emit(OpCodes.Ldnull);
                        cil.Emit(OpCodes.Beq, cilLabels[(API.Domain.Bytecode.Label) b.Argument]);
                        break;
                    case Opcodes.BrNotNull:
                        cil.Emit(OpCodes.Ldnull);
                        cil.Emit(OpCodes.Bne_Un, cilLabels[(API.Domain.Bytecode.Label) b.Argument]);
                        break;
                    case Opcodes.Ret:
                        cil.Emit(OpCodes.Ret);
                        break;
                    case Opcodes.Throw:
                        cil.Emit(OpCodes.Throw);
                        break;

                    default:
                        throw new InvalidOperationException("Unhandled opcode in .NET backend: " + b.Opcode);
                }
            }

            cil.EndScope();
        }

        static void EmitLoadLocal(ILGenerator cil, LocalBuilder local, bool address)
        {
            if (address)
            {
                if (local.LocalIndex < 256)
                    cil.Emit(OpCodes.Ldloca_S, (byte) local.LocalIndex);
                else
                    cil.Emit(OpCodes.Ldloca, (ushort) local.LocalIndex);
            }
            else
            {
                switch (local.LocalIndex)
                {
                    case 0: cil.Emit(OpCodes.Ldloc_0); break;
                    case 1: cil.Emit(OpCodes.Ldloc_1); break;
                    case 2: cil.Emit(OpCodes.Ldloc_2); break;
                    case 3: cil.Emit(OpCodes.Ldloc_3); break;

                    default:
                        if (local.LocalIndex < 256)
                            cil.Emit(OpCodes.Ldloc_S, (byte) local.LocalIndex);
                        else
                            cil.Emit(OpCodes.Ldloc, (ushort) local.LocalIndex);

                        break;
                }
            }
        }

        static void EmitStoreLocal(ILGenerator cil, LocalBuilder local)
        {
            switch (local.LocalIndex)
            {
                case 0: cil.Emit(OpCodes.Stloc_0); break;
                case 1: cil.Emit(OpCodes.Stloc_1); break;
                case 2: cil.Emit(OpCodes.Stloc_2); break;
                case 3: cil.Emit(OpCodes.Stloc_3); break;
                default:
                    if (local.LocalIndex < 256) cil.Emit(OpCodes.Stloc_S, (byte) local.LocalIndex);
                    else cil.Emit(OpCodes.Stloc, (ushort) local.LocalIndex);
                    break;
            }
        }

        static void EmitLoadArgument(ILGenerator cil, Function func, int index, bool address)
        {
            if (!func.IsStatic) index++;

            if (address)
            {
                if (index < 256) cil.Emit(OpCodes.Ldarga_S, (byte)index);
                else cil.Emit(OpCodes.Ldarga, (ushort)index);
            }
            else
            {
                if (index == 0) cil.Emit(OpCodes.Ldarg_0);
                else if (index == 1) cil.Emit(OpCodes.Ldarg_1);
                else if (index == 2) cil.Emit(OpCodes.Ldarg_2);
                else if (index == 3) cil.Emit(OpCodes.Ldarg_3);
                else if (index < 256) cil.Emit(OpCodes.Ldarg_S, (byte)index);
                else cil.Emit(OpCodes.Ldarg, (ushort)index);
            }
        }

        static void EmitConstant(ILGenerator cil, int v)
        {
            if (v == 0) cil.Emit(OpCodes.Ldc_I4_0);
            else if (v == 1) cil.Emit(OpCodes.Ldc_I4_1);
            else if (v == 2) cil.Emit(OpCodes.Ldc_I4_2);
            else if (v == 3) cil.Emit(OpCodes.Ldc_I4_3);
            else if (v == 4) cil.Emit(OpCodes.Ldc_I4_4);
            else if (v == 5) cil.Emit(OpCodes.Ldc_I4_5);
            else if (v == 6) cil.Emit(OpCodes.Ldc_I4_6);
            else if (v == 7) cil.Emit(OpCodes.Ldc_I4_7);
            else if (v == 8) cil.Emit(OpCodes.Ldc_I4_8);
            else if (v == -1) cil.Emit(OpCodes.Ldc_I4_M1);
            else if (v >= -128 && v < 128) cil.Emit(OpCodes.Ldc_I4_S, (sbyte)v);
            else cil.Emit(OpCodes.Ldc_I4, v);
        }

        static void EmitConstant(ILGenerator cil, object c)
        {
            if (c is long || c is ulong)
            {
                long v = (long)(dynamic)c;

                if (v > uint.MaxValue || v < int.MinValue)
                {
                    cil.Emit(OpCodes.Ldc_I8, v);
                    return;
                }

                if (v > int.MaxValue)
                    c = (uint)v;
                else
                    c = (int)v;

                EmitConstant(cil, (int)(dynamic)c);
                cil.Emit(v < 0 ? OpCodes.Conv_I8 : OpCodes.Conv_U8);
            }
            else if (c is byte || c is sbyte || c is short || c is ushort || c is char || c is int || c is uint)
            {
                EmitConstant(cil, (int)(dynamic)c);
            }
            else if (c is float)
            {
                cil.Emit(OpCodes.Ldc_R4, (float)c);
            }
            else if (c is double)
            {
                cil.Emit(OpCodes.Ldc_R8, (double)c);
            }
            else if (c is string)
            {
                cil.Emit(OpCodes.Ldstr, c as string);
            }
            else if (c is bool)
            {
                cil.Emit((bool)c ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
            }
            else
            {
                throw new ArgumentException("Unsupported constant in .NET backend: " + c.GetType());
            }
        }
    }
}
