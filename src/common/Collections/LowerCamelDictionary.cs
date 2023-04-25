using System.Collections;
using System.Collections.Generic;

namespace Uno.Collections
{
    /**
     * A dictionary that automatically converts all keys to "camelCase.camelCase".
     */
    public class LowerCamelDictionary<T> : IDictionary<string, T>
    {
        readonly Dictionary<string, T> _base = new Dictionary<string, T>();

        public T this[string key]
        {
            get { return _base[key.ToLowerCamelCase()]; }
            set { _base[key.ToLowerCamelCase()] = value; }
        }

        public ICollection<string> Keys => _base.Keys;

        public ICollection<T> Values => _base.Values;

        public int Count => ((ICollection<KeyValuePair<string, T>>)_base).Count;

        public bool IsReadOnly => ((ICollection<KeyValuePair<string, T>>)_base).IsReadOnly;

        public void Add(string key, T value)
        {
            _base.Add(key.ToLowerCamelCase(), value);
        }

        public void Add(KeyValuePair<string, T> item)
        {
            ((ICollection<KeyValuePair<string, T>>)_base).Add(item);
        }

        public void Clear()
        {
            ((ICollection<KeyValuePair<string, T>>)_base).Clear();
        }

        public bool Contains(KeyValuePair<string, T> item)
        {
            return ((ICollection<KeyValuePair<string, T>>)_base).Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return _base.ContainsKey(key.ToLowerCamelCase());
        }

        public void CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, T>>)_base).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, T>>)_base).GetEnumerator();
        }

        public bool Remove(string key)
        {
            return _base.Remove(key.ToLowerCamelCase());
        }

        public bool Remove(KeyValuePair<string, T> item)
        {
            return ((ICollection<KeyValuePair<string, T>>)_base).Remove(item);
        }

        public bool TryGetValue(string key, out T value)
        {
            return _base.TryGetValue(key.ToLowerCamelCase(), out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_base).GetEnumerator();
        }
    }
}
