using Uno;
using Uno.Threading;
using Uno.Testing;

namespace ThreadingTests
{
    public class Helpers
    {
        public static void AlreadyFulfilled<T>(Promise<T> promise, Action<T> onFulfilled, Action<Exception> onRejected)
        {
            promise.Resolve(default(T));
            promise.Then(onFulfilled, onRejected);
        }

        public static void ImmediatelyFulfilled<T>(Promise<T> promise, Action<T> onFulfilled, Action<Exception> onRejected)
        {
            promise.Then(onFulfilled, onRejected);
            promise.Resolve(default(T));
        }

        public static void EventuallyRejected<T>(Promise<T> promise, Action<T> onFulfilled, Action<Exception> onRejected)
        {
            promise.Then(onFulfilled, onRejected);
            Thread.Sleep(50);
            promise.Resolve(default(T));
        }
    }
}
