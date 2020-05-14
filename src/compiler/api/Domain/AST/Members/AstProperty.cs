using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;

namespace Uno.Compiler.API.Domain.AST.Members
{
    public sealed class AstProperty : AstNamedMember
    {
        public readonly AstExpression OptionalInterfaceType;
        public readonly AstAccessor Get;
        public readonly AstAccessor Set;

        public override AstMemberType MemberType => AstMemberType.Property;

        public AstProperty(string comment, IReadOnlyList<AstAttribute> attrs, Modifiers modifiers, string cond, AstExpression dataType, AstExpression optionalInterfaceType, AstIdentifier name, AstAccessor get, AstAccessor set)
            : base(comment, attrs, modifiers, cond, dataType, name)
        {
            OptionalInterfaceType = optionalInterfaceType;
            Get = get;
            Set = set;
        }
    }
}