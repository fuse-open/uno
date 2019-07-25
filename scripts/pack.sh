#!/bin/bash
SELF=`echo $0 | sed 's/\\\\/\\//g'`
cd "`dirname "$SELF"`/.." || exit 1
source scripts/common.sh

# Initialize
DST="release"
shopt -s dotglob
rm -rf ${DST:?}/* 2> /dev/null || :
mkdir -p $DST

# Detect version info
COMMIT=`git rev-parse HEAD 2> /dev/null || :`
VERSION=`cat package.json | grep version | head -1 | awk -F: '{ print $2 }' | sed 's/[\",]//g' | tr -d '[[:space:]]'`

if [ -n "$APPVEYOR_REPO_BRANCH" ]; then
    BRANCH=$APPVEYOR_REPO_BRANCH
elif [ -n "$COMMIT" ]; then
    BRANCH=`git rev-parse --abbrev-ref HEAD`
else
    BRANCH="unknown"
    COMMIT="unknown"
fi

if [ -n "$APPVEYOR_BUILD_NUMBER" ]; then
    BUILD_NUMBER=$APPVEYOR_BUILD_NUMBER
else
    BUILD_NUMBER=0
fi

# Use {dev, master}-COMMIT as prerelease suffix on non-release branches
if [ -n "$APPVEYOR_REPO_TAG_NAME" ]; then
    # Don't set prerelease suffix on AppVeyor builds started by tag
    SUFFIX=
elif [ "$BRANCH" = master ]; then
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

# Trigger release builds
h1 "Installing packages"
nuget restore uno.sln

h1 "Building platform tools"
CONFIGURATION=Release csharp-build uno.sln

h1 "Building core library"
uno doctor --configuration=Release --version=$VERSION lib

# Remove GlobalAssemblyInfo.Override.cs
rm -f src/GlobalAssemblyInfo.Override.cs

h1 "Preparing release"
######################

# Copy assemblies
cp src/main/Uno.CLI.Main/bin/Release/*.{dll,exe,dylib} $DST
cp -f src/testing/Uno.CompilerTestRunner/bin/Release/uno-compiler-test.exe $DST
cp -f src/testing/Uno.TestGenerator/bin/Release/uno-test-gen.exe $DST
cp -f src/testing/Uno.TestRunner.CLI/bin/Release/*.{dll,exe} $DST

# Put app loaders for macOS and Windows in subdirectories to avoid conflicts
mkdir -p $DST/apploader-mac
cp -f src/runtime/Uno.AppLoader-MonoMac/bin/Release/*.{dll,exe,dylib} $DST/apploader-mac
cp -f src/runtime/Uno.AppLoader-MonoMac/bin/Release/monostub $DST/apploader-mac

mkdir -p $DST/apploader-win
cp -f src/runtime/Uno.AppLoader-WinForms/bin/Release/*.{dll,exe} $DST/apploader-win

# Generate config file
cp config/pack.unoconfig $DST/.unoconfig
echo "Packages.SearchPaths += ../lib/build" >> $DST/.unoconfig
