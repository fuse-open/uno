using Uno;
using Uno.Collections;

namespace Uno.Collections
{
    public class ObservableList<T>: IList<T>
    {
        List<T> _items = null;

        Action<T> _added, _removed;

        public ObservableList(Action<T> added, Action<T> removed)
        {
            if (added == null)
                throw new ArgumentNullException(nameof(added));
            if (removed == null)
                throw new ArgumentNullException(nameof(removed));

            _added = added;
            _removed = removed;
        }

        public void Clear()
        {
            if (_items != null)
            {
                var removedItems = _items;
                _items = null;

                foreach (var i in removedItems)
                    _removed(i);
            }
        }

        public bool Contains(T item)
        {
            if (_items == null)
                return false;

            return _items.Contains(item);
        }

        public void Add(T item)
        {
            if (_items == null)
                _items = new List<T>();

            _items.Add(item);
            _added(item);
        }

        public void Insert(int index, T item)
        {
            if (_items == null)
                _items = new List<T>();

            _items.Insert(index, item);
            _added(item);
        }

        public void ReplaceAt(int index, T item)
        {
            if (_items == null)
                throw new IndexOutOfRangeException();

            var old = _items[index];
            _items[index] = item;
            _removed(old);
            _added(item);
        }

        public void RemoveAt(int index)
        {
            if (_items == null)
                throw new IndexOutOfRangeException();

            Remove(_items[index]);
        }

        public bool Remove(T item)
        {
            if (_items == null)
                return false;

            var res = _items.Remove(item);
            if (res)
                _removed(item);
            return res;
        }

        public int Count { get { return _items != null ? _items.Count : 0; } }

        public T this [int index]
        {
            get
            {
                if (_items == null)
                    throw new IndexOutOfRangeException();

                return _items[index];
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (_items == null)
                _items = new List<T>();

            return _items.GetEnumerator();
        }
    }
}
