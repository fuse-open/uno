using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.API.Domain.Graphics
{
    public sealed class Apply : BlockMember
    {
        public readonly ApplyModifier Modifier;
        public readonly Block Block;
        public readonly Expression Object;

        public override BlockMemberType Type => BlockMemberType.Apply;

        public Apply(Source src, ApplyModifier modifier, Block block, Expression obj)
            : base(src)
        {
            Modifier = modifier;
            Block = block;
            Object = obj;
        }
    }
}