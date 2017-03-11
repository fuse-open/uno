using Uno.Compiler.ExportTargetInterop;
using Uno.Math;

namespace Uno.Collections
{
    [extern(DOTNET) DotNetType("System.Collections.Generic.Dictionary`2")]
    public sealed class Dictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        internal enum BucketState
        {
            Empty = 0,
            Used,
            Dummy,
        }

        internal struct Bucket
        {
            public TKey Key;
            public TValue Value;
            public BucketState State;
        }

        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            Dictionary<TKey, TValue> _source;
            KeyValuePair<TKey, TValue> _current;
            int _iterator;
            int _version;

            internal Enumerator(Dictionary<TKey, TValue> source)
            {
                _source = source;
                _version = source._version;
                _iterator = -1;
            }

            public KeyValuePair<TKey, TValue> Current
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
                _current = default(KeyValuePair<TKey, TValue>);
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

                } while (_source._buckets[_iterator].State != BucketState.Used);

                var bucket = _source._buckets[_iterator];
                _current = new KeyValuePair<TKey, TValue>(bucket.Key, bucket.Value);

                return true;
            }
        }

        public class KeyCollection : ICollection<TKey>
        {
            public struct Enumerator : IEnumerator<TKey>
            {
                readonly Dictionary<TKey, TValue> _source;

                TKey _current;
                int _iterator;
                int _version;

                internal Enumerator(Dictionary<TKey, TValue> source)
                {
                    _source = source;
                    _version = source._version;
                    _iterator = -1;
                }

                public TKey Current
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
                    _current = default(TKey);
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

                    } while (_source._buckets[_iterator].State != BucketState.Used);

                    var bucket = _source._buckets[_iterator];
                    _current = bucket.Key;

                    return true;
                }
            }

            readonly Dictionary<TKey, TValue> _source;

            internal KeyCollection(Dictionary<TKey, TValue> source)
            {
                _source = source;
            }

            public Enumerator GetEnumerator()
            {
                return new Enumerator(_source);
            }

            void ICollection<TKey>.Clear()
            {
                throw new InvalidOperationException();
            }

            void ICollection<TKey>.Add(TKey item)
            {
                throw new InvalidOperationException();
            }

            bool ICollection<TKey>.Remove(TKey item)
            {
                throw new InvalidOperationException();
            }

            bool ICollection<TKey>.Contains(TKey item)
            {
                throw new InvalidOperationException();
            }

            public int Count
            {
                get { return _source.Count; }
            }
        }

        public class ValueCollection : ICollection<TValue>
        {
            public struct Enumerator : IEnumerator<TValue>
            {
                Dictionary<TKey, TValue> _source;
                TValue _current;
                int _iterator;
                int _version;

                internal Enumerator(Dictionary<TKey, TValue> source)
                {
                    _source = source;
                    _version = source._version;
                    _iterator = -1;
                }

                public TValue Current
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
                    _current = default(TValue);
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

                    } while (_source._buckets[_iterator].State != BucketState.Used);

                    var bucket = _source._buckets[_iterator];
                    _current = bucket.Value;

                    return true;
                }
            }

            readonly Dictionary<TKey, TValue> _source;

            internal ValueCollection(Dictionary<TKey, TValue> source)
            {
                _source = source;
            }

            public Enumerator GetEnumerator()
            {
                return new Enumerator(_source);
            }

            void ICollection<TValue>.Clear()
            {
                throw new InvalidOperationException();
            }

            void ICollection<TValue>.Add(TValue item)
            {
                throw new InvalidOperationException();
            }

            bool ICollection<TValue>.Remove(TValue item)
            {
                throw new InvalidOperationException();
            }

            bool ICollection<TValue>.Contains(TValue item)
            {
                throw new InvalidOperationException();
            }

            public int Count
            {
                get { return _source.Count; }
            }
        }

        internal int _count;
        internal int _dummies;
        internal Bucket[] _buckets;
        internal int _version;

        public Dictionary()
        {
            _buckets = new Bucket[8];
            _count = 0;
            _dummies = 0;
        }

        public Dictionary(IDictionary<TKey, TValue> dictionary)
            : this()
        {
            // TODO: This should be done differently, using AddRange for instance
            foreach (var keyValuePair in dictionary)
                Add(keyValuePair.Key, keyValuePair.Value);
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public KeyCollection Keys
        {
            get { return new KeyCollection(this); }
        }

        public ValueCollection Values
        {
            get { return new ValueCollection(this); }
        }

        public int Count
        {
            get { return _count; }
        }

        void Rehash()
        {
            var oldBuckets = _buckets;
            _buckets = new Bucket[oldBuckets.Length * 2];

            _count = 0;
            _dummies = 0;

            for (int i = 0; i < oldBuckets.Length; i++)
                if (oldBuckets[i].State == BucketState.Used)
                    Add(oldBuckets[i].Key, oldBuckets[i].Value);
        }

        public void Clear()
        {
            for (int i = 0; i < _buckets.Length; i++)
            {
                _buckets[i].State = BucketState.Empty;
                _buckets[i].Value = default(TValue);
                _buckets[i].Key = default(TKey);
            }

            _count = 0;
            _dummies = 0;
            _version++;
        }

        public void Add(TKey key, TValue value)
        {
            if (_count + _dummies > _buckets.Length / 2)
                Rehash();

            int x = Abs(key.GetHashCode()) & (_buckets.Length - 1);

            while (true)
            {
                if (_buckets[x].State == BucketState.Empty)
                {
                    _buckets[x].State = BucketState.Used;
                    _buckets[x].Value = value;
                    _buckets[x].Key = key;
                    _count++;
                    _version++;
                    return;
                }
                else if (_buckets[x].State == BucketState.Dummy)
                {
                    _buckets[x].State = BucketState.Used;
                    _buckets[x].Value = value;
                    _buckets[x].Key = key;
                    _count++;
                    _dummies--;
                    _version++;
                    return;
                }
                else if (_buckets[x].State == BucketState.Used)
                {
                    if (Generic.Equals(_buckets[x].Key, key))
                        throw new Exception("Dictionary already contains the given key");
                }

                x++;

                if (x >= _buckets.Length)
                    x = 0;
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            int x = Abs(key.GetHashCode()) & (_buckets.Length - 1);

            while (true)
            {
                if (_buckets[x].State == BucketState.Used)
                {
                    if (Generic.Equals(_buckets[x].Key, key))
                    {
                        value = _buckets[x].Value;
                        return true;
                    }
                }
                else if (_buckets[x].State == BucketState.Empty)
                {
                    value = default(TValue);
                    return false;
                }

                x++;

                if (x >= _buckets.Length)
                    x = 0;
            }
        }

        public bool Remove(TKey key)
        {
            int x = Abs(key.GetHashCode()) & (_buckets.Length - 1);

            while (true)
            {
                if (_buckets[x].State == BucketState.Used)
                {
                    if (Generic.Equals(_buckets[x].Key, key))
                    {
                        _buckets[x].State = BucketState.Dummy;
                        _buckets[x].Value = default(TValue);
                        _buckets[x].Key = default(TKey);
                        _count--;
                        _dummies++;
                        _version++;
                        return true;
                    }
                }
                else if (_buckets[x].State == BucketState.Empty)
                {
                    return false;
                }

                x++;

                if (x >= _buckets.Length)
                    x = 0;
            }
        }

        public bool ContainsKey(TKey key)
        {
            int x = Abs(key.GetHashCode()) & (_buckets.Length - 1);

            while (true)
            {
                if (_buckets[x].State == BucketState.Used)
                {
                    if (Generic.Equals(_buckets[x].Key, key))
                    {
                        return true;
                    }
                }
                else if (_buckets[x].State == BucketState.Empty)
                {
                    return false;
                }

                x++;

                if (x >= _buckets.Length)
                    x = 0;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                int x = Abs(key.GetHashCode()) & (_buckets.Length - 1);

                while (true)
                {
                    if (_buckets[x].State == BucketState.Used)
                    {
                        if (Generic.Equals(_buckets[x].Key, key))
                        {
                            return _buckets[x].Value;
                        }
                    }
                    else if (_buckets[x].State == BucketState.Empty)
                    {
                        throw new Exception("Dictionary did not contain the given key");
                    }

                    x++;

                    if (x >= _buckets.Length)
                        x = 0;
                }
            }
            set
            {
                int x = Abs(key.GetHashCode()) & (_buckets.Length - 1);

                while (true)
                {
                    if (_buckets[x].State == BucketState.Used)
                    {
                        if (Generic.Equals(_buckets[x].Key, key))
                        {
                            _buckets[x].Value = value;
                            _version++;
                            return;
                        }
                    }
                    else if (_buckets[x].State == BucketState.Empty)
                    {
                        Add(key, value);
                        return;
                    }

                    x++;

                    if (x >= _buckets.Length)
                        x = 0;
                }
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new InvalidOperationException();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new InvalidOperationException();
        }
    }
}
