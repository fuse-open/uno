using Uno;
using Uno.Threading;
using Uno.Testing;

namespace ThreadingTests
{
    public class PromiseTests_2_3_2
    {
        bool _wasFulfilled = false;
        bool _wasRejected = false;
        int _filfillValue = default(int);
        DummyException _rejectReason = null;

        [Test]
        public void FromFulfilledPromise()
        {
            var promise = new Promise<int>();
            promise.Resolve(default(int));
            promise.Then(OnBaseFulfilled).Then(OnFulfilled, OnRejected);

            Assert.IsFalse(_wasFulfilled);
            Assert.IsFalse(_wasRejected);
        }

        [Test]
        public void FromRejectedPromise()
        {
            var promise = new Promise<int>();
            promise.Reject(new DummyException("Rejected"));
            promise.Then(OnBaseFulfilled).Then(OnFulfilled, OnRejected);

            Assert.IsFalse(_wasFulfilled);
            Assert.IsFalse(_wasRejected);
        }

        [Test]
        public void FulfillWithTheSameValue()
        {
            var promise = new Promise<int>();
            promise.Resolve(default(int));
            promise = promise.Then(OnBaseFulfilled) as Promise<int>;
            _filfillValue = 5;
            promise.Resolve(_filfillValue);
            promise.Then(OnFulfilledWithTheSameValue);
        }

        [Test]
        public void EventuallyFulfillWithTheSameValue()
        {
            var promise = new Promise<int>();
            promise.Resolve(default(int));
            promise = promise.Then(OnBaseFulfilled) as Promise<int>;
            promise.Then(OnFulfilledWithTheSameValue);
            _filfillValue = 5;
            promise.Resolve(_filfillValue);
        }

        [Test]
        public void RejectWithTheSameReason()
        {
            var promise = new Promise<int>();
            promise.Resolve(default(int));
            promise = promise.Then(OnBaseFulfilled) as Promise<int>;
            _rejectReason = new DummyException("Rejected");
            promise.Reject(_rejectReason);
            promise.Then(null, OnRejectedWithTheSameValue);
        }

        [Test]
        public void EventuallyRejectWithTheSameReason()
        {
            var promise = new Promise<int>();
            promise.Resolve(default(int));
            promise = promise.Then(OnBaseFulfilled) as Promise<int>;
            promise.Then(null, OnRejectedWithTheSameValue);
            _rejectReason = new DummyException("Rejected");
            promise.Reject(_rejectReason);
        }

        void OnBaseFulfilled(int res)
        {
        }

        void OnFulfilled(int res)
        {
            _wasFulfilled = true;
        }

        void OnRejected(Exception ex)
        {
            _wasRejected = true;
        }

        void OnFulfilledWithTheSameValue(int res)
        {
            Assert.AreEqual(_filfillValue, res);
        }

        void OnRejectedWithTheSameValue(Exception ex)
        {
            Assert.AreEqual(_rejectReason, ex);
        }
    }
}
