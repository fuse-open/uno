using Uno;
using Uno.Threading;
using Uno.Testing;

namespace ThreadingTests
{
    public class AutoResetEventTest
    {
        class TestHelper : IDisposable
        {
            public bool Result { get; private set; }

            Thread _thread;
            Func<bool> _func;
            int _timeout;

            public TestHelper(Func<bool> func, int timeout = 0)
            {
                _func = func;
                _timeout = timeout;
                _thread = new Thread(Entrypoint);
                _thread.Start();
            }

            void Entrypoint()
            {
                Thread.Sleep(_timeout);
                Result = _func();
            }

            public void Dispose()
            {
                _thread.Join();
            }
        }

        [Test]
        public void WaitOne1()
        {
            using (var x = new AutoResetEvent(false))
            using (var t = new TestHelper(x.Set))
            Assert.IsTrue(x.WaitOne());
        }

        [Test]
        public void WaitOne2()
        {
            using (var x = new AutoResetEvent(false))
            {
                var t = new TestHelper(x.WaitOne, 100);
                Assert.IsTrue(x.Set());
                t.Dispose();
                Assert.IsTrue(t.Result);
            }
        }

        [Test]
        public void WaitOneTimeout1()
        {
            using (var x = new AutoResetEvent(false))
            using (var t = new TestHelper(x.Set))
            Assert.IsTrue(x.WaitOne(1000));
        }

        [Test]
        public void WaitOneTimeout2()
        {
            using (var x = new AutoResetEvent(false))
            Assert.IsFalse(x.WaitOne(1000));
        }

        [Test]
        public void WaitOneTimeout3()
        {
            using (var x = new AutoResetEvent(false))
            Assert.IsFalse(x.WaitOne(5000));
        }

        [Test]
        public void WaitOneTimeout4()
        {
            using (var x = new AutoResetEvent(false))
            Assert.IsFalse(x.WaitOne(0));
        }

        [Test]
        public void AutoReset1()
        {
            using (var x = new AutoResetEvent(true))
            {
                Assert.IsTrue(x.WaitOne());
                using (var t = new TestHelper(x.Set))
                {
                    Assert.IsTrue(x.WaitOne());
                }
            }
        }

        [Test]
        public void AutoReset2()
        {
            using (var x = new AutoResetEvent(true))
            {
                var t = new TestHelper(x.WaitOne);
                t.Dispose();
                Assert.IsTrue(t.Result);

                x.Set();

                t = new TestHelper(x.WaitOne);
                t.Dispose();
                Assert.IsTrue(t.Result);
            }
        }
    }
}
