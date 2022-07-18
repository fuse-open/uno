using Uno.Compiler.ExportTargetInterop;

namespace Uno.Collections
{
    [extern(DOTNET) DotNetType("System.Collections.Generic.IList`1")]
    public interface IList<T> : ICollection<T>
    {
        void Insert(int index, T item);
        void RemoveAt(int index);

        T this[int index]
        {
            get;
            //set;
        }
    }
}
