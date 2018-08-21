#include <Implementation/Posix/posix_reset_event.h>
#include <errno.h>
#include <sys/time.h>
#include <stdint.h>

uPosixResetEvent* uPosixCreateResetEvent(bool initialState, bool autoReset)
{
    uPosixResetEvent* resetEvent = new uPosixResetEvent();

    pthread_mutex_init(&resetEvent->mutex, NULL);
    pthread_cond_init(&resetEvent->cond, NULL);
    resetEvent->flag = initialState;
    resetEvent->autoReset = autoReset;

    return resetEvent;
}

// Need to prefix with ResetEvent_ because of include hell
bool uPosixSetResetEvent(uPosixResetEvent* resetEvent)
{
    pthread_mutex_lock(&resetEvent->mutex);
    resetEvent->flag = true;
    pthread_mutex_unlock(&resetEvent->mutex);
    pthread_cond_signal(&resetEvent->cond);

    return true;
}

bool uPosixResetResetEvent(uPosixResetEvent* resetEvent)
{
    pthread_mutex_lock(&resetEvent->mutex);
    resetEvent->flag = false;
    pthread_mutex_unlock(&resetEvent->mutex);

    return true;
}

bool uPosixWaitOneResetEvent(uPosixResetEvent* resetEvent, int timeoutMillis)
{
    pthread_mutex_lock(&resetEvent->mutex);

    struct timeval now;
    gettimeofday(&now, NULL);
    uint64_t nanoseconds = (now.tv_sec * 1000000000ull) + (now.tv_usec * 1000);
    uint64_t timeoutNanoseconds = (timeoutMillis * 1000000ull) + nanoseconds;

    struct timespec timeout;
    timeout.tv_sec = (time_t)(timeoutNanoseconds / 1000000000ull);
    timeout.tv_nsec = (long)(timeoutNanoseconds % 1000000000ull);

    auto result = true;
    while (!resetEvent->flag)
    {
        if (timeoutMillis < 0)
            pthread_cond_wait(&resetEvent->cond, &resetEvent->mutex);
        else
        {
            if (pthread_cond_timedwait(&resetEvent->cond, &resetEvent->mutex, &timeout) == ETIMEDOUT)
            {
                result = false;
                break;
            }
        }
    }

    if (resetEvent->autoReset)
        resetEvent->flag = false;

    pthread_mutex_unlock(&resetEvent->mutex);

    return result;
}

void uPosixDisposeResetEvent(uPosixResetEvent* resetEvent)
{
    pthread_mutex_destroy(&resetEvent->mutex);
    pthread_cond_destroy(&resetEvent->cond);
    delete resetEvent;
}
