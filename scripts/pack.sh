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
VERSION=`cat package.json | grep version | head -1 | awk -F: '{ print $2 }' | sed 's/[\",]//g' | tr -d '[[:space:]]'`
BUILD_NUMBER="0"
COMMIT=""

if [ -n "$APPVEYOR" ]; then
    BUILD_NUMBER=$APPVEYOR_BUILD_NUMBER
    COMMIT=$APPVEYOR_REPO_COMMIT
elif [ -d .git ]; then
    # Get commit SHA from local git-repo.
    COMMIT=`git rev-parse HEAD 2> /dev/null || :`
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

# Release build
bash scripts/build.sh --release --version=$VERSION

# Remove GlobalAssemblyInfo.Override.cs
rm -f src/GlobalAssemblyInfo.Override.cs

h1 "Preparing release"
######################

# Copy assemblies
cp src/main/Uno.CLI.Main/bin/Release/*.{dll,exe,dylib} $DST
cp -f src/testing/Uno.CompilerTestRunner/bin/Release/uno-compiler-test.exe $DST
cp -f src/testing/Uno.TestRunner.CLI/bin/Release/*.{dll,exe} $DST

# Put app loaders for macOS and Windows in subdirectories to avoid conflicts
mkdir -p $DST/apploader-mac
cp -f src/runtime/Uno.AppLoader-MonoMac/bin/Release/*.{dll,exe,dylib} $DST/apploader-mac
cp -f src/runtime/Uno.AppLoader-MonoMac/bin/Release/monostub $DST/apploader-mac

mkdir -p $DST/apploader-win
cp -f src/runtime/Uno.AppLoader-WinForms/bin/Release/*.{dll,exe} $DST/apploader-win

# Generate config file
cat <<EOF >> $DST/.unoconfig
Assemblies.Test: uno-test.exe
Assemblies.Uno: uno.exe

if WIN32 {
    Paths.AppLoader: apploader-win
} else if MAC {
    Paths.AppLoader: apploader-mac
}

Packages.SearchPaths += ../lib/build
EOF
