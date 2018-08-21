using Uno;
using Uno.Threading;
using Uno.Testing;

namespace ThreadingTests
{
    public class PromiseTests_2_2_2
    {
        bool _onFulfilledCalled = false;
        int _fulfilledCalled = 0;
        bool _onRejectCalled = false;
        DummyObject _dummyObject = new DummyObject("test");

        [Test]
        public void AlreadyResolved()
        {
            var promise = new Promise<DummyObject>();
            promise.Resolve(_dummyObject);
            promise.Then(OnFulfilled, OnRejected);
        }

        [Test]
        public void ImmediatelyResolved()
        {
            var promise = new Promise<DummyObject>();
            promise.Then(OnFulfilled, OnRejected);
            promise.Resolve(_dummyObject);
        }

        [Test]
        public void EventuallyResolved()
        {
            var promise = new Promise<DummyObject>();
            promise.Then(OnFulfilled, OnRejected);
            promise.Resolve(_dummyObject);
        }

        [Test]
        public void NeverResolved()
        {
            var promise = new Promise<DummyObject>();
            promise.Then(OnFulfilled, OnRejected);

            Assert.IsFalse(_onFulfilledCalled);
            Assert.IsFalse(_onRejectCalled);
        }

        [Test]
        public void AlreadyResolvedMoreThanOnce()
        {
            var promise = new Promise<DummyObject>();
            promise.Resolve(_dummyObject);
            Assert.Throws<Exception>(() => promise.Resolve(_dummyObject));
            promise.Then(OnFulfilled, OnRejected);

            Assert.AreEqual(1, _fulfilledCalled);
        }

        [Test]
        public void ImmediatelyResolvedMoreThanOnce()
        {
            var promise = new Promise<DummyObject>();
            promise.Then(OnFulfilled, OnRejected);
            promise.Resolve(_dummyObject);
            Assert.Throws<Exception>(() => promise.Resolve(_dummyObject));

            Assert.AreEqual(1, _fulfilledCalled);
        }

        [Test]
        public void EventuallyResolvedWithFewThen()
        {
            var promise = new Promise<DummyObject>();
            promise.Then(OnFulfilled, OnRejected);
            promise.Then(OnFulfilled, OnRejected);
            promise.Resolve(_dummyObject);

            Assert.AreEqual(2, _fulfilledCalled);
        }

        [Test]
        public void EventuallyResolvedWithFewThenInterleaved()
        {
            var promise = new Promise<DummyObject>();
            promise.Then(OnFulfilled, OnRejected);
            promise.Resolve(_dummyObject);
            promise.Then(OnFulfilled, OnRejected);

            Assert.AreEqual(2, _fulfilledCalled);
        }

        void OnFulfilled(DummyObject obj)
        {
            _onFulfilledCalled = true;
            _fulfilledCalled++;
            Assert.AreEqual(_dummyObject, obj as DummyObject);
        }

        void OnRejected(Exception ex)
        {
            _onRejectCalled = true;
            Assert.AreEqual(false, _onFulfilledCalled);
        }
    }
}
