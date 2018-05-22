using Uno.Compiler.ExportTargetInterop;

namespace Uno.Collections
{
    [extern(DOTNET) DotNetType("System.Collections.Generic.HashSet`1")]
    public sealed class HashSet<T> : IEnumerable<T>
    {
        public struct Enumerator : IEnumerator<T>
        {
            Dictionary<T, bool> _source;
            T _current;
            int _iterator;
            int _version;

            internal Enumerator(Dictionary<T, bool> source)
            {
                _source = source;
                _version = source._version;
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

            void IEnumerator.Reset()
            {
                _iterator = -1;
                _version = _source._version;
                _current = default(T);
            }

            public bool MoveNext()
            {
                if (_version != _source._version)
                    throw new InvalidOperationException("Collection was modified");

                do
                {
                    _iterator++;

                    if (_iterator >= _source._buckets.Length)
                        return false;

                } while (_source._buckets[_iterator].State != Dictionary<T, bool>.BucketState.Used);

                var bucket = _source._buckets[_iterator];
                _current = bucket.Key;

                return true;
            }
        }

        Dictionary<T, bool> _map = new Dictionary<T, bool>();

        public HashSet() { }

        public HashSet(IEnumerable<T> items)
        {
            foreach (var item in items)
                Add(item);
        }

        public bool SetEquals(IEnumerable<T> that)
        {
            var thatSet = new HashSet<T>();
            foreach (var item in that)
            {
                if (!this.Contains(item))
                    return false;
                thatSet.Add(item);
            }

            foreach (var item in this)
                if (!thatSet.Contains(item))
                    return false;

            return true;
        }

        public bool Add(T item)
        {
            var result = _map.ContainsKey(item);
            _map[item] = true;
            return result;
        }

        public bool Contains(T item)
        {
            return _map.ContainsKey(item);
        }

        public bool Remove(T item)
        {
            return _map.Remove(item);
        }

        public int Count
        {
            get { return _map.Count; }
        }

        public void Clear()
        {
            _map.Clear();
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(_map);
        }
    }
}