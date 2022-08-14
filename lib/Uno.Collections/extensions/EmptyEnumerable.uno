namespace Uno.Collections
{
    public class EmptyEnumerable<T> : IEnumerable<T>
    {
        public IEnumerator<T> GetEnumerator()
        {
            return new EmptyEnumerator<T>();
        }
    }

    class EmptyEnumerator<T> : IEnumerator<T>
    {
        public T Current { get { throw new InvalidOperationException(); } }
        public void Reset()  { }
        public bool MoveNext() { return false; }
        public void Dispose() { }
    }
}
