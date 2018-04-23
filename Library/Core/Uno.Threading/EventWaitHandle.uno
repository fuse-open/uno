using Uno.Compiler.ExportTargetInterop;

namespace Uno.Threading
{
    [DotNetType("System.Threading.EventResetMode")]
    public enum EventResetMode
    {
        AutoReset = 0,
        ManualReset = 1
    }

    [DotNetType("System.Threading.EventWaitHandle")]
    public class EventWaitHandle : IDisposable
    {
        extern(WIN32) readonly Win32Helpers.ResetEventHandle _resetEventHandle;
        extern(UNIX) readonly PthreadHelpers.ResetEventHandle _resetEventHandle;

        public EventWaitHandle(bool initialState, EventResetMode mode)
        {
            if defined(UNIX)
                _resetEventHandle = PthreadHelpers.CreateResetEvent(initialState, mode == EventResetMode.AutoReset);
            else if defined(WIN32)
                _resetEventHandle = Win32Helpers.CreateResetEvent(initialState,  mode == EventResetMode.AutoReset);
            else
                build_error;
        }

        public virtual bool WaitOne()
        {
            return WaitOne(-1);
        }

        public virtual bool WaitOne(int timeoutMillis)
        {
            if defined(UNIX)
                return PthreadHelpers.WaitOneResetEvent(_resetEventHandle, timeoutMillis);
            else if defined(WIN32)
                return Win32Helpers.WaitOneResetEvent(_resetEventHandle, timeoutMillis);
            else
                build_error;
        }

        public virtual bool Reset()
        {
            if defined(UNIX)
                return PthreadHelpers.ResetResetEvent(_resetEventHandle);
            else if defined(WIN32)
                return Win32Helpers.ResetResetEvent(_resetEventHandle);
            else
                build_error;
        }

        public virtual bool Set()
        {
            if defined(UNIX)
                return PthreadHelpers.SetResetEvent(_resetEventHandle);
            else if defined(WIN32)
                return Win32Helpers.SetResetEvent(_resetEventHandle);
            else
                build_error;
        }

        public void Dispose()
        {
            if defined(UNIX)
                PthreadHelpers.DisposeResetEvent(_resetEventHandle);
            else if defined(WIN32)
                Win32Helpers.DisposeResetEvent(_resetEventHandle);
            else
                build_error;
        }
    }
}
