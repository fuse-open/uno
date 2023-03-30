#!/bin/bash
SELF=`echo $0 | sed 's/\\\\/\\//g'`
cd "`dirname "$SELF"`/.." || exit 1
source scripts/common.sh

# Detect version info
VERSION=`bash scripts/get-version.sh`

# Build and pack
uno build lib/UnoCore -DLIBRARY --release
nuget pack lib/UnoCore -Version $VERSION
