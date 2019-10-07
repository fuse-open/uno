#!/bin/sh
# @(MSG_ORIGIN)
# @(MSG_EDIT_WARNING)
set -e
cd "`dirname "$0"`"

if ! which cmake > /dev/null 2>&1; then
    echo "ERROR: Unable to find the 'cmake' command. Make sure CMake is installed and added to PATH." >&2
#if @(MAC:Defined)
    echo -e "\nOn macOS, you can install CMake using Homebrew:" >&2
    echo -e "\n    brew install cmake\n" >&2
#endif
    exit 1
fi

#if @(MAC:Defined)
if ! which xcodebuild > /dev/null 2>&1; then
    echo "ERROR: Unable to find the 'xcodebuild' command." >&2
    echo -e "\nYou can download Xcode Command Line Tools from this page:" >&2
    echo -e "\n    https://developer.apple.com/xcode/\n" >&2
    exit 1
fi

cmake -GXcode "$@" .
xcodebuild -configuration @(Native.Configuration)
#else
cmake -DCMAKE_BUILD_TYPE=@(Native.Configuration) "$@" .

if [ -f /proc/cpuinfo ]; then
    BUILD_ARGS=-j`grep processor /proc/cpuinfo | wc -l`
fi

make -s $BUILD_ARGS
#endif
