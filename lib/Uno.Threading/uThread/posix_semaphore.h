#ifndef POSIX_SEMAPHORE_H
#define POSIX_SEMAPHORE_H

#include <pthread.h>

struct uPosixSemaphore
{
    pthread_mutex_t mutex;
    pthread_cond_t  cond;
    int count;
    int maxCount;
};

uPosixSemaphore* uPosixCreateSemaphore(int initialCount, int maxCount);
void uPosixDisposeSemaphore(uPosixSemaphore* semaphoreHandle);

bool uPosixWaitOneSemaphore(uPosixSemaphore* semaphoreHandle, int timeoutMillis);
int uPosixReleaseSemaphore(uPosixSemaphore* semaphoreHandle, int releaseCount);

#endif
