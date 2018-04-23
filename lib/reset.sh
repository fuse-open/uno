#!/bin/bash
SELF=`echo $0 | sed 's/\\\\/\\//g'`
cd "`dirname "$SELF"`" || exit 1

echo "WARNING: This script is deprecated, see https://github.com/fusetools/uno#standard-library-dev"

for f in *; do
    if [ -d $f/.git ]; then
        pushd $f > /dev/null
        echo "`pwd`"
        git reset --hard HEAD
        git submodule update -f
        popd > /dev/null
    fi
done
