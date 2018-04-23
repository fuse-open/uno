namespace Uno.Collections
{
    class UnionEnumerable<T> : IEnumerable<T>
    {
        IEnumerable<T> _first;
        IEnumerable<T> _second;

        public UnionEnumerable(IEnumerable<T> first, IEnumerable<T> second)
        {
            _first = first;
            _second = second;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new UnionEnumerator<T>(_first.GetEnumerator(), _second.GetEnumerator());
        }
    }

    class UnionEnumerator<T> : IEnumerator<T>
    {
        IEnumerator<T> _current;
        IEnumerator<T> _first;
        IEnumerator<T> _second;

        public UnionEnumerator(IEnumerator<T> first, IEnumerator<T> second)
        {
            _first = first;
            _second = second;
            _current = _first;
        }

        public T Current
        {
            get { return _current.Current; }
        }

        public void Dispose()
        {
            // TODO ?
        }

        public void Reset()
        {
            _first.Reset();
            _second.Reset();
            _current = _first;
        }

        public bool MoveNext()
        {
            if (_current.MoveNext())
                return true;

            if (_current == _first)
            {
                _current = _second;
                return MoveNext(); // correct if MoveNext is called before first item
            }

            return false;
        }
    }
}
