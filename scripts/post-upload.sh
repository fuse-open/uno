#!/bin/sh
set -e

echo Writing `pwd -P`/uno.stuff

WIN32=`cat upload/uno-win32.stuff`
MAC=`cat upload/uno-macOS.stuff`

cat <<EOF > uno.stuff
if WIN32 {
    $WIN32
} else if MAC {
    $MAC
}
EOF
