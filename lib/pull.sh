#!/bin/bash
SELF=`echo $0 | sed 's/\\\\/\\//g'`
cd "`dirname "$SELF"`" || exit 1

echo "WARNING: This script is deprecated, see https://github.com/fusetools/uno#standard-library-dev"

for f in *; do
    if [ -d $f/.git ]; then
        pushd $f > /dev/null
        echo "`pwd`"
        if [ -n "$1" ]; then
            git checkout -f $1 || continue
        fi
        git pull --rebase
        git submodule update --init --recursive
        popd > /dev/null
    fi
done
