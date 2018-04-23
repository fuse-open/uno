using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL.Expressions;

namespace Uno.Compiler.API.Domain.IL.Statements
{
    public class Scope : Statement
    {
        public readonly List<Statement> Statements = new List<Statement>();

        public Scope(Source src, params Statement[] bodySequence)
            : base(src)
        {
            Statements.AddRange(bodySequence);
        }

        public Scope()
            : base(Source.Unknown)
        {
        }

        public override StatementType StatementType => StatementType.Scope;

        public override void Visit(Pass p, ExpressionUsage u = ExpressionUsage.Statement)
        {
            p.BeginScope(this);

            for (var i = 0; i < Statements.Count; i++)
            {
                p.Next(this);
                var s = Statements[i];
                p.VisitNullable(ref s);
                Statements[i] = s;
            }

            p.EndScope(this);
        }

        public override Statement CopyStatement(CopyState state)
        {
            var r = new Scope(Source);

            foreach (var s in Statements)
                r.Statements.Add(s.CopyStatement(state));

            return r;
        }
    }
}