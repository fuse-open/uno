using Uno.Collections;

namespace Uno.Internal
{
    public sealed class ArrayEnumerator<T> : IEnumerator<T>
    {
        readonly T[] _source;
        int _iterator;
        T _current;

        public ArrayEnumerator(T[] source)
        {
            _source = source;
            _iterator = -1;
        }

        public T Current
        {
            get { return _current; }
        }

        public void Dispose()
        {
            // TODO
        }

        public void Reset()
        {
            _iterator = -1;
            _current = default(T);
        }

        public bool MoveNext()
        {
            _iterator++;

            if (_iterator < _source.Length)
            {
                _current = _source[_iterator];
                return true;
            }

            return false;
        }
    }

    public sealed class ArrayEnumerable<T> : IEnumerable<T>
    {
        readonly T[] _source;

        public ArrayEnumerable(T[] source)
        {
            _source = source;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new ArrayEnumerator<T>(_source);
        }
    }

    public sealed class ArrayList<T> : IList<T>
    {
        readonly T[] _source;

        public ArrayList(T[] source)
        {
            _source = source;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new ArrayEnumerator<T>(_source);
        }

        public int Count { get { return _source.Length; } }

        public T this[int index]
        {
            get { return _source[index]; }
            set { _source[index] = value; }
        }

        public bool Contains(T item)
        {
            foreach (var t in _source)
                if (t.Equals(item))
                    return true;

            return false;
        }

        // Not supported:

        public void Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public void Add(T item)
        {
            throw new NotSupportedException();
        }

        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }
    }
}
