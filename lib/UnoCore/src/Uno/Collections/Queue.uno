using Uno.Compiler.ExportTargetInterop;

namespace Uno.Collections
{
    [extern(DOTNET) DotNetType("System.Collections.Generic.Queue`1")]
    public sealed class Queue<T> : IEnumerable<T>
    {
        public struct Enumerator : IEnumerator<T>
        {
            Queue<T> _source;
            int _version;
            int _index;
            T _current;

            internal Enumerator(Queue<T> source)
            {
                _source = source;
                _current = default(T);
                _version = source._version;
                _index = -1;
            }

            public T Current
            {
                get
                {
                    if (_index < 0)
                    {
                        throw new InvalidOperationException("Enumerator is invalid");
                    }
                    return _current;
                }
            }

            public void Dispose()
            {
            }

            void IEnumerator.Reset()
            {
                _index = -1;
                _current = default(T);
                _version = _source._version;
            }

            public bool MoveNext()
            {
                if (_source._version != _version)
                    throw new InvalidOperationException("Collection was modified");

                _index++;

                if (_index < _source._size)
                {
                    _current = _source.ElementAt(_index);
                    return true;
                }

                return false;
            }
        }

        T[] _data;
        int _head;
        int _tail;
        int _size;
        int _version;

        public Queue()
        {
            _data = null;
            _head = 0;
            _tail = 0;
            _size = 0;
        }

        public Queue(int capacity)
        {
            _data = new T[capacity];
            _head = 0;
            _tail = 0;
            _size = 0;
        }

        public int Count
        {
            get { return _size; }
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public void Clear()
        {
            _data = null;
            _head = 0;
            _tail = 0;
            _size = 0;
            _version++;
        }

        public bool Contains(T item)
        {
            for (int i = _head; i < _tail; i++)
                if (Generic.Equals(_data[i], item))
                    return true;

            return false;
        }

        public void Enqueue(T item)
        {
            EnsureCapacity();
            _data[_tail] = item;
            _tail++;
            _size++;
            _version++;
        }

        public T Dequeue()
        {
            if (_size == 0)
            {
                throw new InvalidOperationException("Queue is empty");
            }
            T result = _data[_head];
            _data[_head] = default(T);
            _head++;
            _size--;
            _version++;
            return result;
        }

        public T Peek()
        {
            if (_size == 0)
            {
                throw new InvalidOperationException("Queue is empty");
            }
            return _data[_head];
        }

        void EnsureCapacity()
        {
            if (_data == null)
            {
                _data = new T[2];
            }
            if (_tail == _data.Length)
            {
                var newSize = (_size != 0) ? _size * 2 : _data.Length;

                T[] newData = new T[newSize];

                for (int i = 0; i < _size; i++)
                    newData[i] = ElementAt(i);

                _data = newData;
                _head = 0;
                _tail = _size;
            }
        }

        T ElementAt(int index)
        {
            return _data[_head + index];
        }

    }
}
