using System;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL.Expressions;

namespace Uno.Compiler.API.Domain.IL.Statements
{
    public class Draw : Statement
    {
        public readonly DrawState State;

        public Draw(Source src, DrawState dc)
            : base(src)
        {
            State = dc;
        }

        public override StatementType StatementType => StatementType.Draw;

        public override void Visit(Pass p, ExpressionUsage u)
        {
            State.Visit(this, p);
        }

        public override Statement CopyStatement(CopyState state)
        {
            throw new NotImplementedException();
        }
    }
}