using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;

namespace Uno.Compiler.API.Domain.AST.Members
{
    public sealed class AstEvent : AstClassMember
    {
        public readonly AstExpression DelegateType;
        public readonly AstExpression OptionalInterfaceType;
        public readonly AstIdentifier Name;
        public readonly AstAccessor Add;
        public readonly AstAccessor Remove;

        public override AstMemberType MemberType => AstMemberType.Event;

        public AstEvent(string comment, IReadOnlyList<AstAttribute> attrs, Modifiers modifiers, string cond, AstExpression delegateType, AstExpression optionalInterfaceType, AstIdentifier name, AstAccessor add, AstAccessor remove)
            : base(comment, attrs, modifiers, cond)
        {
            DelegateType = delegateType;
            OptionalInterfaceType = optionalInterfaceType;
            Name = name;
            Add = add;
            Remove = remove;
        }
    }
}
