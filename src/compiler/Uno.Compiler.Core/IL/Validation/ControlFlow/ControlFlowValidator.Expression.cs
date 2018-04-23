using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Core.IL.Validation.ControlFlow
{
    public partial class ControlFlowValidator
    {
        public void CompileFunctionCall(Source s, Parameter[] paramList, Expression[] args)
        {
            // Compile arguments while setting the 'out' flag when appropriate
            // The out-flag will suspend emitting of node reads for variables, fields and parameters
            var outNodes = new MemoryNode[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                if (paramList[i].Modifier == ParameterModifier.Out)
                    outNodes[i] = CompileExpression(args[i], null, false, false);
                else
                    CompileExpression(args[i]);
            }

            // Go over out-nodes again and mark them as written
            foreach (var o in outNodes)
            {
                if (o == null)
                    continue;

                EmitWriteNode(s, o);
            }
        }

        MemoryNode CompileExpression(Expression e, Condition cond = null, bool addressMode = false, bool isRealRead = true)
        {
            switch (e.ExpressionType)
            {
                case ExpressionType.AddressOf:
                    return CompileExpression((e as AddressOf).Operand, null, true, isRealRead);

                case ExpressionType.StoreThis:
                    return memory.This = null;

                case ExpressionType.This:
                {
                    // if 'this' is not initialized, 'this' cannot be used before all fields are initialized
                    if (isRealRead && memory.This != null)
                        EmitValidateNodeIsInitialized(e.Source, memory.This);

                    return memory.This;
                }
                case ExpressionType.SequenceOp:
                {
                    var s = e as SequenceOp;
                    CompileExpression(s.Left);
                    return CompileExpression(s.Right);
                }
                case ExpressionType.IsOp:
                {
                    var s = e as IsOp;
                    CompileExpression(s.Operand);
                    break;
                }
                case ExpressionType.AsOp:
                {
                    var s = e as AsOp;
                    CompileExpression(s.Operand);
                    break;
                }
                case ExpressionType.LoadLocal:
                {
                    var ll = e as LoadLocal;
                    var node = memory.AddLocal(ll.Variable);

                    if (isRealRead)
                        EmitReadNode(ll.Source, node);

                    return node;
                }
                case ExpressionType.LoadField:
                {
                    var s = e as LoadField;

                    if (s.Object != null)
                    {
                        var node = CompileExpression(s.Object, null, false, !(s.Object.ReturnType is StructType));

                        if (node?.Children != null)
                        {
                            node = node.Children[s.Field];

                            if (isRealRead)
                                EmitReadNode(s.Source, node);

                            return node;
                        }
                    }
                    break;
                }
                case ExpressionType.LoadElement:
                {
                    var s = e as LoadElement;
                    CompileExpression(s.Array);
                    CompileExpression(s.Index);
                    break;
                }
                case ExpressionType.LoadArgument:
                {
                    var lp = e as LoadArgument;

                    if (memory.OutParameters != null)
                    {
                        MemoryNode node;
                        if (memory.OutParameters.TryGetValue(lp.Index, out node))
                        {
                            if (isRealRead)
                                EmitReadNode(lp.Source, node);

                            return node;
                        }
                    }
                    break;
                }
                case ExpressionType.StoreLocal:
                {
                    var s = e as StoreLocal;
                    CompileExpression(s.Value);
                    memory.AddLocal(s.Variable);

                    var node = memory.Locals[s.Variable];
                    EmitWriteNode(s.Source, node);
                    EmitReadNode(s.Source, node);
                    break;
                }
                case ExpressionType.StoreArgument:
                {
                    var s = e as StoreArgument;
                    CompileExpression(s.Value);

                    if (memory.OutParameters != null)
                    {
                        MemoryNode node;
                        if (memory.OutParameters.TryGetValue(s.Index, out node))
                        {
                            EmitWriteNode(s.Source, node);
                            EmitReadNode(s.Source, node);
                            return node;
                        }
                    }
                    break;
                }
                case ExpressionType.StoreField:
                {
                    var s = e as StoreField;
                    MemoryNode node = null;

                    if (s.Object != null)
                        node = CompileExpression(s.Object, null, false, !(s.Object.ReturnType is StructType));

                    CompileExpression(s.Value);

                    if (node?.Children != null)
                    {
                        node = node.Children[s.Field];
                        EmitWriteNode(s.Source, node);
                        EmitReadNode(s.Source, node);
                        return node;
                    }
                    break;
                }
                case ExpressionType.StoreElement:
                {
                    var s = e as StoreElement;
                    CompileExpression(s.Array);
                    CompileExpression(s.Index);
                    CompileExpression(s.Value);
                    break;
                }
                case ExpressionType.SetProperty:
                {
                    var s = e as SetProperty;

                    if (s.Object != null)
                        CompileExpression(s.Object);

                    foreach (var arg in s.Arguments)
                        CompileExpression(arg);

                    CompileExpression(s.Value);
                    break;
                }
                case ExpressionType.GetProperty:
                {
                    var s = e as GetProperty;

                    if (s.Object != null)
                        CompileExpression(s.Object);
                    foreach (var arg in s.Arguments)
                        CompileExpression(arg);
                    break;
                }
                case ExpressionType.AddListener:
                {
                    var s = e as AddListener;
                    if (s.Object != null)
                        CompileExpression(s.Object);
                    CompileExpression(s.Listener);
                    break;
                }
                case ExpressionType.RemoveListener:
                {
                    var s = e as RemoveListener;

                    if (s.Object != null)
                        CompileExpression(s.Object);

                    CompileExpression(s.Listener);
                    break;
                }
                case ExpressionType.ExternOp:
                {
                    var s = (ExternOp) e;

                    if (memory.OutParameters != null)
                    {
                        foreach (var a in s.Arguments)
                        {
                            var p = a as StoreArgument;
                            if (p != null && p.Parameter.Modifier == ParameterModifier.Out)
                            {
                                MemoryNode node;
                                if (memory.OutParameters.TryGetValue(p.Index, out node))
                                {
                                    EmitWriteNode(s.Source, node);
                                    EmitReadNode(s.Source, node);
                                }
                            }
                        }

                    }
                    break;
                }
                case ExpressionType.BranchOp:
                {
                    var s = e as BranchOp;

                    if (s.BranchType == BranchType.And)
                    {
                        if (cond != null && !cond.Handled)
                        {
                            var brtrue = NewBlock(s.Source);
                            CompileCondition(s.Left, brtrue, cond.FalseBlock ?? (cond.FalseBlock = NewBlock(s.Source)));

                            _current = brtrue;
                            CompileCondition(s.Right, cond.TrueBlock ?? (cond.TrueBlock = NewBlock(s.Source)), cond.FalseBlock ?? (cond.FalseBlock = NewBlock(s.Source)));

                            cond.Handled = true;
                            return null;
                        }
                        else
                        {
                            var c = CompileCondition(s.Left);
                            var brafter = NewBlock(s.Source);

                            _current = c.TrueBlock;
                            CompileExpression(s.Right);
                            EndBlock(BlockEnding.Br, brafter);

                            _current = c.FalseBlock;
                            EndBlock(BlockEnding.Br, brafter);

                            _current = brafter;
                        }
                    }
                    else
                    {
                        if (cond != null && !cond.Handled)
                        {
                            var brfalse = NewBlock(s.Source);
                            CompileCondition(s.Left, cond.TrueBlock ?? (cond.TrueBlock = NewBlock(s.Source)), brfalse);

                            _current = brfalse;
                            CompileCondition(s.Right, cond.TrueBlock ?? (cond.TrueBlock = NewBlock(s.Source)), cond.FalseBlock ?? (cond.FalseBlock = NewBlock(s.Source)));

                            cond.Handled = true;
                            return null;
                        }
                        else
                        {
                            var c = CompileCondition(s.Left);
                            var brafter = NewBlock(s.Source);

                            _current = c.TrueBlock;
                            EndBlock(BlockEnding.Br, brafter);

                            _current = c.FalseBlock;
                            CompileExpression(s.Right);
                            EndBlock(BlockEnding.Br, brafter);

                            _current = brafter;
                        }
                    }

                    break;
                }
                case ExpressionType.ConditionalOp:
                {
                    var s = e as ConditionalOp;
                    var brtrue = NewBlock(s.Source);
                    var brfalse = NewBlock(s.Source);
                    var brafter = NewBlock(s.Source);
                    CompileCondition(s.Condition, brtrue, brfalse);

                    _current = brtrue;
                    CompileExpression(s.True);
                    EndBlock(BlockEnding.Br, brafter);

                    _current = brfalse;
                    CompileExpression(s.False);
                    EndBlock(BlockEnding.Br, brafter);

                    _current = brafter;
                    break;
                }
                case ExpressionType.NullOp:
                {
                    var s = e as NullOp;
                    var brnull = NewBlock(s.Source);
                    var brafter = NewBlock(s.Source);

                    CompileExpression(s.Left);
                    EndBlock(BlockEnding.CondBr, brafter, brnull);

                    _current = brnull;
                    CompileExpression(s.Right);
                    EndBlock(BlockEnding.Br, brafter);

                    _current = brafter;
                    break;
                }
                case ExpressionType.CallCast:
                {
                    var s = e as CallCast;
                    CompileExpression(s.Operand);
                    break;
                }
                case ExpressionType.CallMethod:
                {
                    var s = e as CallMethod;
                    if (s.Object != null)
                        CompileExpression(s.Object);

                    CompileFunctionCall(s.Source, s.Method.Parameters, s.Arguments);
                    break;
                }
                case ExpressionType.CallConstructor:
                {
                    var s = e as CallConstructor;

                    for (int i = 0; i < s.Arguments.Length; i++)
                        CompileExpression(s.Arguments[i]);

                    break;
                }
                case ExpressionType.CallDelegate:
                {
                    var s = e as CallDelegate;
                    CompileExpression(s.Object);
                    CompileFunctionCall(s.Source, s.Object.ReturnType.Parameters, s.Arguments);
                    break;
                }
                case ExpressionType.ReferenceOp:
                {
                    var s = e as ReferenceOp;
                    CompileExpression(s.Left);
                    CompileExpression(s.Right);

                    if (cond != null && !cond.Handled)
                    {
                        EndBlock(BlockEnding.CondBr, cond.TrueBlock ?? (cond.TrueBlock = NewBlock(s.Source)), (cond.FalseBlock ?? (cond.FalseBlock = NewBlock(s.Source))));
                        cond.Handled = true;
                    }
                    break;
                }
                case ExpressionType.CallBinOp:
                {
                    var s = e as CallBinOp;
                    CompileExpression(s.Left);
                    CompileExpression(s.Right);

                    if (cond != null && !cond.Handled)
                    {
                        EndBlock(BlockEnding.CondBr, cond.TrueBlock ?? (cond.TrueBlock = NewBlock(s.Source)), (cond.FalseBlock ?? (cond.FalseBlock = NewBlock(s.Source))));
                        cond.Handled = true;
                    }
                    break;
                }
                case ExpressionType.CallUnOp:
                {
                    var s = e as CallUnOp;
                    CompileExpression(s.Operand);
                    break;
                }
                case ExpressionType.CastOp:
                {
                    var s = e as CastOp;
                    CompileExpression(s.Operand);
                    break;
                }
                case ExpressionType.NewObject:
                {
                    var s = e as NewObject;
                    CompileFunctionCall(s.Source, s.Constructor.Parameters, s.Arguments);
                    break;
                }
                case ExpressionType.NewDelegate:
                {
                    var s = e as NewDelegate;

                    if (s.Object != null)
                        CompileExpression(s.Object);
                    break;
                }
                case ExpressionType.NewArray:
                {
                    var s = e as NewArray;

                    if (s.Size != null)
                        CompileExpression(s.Size);

                    if (s.Initializers != null)
                        for (int i = 0; i < s.Initializers.Length; i++)
                            CompileExpression(s.Initializers[i]);
                    break;
                }
                case ExpressionType.FixOp:
                {
                    var fo = e as FixOp;
                    var node = CompileExpression(fo.Operand);

                    if (node != null)
                    {
                        EmitReadNode(fo.Source, node);
                        EmitWriteNode(fo.Source, node);
                    }
                    break;
                }
                case ExpressionType.Swizzle:
                {
                    var sw = e as Swizzle;
                    CompileExpression(sw.Object);
                    break;
                }
            }

            return null;
        }
    }
}
