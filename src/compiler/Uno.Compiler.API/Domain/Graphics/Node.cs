namespace Uno.Compiler.API.Domain.Graphics
{
    public sealed class Node : BlockMember
    {
        public readonly MetaBlock Block;

        public override BlockMemberType Type => BlockMemberType.Node;

        public Node(MetaBlock block)
            : base(block.Source)
        {
            Block = block;
        }
    }
}