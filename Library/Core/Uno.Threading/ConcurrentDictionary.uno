using Uno;
using Uno.Collections;

namespace Uno.Threading
{
    public class ConcurrentDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        readonly Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();
        readonly object _mutex = new object();

        // Note: returns a copy of the KeyValuePairs at the time of calling
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            lock (_mutex)
            {
                var result = new List<KeyValuePair<TKey, TValue>>(_dictionary.Count);
                result.AddRange(_dictionary);
                return result.GetEnumerator();
            }
        }

        [Obsolete]
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)this).GetEnumerator();
        }

        // Note: returns a copy of the keys at the time of calling
        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get
            {
                lock (_mutex)
                {
                    var result = new List<TKey>(_dictionary.Count);
                    result.AddRange(_dictionary.Keys);
                    return result;
                }
            }
        }

        public ICollection<TKey> Keys { get { return ((IDictionary<TKey, TValue>)this).Keys; } }

        // Note: returns a copy of the values at the time of calling
        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get
            {
                lock (_mutex)
                {
                    var result = new List<TValue>(_dictionary.Count);
                    result.AddRange(_dictionary.Values);
                    return result;
                }
            }
        }

        public ICollection<TValue> Values { get { return ((IDictionary<TKey, TValue>)this).Values; } }


        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (_mutex)
                return _dictionary.TryGetValue(key, out value);
        }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            lock (_mutex)
                _dictionary.Add(key, value);
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            lock (_mutex)
                return _dictionary.Remove(key);
        }

        [Obsolete]
        public void Add(TKey key, TValue value)
        {
            ((IDictionary<TKey, TValue>)this).Add(key, value);
        }

        [Obsolete]
        public void Remove(TKey key)
        {
            ((IDictionary<TKey, TValue>)this).Remove(key);
        }

        public bool ContainsKey(TKey key)
        {
            lock (_mutex)
                return _dictionary.ContainsKey(key);
        }

        public TValue this[TKey key]
        {
            get
            {
                lock (_mutex)
                    return _dictionary[key];
            }
            set
            {
                lock (_mutex)
                    _dictionary[key] = value;
            }
        }

        public void AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateFun)
        {
            lock (_mutex)
            {
                TValue oldValue = default(TValue);
                _dictionary[key] = _dictionary.TryGetValue(key, out oldValue)
                    ? updateFun(key, oldValue)
                    : addValue;
            }
        }

        public void Clear()
        {
            lock (_mutex)
                _dictionary.Clear();
        }

        public int Count
        {
            get
            {
                lock (_mutex)
                    return _dictionary.Count;
            }
        }

        public void Add(KeyValuePair<TKey, TValue> keyValue)
        {
            lock (_mutex)
                ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Add(keyValue);
        }

        public bool Remove(KeyValuePair<TKey, TValue> keyValue)
        {
            lock (_mutex)
                return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Remove(keyValue);
        }

        public bool Contains(KeyValuePair<TKey, TValue> keyValue)
        {
            lock (_mutex)
                return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Contains(keyValue);
        }
    }
}
