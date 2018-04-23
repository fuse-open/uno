using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.Core.IL.Building.Functions
{
    class IndirectionTransform : CompilerPass
    {
        readonly HashSet<Variable> _declared = new HashSet<Variable>();
        readonly List<Variable> _missing = new List<Variable>();

        public IndirectionTransform(CompilerPass parent)
            : base(parent)
        {
        }

        public override bool Begin(Function f)
        {
            _declared.Clear();
            _missing.Clear();
            return f.HasBody;
        }

        public override void End(Function f)
        {
            for (int i = _missing.Count - 1; i >= 0; i--)
            {
                var v = _missing[i];
                f.Body.Statements.Insert(0, new VariableDeclaration(v));

                if (v.OptionalValue != null &&
                    v.OptionalValue.ExpressionType != ExpressionType.Default)
                    Log.Warning(v.Source, ErrorCode.I0000, "Invalid indirect variable with initializer");
            }
        }

        public override void EndScope(Scope scope)
        {
            for (int i = 0; i < scope.Statements.Count; i++)
            {
                var e = scope.Statements[i];

                switch (e.StatementType)
                {
                    case StatementType.VariableDeclaration:
                    {
                        var s = e as VariableDeclaration;
                        for (var var = s.Variable; var != null; var = var.Next)
                        {
                            _declared.Add(var);
                            if (var.OptionalValue is SequenceOp)
                            {
                                var v = var.OptionalValue as SequenceOp;
                                var.OptionalValue = v.Right;
                                scope.Statements.Insert(i, v.Left);
                                i--;
                            }                            
                        }
                        break;
                    }
                    case StatementType.FixedArrayDeclaration:
                    {
                        _declared.Add((e as FixedArrayDeclaration).Variable);
                        break;
                    }
                    case StatementType.TryCatchFinally:
                    {
                        foreach (var c in (e as TryCatchFinally).CatchBlocks)
                            _declared.Add(c.Exception);
                        break;
                    }
                    case StatementType.Return:
                    {
                        var s = e as Return;
                        if (s.Value is SequenceOp)
                        {
                            var v = s.Value as SequenceOp;
                            s.Value = v.Right;
                            scope.Statements.Insert(i, v.Left);
                            i--;
                        }
                        break;
                    }
                    case StatementType.Expression:
                    {
                        var s = e as Expression;

                        switch (s.ExpressionType)
                        {
                            case ExpressionType.NoOp:
                            {
                                scope.Statements.RemoveAt(i);
                                i--;
                                break;
                            }
                            case ExpressionType.SequenceOp:
                            {
                                var v = e as SequenceOp;
                                scope.Statements[i] = v.Right;
                                scope.Statements.Insert(i, v.Left);
                                i--;
                                break;
                            }
                            case ExpressionType.StoreLocal:
                            {
                                var v = e as StoreLocal;

                                if (!_declared.Contains(v.Variable) && v.Variable.IsIndirection)
                                {
                                    _declared.Add(v.Variable);
                                    v.Variable.OptionalValue = v.Value;
                                    scope.Statements[i] = new VariableDeclaration(v.Variable);
                                    i--;
                                }
                                break;
                            }
                        }
                        break;
                    }
                }
            }
        }

        public override void Begin(ref Statement e)
        {
            switch (e.StatementType)
            {
                case StatementType.VariableDeclaration:
                {
                    for (var var = (e as VariableDeclaration).Variable; var != null; var = var.Next)
                        _declared.Add(var);
                    break;
                }
                case StatementType.FixedArrayDeclaration:
                {
                    _declared.Add((e as FixedArrayDeclaration).Variable);
                    break;
                }
                case StatementType.TryCatchFinally:
                {
                    foreach (var c in (e as TryCatchFinally).CatchBlocks)
                        _declared.Add(c.Exception);
                    break;
                }
                case StatementType.For:
                {
                    var s = e as For;

                    if (s.OptionalBody is SequenceOp)
                        s.OptionalBody = new Scope(s.OptionalBody.Source, s.OptionalBody);
                    break;
                }
                case StatementType.While:
                {
                    var s = e as While;

                    if (s.OptionalBody is SequenceOp)
                        s.OptionalBody = new Scope(s.OptionalBody.Source, s.OptionalBody);
                    break;
                }
                case StatementType.IfElse:
                {
                    var s = e as IfElse;

                    if (s.OptionalIfBody is SequenceOp)
                        s.OptionalIfBody = new Scope(s.OptionalIfBody.Source, s.OptionalIfBody);
                    if (s.OptionalElseBody is SequenceOp)
                        s.OptionalElseBody = new Scope(s.OptionalElseBody.Source, s.OptionalElseBody);
                    break;
                }
            }
        }

        public override void End(ref Statement e)
        {
            if (e is LoadLocal && (e as LoadLocal).Variable.IsIndirection)
                e = new NoOp(e.Source);
        }

        public override void Begin(ref Expression e, ExpressionUsage u)
        {
            switch (e.ExpressionType)
            {
                case ExpressionType.StoreLocal:
                {
                    var s = (StoreLocal) e;

                    if (!_declared.Contains(s.Variable))
                    {
                        _missing.Add(s.Variable);
                        _declared.Add(s.Variable);
                    }
                    break;
                }
                case ExpressionType.LoadLocal:
                {
                    var s = (LoadLocal) e;

                    if (!_declared.Contains(s.Variable))
                    {
                        _missing.Add(s.Variable);
                        _declared.Add(s.Variable);
                    }
                    break;
                }
                case ExpressionType.SequenceOp:
                {
                    var s = (SequenceOp) e;

                    if (s.Left.ExpressionType == ExpressionType.NoOp)
                    {
                        e = s.Right;
                        Begin(ref e, u);
                        break;
                    }

                    if (s.Right.ExpressionType == ExpressionType.NoOp)
                    {
                        e = s.Left;
                        Begin(ref e, u);
                    }
                    break;
                }
            }
        }
    }
}
