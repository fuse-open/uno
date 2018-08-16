using Uno.Compiler.ExportTargetInterop;

namespace Uno.Threading
{
    [extern(DOTNET) DotNetType("System.Threading.Monitor")]
    [extern(CPLUSPLUS) Require("Source.Include", "Uno/ObjectMonitor.h")]
    [extern(CPLUSPLUS) Require("Source.Include", "Uno/Support.h")]
    public static class Monitor
    {
        public static void Enter(object obj)
        {
            if defined(CPLUSPLUS)
            @{
                if (uEnterCriticalIfNull(&uPtr($0)->__monitor))
                {
                    $0->__monitor = new uObjectMonitor();
                    uExitCritical();
                }

                $0->__monitor->Mutex.lock();
                $0->__monitor->LockCount++;
            @}
            else
                throw new NotImplementedException();
        }

        public static void Enter(object obj, ref bool lockTaken)
        {
            lockTaken = TryEnter(obj);
        }

        public static void Exit(object obj)
        {
            if defined(CPLUSPLUS)
            @{
                uPtr(uPtr($0)->__monitor)->LockCount--;
                $0->__monitor->Mutex.unlock();
            @}
            else
                throw new NotImplementedException();
        }

        public static bool IsEntered(object obj)
        {
            if defined(CPLUSPLUS)
            @{
                return uPtr($0)->__monitor && $0->__monitor->LockCount;
            @}
            else
                return false;
        }

        public static void Pulse(object obj)
        {
            if defined(CPLUSPLUS)
            @{
                uPtr(uPtr($0)->__monitor)->Cond.notify_one();
            @}
            else
                throw new NotImplementedException();
        }

        public static void PulseAll(object obj)
        {
            if defined(CPLUSPLUS)
            @{
                uPtr(uPtr($0)->__monitor)->Cond.notify_all();
            @}
            else
                throw new NotImplementedException();
        }

        public static bool TryEnter(object obj)
        {
            if defined(CPLUSPLUS)
            @{
                if (uEnterCriticalIfNull(&uPtr($0)->__monitor))
                {
                    $0->__monitor = new uObjectMonitor();
                    uExitCritical();
                }

                if (!$0->__monitor->Mutex.try_lock())
                    return false;

                $0->__monitor->LockCount++;
                return true;
            @}
            else
                throw new NotImplementedException();
        }

        public static void TryEnter(object obj, ref bool lockTaken)
        {
            lockTaken = TryEnter(obj);
        }

        public static bool TryEnter(object obj, int millisecondsTimeout)
        {
            if defined(CPLUSPLUS)
            @{
#if HAVE_STD_RECURSIVE_TIMED_MUTEX
                if (uEnterCriticalIfNull(&uPtr($0)->__monitor))
                {
                    $0->__monitor = new uObjectMonitor();
                    uExitCritical();
                }

                if (!$0->__monitor->Mutex.try_lock_for(std::chrono::milliseconds($1)))
                    return false;

                $0->__monitor->LockCount++;
                return true;
#else
                U_FATAL("std::recursive_timed_mutex is not implemented (on Android, API >= 21 is required).");
#endif
            @}
            else
                throw new NotImplementedException();
        }

        public static void TryEnter(object obj, int millisecondsTimeout, ref bool lockTaken)
        {
            lockTaken = TryEnter(obj, millisecondsTimeout);
        }

        public static bool Wait(object obj)
        {
            if defined(CPLUSPLUS)
            @{
                if (uEnterCriticalIfNull(&uPtr($0)->__monitor))
                {
                    $0->__monitor = new uObjectMonitor();
                    uExitCritical();
                }

#if HAVE_STD_RECURSIVE_TIMED_MUTEX
                std::unique_lock<std::recursive_timed_mutex> lock($0->__monitor->Mutex);
#else
                std::unique_lock<std::recursive_mutex> lock($0->__monitor->Mutex);
#endif
                $0->__monitor->Cond.wait(lock);
                return true;
            @}
            else
                throw new NotImplementedException();
        }

        public static bool Wait(object obj, int millisecondsTimeout)
        {
            if defined(CPLUSPLUS)
            @{
#if HAVE_STD_RECURSIVE_TIMED_MUTEX
                if (uEnterCriticalIfNull(&uPtr($0)->__monitor))
                {
                    $0->__monitor = new uObjectMonitor();
                    uExitCritical();
                }

                std::unique_lock<std::recursive_timed_mutex> lock($0->__monitor->Mutex);
                $0->__monitor->Cond.wait_for(lock, std::chrono::milliseconds($1));
                return true;
#else
                U_FATAL("std::recursive_timed_mutex is not implemented (on Android, API >= 21 is required).");
#endif
            @}
            else
                throw new NotImplementedException();
        }
    }
}
