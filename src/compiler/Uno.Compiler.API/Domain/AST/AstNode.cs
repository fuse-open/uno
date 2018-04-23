using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.AST.Members;

namespace Uno.Compiler.API.Domain.AST
{
    public sealed class AstNode : AstBlockBase
    {
        public readonly AstNodeType NodeType;

        public override AstMemberType MemberType => AstMemberType.NodeBlock;

        public AstNode(AstNodeType type, AstIdentifier name, IReadOnlyList<AstBlockMember> members)
            : base(null, null, 0, null, name, members)
        {
            NodeType = type;
        }
    }
}
