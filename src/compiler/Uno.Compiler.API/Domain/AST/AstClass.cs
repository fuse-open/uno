using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.AST.Members;

namespace Uno.Compiler.API.Domain.AST
{
    public sealed  class AstClass : AstBlockBase
    {
        public readonly AstClassType Type;
        public readonly AstGenericSignature OptionalGeneric;
        public readonly IReadOnlyList<AstExpression> Bases;
        public readonly IReadOnlyList<AstExpression> Swizzlers;

        public override AstMemberType MemberType => AstMemberType.Class;

        public AstClass(
            string comment,
            IReadOnlyList<AstAttribute> attrs, 
            Modifiers modifiers, 
            string cond, 
            AstClassType type, 
            AstIdentifier name, 
            IReadOnlyList<AstExpression> bases, 
            AstGenericSignature optionalGenericSig,
            IReadOnlyList<AstBlockMember> members,
            IReadOnlyList<AstExpression> swizzlers)
            : base(comment, attrs, modifiers, cond, name, members)
        {
            Type = type;
            Bases = bases;
            Swizzlers = swizzlers;
            OptionalGeneric = optionalGenericSig;
        }
    }
}
