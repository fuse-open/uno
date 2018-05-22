using Uno.Collections;

namespace Uno.Runtime.Implementation.Internal
{
    public sealed class StringEnumerable : IEnumerable<char>
    {
        readonly string _source;

        public StringEnumerable(string source)
        {
            _source = source;
        }

        public IEnumerator<char> GetEnumerator()
        {
            return new StringEnumerator(_source);
        }
    }
}
