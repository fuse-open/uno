#!/bin/sh

ROOT=`dirname $0`/..
COMPILATION_DIR=/tmp/PackageCompilationTest

"$ROOT/prebuilt/uno" test Library $*
"$ROOT/prebuilt/uno" test-gen "$ROOT/Library/Core" "$COMPILATION_DIR"
"$ROOT/prebuilt/uno" build --target=cmake --no-strip --clean "$COMPILATION_DIR"
