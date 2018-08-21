using Uno.Compiler.ExportTargetInterop;

namespace Uno.Collections
{
    [extern(DOTNET) DotNetType("System.Collections.Generic.Stack`1")]
    public sealed class Stack<T> : IEnumerable<T>
    {
        T[] _data;
        int _used;
        int _version;

        public Stack()
        {
            _data = null;
            _used = 0;
        }

        public Stack(int capacity)
        {
            _data = new T[capacity];
            _used = 0;
        }

        public int Count
        {
            get
            {
                return _used;
            }
        }

         public struct Enumerator : IEnumerator<T>
        {
            Stack<T> _source;
            int _version;
            int _iterator;
            T _current;

            internal Enumerator(Stack<T> source)
            {
                _source = source;
                _version = source._version;
                _iterator = source._used;
            }

            public T Current
            {
                get
                {
                    return _current;
                }
            }

            public void Dispose()
            {
                // TODO
            }

            void IEnumerator.Reset()
            {
                _iterator = _source._used;
                _current = default(T);
                _version = _source._version;
            }

            public bool MoveNext()
            {
                if (_source._version != _version)
                    throw new InvalidOperationException("Collection was modified");

                _iterator--;

                if (_iterator >= 0)
                {
                    _current = _source._data[_iterator];
                    return true;
                }

                return false;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
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

        public void Push(T item)
        {
            EnsureCapacity();
            _data[_used++] = item;
            _version++;
        }

        public T Pop()
        {
            if (_used == 0)
            {
                throw new InvalidOperationException("Stack is empty");
            }
            _version++;
            T result = _data[--_used];
            _data[_used] = default(T);
            return result;
        }

        public T Peek()
        {
            if (_used == 0)
            {
                throw new InvalidOperationException("Stack is empty");
            }
            return _data[_used - 1];;
        }

        void EnsureCapacity()
        {
            if (_data == null)
            {
                _data = new T[2];
            }
            else if (_used + 1 == _data.Length)
            {
                var newData = new T[_data.Length * 2];

                for (int i = 0; i < _used; i++)
                    newData[i] = _data[i];

                _data = newData;
            }
        }
    }
}
