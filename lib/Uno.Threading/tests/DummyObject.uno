using Uno;
using Uno.Threading;
using Uno.Testing;

namespace ThreadingTests
{
    public class DummyObject
    {
        public DummyObject(string property)
        {
            _property = property;
        }

        private string _property;
        public string Property
        {
            get { return _property; }
        }
    }
}
