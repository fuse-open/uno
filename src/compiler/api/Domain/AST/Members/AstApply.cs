using Uno.Compiler.API.Domain.AST.Expressions;

namespace Uno.Compiler.API.Domain.AST.Members
{
    public sealed class AstApply : AstBlockMember
    {
        public readonly ApplyModifier Modifier;
        public readonly AstExpression Block;

        public override AstMemberType MemberType => AstMemberType.ApplyStatement;

        public AstApply(ApplyModifier modifier, AstExpression block)
        {
            Modifier = modifier;
            Block = block;
        }
    }
}
