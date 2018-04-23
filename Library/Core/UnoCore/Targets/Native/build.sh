#!/bin/sh
# @(MSG_ORIGIN)
# @(MSG_EDIT_WARNING)
set -e
cd "`dirname "$0"`"

if ! which @(CMake) > /dev/null 2>&1; then
    echo "ERROR: Unable to find the 'cmake' command. Make sure CMake is installed and added to PATH." >&2
    exit 1
fi

if [ -f /proc/cpuinfo ]; then
    BUILD_ARGS=-j`grep processor /proc/cpuinfo | wc -l`
elif [ "`uname`" = "Darwin" ]; then
    BUILD_ARGS=-j`sysctl hw.ncpu | cut -d " " -f 2`
elif [ -n "$NUMBER_OF_PROCESSORS" ]; then
    BUILD_ARGS=-j$NUMBER_OF_PROCESSORS
fi

# CMake fails if previously run using a different generator (Xcode).
# We can avoid that by deleting the cache.
rm -f CMakeCache.txt

@(CMake) -DCMAKE_BUILD_TYPE=@(Native.Configuration) "$@" .
@(CMake) --build . --use-stderr -- $BUILD_ARGS
