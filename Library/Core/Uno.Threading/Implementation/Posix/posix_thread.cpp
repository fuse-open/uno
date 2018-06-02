#include <Uno/Uno.h>
#include <assert.h>
#include <Implementation/Posix/posix_thread.h>

#if !@{Uno.Threading.Thread.Start():IsStripped} || !@{Uno.Threading.Thread.CurrentThread:IsStripped}

static pthread_key_t currentThread;
void EnsureInitialized()
{
    if (uEnterCriticalIfNull(&currentThread))
    {
        pthread_key_create(&currentThread, NULL);
        uExitCritical();
    }
}

#endif // !@{Uno.Threading.Thread.Start():IsStripped} || !@{Uno.Threading.Thread.CurrentThread:IsStripped}

#if !@{Uno.Threading.Thread.Start():IsStripped}

static void* ThreadStartup(void* arg)
{
    uAutoReleasePool pool;

    @{Uno.Threading.Thread} thread = (@{Uno.Threading.Thread})arg;
    uDelegate* threadStart = @{Uno.Threading.Thread:Of(thread)._threadStart:Get()};
    assert(threadStart != NULL);

    EnsureInitialized();
    pthread_setspecific(currentThread, (void*)thread);

    @{Uno.Action:Of(threadStart):Call()};

    pthread_setspecific(currentThread, NULL);
    uRelease(thread);
    return 0;
}

bool uPthreadsCreateThread(@{Uno.Threading.Thread} thread, pthread_t* threadHandle)
{
    // This is a bit tricky. We need to retain *before* the thread starts,
    // otherwise the thread might be deleted before threadStartup has
    // started. To avoid leaks, threadStartup needs to release thread.

    uRetain(thread);
    return pthread_create(threadHandle, NULL, ThreadStartup, (void*)thread) == 0;
}

#endif // !@{Uno.Threading.Thread.Start():IsStripped}

#if !@{Uno.Threading.Thread.CurrentThread:IsStripped}

@{Uno.Threading.Thread} uPthreadsGetCurrentThread()
{
    EnsureInitialized();
    return (@{Uno.Threading.Thread})pthread_getspecific(currentThread);
}

void uPthreadsSetCurrentThread(@{Uno.Threading.Thread} thread)
{
    EnsureInitialized();
    pthread_setspecific(currentThread, (void*)thread);
}

#endif // !@{Uno.Threading.Thread.CurrentThread:IsStripped}
