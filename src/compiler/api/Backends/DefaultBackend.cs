using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.API.Backends
{
    public class DefaultBackend : Backend
    {
        public override string Name => "Uno";

        public DefaultBackend()
        {
            Options = BackendOptions.ExportDontExports;
        }

        public override bool CanLink(Function f)
        {
            return f.IsExtern || !f.Source.Package.IsStartup;
        }
    }
}
