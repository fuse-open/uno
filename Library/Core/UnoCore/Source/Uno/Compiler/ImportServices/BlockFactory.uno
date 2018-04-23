using Uno.Compiler.ExportTargetInterop;

namespace Uno.Compiler.ImportServices
{
    [DontExport, Obsolete]
    public abstract class BlockFactory
    {
        public abstract string GetCacheKey();
        public abstract Ast.Block CreateBlock(BlockFactoryContext ctx);
    }
}
