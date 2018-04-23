using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;

namespace Uno.Compiler.API.Domain.AST.Members
{
    public class AstAttribute
    {
        public static readonly AstAttribute[] Empty = new AstAttribute[0];

        public readonly string OptionalCondition;
        public readonly AstAttributeModifier Modifier;
        public readonly AstExpression Attribute;
        public readonly IReadOnlyList<AstArgument> Arguments;

        public AstAttribute(string cond, AstAttributeModifier modifier, AstExpression attribute, IReadOnlyList<AstArgument> args)
        {
            OptionalCondition = cond;
            Modifier = modifier;
            Attribute = attribute;
            Arguments = args;
        }
    }
}