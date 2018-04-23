using Uno;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;

namespace Uno.Threading
{
    [DotNetType("System.Threading.Mutex")]
    public sealed class Mutex : IDisposable
    {
        extern(WIN32) readonly Win32Helpers.MutexHandle _mutexHandle;
        extern(UNIX) PthreadHelpers.MutexHandle _mutexHandle;

        public Mutex()
        {
            if defined(WIN32)
                _mutexHandle = Win32Helpers.CreateMutex();
            else if defined(UNIX)
                PthreadHelpers.CreateMutex(ref _mutexHandle);
            else
                build_error;
        }

        public bool WaitOne()
        {
            return WaitOne(-1);
        }

        public bool WaitOne(int millisecondsTimeout)
        {
            if defined(WIN32)
                return Win32Helpers.WaitOneMutex(_mutexHandle, millisecondsTimeout);
            else if defined(UNIX)
                return PthreadHelpers.WaitOneMutex(ref _mutexHandle, millisecondsTimeout);
            else
                build_error;
        }

        public void ReleaseMutex()
        {
            if defined(WIN32)
                Win32Helpers.ReleaseMutex(_mutexHandle);
            else if defined(UNIX)
                PthreadHelpers.ReleaseMutex(ref _mutexHandle);
            else
                build_error;
        }

        public void Dispose()
        {
            if defined(WIN32)
                Win32Helpers.DisposeMutex(_mutexHandle);
            else if defined(UNIX)
                PthreadHelpers.DisposeMutex(ref _mutexHandle);
            else
                build_error;
        }
    }
}
