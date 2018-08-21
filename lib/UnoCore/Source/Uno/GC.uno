using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.GC")]
    public static class GC
    {
        public static void SuppressFinalize(object obj)
        {
            // TODO: Silent ignore on non-.NET for now
        }
    }
}
