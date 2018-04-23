#ifndef POSIX_AUTORESETEVENT_H
#define POSIX_AUTORESETEVENT_H

#include <pthread.h>

struct uPosixResetEvent
{
    pthread_mutex_t mutex;
    pthread_cond_t cond;
    bool flag;
    bool autoReset;
};

uPosixResetEvent* uPosixCreateResetEvent(bool initialState, bool autoReset);
bool uPosixSetResetEvent(uPosixResetEvent* state);
bool uPosixResetResetEvent(uPosixResetEvent* state);
bool uPosixWaitOneResetEvent(uPosixResetEvent* state, int timeoutMillis);
void uPosixDisposeResetEvent(uPosixResetEvent* state);

#endif
