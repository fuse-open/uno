using Uno;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;

namespace Uno.Threading
{
    [DotNetType("System.Threading.ThreadStart")]
    public delegate void ThreadStart();

    [DotNetType("System.Threading.ThreadStateException")]
    public class ThreadStateException : Exception
    {
        public ThreadStateException(String message) : base(message)
        {
        }
    }

    [DotNetType("System.Threading.Thread")]
    public sealed class Thread
    {
        extern(UNIX) PthreadHelpers.ThreadHandle _threadHandle;
        extern(WIN32) Win32Helpers.ThreadHandle _threadHandle;

        ThreadStart _threadStart;
        public Thread(ThreadStart start)
        {
            if (start == null)
                throw new ArgumentNullException(nameof(start));

            _threadStart = start;
        }

        private Thread(bool started)
        {
            _started = started;
        }

        public bool IsBackground
        {
            get { return false; }
            set
            {
                if (value != false)
                    throw new NotImplementedException();
            }
        }

        bool _started;
        public void Start()
        {
            if (_started)
                throw new ThreadStateException("Thread is running or terminated; it cannot restart.");

            if defined(UNIX)
                _threadHandle = PthreadHelpers.CreateThread(this);
            else if defined(WIN32)
                _threadHandle = Win32Helpers.CreateThread(this);
            else
                build_error;

            _started = true;
        }

        public void Join()
        {
            if (!_started)
                throw new ThreadStateException("Thread has not been started.");

            if defined(UNIX)
                PthreadHelpers.JoinThread(_threadHandle);
            else if defined(WIN32)
                Win32Helpers.JoinThread(_threadHandle);
            else
                build_error;
        }

        public string Name { get; set; }

        public static Thread CurrentThread
        {
            get
            {
                Thread ret = null;
                if defined(UNIX)
                    ret = PthreadHelpers.GetCurrentThread();
                else if defined(WIN32)
                    ret = Win32Helpers.GetCurrentThread();
                else
                    build_error;

                if (ret == null)
                {
                    // create a new, started thread-object, and set it current
                    ret = new Thread(true);

                    // set it as the current thread
                    if defined(UNIX)
                        PthreadHelpers.SetCurrentThread(ret);
                    else if defined(WIN32)
                        Win32Helpers.SetCurrentThread(ret);
                    else
                        build_error;
                }

                return ret;
            }
        }

        public static void Sleep(int millis)
        {
            if defined(UNIX)
                PthreadHelpers.Sleep(millis);
            else if defined(WIN32)
                Win32Helpers.Sleep(millis);
            else
                build_error;
        }
    }
}
