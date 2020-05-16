using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.AST.Members;

namespace Uno.Compiler.API.Domain.AST
{
    public sealed class AstBlock : AstBlockBase
    {
        public readonly IReadOnlyList<AstExpression> UsingBlocks;

        public override AstMemberType MemberType => AstMemberType.Block;

        public AstBlock(string comment, IReadOnlyList<AstAttribute> attributes, Modifiers modifiers, string cond, AstIdentifier name, IReadOnlyList<AstExpression> usingBlocks, IReadOnlyList<AstBlockMember> members)
            : base(comment, attributes, modifiers, cond, name, members)
        {
            UsingBlocks = usingBlocks;
        }

        public AstBlock(AstIdentifier name, IReadOnlyList<AstExpression> usingBlocks, IReadOnlyList<AstBlockMember> members)
            : base(null, AstAttribute.Empty, Modifiers.Public | Modifiers.Generated, null, name, members)
        {
            UsingBlocks = usingBlocks;
        }

        public AstBlock(AstIdentifier name, IReadOnlyList<AstBlockMember> members)
            : base(null, AstAttribute.Empty, Modifiers.Public | Modifiers.Generated, null, name, members)
        {
            UsingBlocks = new AstExpression[0];
        }
    }
}
