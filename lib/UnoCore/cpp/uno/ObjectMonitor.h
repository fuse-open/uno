// @(MSG_ORIGIN)
// @(MSG_EDIT_WARNING)

#pragma once
#include <condition_variable>
#include <mutex>

// Android API < 21 doesn't support std::recursive_timed_mutex :(
#if !defined(__ANDROID_API__) || __ANDROID_API__ >= 21
#define HAVE_STD_RECURSIVE_TIMED_MUTEX 1
#endif

// Keep this in its own header to avoid polluting with the includes we use here,
// only two source files need to interact with this class.
struct uObjectMonitor
{
    std::condition_variable_any Cond;
#if HAVE_STD_RECURSIVE_TIMED_MUTEX
    std::recursive_timed_mutex Mutex;
#else
    std::recursive_mutex Mutex;
#endif
    size_t LockCount;
};
