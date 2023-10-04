using System.Collections.Generic;

namespace Uno.Collections
{
    public class LowerCamelListDictionary<TValue> : LowerCamelDictionary<List<TValue>>
    {
        public void Add(string key, TValue value)
        {
            GetList(key).Add(value);
        }

        public void AddRange(string key, IEnumerable<TValue> values)
        {
            GetList(key).AddRange(values);
        }

        public List<TValue> GetList(string key)
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