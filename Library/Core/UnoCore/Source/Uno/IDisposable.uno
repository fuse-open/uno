using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.IDisposable")]
    public interface IDisposable
    {
        void Dispose();
    }
}
