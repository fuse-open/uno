using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;

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

            // TODO: This is a temp hack that replaces [ExportCondition("CONDITION")] with extern(CONDITION)
            if (OptionalCondition == null)
            {
                foreach (var attr in Attributes)
                {
                    var name = attr.Attribute as AstIdentifier;
                    if (name != null && name.Symbol == "ExportCondition" &&
                        attr.Arguments.Count == 1 && attr.Arguments[0].Value is AstString)
                    {
                        var val = (AstString) attr.Arguments[0].Value;
                        OptionalCondition = val.Value;
                        Modifiers |= Modifiers.Extern;
                    }
                }
            }
        }
    }
}