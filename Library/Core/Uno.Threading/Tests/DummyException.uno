using Uno;
using Uno.Threading;
using Uno.Testing;

namespace ThreadingTests
{
    public class DummyException: Exception
    {
        public DummyException(string message)
            : base(message)
        {
        }
    }
}
