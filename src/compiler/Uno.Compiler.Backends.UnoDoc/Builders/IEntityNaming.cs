using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.Backends.UnoDoc.Builders
{
    public interface IEntityNaming
    {
        string GetPageTitle(IEntity entity);
        string GetIndexTitle(IEntity entity);
        string GetFullIndexTitle(IEntity entity);
        string GetNavigationTitle(IEntity entity);
    }
}