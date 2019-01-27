#!/bin/bash
SELF=`echo $0 | sed 's/\\\\/\\//g'`
cd "`dirname "$SELF"`/.." || exit 1
source scripts/common.sh

# Configuration
CONFIGURATION="Debug"

# Parse options
for arg in "$@"; do
    case $arg in
    -h|--help)
        echo "Build options:"
        echo "  --debug     Use 'Debug' configuration (default)"
        echo "  --release   Use 'Release' configuration"
        exit 0
        ;;
    --debug)
        CONFIGURATION="Debug"
        ;;
    --release)
        CONFIGURATION="Release"
        ;;
    *)
        echo "ERROR: Invalid argument '$arg'" >&2
        exit 1
        ;;
    esac
done

h1 "Installing packages"
nuget restore uno.sln

h1 "Building platform tools"
csharp-build uno.sln

h1 "Building core library"
uno doctor -ec$CONFIGURATION lib
