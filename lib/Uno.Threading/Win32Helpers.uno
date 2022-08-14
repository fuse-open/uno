using Uno.Compiler.ExportTargetInterop;

namespace Uno.Threading
{
    extern(WIN32) static class Win32Helpers
    {
        [TargetSpecificType]
        [Set("TypeName", "void *")] // HANDLE in winapi is really just a void-pointer!
        public struct MutexHandle
        {
        }

        [TargetSpecificType]
        [Set("TypeName", "void *")] // HANDLE in winapi is really just a void-pointer!
        public struct SemaphoreHandle
        {
        }

        [TargetSpecificType]
        [Set("TypeName", "void *")] // HANDLE in winapi is really just a void-pointer!
        public struct ResetEventHandle
        {
        }

        [Require("Source.Include", "Uno/WinAPIHelper.h")]
        [Require("Source.Include", "@{Uno.Exception:Include}")]
        public static MutexHandle CreateMutex()
        @{
            HANDLE mutexHandle = ::CreateMutexW(nullptr, false, nullptr);

            if (mutexHandle == nullptr)
                U_THROW(@{Uno.Exception(string):New(uString::Utf8("CreateMutex failed!"))});

            return mutexHandle;
        @}

        [Require("Source.Include", "Uno/WinAPIHelper.h")]
        public static bool WaitOneMutex(MutexHandle mutexHandle, int millisecondsTimeout)
        @{
            return ::WaitForSingleObject(mutexHandle, millisecondsTimeout == -1 ? INFINITE : millisecondsTimeout) == WAIT_OBJECT_0;
        @}

        [Require("Source.Include", "Uno/WinAPIHelper.h")]
        public static void ReleaseMutex(MutexHandle mutexHandle)
        @{
            ::ReleaseMutex(mutexHandle);
        @}

        [Require("Source.Include", "Uno/WinAPIHelper.h")]
        public static void DisposeMutex(MutexHandle mutexHandle)
        @{
            ::CloseHandle(mutexHandle);
        @}

        [Require("Source.Include", "Uno/WinAPIHelper.h")]
        public static SemaphoreHandle CreateSemaphore(int initialCount, int maxCount)
        @{
            HANDLE semaphoreHandle = ::CreateSemaphoreW(nullptr, $0, $1, nullptr);

            if (semaphoreHandle == nullptr)
                U_THROW(@{Uno.Exception(string):New(uString::Utf8("CreateSemaphoreW failed!"))});

            return semaphoreHandle;
        @}

        [Require("Source.Include", "Uno/WinAPIHelper.h")]
        public static bool WaitOneSemaphore(SemaphoreHandle semaphoreHandle, int timeoutMillis)
        @{
            if (timeoutMillis == -1)
                return ::WaitForSingleObject(semaphoreHandle, INFINITE) == WAIT_OBJECT_0;
            else
                return ::WaitForSingleObject(semaphoreHandle, timeoutMillis) == WAIT_OBJECT_0;
        @}

        [Require("Source.Include", "Uno/WinAPIHelper.h")]
        public static int ReleaseSemaphore(SemaphoreHandle semaphoreHandle, int releaseCount)
        @{
            LONG res;
            if (!::ReleaseSemaphore(semaphoreHandle, releaseCount, &res))
                U_THROW(@{Uno.Exception(string):New(uString::Utf8("ReleaseSemaphore failed!"))});
            return res;
        @}

        [Require("Source.Include", "Uno/WinAPIHelper.h")]
        public static void DisposeSemaphore(SemaphoreHandle semaphoreHandle)
        @{
            ::CloseHandle(semaphoreHandle);
        @}

        [Require("Source.Include", "Uno/WinAPIHelper.h")]
        public static ResetEventHandle CreateResetEvent(bool initialState, bool autoReset)
        @{
            HANDLE resetEventHandle = ::CreateEvent(nullptr, autoReset, initialState, nullptr);

            if (resetEventHandle == nullptr)
                U_THROW(@{Uno.Exception(string):New(uString::Utf8("CreateEvent failed!"))});

            return resetEventHandle;
        @}

        [Require("Source.Include", "Uno/WinAPIHelper.h")]
        public static bool WaitOneResetEvent(ResetEventHandle resetEventHandle, int timeoutMillis)
        @{
            return ::WaitForSingleObject(resetEventHandle, timeoutMillis == -1 ? INFINITE : timeoutMillis) == WAIT_OBJECT_0;
        @}

        [Require("Source.Include", "Uno/WinAPIHelper.h")]
        public static bool ResetResetEvent(ResetEventHandle resetEventHandle)
        @{
            return ::ResetEvent(resetEventHandle) != FALSE;
        @}

        [Require("Source.Include", "Uno/WinAPIHelper.h")]
        public static bool SetResetEvent(ResetEventHandle resetEventHandle)
        @{
            return ::SetEvent(resetEventHandle) != FALSE;
        @}

        [Require("Source.Include", "Uno/WinAPIHelper.h")]
        public static void DisposeResetEvent(ResetEventHandle resetEventHandle)
        @{
            ::CloseHandle(resetEventHandle);
        @}
    }
}
