#!/bin/sh
# @(MSG_ORIGIN)
# @(MSG_EDIT_WARNING)
set -e
cd "`dirname "$0"`"

case $1 in
debug)
    shift
    rm -f CMakeCache.txt
    @(CMake) -GXcode "$@" .
    echo "Opening Xcode"
    open -aXcode "@(Project.Name).xcodeproj"
    exit $?
    ;;
valgrind)
    shift
    valgrind "$@" @(Product:QuoteSpace)
    exit $?
    ;;
esac

@(Product:QuoteSpace)
