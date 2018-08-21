namespace Uno.Collections
{
    class WhereEnumerable<T> : IEnumerable<T>
    {
        readonly IEnumerable<T> _source;
        readonly Predicate<T> _predicate;

        public WhereEnumerable(IEnumerable<T> source, Predicate<T> predicate)
        {
            _source = source;
            _predicate = predicate;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new WhereEnumerator<T>(_source.GetEnumerator(), _predicate);
        }
    }

    class WhereEnumerator<T> : IEnumerator<T>
    {
        readonly IEnumerator<T> _source;
        readonly Predicate<T> _predicate;
        T _current;

        public WhereEnumerator(IEnumerator<T> source, Predicate<T> predicate)
        {
            _source = source;
            _predicate = predicate;
        }

        public T Current
        {
            get { return _current; }
        }

        public void Dispose()
        {
            // TODO ?
        }

        public void Reset()
        {
            _source.Reset();
        }

        public bool MoveNext()
        {
            while (true)
            {
                if (!_source.MoveNext())
                    return false;

                if (_predicate(_source.Current))
                {
                    _current = _source.Current;
                    return true;
                }
            }
            return false;
        }
    }
}
