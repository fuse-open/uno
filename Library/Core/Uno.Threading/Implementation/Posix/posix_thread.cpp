#include <Uno/Uno.h>
#include <assert.h>
#include <Implementation/Posix/posix_thread.h>

#if !@{Uno.Threading.Thread.Start():IsStripped} || !@{Uno.Threading.Thread.CurrentThread:IsStripped}

#if IOS

static pthread_key_t currentThread;
void EnsureInitialized()
{
    if (uEnterCriticalIfNull(&currentThread))
    {
        pthread_key_create(&currentThread, NULL);
        uExitCritical();
    }
}

#else

static __thread @{Uno.Threading.Thread} currentThread;

#endif

#endif // !@{Uno.Threading.Thread.Start():IsStripped} || !@{Uno.Threading.Thread.CurrentThread:IsStripped}

#if !@{Uno.Threading.Thread.Start():IsStripped}

static void* ThreadStartup(void* arg)
{
    uAutoReleasePool pool;

    @{Uno.Threading.Thread} thread = (@{Uno.Threading.Thread})arg;
    uDelegate* threadStart = @{Uno.Threading.Thread:Of(thread)._threadStart:Get()};
    assert(threadStart != NULL);

#if IOS
    EnsureInitialized();
    pthread_setspecific(currentThread, (void*)thread);
#else
    currentThread = thread;
#endif

    @{Uno.Action:Of(threadStart):Call()};

#if IOS
    pthread_setspecific(currentThread, NULL);
#else
    currentThread = NULL;
#endif

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
#if IOS
    EnsureInitialized();
    return (@{Uno.Threading.Thread})pthread_getspecific(currentThread);
#else
    return currentThread;
#endif
}

void uPthreadsSetCurrentThread(@{Uno.Threading.Thread} thread)
{
#if IOS
    EnsureInitialized();
    pthread_setspecific(currentThread, (void*)thread);
#else
    currentThread = thread;
#endif
}


#endif // !@{Uno.Threading.Thread.CurrentThread:IsStripped}
