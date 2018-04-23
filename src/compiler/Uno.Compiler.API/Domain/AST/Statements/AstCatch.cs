using Uno.Compiler.API.Domain.AST.Expressions;

namespace Uno.Compiler.API.Domain.AST.Statements
{
    public struct AstCatch
    {
        public readonly AstExpression OptionalType;
        public readonly AstIdentifier Name;
        public readonly AstScope Body;

        public AstCatch(AstExpression optionalType, AstIdentifier name, AstScope body)
        {
            Body = body;
            OptionalType = optionalType;
            Name = name;
        }
    }
}