using Uno.Compiler.API.Domain.AST.Expressions;

namespace Uno.Compiler.API.Domain.AST.Statements
{
    public sealed class AstLoop : AstStatement
    {
        public readonly AstLoopType Type;
        public readonly AstExpression Condition;
        public readonly AstStatement OptionalBody;

        public override AstStatementType StatementType => (AstStatementType) Type;

        public AstLoop(Source src, AstLoopType type, AstExpression cond, AstStatement action)
            : base(src)
        {
            Type = type;
            Condition = cond;
            OptionalBody = action;
        }
    }
}