#!/bin/bash
SELF=`echo $0 | sed 's/\\\\/\\//g'`
cd "`dirname "$SELF"`/.." || exit 1
source scripts/common.sh

SRC_DIR="lib/UnoCore"
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
