using Uno;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;

namespace Uno.Threading
{
    [DotNetType("System.Threading.Semaphore")]
    public sealed class Semaphore : IDisposable
    {
        extern(WIN32) readonly Win32Helpers.SemaphoreHandle _semaphoreHandle;
        extern(UNIX) readonly PthreadHelpers.SemaphoreHandle _semaphoreHandle;

        public Semaphore(int initialCount, int maxCount)
        {
            if (initialCount < 0)
                throw new ArgumentOutOfRangeException(nameof(initialCount));

            if (maxCount < 1)
                throw new ArgumentOutOfRangeException(nameof(maxCount));

            if (initialCount > maxCount)
                throw new ArgumentException(nameof(initialCount));

            if defined(WIN32)
                _semaphoreHandle = Win32Helpers.CreateSemaphore(initialCount, maxCount);
            else if defined(UNIX)
                _semaphoreHandle = PthreadHelpers.CreateSemaphore(initialCount, maxCount);
            else
                build_error;
        }

        public bool WaitOne()
        {
            return WaitOne(-1);
        }

        public bool WaitOne(int timeoutMillis)
        {
            if defined(WIN32)
                return Win32Helpers.WaitOneSemaphore(_semaphoreHandle, timeoutMillis);
            else if defined(UNIX)
                return PthreadHelpers.WaitOneSemaphore(_semaphoreHandle, timeoutMillis);
            else
                build_error;
        }

        public int Release()
        {
            return Release(1);
        }

        public int Release(int releaseCount)
        {
            if defined(WIN32)
                return Win32Helpers.ReleaseSemaphore(_semaphoreHandle, releaseCount);
            else if defined(UNIX)
                return PthreadHelpers.ReleaseSemaphore(_semaphoreHandle, releaseCount);
            else
                build_error;
        }

        public void Dispose()
        {
            if defined(WIN32)
                Win32Helpers.DisposeSemaphore(_semaphoreHandle);
            else if defined(UNIX)
                PthreadHelpers.DisposeSemaphore(_semaphoreHandle);
            else
                build_error;
        }
    }
}
