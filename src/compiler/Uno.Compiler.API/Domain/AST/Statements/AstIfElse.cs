using Uno.Compiler.API.Domain.AST.Expressions;

namespace Uno.Compiler.API.Domain.AST.Statements
{
    public sealed class AstIfElse : AstStatement
    {
        public readonly AstExpression Condition;
        public readonly AstStatement OptionalIfBody, OptionalElseBody;

        public override AstStatementType StatementType => AstStatementType.IfElse;

        public AstIfElse(Source src, AstExpression cond, AstStatement ifBody, AstStatement elseBody = null)
            : base(src)
        {
            Condition = cond;
            OptionalIfBody = ifBody;
            OptionalElseBody = elseBody;
        }
    }
}