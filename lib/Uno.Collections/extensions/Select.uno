namespace Uno.Collections
{
    class SelectEnumerable<T, TRet> : IEnumerable<TRet>
    {
        readonly IEnumerable<T> _source;
        readonly Func<T, TRet> _select;

        public SelectEnumerable(IEnumerable<T> source, Func<T, TRet> select)
        {
            _source = source;
            _select = select;
        }

        public IEnumerator<TRet> GetEnumerator()
        {
            return new SelectEnumerator<T, TRet>(_source.GetEnumerator(), _select);
        }
    }

    class SelectEnumerator<T, TRet> : IEnumerator<TRet>
    {
        readonly IEnumerator<T> _source;
        readonly Func<T, TRet> _select;

        public SelectEnumerator(IEnumerator<T> source, Func<T, TRet> select)
        {
            _source = source;
            _select = select;
        }

        public TRet Current
        {
            get { return _select(_source.Current); }
        }

        public void Dispose()
        {
            // ?
        }

        public void Reset()
        {
            _source.Reset();
        }

        public bool MoveNext()
        {
            return _source.MoveNext();
        }
    }
}
