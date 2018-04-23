using Uno;
using Uno.Threading;
using Uno.Testing;

namespace ThreadingTests
{
    public class PromiseTests_2_1_3
    {
        bool _onRejectedCalled = false;

        [Test]
        public void AlreadyRejected()
        {
            var promise = new Promise<int>();
            promise.Reject(new Exception("Rejected"));
            promise.Then(OnFulfilled, OnRejected);
        }

        [Test]
        public void ImmediatelyRejected()
        {
            var promise = new Promise<int>();
            promise.Then(OnFulfilled, OnRejected);
            promise.Reject(new Exception("Rejected"));
        }

        [Test]
        public void EventuallyRejected()
        {
            var promise = new Promise<int>();
            promise.Then(OnFulfilled, OnRejected);
            promise.Reject(new Exception("Rejected"));
        }

        [Test]
        public void RejectAndImmediatelyFulfill()
        {
            var promise = new Promise<int>();
            promise.Then(OnFulfilled, OnRejected);
            promise.Reject(new Exception("Rejected"));
            Assert.Throws<Exception>(() => promise.Resolve(0));
        }

        void OnFulfilled(int res)
        {
            Assert.AreEqual(false, _onRejectedCalled);
        }

        void OnRejected(Exception ex)
        {
            _onRejectedCalled = true;
        }
    }
}
