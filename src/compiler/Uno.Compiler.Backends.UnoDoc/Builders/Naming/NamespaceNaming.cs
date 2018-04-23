using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.Backends.UnoDoc.Builders.Naming
{
    internal class NamespaceNaming : Naming, IEntityNaming
    {
        public string GetPageTitle(IEntity entity)
        {
            var ns = (Namespace) entity;
            return ns.FullName + " Namespace";
        }

        public string GetIndexTitle(IEntity entity)
        {
            var ns = (Namespace) entity;
            return ns.FullName;
        }

        public string GetFullIndexTitle(IEntity entity)
        {
            var ns = (Namespace) entity;
            return ns.FullName;
        }

        public string GetNavigationTitle(IEntity entity)
        {
            var ns = (Namespace) entity;
            return ns.FullName + " Namespace";
        }
    }
}