using System.Collections.Generic;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Logging;

namespace Uno.Compiler.Core.IL.Validation.ControlFlow
{
    public partial class ControlFlowValidator
    {
        readonly List<Variable> locals = new List<Variable>();
        readonly Stack<BasicBlock> breakBlocks = new Stack<BasicBlock>();
        readonly Stack<BasicBlock> continueBlocks = new Stack<BasicBlock>();

        public void CompileStatement(Statement s)
        {
            if (_current == null)
            {
                // TODO: Commenting out this error because it triggers alot when constant folding
                // 'if defined()' statements and 'defined() ?:' expressions.
                //Log.ReportError(s.Source, ErrorCode.E4517, "Unreachable code detected");
                return;
            }

            switch (s.StatementType)
            {
                case StatementType.Expression:
                {
                    var e = s as Expression;
                    CompileExpression(e);
                    break;
                }
                case StatementType.VariableDeclaration:
                {
                    var ld = s as VariableDeclaration;
                    for (var var = ld.Variable; var != null; var = var.Next)
                    {
                        locals.Add(var);
                        var node = memory.AddLocal(var);

                        if (var.OptionalValue != null)
                        {
                            CompileExpression(var.OptionalValue);
                            EmitWriteNode(var.Source, node);
                        }
                        else if (var.IsExtern)
                            EmitWriteNode(var.Source, node);
                    }
                    break;
                }
                case StatementType.FixedArrayDeclaration:
                {
                    var ld = s as FixedArrayDeclaration;
                    locals.Add(ld.Variable);

                    var node = memory.AddLocal(ld.Variable);

                    if (ld.OptionalInitializer != null)
                        foreach (var e in ld.OptionalInitializer)
                            CompileExpression(e);

                    EmitWriteNode(ld.Source, node);
                    break;
                }
                case StatementType.Scope:
                    foreach (var st in (s as Scope).Statements)
                        CompileStatement(st);
                    break;

                case StatementType.While:
                {
                    var w = s as While;
                    var startBlock = _current;

                    var cond = NewBlock(w.Source);
                    _current = cond;

                    var c = CompileCondition(w.Condition);

                    var body = c.TrueBlock;
                    var after = c.FalseBlock;

                    _current = startBlock;

                    EndBlock(BlockEnding.Br,
                        !w.DoWhile
                            ? cond
                            : body);

                    breakBlocks.Push(after);
                    continueBlocks.Push(cond);

                    _current = body;

                    if (w.OptionalBody != null)
                        CompileStatement(w.OptionalBody);

                    if (_current != null)
                        EndBlock(BlockEnding.Br, cond);

                    breakBlocks.Pop();
                    continueBlocks.Pop();

                    _current = after;
                    break;
                }
                case StatementType.For:
                {
                    var f = s as For;
                    var body = NewBlock(s.Source);
                    var inc = NewBlock(s.Source);
                    var cond = NewBlock(s.Source);
                    var after = NewBlock(s.Source);

                    breakBlocks.Push(after);
                    continueBlocks.Push(inc);

                    if (f.OptionalInitializer != null)
                        CompileStatement(f.OptionalInitializer);

                    EndBlock(BlockEnding.Br, cond);

                    _current = body;
                    if (f.OptionalBody != null)
                        CompileStatement(f.OptionalBody);
                    if (_current != null)
                        EndBlock(BlockEnding.Br, inc);

                    _current = inc;
                    if (f.OptionalIncrement != null)
                        CompileExpression(f.OptionalIncrement);
                    EndBlock(BlockEnding.Br, cond);

                    _current = cond;
                    if (f.OptionalCondition != null)
                        CompileCondition(f.OptionalCondition, body, after);
                    else
                        EndBlock(BlockEnding.Br, body);

                    breakBlocks.Pop();
                    continueBlocks.Pop();

                    _current = after;
                    break;
                }
                case StatementType.IfElse:
                {
                    var ife = s as IfElse;
                    var cond = CompileCondition(ife.Condition);

                    var brafter = cond.FalseBlock;

                    if (ife.OptionalElseBody != null)
                        brafter = NewBlock(ife.Source);

                    _current = cond.TrueBlock;

                    if (ife.OptionalIfBody != null)
                        CompileStatement(ife.OptionalIfBody);

                    if (_current != null)
                        EndBlock(BlockEnding.Br, brafter);

                    if (ife.OptionalElseBody != null)
                    {
                        _current = cond.FalseBlock;
                        CompileStatement(ife.OptionalElseBody);

                        if (_current != null)
                            EndBlock(BlockEnding.Br, brafter);
                    }

                    _current = brafter;
                    break;
                }
                case StatementType.Return:
                {
                    if ((s as Return).Value != null)
                    {
                        CompileExpression((s as Return).Value);
                        EndBlock(BlockEnding.RetNonVoid);
                    }
                    else
                        EndBlock(BlockEnding.RetVoid);
                    break;
                }
                case StatementType.ExternScope:
                {
                    var e = (ExternScope) s;
                    if (memory.OutParameters != null)
                    {
                        for (int i = 0; i < Function.Parameters.Length; i++)
                        {
                            var p = Function.Parameters[i];
                            if (p.Modifier == ParameterModifier.Out)
                            {
                                MemoryNode node;
                                if (memory.OutParameters.TryGetValue(i, out node))
                                {
                                    EmitWriteNode(s.Source, node);
                                    EmitReadNode(s.Source, node);
                                }
                            }
                        }
                    }

                    if (e.String.Contains("return"))
                        EndBlock(Function.ReturnType.IsVoid
                            ? BlockEnding.RetVoid
                            : BlockEnding.RetNonVoid);
                    break;
                }
                case StatementType.Break:
                {
                    if (breakBlocks.Count == 0)
                        throw new FatalException(s.Source, ErrorCode.E0015, "Invalid break");

                    EndBlock(BlockEnding.Br, breakBlocks.Peek());
                    break;
                }
                case StatementType.Continue:
                {
                    if (continueBlocks.Count == 0)
                        throw new FatalException(s.Source, ErrorCode.E0016, "Invalid continue");

                    EndBlock(BlockEnding.Br, continueBlocks.Peek());
                    break;
                }
                case StatementType.TryCatchFinally:
                {
                    var tc = s as TryCatchFinally;

                    _tryStack.Push(tc);
                    var tryBlock = NewBlock(tc.TryBody.Source);
                    var prevBlock = _current;
                    EndBlock(BlockEnding.Br, tryBlock);
                    _current = tryBlock;

                    CompileStatement(tc.TryBody);

                    _tryStack.Pop();
                    var afterTryBlock = NewBlock(tc.TryBody.Source);

                    bool anyBranchToAfterTryBlock = false;

                    if (_current != null)
                    {
                        _current.Terminate(BlockEnding.Br, afterTryBlock);
                        anyBranchToAfterTryBlock = true;
                    }

                    foreach (var c in tc.CatchBlocks)
                    {
                        var catchBlock = NewBlock(c.Source);
                        _current = catchBlock;

                        prevBlock.Successors.Add(catchBlock);

                        locals.Add(c.Exception);
                        var v = memory.AddLocal(c.Exception);

                        EmitWriteNode(c.Body.Source, v);
                        CompileStatement(c.Body);

                        if (_current != null)
                        {
                            _current.Terminate(BlockEnding.Br, afterTryBlock);
                            anyBranchToAfterTryBlock = true;
                        }
                    }

                    if (tc.OptionalFinallyBody != null)
                    {
                        var finallyBlock = NewBlock(tc.OptionalFinallyBody.Source);
                        _current = finallyBlock;

                        prevBlock.Successors.Add(finallyBlock);

                        CompileStatement(tc.OptionalFinallyBody);

                        if (_current != null)
                        {
                            if (anyBranchToAfterTryBlock)
                                _current.Terminate(BlockEnding.Br, afterTryBlock);
                            else
                                _current.Terminate(BlockEnding.Br);
                        }
                    }

                    _current = afterTryBlock;
                    break;
                }
                case StatementType.Switch:
                {
                    var sw = s as Switch;
                    CompileExpression(sw.ControlVariable);
                    var after = NewBlock(s.Source);

                    breakBlocks.Push(after);

                    var nextCond = _current;
                    var defaultBlock = after;

                    for (var i = 0; i < sw.Cases.Length; i++)
                    {
                        var c = NewBlock(s.Source);

                        for (var v = 0; v < sw.Cases[i].Values.Length; v++)
                        {
                            _current = nextCond;
                            nextCond = NewBlock(s.Source);
                            CompileExpression(sw.Cases[i].Values[v]);
                            EndBlock(BlockEnding.CondBr, c, nextCond);
                        }

                        if (sw.Cases[i].HasDefault)
                            defaultBlock = c;

                        _current = c;
                        CompileStatement(sw.Cases[i].Scope);

                        if (_current != null &&
                            _current.Ending == BlockEnding.Open)
                            Log.Error(sw.Cases[i].Scope.Source, ErrorCode.E4515, "Control cannot fall through from one case label (" + sw.Cases[i].Values + ") to another ");
                    }

                    _current = nextCond;
                    EndBlock(BlockEnding.Br, defaultBlock);

                    _current = after;
                    break;
                }
                case StatementType.Throw:
                {
                    var t = s as Throw;
                    CompileExpression(t.Exception);
                    EndBlock(BlockEnding.Throw);
                    break;
                }
                case StatementType.Draw:
                case StatementType.DrawDispose:
                    break;

                default:
                    Log.Warning(s.Source, ErrorCode.I0017, "Statement type not supported in control flow validator: " + s.StatementType.ToString());
                    break;
            }
        }
    }
}
