using Uno.Compiler.API.Domain.AST.Expressions;

namespace Uno.Compiler.API.Domain.AST.Statements
{
    public sealed class AstFor : AstStatement
    {
        public readonly AstStatement OptionalInitializer;
        public readonly AstExpression OptionalCondition;
        public readonly AstExpression OptionalIncrement;
        public readonly AstStatement OptionalBody;

        public override AstStatementType StatementType => AstStatementType.For;

        public AstFor(Source src, AstStatement optInit, AstExpression optCond, AstExpression optIncr, AstStatement optBody)
            : base(src)
        {
            OptionalInitializer = optInit;
            OptionalCondition = optCond;
            OptionalIncrement = optIncr;
            OptionalBody = optBody;
        }
    }
}