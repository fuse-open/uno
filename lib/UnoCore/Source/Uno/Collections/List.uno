using Uno.Compiler.ExportTargetInterop;
using Uno.Math;

namespace Uno.Collections
{
    [extern(DOTNET) DotNetType("System.Collections.Generic.List`1")]
    public sealed class List<T> : IList<T>
    {
        T[] _data;
        int _used;
        int _version;

        public struct Enumerator : IEnumerator<T>
        {
            List<T> _source;
            int _version;
            int _iterator;
            T _current;

            internal Enumerator(List<T> source)
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
                _current = default(T);
                _version = _source._version;
            }

            public bool MoveNext()
            {
                if (_source._version != _version)
                    throw new InvalidOperationException("Collection was modified");

                _iterator++;

                if (_iterator < _source._used)
                {
                    _current = _source._data[_iterator];
                    return true;
                }

                return false;
            }
        }

        public List()
        {
            _data = null;
            _used = 0;
        }

        public List(int capacity)
        {
            _data = new T[capacity];
            _used = 0;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public T[] ToArray()
        {
            var t = new T[_used];

            for (int i = 0; i < _used; i++)
                t[i] = _data[i];

            return t;
        }

        void BoundsCheck(int index, string name)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(name);
        }

        void EnsureCapacity()
        {
            if (_data == null)
                _data = new T[2];
            else if (_used + 1 >= _data.Length)
            {
                var newData = new T[Max(2, _data.Length * 2)];

                if defined(CPLUSPLUS)
                @{
                    uType* type = @{T:TypeOf};
                    size_t size = type->ValueSize;
                    uint8_t* src = (uint8_t*) @{$$._data}->Ptr();
                    uint8_t* dst = (uint8_t*) newData->Ptr();
                    memcpy(dst, src, size * @{$$._used});
                    memset(src, 0, size * @{$$._used});
                @}
                else
                {
                    for (int i = 0; i < _used; i++)
                        newData[i] = _data[i];
                }

                _data = newData;
            }
        }

        public int Count
        {
            get { return _used; }
        }

        public void Add(T item)
        {
            EnsureCapacity();
            _data[_used++] = item;
            _version++;
        }

        public void AddRange(IEnumerable<T> items)
        {
            foreach (var i in items) Add(i);
        }

        public void Insert(int index, T item)
        {
            if (index < 0 || index > _used)
                throw new ArgumentOutOfRangeException(nameof(index));

            EnsureCapacity();

            if defined(CPLUSPLUS)
            @{
                uType* type = @{T:TypeOf};
                size_t size = type->ValueSize;
                uint8_t* src = (uint8_t*) @{$$._data}->Ptr() + size * $0;
                memmove(src + size, src, size * (@{$$._used} - $0));
                memset(src, 0, size);
            @}
            else
            {
                for (int i = _used; i >= index; i--)
                    _data[i + 1] = _data[i];
            }

            _data[index] = item;
            _version++;
            _used++;
        }

        public int IndexOf(T item)
        {
            for (int i = 0; i < _used; i++)
                if (Generic.Equals(_data[i], item))
                    return i;

            return -1;
        }

        public bool Remove(T item)
        {
            for (int i = 0; i < _used; i++)
            {
                if (Generic.Equals(_data[i], item))
                {
                    RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public void RemoveAt(int index)
        {
            BoundsCheck(index, nameof(index));
            _version++;

            if defined(CPLUSPLUS)
            @{
                uType* type = @{T:TypeOf};
                size_t size = type->ValueSize;
                uint8_t* dst = (uint8_t*) @{$$._data}->Ptr() + size * $0;

                if (type->Flags & uTypeFlagsRetainStruct)
                    uAutoReleaseStruct(type, dst);
                else if (U_IS_OBJECT(type))
                    uAutoRelease(*(uObject**) dst);

                memmove(dst, dst + size, size * (@{$$._used} - $0 - 1));
                @{$$._used}--;
                memset((uint8_t*) @{$$._data}->Ptr() + size * @{$$._used}, 0, size);
            @}
            else
            {
                for (int i = index; i < _used - 1; i++)
                    _data[i] = _data[i + 1];

                _used--;
                _data[_used] = default(T);
            }
        }

        public void Clear()
        {
            _data = null;
            _used = 0;
            _version++;
        }

        public bool Contains(T item)
        {
            for (int i = 0; i < _used; i++)
                if (Generic.Equals(_data[i], item))
                    return true;

            return false;
        }

        public T this[int index]
        {
            get
            {
                BoundsCheck(index, nameof(index));
                return _data[index];
            }
            set
            {
                BoundsCheck(index, nameof(index));
                _data[index] = value;
                _version++;
            }
        }

        public void Sort(Comparison<T> comparer)
        {
            Uno.Array.Sort(_data, 0, _used, comparer);
            _version++;
        }
    }
}
