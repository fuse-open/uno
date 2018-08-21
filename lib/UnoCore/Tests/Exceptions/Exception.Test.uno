using Uno;
using Uno.Diagnostics;
using Uno.Testing;
using Uno.Collections;

namespace Exceptions.Test
{
    class MockException : Exception
    {
        public MockException(string message) : base(message)
        {
        }

        public MockException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class ExceptionTest
    {
        void ThrowMockExceptionWithInnerException()
        {
            Exception e;
            try
            {
                throw new MockException("inner-exception");
            }
            catch (Exception innerException)
            {
                e = new MockException("exception", innerException);
            }
            throw e;
        }

        [Test]
        public void InnerException()
        {
            var e = Assert.Throws<MockException>(ThrowMockExceptionWithInnerException);
            Assert.AreNotEqual(e.InnerException, null);
        }

        [Test]
        new public void ToString()
        {
            var e = Assert.Throws<MockException>(ThrowMockExceptionWithInnerException);
            var s = e.ToString();
            Assert.Contains("Exceptions.Test.MockException: exception", s);
            Assert.Contains("Exceptions.Test.MockException: inner-exception", s);
            Assert.Contains("--- End of inner exception stack trace ---", s);
        }

        static void ThrowingFunction()
        {
            throw new Exception("Error!");
        }

        [Test]
        public extern(CPLUSPLUS) void NativeStackTrace()
        {
            try
            {
                ThrowingFunction();
                Assert.Fail("Exception expected!");
            }
            catch (Exception e)
            {
                var nativeStackTrace = e.NativeStackTrace;
                Assert.IsTrue(nativeStackTrace != null);
                Assert.IsTrue(nativeStackTrace.Length > 1);
            }
        }
    }
}
