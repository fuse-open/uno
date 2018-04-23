using Uno.Compiler.API.Domain.AST.Statements;

namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public abstract class AstExpression : AstStatement
    {
        public override AstStatementType StatementType => (AstStatementType) ExpressionType;
        public abstract AstExpressionType ExpressionType { get; }

        protected AstExpression(Source src)
            : base(src)
        {
        }
    }
}
