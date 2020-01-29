#!/bin/bash
SELF=`echo $0 | sed 's/\\\\/\\//g'`
cd "`dirname "$SELF"`/.." || exit 1
source scripts/common.sh

if [ "$1" = --release ]; then
    CONFIGURATION="Release"
    shift
elif [ -z "$CONFIGURATION" ]; then
    CONFIGURATION="Debug"
fi

h1 "Installing packages"
nuget restore uno.sln
nuget restore runtime.sln

h1 "Building uno"
csharp-build uno.sln

h1 "Building runtime"
uno build lib/UnoCore -DLIBRARY
csharp-build runtime.sln

h1 "Building lib"
uno doctor -ec$CONFIGURATION lib "$@"
