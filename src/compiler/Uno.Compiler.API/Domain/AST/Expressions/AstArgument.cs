namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public class AstArgument
    {
        public static readonly AstArgument[] Empty = new AstArgument[0];

        public readonly AstIdentifier OptionalName;
        public readonly ParameterModifier Modifier;
        public readonly AstExpression Value;

        public AstArgument(AstIdentifier optionalName, ParameterModifier modifiers, AstExpression value)
        {
            OptionalName = optionalName;
            Modifier = modifiers;
            Value = value;
        }

        public static implicit operator AstArgument(AstExpression value)
        {
            return new AstArgument(null, 0, value);
        }
    }
}
