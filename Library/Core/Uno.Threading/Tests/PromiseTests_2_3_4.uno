using Uno;
using Uno.Threading;
using Uno.Testing;

namespace ThreadingTests
{
    public class PromiseTests_2_3_4
    {
        int _filfillValue = default(int);
        DummyException _rejectReason = null;

        [Test]
        public void FulfilledPromise()
        {
            var promise = new Promise<int>();
            promise.Then(OnFulfilled, OnRejected);
            _filfillValue = 55;
            promise.Resolve(_filfillValue);
        }

        [Test]
        public void RejectedPromise()
        {
            var promise = new Promise<int>();
            promise.Then(OnFulfilled, OnRejected);
            _rejectReason = new DummyException("Rejected");
            promise.Reject(_rejectReason);
        }

        void OnFulfilled(int res)
        {
            Assert.AreEqual(_filfillValue, res);
        }

        void OnRejected(Exception ex)
        {
            Assert.AreEqual(_rejectReason, ex);
        }
    }
}
