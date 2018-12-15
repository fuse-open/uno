#!/bin/bash
SELF=`echo $0 | sed 's/\\\\/\\//g'`
cd "`dirname "$SELF"`/.." || exit 1
source scripts/common.sh

# Configuration
DST="release"
BIN="$DST/bin"
LIB="$DST/lib"

# Detect version info
COMMIT=`git rev-parse HEAD`
VERSION=`cat VERSION.txt`

if [ -n "$APPVEYOR_REPO_BRANCH" ]; then
    BRANCH=$APPVEYOR_REPO_BRANCH
else
    BRANCH=`git rev-parse --abbrev-ref HEAD`
fi

if [ -n "$APPVEYOR_BUILD_NUMBER" ]; then
    BUILD_NUMBER=$APPVEYOR_BUILD_NUMBER
else
    BUILD_NUMBER=0
fi

# Use {dev, master}-COMMIT as prerelease suffix on non-release branches
if [ "$BRANCH" = master ]; then
    SUFFIX="master-${COMMIT:0:7}"
elif [[ "$BRANCH" != release-* ]]; then
    SUFFIX="dev-${COMMIT:0:7}"
fi

if [ -n "$SUFFIX" ]; then
    VERSION="$VERSION-$SUFFIX"
fi

echo "Version: $VERSION (build $BUILD_NUMBER)"
echo "Commit: $COMMIT"

# Extract the X.Y.Z part of version, removing the suffix if any
VERSION_TRIPLET=`echo $VERSION | sed -n -e 's/\([^-]*\).*/\1/p'`

# Create GlobalAssemblyInfo.Override.cs
sed -e 's/\(AssemblyVersion("\)[^"]*\(")\)/\1'$VERSION_TRIPLET.$BUILD_NUMBER'\2/' \
      -e 's/\(AssemblyFileVersion("\)[^"]*\(")\)/\1'$VERSION_TRIPLET.$BUILD_NUMBER'\2/' \
      -e 's/\(AssemblyInformationalVersion("\)[^"]*\(")\)/\1'$VERSION'\2/' \
      -e 's/\(AssemblyConfiguration("\)[^"]*\(")\)/\1'$COMMIT'\2/' \
      src/GlobalAssemblyInfo.cs > src/GlobalAssemblyInfo.Override.cs

# Build release configuration
bash scripts/build.sh --release

# Remove GlobalAssemblyInfo.Override.cs
rm -f src/GlobalAssemblyInfo.Override.cs

h1 "Preparing release"
######################

# Initialize
rm -rf ${BIN:?}/* ${LIB:?}/*
rm ${DST:?}/* 2> /dev/null || :
mkdir -p $BIN $LIB

# Core assemblies
p cp src/main/Uno.CLI.Main/bin/Release/*.{dll,exe,dylib} $BIN
p cp -f src/testing/Uno.CompilerTestRunner/bin/Release/uno-compiler-test.exe $BIN
p cp -f src/testing/Uno.TestGenerator/bin/Release/uno-test-gen.exe $BIN
p cp -f src/testing/Uno.TestRunner.CLI/bin/Release/*.{dll,exe} $BIN

# Core packages (used for testing)
p cp -R lib/build/* $LIB

# Put app loaders for macOS and Windows in subdirectories, to avoid conflicts
mkdir -p $BIN/apploader-mac
p cp -f src/runtime/Uno.AppLoader-MonoMac/bin/Release/*.{dll,exe,dylib} $BIN/apploader-mac
p cp -f src/runtime/Uno.AppLoader-MonoMac/bin/Release/monostub $BIN/apploader-mac

mkdir -p $BIN/apploader-win
p cp -f src/runtime/Uno.AppLoader-WinForms/bin/Release/*.{dll,exe} $BIN/apploader-win
p cp -rf src/runtime/Uno.AppLoader-WinForms/bin/Release/x86 $BIN/apploader-win
p cp -rf src/runtime/Uno.AppLoader-WinForms/bin/Release/x64 $BIN/apploader-win

# Generate config
p cp config/pack.unoconfig $BIN/.unoconfig
cat config/common.unoconfig >> $BIN/.unoconfig

# Generate launcher
p cp bin/uno bin/uno.exe $DST
echo "Packages.InstallDirectory: lib" > $DST/.unoconfig
echo "bin" > $DST/.unopath
