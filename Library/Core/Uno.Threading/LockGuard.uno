using Uno;
using Uno.Collections;

namespace Uno.Threading
{
    public class LockGuard : IDisposable
    {
        readonly Mutex _mutex;

        LockGuard(Mutex mutex)
        {
            _mutex = mutex;
            _mutex.WaitOne();
        }

        public static IDisposable Acquire(Mutex mutex)
        {
            return new LockGuard(mutex);
        }

        void IDisposable.Dispose()
        {
            _mutex.ReleaseMutex();
        }
    }
}
