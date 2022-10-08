#!/bin/bash
SELF=`echo $0 | sed 's/\\\\/\\//g'`
cd "`dirname "$SELF"`/.." || exit 1
source scripts/common.sh

while [ $# -gt 0 ]; do
    case "$1" in
    -i|--install)
        shift
        INSTALL=1
        ;;
    -r|--release)
        shift
        CONFIGURATION="Release"
        ;;
    *)
        break
        ;;
    esac
done

if [ -z "$CONFIGURATION" ]; then
    CONFIGURATION="Debug"
fi

if [ -z "$VERBOSITY" ]; then
    VERBOSITY="minimal"
fi

if [ ! -d node_modules/ ]; then
    INSTALL=1
fi

if [ "$INSTALL" = 1 ]; then
    h1 "Installing modules"
    npm install
fi

h1 "Building uno"
dotnet build --configuration $CONFIGURATION --verbosity $VERBOSITY uno.sln

h1 "Building runtime"
uno build lib/UnoCore -DLIBRARY --trace
dotnet build --configuration $CONFIGURATION --verbosity $VERBOSITY runtime.sln

h1 "Building lib"
uno doctor -ec$CONFIGURATION lib "$@"
