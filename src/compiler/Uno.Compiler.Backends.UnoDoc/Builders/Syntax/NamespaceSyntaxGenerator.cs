using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.Backends.UnoDoc.Builders.Syntax
{
    internal class NamespaceSyntaxGenerator : SyntaxGenerator, ISyntaxGenerator
    {
        public NamespaceSyntaxGenerator(IExportableCheck exportableCheck, IEntityNaming entityNaming)
                : base(exportableCheck, entityNaming) {}

        public string BuildUnoSyntax(IEntity entity, IEntity context = null)
        {
            return "namespace " + entity.Name + " {}";
        }

        public string BuildUxSyntax(IEntity entity)
        {
            return null;
        }
    }
}