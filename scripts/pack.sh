#!/bin/bash
SELF=`echo $0 | sed 's/\\\\/\\//g'`
cd "`dirname "$SELF"`/.." || exit 1
source scripts/common.sh

# Initialize
shopt -s dotglob
rm -rf bin/* 2> /dev/null || :
mkdir -p bin

# Detect version info
VERSION=`bash scripts/get-version.sh`
BUILD_NUMBER="0"
COMMIT=""

if [ -n "$APPVEYOR" ]; then
    BUILD_NUMBER=$APPVEYOR_BUILD_NUMBER
    COMMIT=$APPVEYOR_REPO_COMMIT
elif [ -n "$TRAVIS" ]; then
    BUILD_NUMBER=$TRAVIS_BUILD_NUMBER
    COMMIT=$TRAVIS_COMMIT
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
bash scripts/build.sh --install --release --version=$VERSION

# Remove GlobalAssemblyInfo.Override.cs
rm -f src/GlobalAssemblyInfo.Override.cs

# Clean up
function find-all {
    local root="$1"
    shift
    while [ $# -gt 0 ]; do
        bash -lc "find \"$root\" -name \"$1\""
        shift
    done
}

function rm-all {
    IFS=$'\n'
    for i in `find-all "$@"`; do
        rm -rf "$i"
    done
}

function filecompare {
    node_modules/.bin/filecompare "$i" "$file" | grep true > /dev/null
}

function rm-identical {
    local root=$1
    shift
    IFS=$'\n'
    for i in `find-all "$@"`; do
        local file="$root/`basename $i`"
        [ -f "$file" ] || continue
        filecompare "$i" "$file" || continue
        echo "stripping $file"
        rm -rf "$file"
        # Add placeholder for restore.js
        touch "$file.restore"
    done
}

h1 "Optimizing package"

# OpenTK and Xamarin.Mac will be added back by restore.js
rm-identical bin node_modules/@fuse-open/xamarin-mac *.dll *.dylib
rm-identical bin/mac node_modules/@fuse-open/xamarin-mac *.dll *.dylib
rm-identical bin/win node_modules/@fuse-open/opentk *.dll

# Drop superfluous build artifacts
rm-all bin *.config *.mdb *.pdb *.xml
