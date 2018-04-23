using Uno.Compiler.API.Domain.IL.Expressions;

namespace Uno.Compiler.API.Domain.IL.Statements
{
    public class Return : Statement
    {
        public Expression Value;

        public override StatementType StatementType => StatementType.Return;

        public Return(Source src, Expression value = null)
            : base(src)
        {
            Value = value;
        }

        public Return(Expression value = null)
            : this(Source.Unknown, value)
        {
        }

        public override void Visit(Pass p, ExpressionUsage u)
        {
            p.VisitNullable(ref Value);
        }

        public override Statement CopyStatement(CopyState state)
        {
            return new Return(Source, Value.CopyNullable(state));
        }
    }
}