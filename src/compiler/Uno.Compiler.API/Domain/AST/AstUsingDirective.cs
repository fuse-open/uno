using Uno.Compiler.API.Domain.AST.Expressions;

namespace Uno.Compiler.API.Domain.AST
{
    public sealed class AstUsingDirective
    {
        public readonly AstExpression Expression;
        public readonly AstIdentifier OptionalAlias;
        public readonly bool IsStatic;

        public AstUsingDirective(AstExpression expression, AstIdentifier alias = null, bool isStatic = false)
        {
            Expression = expression;
            OptionalAlias = alias;
            IsStatic = isStatic;
        }
    }
}