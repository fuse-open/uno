using Uno.Compiler.API.Domain.AST.Expressions;

namespace Uno.Compiler.API.Domain.AST.Statements
{
    public struct AstVariable
    {
        public readonly AstIdentifier Name;
        public readonly AstExpression OptionalValue;

        public AstVariable(AstIdentifier name, AstExpression value = null)
        {
            Name = name;
            OptionalValue = value;
        }
    }
}