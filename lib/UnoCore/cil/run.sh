#!/bin/sh
# @(MSG_ORIGIN)
# @(MSG_EDIT_WARNING)
set -e
cd "`dirname "$0"`"

#if @(LIBRARY:defined) || @(PREVIEW:defined)
echo "ERROR: @(product) is a library and cannot be run directly." >&2
exit 1
#elif @(CONSOLE:defined) || @(TEST:defined) && !@(APPTEST:defined) || !@(HOST_MAC:defined)
exec dotnet @(product:quoteSpace) "$@"
#else
exec @(product:quoteSpace) "$@"
#endif
