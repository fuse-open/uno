using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.AST.Members;

namespace Uno.Compiler.API.Domain.AST
{
    public class AstLiteral
    {
        public readonly string DocComment;
        public readonly IReadOnlyList<AstAttribute> Attributes;
        public readonly AstIdentifier Name;
        public readonly AstExpression OptionalValue;

        public AstLiteral(string comment, IReadOnlyList<AstAttribute> attrs, AstIdentifier name, AstExpression optionalValue)
        {
            DocComment = comment;
            Attributes = attrs ?? AstAttribute.Empty;
            Name = name;
            OptionalValue = optionalValue;
        }
    }
}
