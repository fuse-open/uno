using Uno.Compiler.ExportTargetInterop;

namespace Uno.Collections
{
    [extern(DOTNET) DotNetType("System.Collections.ObjectModel.ReadOnlyCollection`1")]
    public sealed class ReadOnlyCollection<T> : IReadOnlyList<T>
    {
        IList<T> _list;

        public ReadOnlyCollection(IList<T> list)
        {
            _list = list;
        }

        public int Count { get { return _list.Count; } }

        public T this[int index] { get { return _list[index]; } }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}
