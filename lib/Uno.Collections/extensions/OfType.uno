namespace Uno.Collections
{
    class OfTypeEnumerable<T, U> : IEnumerable<U>
    {
        readonly IEnumerable<T> _source;

        public OfTypeEnumerable(IEnumerable<T> source)
        {
            _source = source;
        }

        public IEnumerator<U> GetEnumerator()
        {
            return new OfTypeEnumerator<T, U>(_source.GetEnumerator());
        }
    }

    class OfTypeEnumerator<T, U> : IEnumerator<U>
    {
        readonly IEnumerator<T> _source;
        U _current;

        public OfTypeEnumerator(IEnumerator<T> source)
        {
            _source = source;
        }

        public U Current
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

                if (((object)_source.Current) is U)
                {
                    var obj = ((object)_source.Current);
                    _current = (U)obj;
                    return true;
                }
            }
            return false;
        }
    }
}
