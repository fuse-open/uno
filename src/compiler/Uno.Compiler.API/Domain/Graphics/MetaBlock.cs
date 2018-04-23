using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.API.Domain.Graphics
{
    public sealed class MetaBlock : BlockBase
    {
        public static readonly MetaBlock Invalid = new MetaBlock(Source.Unknown, Block.Invalid, "<invalid>", MetaBlockType.Scope);

        public readonly MetaBlockType MetaBlockType;

        public MetaBlock(Source src, Namescope parent, string name, MetaBlockType type)
            : base(src, parent, name)
        {
            MetaBlockType = type;
        }

        public override BlockType BlockType => BlockType.MetaBlock;
    }
}