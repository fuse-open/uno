#!/bin/bash
SELF=`echo $0 | sed 's/\\\\/\\//g'`
cd "`dirname "$SELF"`/.." || exit 1
source scripts/common.sh

# Clean pack.sh artifacts
rm -rf release upload

# Clean Uno projects in `Packages.SourcePaths`
IFS=$'\n'
for dir in `uno config Packages.SourcePaths`; do
    if [ -d "$dir" ]; then
        rm -rf "$dir/build"
        uno clean --recursive "$dir"
    fi
done

# Clean Uno projects in tests/
uno clean --recursive tests

# Clean C# projects
csharp-clean runtime.sln 1> /dev/null
csharp-clean uno.sln 1> /dev/null
