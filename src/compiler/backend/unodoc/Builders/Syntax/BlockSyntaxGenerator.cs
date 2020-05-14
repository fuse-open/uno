using System.Text;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.Backends.UnoDoc.Builders.Extensions;

namespace Uno.Compiler.Backends.UnoDoc.Builders.Syntax
{
    internal class BlockSyntaxGenerator : SyntaxGenerator, ISyntaxGenerator
    {
        public BlockSyntaxGenerator(IExportableCheck exportableCheck, IEntityNaming entityNaming)
                : base(exportableCheck, entityNaming) {}

        public string BuildUnoSyntax(IEntity entity, IEntity context = null)
        {
            var block = (Block) entity;
            var result = new StringBuilder();

            result.Append(BuildAttributes(block.Attributes, block));
            result.Append(BuildModifiers(block.GetModifierNames()));
            result.Append(block.BlockType.ToString("G").ToLowerInvariant());
            result.Append(block.Name);
            result.Append(" {}");

            return result.ToString();
        }

        public string BuildUxSyntax(IEntity entity)
        {
            return null;
        }
    }
}