using Uno.Compiler.ExportTargetInterop;

namespace Uno.Collections
{
    [extern(DOTNET) DotNetType("System.Collections.IEnumerator")]
    public interface IEnumerator
    {
        void Reset();
        bool MoveNext();
    }

    [extern(DOTNET) DotNetType("System.Collections.Generic.IEnumerator`1")]
    public interface IEnumerator<T> : IDisposable, IEnumerator
    {
        T Current { get; }
    }
}
