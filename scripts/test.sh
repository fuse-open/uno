#!/bin/bash
SELF=`echo $0 | sed 's/\\\\/\\//g'`
cd "`dirname "$SELF"`/.." || exit 1
source scripts/common.sh

# Arguments
TARGET=$1

# Run uno tests
uno test $TARGET lib
uno test $TARGET tests/src/{Uno,UX}Test

# Run compiler tests
function uno-compiler-test {
    for config in Debug Release; do
        exe=src/testing/Uno.CompilerTestRunner/bin/$config/uno-compiler-test.exe
        if [ -f $exe ]; then
            dotnet-clr $exe
            return $?
        fi
    done

    echo "ERROR: uno-compiler-test.exe was not found"
    return 1
}

uno-compiler-test

# Check that all packages build without errors
function packages-build-test {
    dir=.test/build-$1
    uno test-gen $1 $dir
    uno build $TARGET --no-strip $dir
}

packages-build-test lib
