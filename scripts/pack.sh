#!/bin/bash
SELF=`echo $0 | sed 's/\\\\/\\//g'`
cd "`dirname "$SELF"`/.." || exit 1
source scripts/common.sh

# Configuration
DST="release"
BIN="$DST/bin"
LIB="$DST/lib"
OUT="upload"
PLATFORM=`get-platform`

# Detect version
if [ -n "$RELEASE_VERSION" ]; then
    # BRANCH is set by TeamCity
    COMMIT="$BUILD_VCS_NUMBER"
    VERSION="$RELEASE_VERSION"
else
    BRANCH=`git rev-parse --abbrev-ref HEAD`
    COMMIT=`git rev-parse HEAD`
    VERSION=`cat VERSION.txt`
fi

# Use {dev, master}-COMMIT as prerelease suffix on non-release branches
if [ "$BRANCH" = master ]; then
    SUFFIX="master-${COMMIT:0:7}"
elif [[ "$BRANCH" != release-* ]]; then
    SUFFIX="dev-${COMMIT:0:7}"
fi

if [ -n "$SUFFIX" ]; then
    SUFFIX_OPT="--suffix=$SUFFIX"
    VERSION="$VERSION-$SUFFIX"
fi

# Build
if [ "$1" != --no-build ]; then
    bash scripts/build.sh --release; echo ""
fi

h1 "Creating distribution ($PLATFORM)"
######################################

# Initialize
rm -rf ${BIN:?}/* ${LIB:?}/* ${OUT:?}/*
rm ${DST:?}/* 2> /dev/null || :
mkdir -p $BIN $LIB $OUT
touch $BIN/uno.stuff-pack

# Core assemblies
p cp src/main/Uno.CLI.Main/bin/Release/*.{dll,exe} $BIN
p cp -f src/testing/Uno.CompilerTestRunner/bin/Release/uno-compiler-test.exe $BIN
p cp -f src/testing/Uno.TestGenerator/bin/Release/uno-test-gen.exe $BIN
p cp -f src/testing/Uno.TestRunner.CLI/bin/Release/*.{dll,exe} $BIN

# Core packages (used for testing)
p cp -R Library/Core/build/* $LIB

# Platform specific
case $OSTYPE in
darwin*)
    p cp -f src/runtime/Uno.AppLoader-MonoMac/bin/Release/*.{dll,exe,dylib} $BIN
    p cp -f src/runtime/Uno.AppLoader-MonoMac/bin/Release/monostub $BIN
    ;;
msys*)
    p cp -f src/runtime/Uno.AppLoader-WinForms/bin/Release/*.{dll,exe} $BIN
    p cp -rf src/runtime/Uno.AppLoader-WinForms/bin/Release/x86 $BIN
    p cp -rf src/runtime/Uno.AppLoader-WinForms/bin/Release/x64 $BIN
    ;;
esac

# Generate config
p cp config/pack.unoconfig $BIN/.unoconfig
cat config/common.unoconfig >> $BIN/.unoconfig

# Generate launcher
p cp prebuilt/uno prebuilt/uno.exe $DST
echo "Packages.InstallDirectory: lib" > $DST/.unoconfig
echo "bin" > $DST/.unopath

# Create Stuff package for Uno
uno stuff pack $DST \
    --name=uno-$PLATFORM \
    --suffix=-$VERSION-$PLATFORM \
    --out-dir=$OUT \
    --modular

# Create Uno packages
for f in Library/Core/*; do
    NAME=`basename "$f"`
    PROJECT=$f/$NAME.unoproj
    if [ -f "$PROJECT" ]; then
        uno pack $PROJECT \
            --out-dir $OUT \
            $SUFFIX_OPT
    fi
done
