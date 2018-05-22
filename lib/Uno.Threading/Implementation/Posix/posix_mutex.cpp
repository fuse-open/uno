#include <Implementation/Posix/posix_mutex.h>
#include <pthread.h>
#include <errno.h>
#include <uBase/Time.h>

bool uPthreadCreateMutex(pthread_mutex_t* mutex)
{
    pthread_mutexattr_t attr;
    if (pthread_mutexattr_init(&attr) != 0 ||
        pthread_mutexattr_settype(&attr, PTHREAD_MUTEX_RECURSIVE) != 0)
        return false;

    return pthread_mutex_init(mutex, &attr) == 0;
}

bool uPthreadWaitOneMutex(pthread_mutex_t* mutexHandle, int millisecondsTimeout)
{
    if (millisecondsTimeout < 0)
        return pthread_mutex_lock(mutexHandle) == 0;
    else if (millisecondsTimeout == 0)
        return pthread_mutex_trylock(mutexHandle) == 0;

    // spin-based emulation
    long long timeout = uBase::GetTicks() + millisecondsTimeout * 10000ll;
    while (pthread_mutex_trylock(mutexHandle) == EBUSY)
    {
        long long now = uBase::GetTicks();
        if (now >= timeout)
            return false;

        sched_yield();
    }
    return true;
}
