using Uno.Compiler.API.Domain.Graphics;

namespace Uno.Compiler.Core.Syntax.Binding
{
    public class PartialBlock : PartialExpression
    {
        public override PartialExpressionType ExpressionType => PartialExpressionType.Block;

        public readonly Block Block;

        public PartialBlock(Source src, Block b)
            : base(src)
        {
            Block = b;
        }

        public override string ToString()
        {
            return Block.ToString();
        }
    }
}