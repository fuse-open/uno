using Uno;
using Uno.Collections;
using Uno.Threading;
using Uno.Testing;

namespace ThreadingTests
{
    public class SemaphoreTests
    {
        [Test]
        public void Construct()
        {
            var sem = new Semaphore(0, 1);
            var sem2 = new Semaphore(10, 10);

            Assert.Throws<ArgumentOutOfRangeException>(() => new Semaphore(-1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Semaphore(0, 0));
            Assert.Throws<ArgumentException>(() => new Semaphore(11, 10));
        }

        Semaphore _sem;
        ConcurrentQueue<string> _events;

        void SemaphoreEvent(string evt)
        {
            _events.Enqueue(Thread.CurrentThread.Name + ": " + evt);
        }

        void SemaphoreTester()
        {
            SemaphoreEvent("waiting");
            if (!_sem.WaitOne(100000))
                SemaphoreEvent("failed to get semaphore");
            else
            {
                try
                {
                    SemaphoreEvent("got semaphore");
                    Thread.Sleep(1000); // wait a bit to enforce ordering
                    SemaphoreEvent("releasing semaphore");
                }
                finally
                {
                    _sem.Release();
                }
            }
        }

        public static T[] ToArray<T>(ConcurrentQueue<T> queue)
        {
            var list = new List<T>();

            T tmp;
            while (queue.TryDequeue(out tmp))
                list.Add(tmp);

            return list.ToArray();
        }

        [Test]
        public void SemaphoreOrdering()
        {
            _sem = new Semaphore(2, 2);
            _events = new ConcurrentQueue<string>();

            var threads = new List<Thread>();
            for (int i = 0; i < 3; ++i)
            {
                var thread = new Thread(SemaphoreTester);
                thread.Name = "thread" + i;
                thread.Start();
                Thread.Sleep(100); // wait a bit to enforce ordering
                threads.Add(thread);
            }

            foreach (var thread in threads)
                thread.Join();

            Assert.AreEqual(new string[] {
                "thread0: waiting",
                "thread0: got semaphore",
                "thread1: waiting",
                "thread1: got semaphore",
                "thread2: waiting",
                "thread0: releasing semaphore",
                "thread2: got semaphore",
                "thread1: releasing semaphore",
                "thread2: releasing semaphore"
            }, ToArray(_events));
        }

        [Test]
        public void WaitOneTimeout()
        {
            var sem = new Semaphore(0, 1);

            var start = Uno.Diagnostics.Clock.GetSeconds();
            var ret = sem.WaitOne(1000);
            var stop = Uno.Diagnostics.Clock.GetSeconds();

            Assert.AreEqual(false, ret);
            Assert.AreEqual(1000.0, (stop - start) * 1000, 100.0); // valid range is 900.0 through 1100.0
        }

        [Test]
        public void Release()
        {
            var sem = new Semaphore(0, 2);
            Assert.AreEqual(0, sem.Release());
            Assert.AreEqual(1, sem.Release());

            var sem2 = new Semaphore(0, 2);
            Assert.Throws<Exception>(() => sem2.Release(3));
        }
    }
}
