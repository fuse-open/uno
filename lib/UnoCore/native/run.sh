#!/bin/sh
# @(MSG_ORIGIN)
# @(MSG_EDIT_WARNING)
set -e
cd "`dirname "$0"`"

case $1 in
debug)
    shift
#if @(MAC:defined)
    cmake -GXcode "$@" .
    echo "Opening Xcode"
    open -aXcode "@(project.name).xcodeproj"
    exit $?
#else
    echo "Debugging is not supported on this platform." >&2
    exit 1
#endif
    ;;
valgrind)
    shift
    valgrind "$@" @(product:quoteSpace)
    exit $?
    ;;
esac

#if @(LIBRARY:defined)
echo "ERROR: @(product) is a library and cannot be run directly." >&2
exit 1
#else
@(product:quoteSpace)
#endif
