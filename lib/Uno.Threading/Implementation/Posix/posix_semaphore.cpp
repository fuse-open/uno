#include <Implementation/Posix/posix_semaphore.h>
#include <errno.h>
#include <time.h>
#include <sys/time.h>
#include <stdint.h>

uPosixSemaphore* uPosixCreateSemaphore(int initialCount, int maxCount)
{
    uPosixSemaphore *ret = new uPosixSemaphore();

    if (pthread_mutex_init(&ret->mutex, NULL) != 0 ||
        pthread_cond_init(&ret->cond, NULL) != 0)
    {
        delete ret;
        return NULL;
    }

    ret->count = initialCount;
    ret->maxCount = maxCount;
    return ret;
}

void uPosixDisposeSemaphore(uPosixSemaphore* semaphoreHandle)
{
    pthread_mutex_destroy(&semaphoreHandle->mutex);
    pthread_cond_destroy(&semaphoreHandle->cond);
    delete semaphoreHandle;
}

bool uPosixWaitOneSemaphore(uPosixSemaphore* semaphoreHandle, int timeoutMillis)
{
    struct timeval now;
    gettimeofday(&now, NULL);
    uint64_t nanoseconds = (now.tv_sec * 1000000000ull) + (now.tv_usec * 1000);
    uint64_t timeoutNanoseconds = (timeoutMillis * 1000000ull) + nanoseconds;

    struct timespec timeout;
    timeout.tv_sec = (time_t)(timeoutNanoseconds / 1000000000ull);
    timeout.tv_nsec = (long)(timeoutNanoseconds % 1000000000ull);

    pthread_mutex_lock(&semaphoreHandle->mutex);
    while (semaphoreHandle->count == 0)
    {
        if (timeoutMillis < 0)
            pthread_cond_wait(&semaphoreHandle->cond, &semaphoreHandle->mutex);
        else
        {
            if (pthread_cond_timedwait(&semaphoreHandle->cond, &semaphoreHandle->mutex, &timeout) == ETIMEDOUT)
            {
                pthread_mutex_unlock(&semaphoreHandle->mutex);
                return false;
            }
        }
    }

    semaphoreHandle->count--;

    pthread_mutex_unlock(&semaphoreHandle->mutex);
    return true;
}

int uPosixReleaseSemaphore(uPosixSemaphore* semaphoreHandle, int releaseCount)
{
    pthread_mutex_lock(&semaphoreHandle->mutex);

    int ret = semaphoreHandle->count;

    if (semaphoreHandle->count + releaseCount <= semaphoreHandle->maxCount)
    {
        for (int i = 0; i < releaseCount; ++i)
            pthread_cond_signal(&semaphoreHandle->cond);

        semaphoreHandle->count += releaseCount;
    }
    else
        ret = -1;

    pthread_mutex_unlock(&semaphoreHandle->mutex);

    return ret;
}
