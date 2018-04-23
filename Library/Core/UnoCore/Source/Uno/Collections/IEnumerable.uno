using Uno.Compiler.ExportTargetInterop;

namespace Uno.Collections
{
    [extern(DOTNET) DotNetType("System.Collections.Generic.IEnumerable`1")]
    public interface IEnumerable<T>
    {
        IEnumerator<T> GetEnumerator();
    }
}
