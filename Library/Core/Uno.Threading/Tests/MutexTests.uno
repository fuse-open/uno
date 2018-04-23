using Uno.Threading;
using Uno.Testing;

namespace ThreadingTests
{
    public class MutexTests
    {
        [Test]
        public void TryLockLockedRecursive()
        {
            var m = new Mutex();
            m.WaitOne();
            Assert.IsTrue(m.WaitOne(0)); // Locks are recursive
            m.ReleaseMutex();
        }

        [Test]
        public void TryLockUnlocked()
        {
            var m = new Mutex();
            Assert.IsTrue(m.WaitOne(0));
            m.ReleaseMutex();
        }

        class Locker
        {
            Mutex _m;
            Mutex _done;
            MutexTests _parent;

            public Locker(MutexTests parent, Mutex m, Mutex done)
            {
                _parent = parent;
                _m = m;
                _done = done;
            }

            public void DoIt()
            {
                _m.WaitOne();
                _parent._started = true;

                _done.WaitOne();
            }
        }

        bool _started = false;

        [Test]
        public void TryLockLocked()
        {
            var m = new Mutex();
            var done = new Mutex();
            done.WaitOne();
            new Thread(new Locker(this, m, done).DoIt).Start();
            while (!_started) { Thread.Sleep(1); }

            Assert.IsFalse(m.WaitOne(0));
            done.ReleaseMutex();
        }

        Mutex _waitOneTimeoutMutex;
        double _waitOneTimeout100Elapsed;
        double _waitOneTimeout1000Elapsed;

        double WaitOneTimeout(int timeout)
        {
            var start = Uno.Diagnostics.Clock.GetSeconds();
            _waitOneTimeoutMutex.WaitOne(timeout);
            var stop = Uno.Diagnostics.Clock.GetSeconds();
            return stop - start;
        }

        void WaitOneTimeoutProc()
        {
            _waitOneTimeout100Elapsed = WaitOneTimeout(100);
            _waitOneTimeout1000Elapsed = WaitOneTimeout(1000);
        }

        [Test]
        [Ignore("https://github.com/fusetools/uno/issues/1642", "CIL && HOST_OSX")]
        public void WaitOneTimeout()
        {
            _waitOneTimeoutMutex = new Mutex();
            _waitOneTimeoutMutex.WaitOne();

            var thread = new Thread(WaitOneTimeoutProc);
            thread.Start();
            thread.Join();

            Assert.AreEqual(100.0, _waitOneTimeout100Elapsed * 1000, 50.0); // valid range is 50.0 through 150.0
            Assert.AreEqual(1000.0, _waitOneTimeout1000Elapsed * 1000, 100.0); // valid range is 900.0 through 1100.0
        }
    }
}
