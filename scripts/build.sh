#!/bin/bash
SELF=`echo $0 | sed 's/\\\\/\\//g'`
cd "`dirname "$SELF"`/.." || exit 1
source scripts/common.sh

# Configuration
BUILD_PLATFORM=1
BUILD_UNOCORE=0
BUILD_LIBRARY=1
CONFIGURATION="Debug"
REBUILD_LIBRARY=0

# Allow CI system to set the correct SHA when there are multiple VCS roots in the build
if [ "$BUILD_SHA" ]; then
    BUILD_VCS_NUMBER=$BUILD_SHA
fi

# Parse options
for arg in "$@"; do
    case $arg in
    -h|--help)
        echo "Build options:"
        echo "  --unocore   Generate Uno.Runtime.Core.csproj"
        echo "  --rebuild   Rebuild package library"
        echo "  --all       Build everything"
        echo ""
        echo "Configuration options:"
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
    --unocore)
        BUILD_PLATFORM=0
        BUILD_UNOCORE=1
        BUILD_LIBRARY=0
        ;;
    --all)
        BUILD_PLATFORM=1
        BUILD_UNOCORE=1
        BUILD_LIBRARY=1
        REBUILD_LIBRARY=1
        ;;
    --rebuild)
        REBUILD_LIBRARY=1
        ;;
    *)
        echo "ERROR: Invalid argument '$arg'" >&2
        exit 1
        ;;
    esac
done

if [ "$BUILD_UNOCORE" = 1 ]; then
    h1 "Building Uno.Runtime.Core.csproj"
    SRC_DIR="Library/Core/UnoCore"
    OUT_DIR="$SRC_DIR/build/corelib/Debug"
    DST_DIR="src/runtime/Uno.Runtime.Core"

    # Build Uno project
    uno build corelib $SRC_DIR -z

    # Build C# project
    csharp-build $OUT_DIR/*.sln

    # Replace C# project
    rm -rf ${OUT_DIR:?}/{bin,obj}
    rm -rf $DST_DIR
    p cp -R $OUT_DIR $DST_DIR
    rm $DST_DIR/*.sln
fi

if [ "$BUILD_PLATFORM" = 1 ]; then
    h1 "Building platform tools"
    if [ -n "$BUILD_NUMBER" ]; then
        # BUILD_NUMBER is substring after '+' in value from TC
        BUILD_NUMBER=`echo $BUILD_NUMBER | grep -o "+.*"`
        BUILD_NUMBER=${BUILD_NUMBER:1}
        SLN_ARGS="-b$BUILD_NUMBER -c$BUILD_VCS_NUMBER"
    fi

    # Get version number
    VERSION=`cat VERSION.txt`

    # Build C# solution
    csharp-build uno.sln
fi

if [ "$BUILD_LIBRARY" = 1 ]; then
    h1 "Building standard library"
    if [ "$REBUILD_LIBRARY" = 1 ]; then
        uno doctor -ac$CONFIGURATION --build-number=$VERSION
    else
        uno doctor -ec$CONFIGURATION --build-number=$VERSION
    fi
fi
