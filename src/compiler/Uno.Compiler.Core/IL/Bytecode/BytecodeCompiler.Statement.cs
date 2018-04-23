using Uno.Compiler.API.Domain.Bytecode;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Logging;

namespace Uno.Compiler.Core.IL.Bytecode
{
    public partial class BytecodeCompiler
    {
        void MarkSource(Source src)
        {
            if (src.IsUnknown) return;

            //if (lastSource.Path == src.Path && lastSource.Line == src.Line) return;

            Emit(Opcodes.MarkSource, src);
        }

        /// <summary>
        /// Compiles the statement, and returns wether or not control flow falls through to the next statement.
        /// </summary>
        public void CompileStatement(Statement s, bool markSource)
        {
            switch (s.StatementType)
            {
                case StatementType.Expression:
                    {
                        var e = s as Expression;
                        var pop = !e.ReturnType.IsVoid;
                        if (markSource)
                            MarkSource(e.Source);

                        CompileExpression(e, pop, false);
                    }
                    break;

                case StatementType.VariableDeclaration:
                    {
                        var ld = s as VariableDeclaration;
                        for (var var = ld.Variable; var != null; var = var.Next)
                        {
                            Locals.Add(var);
                            if (var.OptionalValue != null)
                            {
                                if (markSource)
                                    MarkSource(var.Source);
                                CompileExpression(var.OptionalValue);
                                Emit(Opcodes.StoreLocal, var);
                            }
                        }
                    }
                    break;

                case StatementType.FixedArrayDeclaration:
                    {
                        var ld = s as FixedArrayDeclaration;
                        Locals.Add(ld.Variable);

                        if (ld.OptionalInitializer != null)
                        {
                            for (int i = 0; i < ld.OptionalInitializer.Length; i++)
                            {
                                if (markSource)
                                    MarkSource(ld.OptionalInitializer[i].Source);
                                Emit(Opcodes.Constant, i);
                                CompileExpression(ld.OptionalInitializer[i]);
                                Emit(Opcodes.StoreArrayElement, ld.Variable);
                            }
                        }
                    }
                    break;

                case StatementType.Scope:
                    {
                        var sc = s as Scope;

                        foreach (var st in sc.Statements)
                        {
                            var scopeCall = st as CallMethod;
                            var markScopeCall = !(scopeCall?.Method != null && scopeCall.Method.IsGenerated);
                            CompileStatement(st, markSource && markScopeCall);
                        }

                        if (sc.Statements.Count == 0 && markSource)
                            MarkSource(sc.Source);
                    }
                    break;

                case StatementType.While:
                    {
                        var w = s as While;

                        var body = NewLabel();
                        var cond = NewLabel();
                        var after = NewLabel();
                        _breakLabels.Push(after);
                        _continueLabels.Push(cond);

                        if (w.DoWhile)
                        {
                            MarkLabel(body);
                            if (w.OptionalBody != null)
                                CompileStatement(w.OptionalBody, markSource);

                            MarkLabel(cond);
                            if (markSource)
                                MarkSource(w.Condition.Source);
                            CompileCondition(w.Condition, ConditionSequence.FalseFollows, body, after);
                        }
                        else
                        {
                            MarkLabel(cond);
                            if (markSource)
                                MarkSource(w.Condition.Source);
                            CompileCondition(w.Condition, ConditionSequence.TrueFollows, body, after);

                            MarkLabel(body);
                            if (w.OptionalBody != null)
                                CompileStatement(w.OptionalBody, markSource);

                            Branch(Opcodes.Br, cond);
                        }

                        _breakLabels.Pop();
                        _continueLabels.Pop();

                        MarkLabel(after);
                    }
                    break;

                case StatementType.For:
                    {
                        var f = s as For;
                        var body = NewLabel();
                        var inc = NewLabel();
                        var cond = NewLabel();
                        var after = NewLabel();

                        _breakLabels.Push(after);
                        _continueLabels.Push(inc);

                        if (f.OptionalInitializer != null)
                            CompileStatement(f.OptionalInitializer, markSource);

                        Branch(Opcodes.Br, cond);

                        MarkLabel(body);
                        if (f.OptionalBody != null)
                            CompileStatement(f.OptionalBody, markSource);

                        MarkLabel(inc);
                        if (f.OptionalIncrement != null)
                        {
                            if (markSource)
                                MarkSource(f.OptionalIncrement.Source);
                            CompileExpression(f.OptionalIncrement, true);
                        }

                        MarkLabel(cond);
                        if (f.OptionalCondition != null)
                        {
                            if (markSource)
                                MarkSource(f.OptionalCondition.Source);
                            CompileCondition(f.OptionalCondition, ConditionSequence.FalseFollows, body, after);
                        }
                        else
                        {
                            Branch(Opcodes.Br, body);
                        }

                        _breakLabels.Pop();
                        _continueLabels.Pop();

                        MarkLabel(after);
                    }
                    break;

                case StatementType.IfElse:
                    {
                        var ife = s as IfElse;
                        if (markSource)
                            MarkSource(ife.Condition.Source);
                        var cond = CompileCondition(ife.Condition, ConditionSequence.TrueFollows);

                        MarkLabel(cond.TrueLabel);
                        if (ife.OptionalIfBody != null)
                            CompileStatement(ife.OptionalIfBody, markSource);

                        if (ife.OptionalElseBody != null)
                        {
                            var after = NewLabel();
                            Branch(Opcodes.Br, after);

                            MarkLabel(cond.FalseLabel);
                            CompileStatement(ife.OptionalElseBody, markSource);

                            MarkLabel(after);
                        }
                        else
                        {
                            MarkLabel(cond.FalseLabel);
                        }
                    }
                    break;

                case StatementType.Return:
                    {
                        var r = s as Return;
                        if (r.Value != null)
                        {
                            if (markSource)
                                MarkSource(r.Value.Source);
                            CompileExpression(r.Value);
                            Return(r.Value.ReturnType);
                        }
                        else
                        {
                            if (markSource)
                                MarkSource(s.Source);
                            Return(null);
                        }
                    }
                    break;

                case StatementType.Break:
                    {
                        if (_breakLabels.Count == 0)
                            throw new FatalException(s.Source, ErrorCode.E0015, "Invalid break");

                        if (markSource)
                            MarkSource(s.Source);
                        Branch(Opcodes.Br, _breakLabels.Peek());
                    }
                    break;

                case StatementType.Continue:
                    {
                        if (_continueLabels.Count == 0)
                            throw new FatalException(s.Source, ErrorCode.E0016, "Invalid continue");

                        if (markSource)
                            MarkSource(s.Source);
                        Branch(Opcodes.Br, _continueLabels.Peek());
                    }
                    break;

                case StatementType.TryCatchFinally:
                    {
                        var tc = s as TryCatchFinally;

                        _tryCatchStack.Add(tc);

                        Emit(Opcodes.BeginExceptionBlock);
                        CompileStatement(tc.TryBody, markSource);

                        foreach (var c in tc.CatchBlocks)
                        {
                            Emit(Opcodes.BeginCatchBlock, c.Exception.ValueType);

                            if (markSource)
                                MarkSource(c.Body.Source);
                            Locals.Add(c.Exception);
                            Emit(Opcodes.StoreLocal, c.Exception);
                            CompileStatement(c.Body, markSource);
                        }

                        _tryCatchStack.RemoveLast();

                        // TODO: finally-clause goes here (_AFTER_ "POP"!)

                        if (tc.OptionalFinallyBody != null)
                        {
                            Emit(Opcodes.BeginFinallyBlock);
                            if (markSource)
                                MarkSource(tc.OptionalFinallyBody.Source);
                            CompileStatement(tc.OptionalFinallyBody, markSource);
                        }

                        Emit(Opcodes.EndExceptionBlock);
                    }
                    break;

                case StatementType.Switch:
                    {
                        var sw = s as Switch;
                        if (markSource)
                            MarkSource(sw.ControlVariable.Source);
                        CompileExpression(sw.ControlVariable);
                        var temp = StoreTempDontDup(sw.ControlVariable.ReturnType);

                        var after = NewLabel();

                        Label defaultLabel = null;
                        _breakLabels.Push(after);

                        var caseLabels = new Label[sw.Cases.Length];

                        for (var i = 0; i < sw.Cases.Length; i++)
                            caseLabels[i] = NewLabel();

                        for (var i = 0; i < sw.Cases.Length; i++)
                        {
                            for (int v = 0; v < sw.Cases[i].Values.Length; v++)
                            {
                                CompileExpression(sw.Cases[i].Values[v]);
                                LoadTemp(temp);
                                Branch(Opcodes.BrEq, caseLabels[i]);
                            }

                            if (sw.Cases[i].HasDefault)
                            {
                                defaultLabel = caseLabels[i];
                            }
                        }

                        Branch(Opcodes.Br, defaultLabel ?? after);

                        for (var i = 0; i < sw.Cases.Length; i++)
                        {
                            MarkLabel(caseLabels[i]);
                            CompileStatement(sw.Cases[i].Scope, markSource);
                            Branch(Opcodes.Br, after);
                        }

                        _breakLabels.Pop();
                        MarkLabel(after);
                    }
                    break;

                case StatementType.Throw:
                    {
                        var t = s as Throw;
                        if (markSource)
                            MarkSource(t.Source);
                        CompileExpression(t.Exception);
                        Emit(Opcodes.Throw);
                        if (markSource)
                            MarkSource(t.Source);
                    }
                    break;

                case StatementType.Draw:
                case StatementType.DrawDispose:
                    break;

                default:
                    throw new FatalException(s.Source, ErrorCode.I0017, "Statement type not supported in bytecode backend: " + s.StatementType);
            }
        }
    }
}
