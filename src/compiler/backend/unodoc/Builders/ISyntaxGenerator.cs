using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.Backends.UnoDoc.Builders
{
    public interface ISyntaxGenerator
    {
        string BuildUnoSyntax(IEntity entity, IEntity context = null);
        string BuildUxSyntax(IEntity entity);
    }
}