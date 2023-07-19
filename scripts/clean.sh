#!/bin/bash
SELF=`echo $0 | sed 's/\\\\/\\//g'`
cd "`dirname "$SELF"`/.." || exit 1
source scripts/common.sh

# Clean stdlib
IFS=$'\n'
for dir in `uno config searchPaths.sources`; do
    if [ -d "$dir" ]; then
        rm -rf "$dir/build"
        uno clean --recursive "$dir"
    fi
done

# Clean tests
uno clean --recursive tests

# Clean .NET solutions
dotnet clean --configuration Debug runtime.sln 1> /dev/null
dotnet clean --configuration Release runtime.sln 1> /dev/null
dotnet clean --configuration Debug uno.sln 1> /dev/null
dotnet clean --configuration Release uno.sln 1> /dev/null

# Clean other artifacts
rm -rf bin \
    FuseOpen.UnoCore.*.nupkg \
    fuse-open-uno-*.tgz
