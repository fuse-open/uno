using Uno;
using Uno.Threading;
using Uno.Testing;

namespace ThreadingTests
{
    public class PromiseTests_2_1_2
    {
        bool _onFulfilledCalled = false;

        [Test]
        public void AlreadyResolved()
        {
            var promise = new Promise<int>();
            promise.Resolve(0);
            promise.Then(OnFulfilled, OnRejected);
        }

        [Test]
        public void ImmediatelyResolved()
        {
            var promise = new Promise<int>();
            promise.Then(OnFulfilled, OnRejected);
            promise.Resolve(0);
        }

        [Test]
        public void EventuallyResolved()
        {
            var promise = new Promise<int>();
            promise.Then(OnFulfilled, OnRejected);
            promise.Resolve(0);
        }

        [Test]
        public void DelayedFulfillAndReject()
        {
            var promise = new Promise<int>();
            promise.Then(OnFulfilled, OnRejected);
            promise.Resolve(0);
            Assert.Throws<Exception>(() => promise.Reject(new Exception("Rejected")));
        }

        void OnFulfilled(int res)
        {
            _onFulfilledCalled = true;
        }

        void OnRejected(Exception ex)
        {
            Assert.AreEqual(false, _onFulfilledCalled);
        }
    }
}
