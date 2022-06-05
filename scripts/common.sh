#!/bin/bash
set -e

function uno {
    dotnet bin/net6.0/uno.dll "$@"
}

function h1 {
    str="$@"
    printf "\n\e[92m$str\n"
    for ((i=1; i<=${#str}; i++)); do 
        echo -n -
    done
    printf "\e[39m\n"
}

function p {
    str="$@"
    printf "\e[90m$str\e[39m\n"
    cmd=$1
    shift
    $cmd "$@"
}
