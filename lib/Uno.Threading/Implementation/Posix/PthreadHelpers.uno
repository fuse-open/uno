using Uno.Compiler.ExportTargetInterop;

namespace Uno.Threading
{
    extern(UNIX) static class PthreadHelpers
    {
        [TargetSpecificType]
        [Set("Include", "pthread.h")]
        [Set("TypeName", "pthread_mutex_t")]
        public struct MutexHandle
        {
        }

        [Require("Source.Include", "Implementation/Posix/posix_mutex.h")]
        public static void CreateMutex(ref MutexHandle mutexHandle)
        @{
            if (!uPthreadCreateMutex(mutexHandle) != 0)
                U_THROW(@{Uno.Exception(string):New(uString::Utf8("uPthreadCreateMutex failed!"))});
        @}

        [Require("Source.Include", "Implementation/Posix/posix_mutex.h")]
        public static bool WaitOneMutex(ref MutexHandle mutexHandle, int millisecondsTimeout)
        @{
            return uPthreadWaitOneMutex(mutexHandle, millisecondsTimeout);
        @}

        public static void ReleaseMutex(ref MutexHandle mutexHandle)
        @{
            pthread_mutex_unlock(mutexHandle);
        @}

        public static void DisposeMutex(ref MutexHandle mutexHandle)
        @{
            pthread_mutex_destroy(mutexHandle);
        @}

        [TargetSpecificType]
        [Set("Include", "Implementation/Posix/posix_semaphore.h")]
        [Set("TypeName", "uPosixSemaphore *")]
        public struct SemaphoreHandle
        {
        }

        [Require("Source.Include", "Implementation/Posix/posix_semaphore.h")]
        public static SemaphoreHandle CreateSemaphore(int initialCount, int maxCount)
        @{
            uPosixSemaphore* semaphoreHandle = uPosixCreateSemaphore(initialCount, maxCount);

            if (semaphoreHandle == nullptr)
                U_THROW(@{Uno.Exception(string):New(uString::Utf8("uPosixCreateSemaphore() failed!"))});

            return semaphoreHandle;
        @}

        [Require("Source.Include", "Implementation/Posix/posix_semaphore.h")]
        public static bool WaitOneSemaphore(SemaphoreHandle semaphoreHandle, int timeoutMillis)
        @{
            return uPosixWaitOneSemaphore(semaphoreHandle, timeoutMillis);
        @}

        [Require("Source.Include", "Implementation/Posix/posix_semaphore.h")]
        public static int ReleaseSemaphore(SemaphoreHandle semaphoreHandle, int releaseCount)
        @{
            int ret = uPosixReleaseSemaphore(semaphoreHandle, releaseCount);
            if (ret < 0)
                U_THROW(@{Uno.Exception(string):New(uString::Utf8("uPosixReleaseSemaphore() failed!"))});
            return ret;
        @}

        public static void DisposeSemaphore(SemaphoreHandle semaphoreHandle)
        @{
            uPosixDisposeSemaphore(semaphoreHandle);
        @}

        [TargetSpecificType]
        [Set("Include", "Implementation/Posix/posix_reset_event.h")]
        [Set("TypeName", "uPosixResetEvent *")]
        public struct ResetEventHandle
        {
        }

        public static ResetEventHandle CreateResetEvent(bool initialState, bool autoReset)
        @{
            uPosixResetEvent *resetEventHandle = uPosixCreateResetEvent(initialState, autoReset);

            if (resetEventHandle == nullptr)
                U_THROW(@{Uno.Exception(string):New(uString::Utf8("uPosixCreateResetEvent() failed!"))});

            return resetEventHandle;
        @}

        public static bool WaitOneResetEvent(ResetEventHandle resetEventHandle, int timeoutMillis)
        @{
            return uPosixWaitOneResetEvent(resetEventHandle, timeoutMillis);
        @}

        public static bool ResetResetEvent(ResetEventHandle resetEventHandle)
        @{
            return uPosixResetResetEvent(resetEventHandle);
        @}

        public static bool SetResetEvent(ResetEventHandle resetEventHandle)
        @{
            return uPosixSetResetEvent(resetEventHandle);
        @}

        public static void DisposeResetEvent(ResetEventHandle resetEventHandle)
        @{
            uPosixDisposeResetEvent(resetEventHandle);
        @}
    }
}
