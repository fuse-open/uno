using Uno;
using Uno.Threading;
using Uno.Testing;

namespace ThreadingTests
{
    public class PromiseTests_2_2_3
    {
        bool _onFulfilledCalled = false;
        bool _onRejectedCalled = false;
        int _rejectedCalled = 0;
        DummyException _dummyException = new DummyException("dummy");

        [Test]
        public void AlreadyRejected()
        {
            var promise = new Promise<int>();
            promise.Reject(_dummyException);
            promise.Then(OnFulfilled, OnRejected);
        }

        [Test]
        public void ImmediatelyRejected()
        {
            var promise = new Promise<int>();
            promise.Then(OnFulfilled, OnRejected);
            promise.Reject(_dummyException);
        }

        [Test]
        public void EventuallyRejected()
        {
            var promise = new Promise<int>();
            promise.Then(OnFulfilled, OnRejected);
            promise.Reject(_dummyException);
        }

        [Test]
        public void NeverRejected()
        {
            var promise = new Promise<int>();
            promise.Then(OnFulfilled, OnRejected);

            Assert.IsFalse(_onFulfilledCalled);
            Assert.IsFalse(_onRejectedCalled);
        }

        [Test]
        public void AlreadyRejectedMoreThanOnce()
        {
            var promise = new Promise<int>();
            promise.Reject(_dummyException);
            Assert.Throws<Exception>(() => promise.Reject(_dummyException));
            promise.Then(OnFulfilled, OnRejected);

            Assert.AreEqual(1, _rejectedCalled);
        }

        [Test]
        public void ImmediatelyRejectedMoreThanOnce()
        {
            var promise = new Promise<int>();
            promise.Then(OnFulfilled, OnRejected);
            promise.Reject(_dummyException);
            Assert.Throws<Exception>(() => promise.Reject(_dummyException));

            Assert.AreEqual(1, _rejectedCalled);
        }

        [Test]
        public void EventuallyRejectedWithFewThen()
        {
            var promise = new Promise<int>();
            promise.Then(OnFulfilled, OnRejected);
            promise.Then(OnFulfilled, OnRejected);

            promise.Reject(_dummyException);

            Assert.AreEqual(2, _rejectedCalled);
        }

        [Test]
        public void EventuallyRejectedWithFewThenInterleaved()
        {
            var promise = new Promise<int>();
            promise.Then(OnFulfilled, OnRejected);

            promise.Reject(_dummyException);
            promise.Then(OnFulfilled, OnRejected);

            Assert.AreEqual(2, _rejectedCalled);
        }

        void OnFulfilled(int obj)
        {
            _onFulfilledCalled = true;
        }

        void OnRejected(Exception ex)
        {
            _rejectedCalled++;
            _onRejectedCalled = true;
            Assert.AreEqual(_dummyException, ex as DummyException);
        }
    }
}
