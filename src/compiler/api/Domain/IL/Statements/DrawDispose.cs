using Uno.Compiler.API.Domain.IL.Expressions;

namespace Uno.Compiler.API.Domain.IL.Statements
{
    public class DrawDispose : Statement
    {
        public DrawDispose(Source src)
            : base(src)
        {
        }

        public override StatementType StatementType => StatementType.DrawDispose;

        public override void Visit(Pass p, ExpressionUsage u)
        {
        }

        public override Statement CopyStatement(CopyState state)
        {
            return this;
        }
    }
}