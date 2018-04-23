#include <Uno/Uno.h>
#include <assert.h>
#include <Implementation/Win32/win32_helpers.h>

#if !@{Uno.Threading.Thread:IsStripped}
static __declspec(thread) @{Uno.Threading.Thread} currentThread;
#endif // !@{Uno.Threading.Thread:IsStripped}

#if !@{Uno.Threading.Thread.Start():IsStripped}

static DWORD WINAPI ThreadStartup(LPVOID lpParam)
{
    uAutoReleasePool pool;

    @{Uno.Threading.Thread} thread = (@{Uno.Threading.Thread})lpParam;
    uDelegate *threadStart = @{Uno.Threading.Thread:Of(thread)._threadStart:Get()};
    assert(threadStart != NULL);

    currentThread = thread;
    @{Uno.Action:Of(threadStart):Call()};
    currentThread = NULL;
    uRelease(thread);

    return 0;
}

HANDLE uWin32CreateThread(@{Uno.Threading.Thread} thread)
{
    // This is a bit tricky. We need to retain *before* the thread starts,
    // otherwise the thread might be deleted before uWin32ThreadStartup has
    // started. But to avoid leaks, threadStartup needs to release thread.

    uRetain(thread);
    return ::CreateThread(NULL, 0, ThreadStartup, (LPVOID)thread, 0, NULL);
}

#endif // !@{Uno.Threading.Thread.Start():IsStripped}

#if !@{Uno.Threading.Thread.CurrentThread:IsStripped}

@{Uno.Threading.Thread} uWin32ThreadGetCurrentThread()
{
    return currentThread;
}

void uWin32ThreadSetCurrentThread(@{Uno.Threading.Thread} thread)
{
    currentThread = thread;
}

#endif // !@{Uno.Threading.Thread.CurrentThread:IsStripped}
