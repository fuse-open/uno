#!/bin/sh
# @(MSG_ORIGIN)
# @(MSG_EDIT_WARNING)
set -e
cd "`dirname "$0"`"

if ! which cmake > /dev/null 2>&1; then
    echo "ERROR: Unable to find the 'cmake' command. Make sure CMake is installed and added to PATH." >&2
    exit 1
fi

if [ -z "$@" ]; then
    if [ -f /proc/cpuinfo ]; then
        BUILD_ARGS=-j`grep processor /proc/cpuinfo | wc -l`
    elif [ "`uname`" = "Darwin" ]; then
        BUILD_ARGS=-j`sysctl hw.ncpu | cut -d " " -f 2`
    elif [ -n "$NUMBER_OF_PROCESSORS" ]; then
        BUILD_ARGS=-j$NUMBER_OF_PROCESSORS
    else
        BUILD_ARGS=-j1
    fi
fi

# remove cache in case -G was specified on command line
rm -f CMakeCache.txt

cmake -DCMAKE_BUILD_TYPE=@(PInvoke.Configuration) "$@" .
cmake --build . --use-stderr -- $BUILD_ARGS
