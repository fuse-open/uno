// @(MSG_ORIGIN)
// @(MSG_EDIT_WARNING)

#include <Uno/Support.h>
#include <mutex>
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
#include <Uno/WinAPIHelper.h>
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
        "Info: ",       // uLogLevelInformation
        "Warning: ",    // uLogLevelWarning
        "Error: ",      // uLogLevelError
        "Fatal: "       // uLogLevelFatal
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
