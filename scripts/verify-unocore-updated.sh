#!/bin/sh

IN_REPO=$1
BUILT=$2

if [ $# != 2 ]; then
    echo "USAGE: $0 <repo version> <fresh version>"
    exit 1
fi

echo "Verifying that the version of unocore in git ($IN_REPO) is up to date with the freshly built $2)"

find $BUILT -name '*.cs'  | while read BUILT_FILE; do
    REPO_FILE=$(echo $BUILT_FILE | sed s#$BUILT#$IN_REPO#)
    echo "Comparing $BUILT_FILE to $REPO_FILE"
    #Grep away some lines that contain paths local to the building machine
    #Can't use process substitution on Windows :(
    `grep -v "global::Uno.Diagnostics.Debug" $BUILT_FILE > __built_file`
    `grep -v "global::Uno.Diagnostics.Debug" $REPO_FILE > __repo_file`
    `grep -v "// This file was generated based on" __built_file > __built_file`
    `grep -v "// This file was generated based on" __repo_file > __repo_file`
    diff -w __built_file __repo_file
    DIFF=$?
    if [ $DIFF != 0 ]; then
        echo "ERROR: It seems you forgot to rebuild UnoCore. Please run 'make unocore'"
        exit 1
    fi
done
