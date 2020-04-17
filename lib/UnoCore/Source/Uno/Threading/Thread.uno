using Uno;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;
using System.Globalization;

namespace Uno.Threading
{
    [TargetSpecificType]
    [extern(UNIX) Set("Include", "pthread.h")]
    [extern(UNIX) Set("TypeName", "pthread_t")]
    [extern(WIN32) Set("TypeName", "void*")] // "HANDLE" in WinAPI
    extern(CPLUSPLUS) struct ThreadHandle
    {
    }

    [TargetSpecificType]
    [Set("Include", "Uno/Support.h")]
    [Set("TypeName", "uThreadLocal*")]
    extern(CPLUSPLUS) struct ThreadLocal
    {
    }

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
    [extern(UNIX) Require("Source.Declaration", "static void* _ThreadFunc(void* arg) { @{ThreadMain(Thread):Call((@{Thread}) arg)}; return nullptr; }")]
    [extern(WIN32) Require("Source.Declaration", "static DWORD WINAPI _ThreadFunc(LPVOID lpParam) { @{ThreadMain(Thread):Call((@{Thread}) lpParam)}; return 0; }")]
    [extern(WIN32) Require("Source.Include", "Uno/WinAPIHelper.h")]
    public sealed class Thread
    {
        extern(CPLUSPLUS) static ThreadLocal _currentThread = extern<ThreadLocal> "uCreateThreadLocal(nullptr)";

        extern(CPLUSPLUS) static void ThreadMain(Thread thread)
        {
            extern "uAutoReleasePool pool";
            extern "uSetThreadLocal(@{_currentThread}, $0)";

            try
            {
                thread._threadStart();
            }
            catch (Exception e)
            {
                // TODO: Use some kind of exception callback..
                debug_log "Unhandled exception in thread: " + e;
            }

            extern "uRelease($0)";
        }

        extern(CPLUSPLUS) ThreadHandle _threadHandle;

        readonly ThreadStart _threadStart;
        bool _started;

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

        public void Start()
        {
            if (_started)
                throw new ThreadStateException("Thread is running or terminated; it cannot restart.");

            _started = true;

            // This is a bit tricky. We need to retain *before* the thread starts,
            // otherwise the thread might be deleted before it has started.
            // To avoid leaks, ThreadMain() will release the thread.
            extern "uRetain($$)";

            if defined(UNIX)
            {
                if (extern<int> "pthread_create(&@{$$._threadHandle}, nullptr, _ThreadFunc, (void*)$$)" != 0)
                {
                    extern "uRelease($$)";
                    throw new InvalidOperationException("pthread_create() failed!");
                }
            }
            else if defined(WIN32)
            {
                extern "@{$$._threadHandle} = ::CreateThread(nullptr, 0, _ThreadFunc, (LPVOID)$$, 0, nullptr)";

                if (extern<bool>(_threadHandle) "!$0" )
                {
                    extern "uRelease($$)";
                    throw new InvalidOperationException("::CreateThread() failed!");
                }
            }
        }

        public void Join()
        {
            if (!_started)
                throw new ThreadStateException("Thread has not been started.");

            if defined(UNIX)
                extern(_threadHandle) "pthread_join($0, nullptr)";
            else if defined(WIN32)
                extern(_threadHandle) "::WaitForSingleObject($0, INFINITE)";
        }

        public string Name { get; set; }

        public static Thread CurrentThread
        {
            get
            {
                var ret = extern<Thread> "(@{Thread}) uGetThreadLocal(@{_currentThread})";

                if (ret == null)
                {
                    // create a new, started thread-object, and set it current
                    ret = new Thread(true);
                    // set it as the current thread
                    extern(ret) "uSetThreadLocal(@{_currentThread}, $0)";
                }

                return ret;
            }
        }

        [extern(UNIX) Require("Source.Include", "unistd.h")]
        public static void Sleep(int millis)
        {
            if defined(UNIX)
            @{
                // TODO: deal with long sleeps (overflow in the multiplication)!
                usleep($0 * 1000);
            @}
            else if defined(WIN32)
            @{
                ::Sleep($0);
            @}
        }

        extern(DOTNET)
        public CultureInfo CurrentCulture { get; set; }
    }
}

namespace System.Globalization
{
    [DotNetType]
    extern(DOTNET)
    public class CultureInfo
    {
        public static extern CultureInfo InvariantCulture { get; }
    }
}
