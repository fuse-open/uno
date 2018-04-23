#ifndef WIN32_HELPERS_H
#define WIN32_HELPERS_H

#if !@{Uno.Threading.Thread:IsStripped}
@{Uno.Threading.Thread:IncludeDirective}
#endif

#include "Uno/WinAPIHelper.h"

#if !@{Uno.Threading.Thread.Start():IsStripped}
HANDLE uWin32CreateThread(@{Uno.Threading.Thread} thread);
#endif // !@{Uno.Threading.Thread.Start():IsStripped}

#if !@{Uno.Threading.Thread.CurrentThread:IsStripped}
@{Uno.Threading.Thread} uWin32ThreadGetCurrentThread();
void uWin32ThreadSetCurrentThread(@{Uno.Threading.Thread} thread);
#endif // !@{Uno.Threading.Thread.CurrentThread:IsStripped}

#endif
