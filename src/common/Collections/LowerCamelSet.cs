using System.Collections;
using System.Collections.Generic;

namespace Uno.Collections
{
    /**
     * A set that automatically converts all keys to "camelCase.camelCase".
     */
    public class LowerCamelSet : IEnumerable<string>
    {
        readonly HashSet<string> _base = new();

        public void Add(string key)
        {
            _base.Add(key.ToLowerCamelCase());
        }

        public bool Contains(string key)
        {
            return _base.Contains(key.ToLowerCamelCase());
        }

        public IEnumerator<string> GetEnumerator()
        {
            return ((IEnumerable<string>)_base).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_base).GetEnumerator();
        }
    }
}
