#!/bin/bash
SELF=`echo $0 | sed 's/\\\\/\\//g'`
cd "`dirname "$SELF"`/.." || exit 1
source scripts/common.sh

# Arguments
TARGET=$1
CONFIGURATION=$2

if [ -z "$CONFIGURATION" ]; then
    CONFIGURATION="Debug"
fi

# Show uno config
uno config -v

h1 "Starting test suite"
########################

# Run library tests
if [[ "$SKIP_LIB_TESTS" != 1 ]]; then
    uno test $TARGET lib $UNO_TEST_ARGS
fi

# Run uno/ux tests
if [[ "$SKIP_UNO_TESTS" != 1 ]]; then
    uno test $TARGET tests/src/{Uno,UX}Test $UNO_TEST_ARGS
fi

# Run compiler tests
if [[ "$TARGET" == dotnet ]]; then
    dotnet src/test/compiler-test/bin/$CONFIGURATION/net6.0/compiler-test.dll
fi

# Check that all libraries build without errors
uno build $TARGET --no-strip tests/libtest

# Run tests from dotnet solutions
if [[ "$TARGET" == dotnet ]]; then
    h1 "Running dotnet tests"
    dotnet test --verbosity detailed \
        src/test/tests/bin/$CONFIGURATION/net6.0/Uno.TestRunner.Tests.dll \
        src/ux/tests/bin/$CONFIGURATION/net6.0/Uno.UX.Markup.Tests.dll
fi

if [[ "$SKIP_SUBSEQUENT" == 1 ]]; then
    exit 0
fi

h1 "Testing subsequent builds"
##############################

if [[ "$SKIP_LIB_TESTS" != 1 ]]; then
    uno test $TARGET lib $UNO_TEST_ARGS --build-only
fi

if [[ "$SKIP_UNO_TESTS" != 1 ]]; then
    uno test $TARGET tests/src/{Uno,UX}Test $UNO_TEST_ARGS --build-only
fi

uno build $TARGET --no-strip tests/libtest
