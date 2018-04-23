using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;

namespace Uno.Compiler.API.Domain.AST.Members
{
    public sealed class AstParameter
    {
        public readonly IReadOnlyList<AstAttribute> Attributes;
        public readonly ParameterModifier Modifier;
        public readonly AstExpression OptionalType;
        public readonly AstIdentifier Name;
        public readonly AstExpression OptionalValue;

        public AstParameter(IReadOnlyList<AstAttribute> attributes, ParameterModifier modifier, AstExpression optionalType, AstIdentifier name, AstExpression value = null)
        {
            Attributes = attributes ?? AstAttribute.Empty;
            OptionalType = optionalType;
            Name = name;
            OptionalValue = value;
            Modifier = modifier;
        }
    }
}