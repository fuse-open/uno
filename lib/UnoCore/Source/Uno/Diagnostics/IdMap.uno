using Uno.Collections;
using Uno.IO;
using Uno;

namespace Uno.Diagnostics
{
    [Obsolete]
    public class IdMap<T>
    {
        private int _nextId;
        private readonly Dictionary<T, int> _objectToInt = new Dictionary<T, int>();
        private readonly List<T> _intToObject = new List<T>();
        private readonly bool _twoWay;

        public IdMap(bool twoWay)
        {
            _twoWay = twoWay;
        }

        public int IdFor(T s)
        {
            int id;
            if (_objectToInt.TryGetValue(s, out id))
                return id;
            id = _nextId++;
            _objectToInt.Add(s, id);
            if (_twoWay)
                _intToObject.Add(s);
            return id;
        }

        public T StringFor(int id)
        {
            if (!_twoWay)
                throw new Exception("Internal error: Cannot do reverse lookup on one-way IdMap.");
            return _intToObject[id];
        }
    }
}
