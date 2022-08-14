using Uno;
using Uno.Collections;
using Uno.Threading;
using Uno.Testing;

namespace ThreadingTests
{
    public class PromiseTests_2_2_6
    {
        List<int> _callIndexes = new List<int>();

        [Test]
        public void CallOrder1()
        {
            var promise = new Promise<int>();
            promise.Then(OnFulfilled1, OnRejected);
            promise.Then(OnFulfilled2, OnRejected);
            promise.Then(OnFulfilled3, OnRejected);
            promise.Resolve(0);

            Assert.AreEqual(new int[] {1, 2, 3}, _callIndexes.ToArray());
        }

        [Test]
        public void CallOrder2()
        {
            var promise = new Promise<int>();
            promise.Then(OnFulfilled3, OnRejected);
            promise.Resolve(0);
            promise.Then(OnFulfilled1, OnRejected);
            promise.Then(OnFulfilled2, OnRejected);

            Assert.AreEqual(new int[] {3, 1, 2}, _callIndexes.ToArray());
        }

        [Test]
        public void CallOrder3()
        {
            var promise = new Promise<int>();
            promise.Then(OnFulfilled2, OnRejected);
            promise.Resolve(0);
            promise.Then(OnFulfilledCreatePromise, OnRejected);

            Assert.AreEqual(new int[] {2, 3, 1}, _callIndexes.ToArray());
        }

        void OnFulfilled1(int obj)
        {
            _callIndexes.Add(1);
        }

        void OnFulfilled2(int obj)
        {
            _callIndexes.Add(2);
        }

        void OnFulfilled3(int obj)
        {
            _callIndexes.Add(3);
        }

        void OnFulfilledCreatePromise(int obj)
        {
            var promise = new Promise<int>();
            promise.Then(OnFulfilled3, OnRejected);
            promise.Resolve(0);

            _callIndexes.Add(1);
        }

        void OnRejected(Exception ex)
        {
        }
    }
}
