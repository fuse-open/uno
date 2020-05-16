using Uno.Compiler.API.Domain.IL.Expressions;

namespace Uno.Compiler.API.Domain.IL.Statements
{
    public class Throw : Statement
    {
        public Expression Exception;
        public bool IsRethrow;

        public Throw(Source src, Expression exception, bool rethrow = false)
            : base(src)
        {
            Exception = exception;
            IsRethrow = rethrow;
        }

        public override StatementType StatementType => StatementType.Throw;

        public override void Visit(Pass p, ExpressionUsage u)
        {
            p.VisitNullable(ref Exception);
        }

        public override Statement CopyStatement(CopyState state)
        {
            return new Throw(Source, Exception.CopyExpression(state), IsRethrow);
        }
    }
}