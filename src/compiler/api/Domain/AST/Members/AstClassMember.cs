using System.Collections.Generic;

namespace Uno.Compiler.API.Domain.AST.Members
{
    public abstract class AstClassMember : AstBlockMember
    {
        public readonly string DocComment;
        public readonly IReadOnlyList<AstAttribute> Attributes;
        public readonly Modifiers Modifiers;
        public readonly string OptionalCondition;

        protected AstClassMember(string comment, IReadOnlyList<AstAttribute> attributes, Modifiers modifiers, string cond)
        {
            DocComment = comment;
            Attributes = attributes ?? AstAttribute.Empty;
            Modifiers = modifiers;
            OptionalCondition = cond;
        }
    }
}