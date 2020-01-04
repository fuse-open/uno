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

h1 "Building platform tools"
csharp-build uno.sln

h1 "Building core library"
uno doctor -ec$CONFIGURATION lib "$@"
