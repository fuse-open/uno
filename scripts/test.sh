#!/bin/sh

ROOT=`dirname $0`/..
COMPILATION_DIR=/tmp/PackageCompilationTest

"$ROOT/prebuilt/uno" test lib $*
"$ROOT/prebuilt/uno" test-gen "$ROOT/lib" "$COMPILATION_DIR"
"$ROOT/prebuilt/uno" build --target=cmake --no-strip --clean "$COMPILATION_DIR"
