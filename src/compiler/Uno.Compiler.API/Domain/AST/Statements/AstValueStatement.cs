using Uno.Compiler.API.Domain.AST.Expressions;

namespace Uno.Compiler.API.Domain.AST.Statements
{
    public sealed class AstValueStatement : AstStatement
    {
        public readonly AstValueStatementType Type;
        public readonly AstExpression Value;

        public override AstStatementType StatementType => (AstStatementType) Type;

        public AstValueStatement(Source src, AstValueStatementType type, AstExpression value)
            : base(src)
        {
            Type = type;
            Value = value;
        }
    }
}