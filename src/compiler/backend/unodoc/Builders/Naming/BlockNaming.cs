using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.Backends.UnoDoc.Builders.Naming
{
    internal class BlockNaming : Naming, IEntityNaming
    {
        public string GetPageTitle(IEntity entity)
        {
            var block = (Block) entity;
            return block.Name + " Block";
        }

        public string GetIndexTitle(IEntity entity)
        {
            var block = (Block) entity;
            return block.Name;
        }

        public string GetFullIndexTitle(IEntity entity)
        {
            var block = (Block) entity;
            return block.FullName;
        }

        public string GetNavigationTitle(IEntity entity)
        {
            var block = (Block) entity;
            return block.Name + " Block";
        }
    }
}