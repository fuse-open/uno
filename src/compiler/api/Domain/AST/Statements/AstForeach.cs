using Uno.Compiler.API.Domain.AST.Expressions;

namespace Uno.Compiler.API.Domain.AST.Statements
{
    public sealed class AstForeach : AstStatement
    {
        public readonly AstExpression ElementType;
        public readonly AstIdentifier ElementName;
        public readonly AstExpression Collection;
        public readonly AstStatement OptionalBody;

        public override AstStatementType StatementType => AstStatementType.Foreach;

        public AstForeach(Source src, AstExpression dt, AstIdentifier iterator, AstExpression collection, AstStatement body): base(src)
        {
            ElementType = dt;
            ElementName = iterator;
            Collection = collection;
            OptionalBody = body;
        }
    }
}