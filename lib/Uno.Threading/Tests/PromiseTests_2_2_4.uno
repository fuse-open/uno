using Uno;
using Uno.Threading;
using Uno.Testing;

namespace ThreadingTests
{
    public class PromiseTests_2_2_4
    {
        bool _thenHasReturned = false;

        [Test]
        public void AlreadyFulfilled()
        {
            var promise = new Promise<int>();
            Helpers.AlreadyFulfilled(promise, OnFulfilled, OnRejected);
            _thenHasReturned = true;
        }

        [Test]
        public void ImmediatelyFulfilled()
        {
            var promise = new Promise<int>();
            Helpers.ImmediatelyFulfilled(promise, OnFulfilled, OnRejected);
            _thenHasReturned = true;
        }

        [Test]
        public void EventuallyRejected()
        {
            var promise = new Promise<int>();
            Helpers.EventuallyRejected(promise, OnFulfilled, OnRejected);
            _thenHasReturned = true;
        }

        void OnFulfilled(int obj)
        {
            Assert.AreEqual(true, _thenHasReturned);
        }

        void OnRejected(Exception ex)
        {
        }
    }
}
