using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.AST.Members;

namespace Uno.Compiler.API.Domain.AST
{
    public sealed class AstEnum : AstBlockBase
    {
        public readonly AstExpression OptionalBaseType;
        public readonly IReadOnlyList<AstLiteral> Literals;

        public override AstMemberType MemberType => AstMemberType.Enum;

        public AstEnum(string comment, IReadOnlyList<AstAttribute> attrs, Modifiers modifiers, string cond, AstIdentifier name, AstExpression optionalBaseType, IReadOnlyList<AstLiteral> literals)
            : base(comment, attrs, modifiers, cond, name, null)
        {
            OptionalBaseType = optionalBaseType;
            Literals = literals;
        }
    }
}