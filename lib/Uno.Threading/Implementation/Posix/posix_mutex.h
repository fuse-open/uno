#ifndef POSIX_MUTEX_H
#define POSIX_MUTEX_H

#include <pthread.h>

bool uPthreadCreateMutex(pthread_mutex_t* mutex);
bool uPthreadWaitOneMutex(pthread_mutex_t* mutexHandle, int millisecondsTimeout);

#endif
