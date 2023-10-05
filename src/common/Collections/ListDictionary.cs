using System.Collections.Generic;

namespace Uno.Collections
{
    public class ListDictionary<TKey, TValue> : Dictionary<TKey, List<TValue>>
    {
        public void Add(TKey key, TValue value)
        {
            GetList(key).Add(value);
        }

        public void AddRange(TKey key, IEnumerable<TValue> values)
        {
            GetList(key).AddRange(values);
        }

        public List<TValue> GetList(TKey key)
        {
            if (!TryGetValue(key, out List<TValue> list))
            {
                list = new List<TValue>();
                Add(key, list);
            }

            return list;
        }
    }
}