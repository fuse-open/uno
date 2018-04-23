using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.Backends.CPlusPlus
{
    class CppFinallyTransform : Pass
    {
        public CppFinallyTransform(CppBackend backend)
            : base(backend)
        {
        }

        Stack<Scope> _prependReturn = new Stack<Scope>();
        Stack<Scope> _prependContinue = new Stack<Scope>();
        Stack<Scope> _prependBreak = new Stack<Scope>();
        bool _insideTry = false;

        void BeginTryCatchFinally(TryCatchFinally tcf)
        {
            if (tcf.OptionalFinallyBody != null)
                _prependReturn.Push(tcf.OptionalFinallyBody);

            _prependContinue.Push(tcf.OptionalFinallyBody);
            _prependBreak.Push(tcf.OptionalFinallyBody);
            _insideTry = true;
        }

        void EndTryCatchFinally(TryCatchFinally tcf)
        {
            if (tcf.OptionalFinallyBody != null)
                _prependReturn.Pop();

            _prependContinue.Pop();
            _prependBreak.Pop();
        }

        public override void Begin(ref Statement s)
        {
            switch (s.StatementType)
            {
                case StatementType.TryCatchFinally:
                    BeginTryCatchFinally((TryCatchFinally)s);
                    break;

                case StatementType.For:
                case StatementType.While:
                    // for and while changes what 'break' and 'continue' affects
                    _prependContinue.Push(null);
                    _prependBreak.Push(null);
                    break;

                case StatementType.Switch:
                    // switch changes what 'break' affects
                    _prependBreak.Push(null);
                    break;
            }
        }

        public override void Next(Statement p)
        {
            if (p.StatementType == StatementType.TryCatchFinally)
                _insideTry = false;
        }

        public override void End(ref Statement s)
        {
            switch (s.StatementType)
            {
                case StatementType.TryCatchFinally:
                    EndTryCatchFinally((TryCatchFinally)s);
                    break;

                case StatementType.For:
                case StatementType.While:
                    // for and while changes what 'break' and 'continue' affects
                    _prependContinue.Pop();
                    _prependBreak.Pop();
                    break;

                case StatementType.Switch:
                    // switch changes what 'break' affects
                    _prependBreak.Pop();
                    break;

                case StatementType.Return:
                    if (_insideTry && _prependReturn.Count > 0)
                    {
                        var statements = new List<Statement>();

                        var r = (Return)s;
                        VariableDeclaration tempVariableDeclaration = null;
                        if (r.Value != null)
                        {
                            tempVariableDeclaration = new VariableDeclaration(r.Source, null, "__uno_retval", r.Value.ReturnType, API.Domain.IL.Members.VariableType.Default, r.Value);
                            statements.Add(tempVariableDeclaration);
                        }

                        var finallyBlocks = _prependReturn.ToArray();
                        for (int i = finallyBlocks.Length - 1; i >= 0; --i)
                            statements.Add(finallyBlocks[i]);

                        if (tempVariableDeclaration != null)
                            statements.Add(new Return(s.Source, new LoadLocal(s.Source, tempVariableDeclaration?.Variable)));
                        else
                            statements.Add(new Return(s.Source));

                        s = new Scope(s.Source, statements.ToArray());
                    }
                    break;

                case StatementType.Continue:
                    if (_insideTry && _prependContinue.Count > 0 && _prependContinue.Peek() != null)
                    {
                        s = new Scope(s.Source, _prependContinue.Peek(), s);
                    }
                    break;

                case StatementType.Break:
                    if (_insideTry && _prependBreak.Count > 0 && _prependBreak.Peek() != null)
                    {
                        s = new Scope(s.Source, _prependBreak.Peek(), s);
                    }
                    break;
            }
        }
    }
}
