using Uno.Compiler.ExportTargetInterop;

namespace Uno.Compiler.ImportServices
{
    [DontExport, Obsolete]
    public abstract class Importer<T>
    {
        public abstract string GetCacheKey();
        public abstract T Import(ImporterContext ctx);
    }
}
