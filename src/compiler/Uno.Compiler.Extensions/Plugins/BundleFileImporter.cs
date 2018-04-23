using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Plugins;

namespace Uno.Compiler.Extensions.Plugins
{
    class BundleFileImporter : Importer<string>
    {
        public override string Class => "Uno.IO.BundleFileImporter";

        public override Expression Import(ImportContext context, string filename)
        {
            return Bundle.AddBundleFile(context.Source, filename);
        }
    }
}
