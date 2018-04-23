#ifndef POSIX_THREAD_H
#define POSIX_THREAD_H

#include <pthread.h>

#if !@{Uno.Threading.Thread:IsStripped}
@{Uno.Threading.Thread:IncludeDirective}
#endif // !@{Uno.Threading.Thread:IsStripped}

#if !@{Uno.Threading.Thread.Start():IsStripped}
bool uPthreadsCreateThread(@{Uno.Threading.Thread} thread, pthread_t* threadHandle);
#endif // !@{Uno.Threading.Thread.Start():IsStripped}

#if !@{Uno.Threading.Thread.CurrentThread:IsStripped}
@{Uno.Threading.Thread} uPthreadsGetCurrentThread();
void uPthreadsSetCurrentThread(@{Uno.Threading.Thread} thread);
#endif // !@{Uno.Threading.Thread.CurrentThread:IsStripped}

#endif // POSIX_THREAD_H
