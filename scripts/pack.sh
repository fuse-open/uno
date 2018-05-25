#!/bin/bash
SELF=`echo $0 | sed 's/\\\\/\\//g'`
cd "`dirname "$SELF"`/.." || exit 1
source scripts/common.sh

# Configuration
DST="release"
BIN="$DST/bin"
LIB="$DST/lib"
OUT="upload"

case $OSTYPE in
  darwin*)
    PLATFORM="macOS"
    ;;
  msys*)
    PLATFORM="win32"
    ;;
  linux*)
    PLATFORM="linux"
    ;;
  *)
    echo "ERROR: unknown OSTYPE: '$OSTYPE'" >&2
    exit 1
esac

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

# Extract the X.Y.Z part of version, removing the suffix if any
VERSION_TRIPLET=`echo $VERSION | sed -n -e 's/\([^-]*\).*/\1/p'`

# Start with updating the local GlobalAssemblyInfo.Override.cs
sed -e 's/\(AssemblyVersion("\)[^"]*\(")\)/\1'$VERSION_TRIPLET'\2/' \
      -e 's/\(AssemblyFileVersion("\)[^"]*\(")\)/\1'$VERSION_TRIPLET'\2/' \
      -e 's/\(AssemblyInformationalVersion("\)[^"]*\(")\)/\1'$VERSION'\2/' \
      src/GlobalAssemblyInfo.cs > src/GlobalAssemblyInfo.Override.cs

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
p cp src/main/Uno.CLI.Main/bin/Release/*.{dll,exe,dylib} $BIN
p cp -f src/testing/Uno.CompilerTestRunner/bin/Release/uno-compiler-test.exe $BIN
p cp -f src/testing/Uno.TestGenerator/bin/Release/uno-test-gen.exe $BIN
p cp -f src/testing/Uno.TestRunner.CLI/bin/Release/*.{dll,exe} $BIN

# Core packages (used for testing)
p cp -R Library/Core/build/* $LIB

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

echo "Making NuGet packages"

for i in `find src -iname "*.nuspec" | sed -e 's/.nuspec$/.csproj/'`; do
    p nuget pack -OutputDirectory "$OUT" -Properties Configuration=Release -IncludeReferencedProjects "$i"
done

p nuget pack -OutputDirectory "$OUT" -Version "$VERSION" "`dirname "$SELF"`/FuseOpen.Uno.Tool.nuspec"

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
            --version $VERSION \
            --out-dir $OUT
    fi
done

# Remove GlobalAssemblyInfo Override
rm -f src/GlobalAssemblyInfo.Override.cs
