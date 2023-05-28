// @(MSG_ORIGIN)
// @(MSG_EDIT_WARNING)

#include <uno/ThreadUtils.h>
#include <mutex>
@{Uno.IntPtr:IncludeDirective}
@{byte:IncludeDirective}

#ifdef ANDROID
#include <android/log.h>
#elif defined(__APPLE__)
#include <TargetConditionals.h>
void uLogApple(const char* prefix, const char* format, va_list args);
#else
#include <cstdio>
#endif

#ifdef WIN32
#include <uno/WinAPI.h>
#else
#include <pthread.h>
#endif

static std::recursive_mutex _Critical;

// Synchronized logging
void uLogv(int level, const char* format, va_list args)
{
    U_ASSERT(uLogLevelDebug == 0 &&
             uLogLevelInformation == 1 &&
             uLogLevelWarning == 2 &&
             uLogLevelError == 3 &&
             uLogLevelFatal == 4);

    if (!format)
        format = "";

    if (level < 0)
        level = 0;
    else if (level > uLogLevelFatal)
        level = uLogLevelFatal;

#ifdef ANDROID
    int logs[] = {
        ANDROID_LOG_DEBUG,  // uLogLevelDebug
        ANDROID_LOG_INFO,   // uLogLevelInformation
        ANDROID_LOG_WARN,   // uLogLevelWarning
        ANDROID_LOG_ERROR,  // uLogLevelError
        ANDROID_LOG_FATAL   // uLogLevelFatal
    };
    __android_log_vprint(logs[level], "@(Project.Name)", format, args);
#else
    static const char* strings[] = {
        "",             // uLogLevelDebug
        "INFO: ",       // uLogLevelInformation
        "WARNING: ",    // uLogLevelWarning
        "ERROR: ",      // uLogLevelError
        "FATAL: "       // uLogLevelFatal
    };
#if TARGET_OS_IPHONE
    // Defined in ObjC file to call NSLog()
    uLogApple(strings[level], format, args);
#else
    FILE* fp = level >= uLogLevelWarning
            ? stderr
            : stdout;
    _Critical.lock();
    fputs(strings[level], fp);
    vfprintf(fp, format, args);
    fputc('\n', fp);
    fflush(fp);
    _Critical.unlock();
#endif
#endif
}

void uLog(int level, const char* format, ...)
{
    va_list args;
    va_start(args, format);
    uLogv(level, format, args);
    va_end(args);
}

void uFatal(const char* src, const char* msg)
{
    uLog(uLogLevelFatal, "Runtime Error in %s: %s",
        src && strlen(src) ? src : "(unknown)",
        msg && strlen(msg) ? msg : "(no message)");
    abort();
}

uThreadLocal* uCreateThreadLocal(void (*destructor)(void*))
{
#ifdef WIN32
    // TODO: Handle destructor...
    return (uThreadLocal*)(intptr_t)::TlsAlloc();
#else
    pthread_key_t handle;
    if (pthread_key_create(&handle, destructor))
        U_THROW_IOE("pthread_key_create() failed");

    return (uThreadLocal*)(intptr_t)handle;
#endif
}

void uDeleteThreadLocal(uThreadLocal* handle)
{
#ifdef WIN32
    ::TlsFree((DWORD)(intptr_t)handle);
#else
    pthread_key_delete((pthread_key_t)(intptr_t)handle);
#endif
}

void uSetThreadLocal(uThreadLocal* handle, void* data)
{
#ifdef WIN32
    ::TlsSetValue((DWORD)(intptr_t)handle, data);
#else
    pthread_setspecific((pthread_key_t)(intptr_t)handle, data);
#endif
}

void* uGetThreadLocal(uThreadLocal* handle)
{
#ifdef WIN32
    return ::TlsGetValue((DWORD)(intptr_t)handle);
#else
    return pthread_getspecific((pthread_key_t)(intptr_t)handle);
#endif
}

bool uEnterCriticalIfNull(void* addr)
{
    if (*(void**)addr)
        return false;

    _Critical.lock();

    if (!*(void**)addr)
        return true;

    _Critical.unlock();
    return false;
}

void uEnterCritical()
{
    _Critical.lock();
}

void uExitCritical()
{
    _Critical.unlock();
}

#ifdef __APPLE__

#include <execinfo.h>
uArray* uGetNativeStackTrace(int skipFrames)
{
    void* callStack[64];
    int callStackDepth = backtrace(callStack, sizeof(callStack) / sizeof(callStack[0]));
    return uArray::New(@{Uno.IntPtr[]:TypeOf}, callStackDepth - skipFrames, callStack + skipFrames);
}

#elif defined(__GNUC__) && !defined(ANDROID)

#include <unwind.h>

struct uUnwindState
{
    int _skipFrames;
    int _callStackDepth;
    void* _callStack[64];
};

static _Unwind_Reason_Code uUnwindCallback(struct _Unwind_Context* context, void *arg)
{
    uUnwindState* state = (uUnwindState*)arg;

    if (state->_skipFrames > 0)
    {
        state->_skipFrames--;
        return _URC_NO_REASON;
    }

    void* pc = (void*)_Unwind_GetIP(context);
    if (pc)
    {
        if (state->_callStackDepth == sizeof(state->_callStack) / sizeof(state->_callStack[0]))
            return _URC_END_OF_STACK;
        else
            state->_callStack[state->_callStackDepth++] = pc;
    }
    return _URC_NO_REASON;
}

// Use GCC's libunwind

uArray* uGetNativeStackTrace(int skipFrames)
{
    uUnwindState state = {
        skipFrames,
        0,
        { 0 }
    };

    _Unwind_Backtrace(uUnwindCallback, &state);
    return uArray::New(@{Uno.IntPtr[]:TypeOf}, state._callStackDepth, state._callStack);
}

#elif defined(WIN32)

// Windows provides a simple API for this
#include <windows.h>

uArray* uGetNativeStackTrace(int skipFrames)
{
    void* callStack[64];
    int callStackDepth = CaptureStackBackTrace(skipFrames, sizeof(callStack) / sizeof(callStack[0]), callStack, nullptr);
    return uArray::New(@{Uno.IntPtr[]:TypeOf}, callStackDepth, callStack);
}

#else

// last resort, we don't have any way of getting a native stack-trace, return nullptr :(
uArray* uGetNativeStackTrace(int skipFrames)
{
    return nullptr;
}

#endif
