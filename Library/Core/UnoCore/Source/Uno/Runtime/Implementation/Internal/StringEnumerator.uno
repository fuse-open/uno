using Uno.Collections;

namespace Uno.Runtime.Implementation.Internal
{
    public sealed class StringEnumerator : IEnumerator<char>
    {
        readonly string _source;
        int _iterator;
        char _current;

        public StringEnumerator(string source)
        {
            _source = source;
            _iterator = -1;
        }

        public char Current
        {
            get { return _current; }
        }

        public void Dispose()
        {
            // TODO
        }

        public void Reset()
        {
            _iterator = -1;
            _current = '\0';
        }

        public bool MoveNext()
        {
            _iterator++;

            if (_iterator < _source.Length)
            {
                _current = _source[_iterator];
                return true;
            }

            return false;
        }
    }
}
