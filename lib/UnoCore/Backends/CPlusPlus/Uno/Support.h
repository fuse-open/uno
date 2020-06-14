// @(MSG_ORIGIN)
// @(MSG_EDIT_WARNING)

#pragma once

/**
    \addtogroup ThreadUtils
    @{
*/
struct uThreadLocal;

uThreadLocal* uCreateThreadLocal(void (*destructor)(void*));
void uDeleteThreadLocal(uThreadLocal* tls);

void uSetThreadLocal(uThreadLocal* tls, void* value);
void* uGetThreadLocal(uThreadLocal* tls);

void uEnterCritical();
bool uEnterCriticalIfNull(void* addr);
void uExitCritical();
/** @} */
