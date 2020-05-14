using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.Backends.UnoDoc.Builders
{
    public interface IExportableCheck
    {
        bool IsExportableAndVisible(IEntity entity);
    }
}