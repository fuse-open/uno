using Uno.Compiler.ExportTargetInterop;

namespace Uno.Collections
{
    [extern(DOTNET) DotNetType("System.Collections.Generic.IReadOnlyList`1")]
    public interface IReadOnlyList<T> : IEnumerable<T>
    {
        int Count { get; }

        T this[int index]
        {
            get;
        }
    }
}
