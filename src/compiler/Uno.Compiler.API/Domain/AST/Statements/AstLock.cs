using Uno.Compiler.API.Domain.AST.Expressions;

namespace Uno.Compiler.API.Domain.AST.Statements
{
    public sealed class AstLock : AstStatement
    {
        public readonly AstExpression Object;
        public readonly AstStatement OptionalBody;

        public override AstStatementType StatementType => AstStatementType.Lock;

        public AstLock(Source src, AstExpression obj, AstStatement optionalBody)
            : base(src)
        {
            Object = obj;
            OptionalBody = optionalBody;
        }
    }
}