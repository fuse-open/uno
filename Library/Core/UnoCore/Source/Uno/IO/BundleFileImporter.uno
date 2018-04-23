using Uno.Compiler.ExportTargetInterop;
using Uno.Compiler.ImportServices;

namespace Uno.IO
{
    [DontExport, Obsolete("Use import(FILENAME) instead")]
    public class BundleFileImporter : Importer<BundleFile>
    {
        public extern BundleFileImporter([Filename] string filename);

        public override extern string GetCacheKey();
        public override extern BundleFile Import(ImporterContext ctx);
    }
}
