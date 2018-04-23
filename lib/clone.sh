#!/bin/bash
SELF=`echo $0 | sed 's/\\\\/\\//g'`
cd "`dirname "$SELF"`" || exit 1

echo "WARNING: This script is deprecated, see https://github.com/fusetools/uno#standard-library-dev"

REPOS="fuselibs unolibs"

# Default to HTTPS
FUSETOOLS="https://github.com/fusetools"

# Pass --ssh for SSH
if [ --ssh = "$*"  ]; then
    FUSETOOLS="git@github.com:fusetools"
fi

git clone --recursive $FUSETOOLS/fuselibs-public fuselibs
git clone --recursive $FUSETOOLS/premiumlibs premiumlibs
