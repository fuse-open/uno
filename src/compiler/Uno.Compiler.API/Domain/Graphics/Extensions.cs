using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.API.Domain.Graphics
{
    public static class Extensions
    {
        public static Block TryFindBlockParent(this BlockBase block)
        {
            if (block.BlockType == BlockType.Block)
                return (Block)block;

            if (block.IsNestedBlock)
                return TryFindBlockParent(block.ParentBlock);

            return null;
        }

        public static DrawBlock TryFindDrawBlockParent(this BlockBase block)
        {
            if (block.BlockType == BlockType.DrawBlock)
                return (DrawBlock)block;

            if (block.IsNestedBlock)
                return TryFindDrawBlockParent(block.ParentBlock);

            return null;
        }

        public static DataType TryFindTypeParent(this BlockBase block)
        {
            if (block == null)
                return null;

            var dt = block.ParentType;

            if (dt != null && dt.Block == block)
                return dt;

            if (block.IsNestedBlock)
                return TryFindTypeParent(block.ParentBlock);

            return null;
        }
    }
}
