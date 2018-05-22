using Uno;
using Uno.Diagnostics;
using Uno.Testing;
using Uno.Collections;

namespace Exceptions.Test
{
    class Foo {}
    class Bar {}

    public class InvalidCastExceptionTest
    {
        void ThrowInvalidCastException()
        {
            var foo = (object)new Foo();
            var bar = (Bar)foo;
        }

        [Test]
        new public void ToString()
        {
            var e = Assert.Throws<InvalidCastException>(ThrowInvalidCastException);

            if defined (!CIL || !HOST_OSX)
                Assert.IsTrue(e.ToString().Contains("Unable to cast object of type 'Exceptions.Test.Foo' to type 'Exceptions.Test.Bar'"));
        }
    }
}
