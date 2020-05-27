#!/bin/bash
SELF=`echo $0 | sed 's/\\\\/\\//g'`
cd "`dirname "$SELF"`/.." || exit 1
source scripts/common.sh

# Arguments
TARGET=$1

# Show uno config
uno config -v

h1 "Starting test suite"
########################

# Run uno tests
if [[ "$SKIP_LIB_TESTS" != 1 ]]; then
    uno test $TARGET lib $UNO_TEST_ARGS
fi

if [[ "$SKIP_UNO_TESTS" != 1 ]]; then
    uno test $TARGET tests/src/{Uno,UX}Test $UNO_TEST_ARGS
fi

# Run compiler tests
function uno-compiler-test {
    for config in Debug Release; do
        exe=src/test/compiler-test/bin/$config/uno-compiler-test.exe
        if [ -f $exe ]; then
            dotnet-run $exe
            return $?
        fi
    done

    echo "ERROR: uno-compiler-test.exe was not found"
    return 1
}

if [[ "$TARGET" == dotnet ]]; then
    uno-compiler-test
fi

# Check that all packages build without errors
uno build $TARGET --no-strip tests/pkgtest
