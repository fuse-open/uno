#!/bin/sh

ROOT=`dirname $0`/..
COMPILATION_DIR=/tmp/PackageCompilationTest

"$ROOT/bin/uno" test lib $*
"$ROOT/bin/uno" test-gen "$ROOT/lib" "$COMPILATION_DIR"
"$ROOT/bin/uno" build --target=cmake --no-strip --clean "$COMPILATION_DIR"
