#!/bin/sh
# @(MSG_ORIGIN)
# @(MSG_EDIT_WARNING)
set -e
cd "`dirname "$0"`"

if ! which @(CMake) > /dev/null 2>&1; then
    echo "ERROR: Unable to find the 'cmake' command. Make sure CMake is installed and added to PATH." >&2
    exit 1
fi

#if @(MAC:Defined)
@(CMake) -GXcode "$@" .
xcodebuild -configuration @(Native.Configuration)
#else
@(CMake) -DCMAKE_BUILD_TYPE=@(Native.Configuration) "$@" .

if [ -f /proc/cpuinfo ]; then
    BUILD_ARGS=-j`grep processor /proc/cpuinfo | wc -l`
fi

make -s $BUILD_ARGS
#endif
