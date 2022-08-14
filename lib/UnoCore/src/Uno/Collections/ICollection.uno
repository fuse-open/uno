using Uno.Compiler.ExportTargetInterop;

namespace Uno.Collections
{
    [extern(DOTNET) DotNetType("System.Collections.Generic.ICollection`1")]
    public interface ICollection<T> : IEnumerable<T>
    {
        void Clear();
        void Add(T item);
        bool Remove(T item);
        bool Contains(T item);
        int Count { get; }
    }
}
