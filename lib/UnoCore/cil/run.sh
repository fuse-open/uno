#!/bin/sh
# @(MSG_ORIGIN)
# @(MSG_EDIT_WARNING)

SOURCE="${BASH_SOURCE[0]}"
while [ -h "$SOURCE" ]; do
    DIR="$( cd -P "$( dirname "$SOURCE" )" && pwd )"
    SOURCE="$(readlink "$SOURCE")"
    [[ "$SOURCE" != /* ]] && SOURCE="$DIR/$SOURCE"
done

#if @(LIBRARY:Defined)
echo "ERROR: @(Product) is a library and cannot be run directly." >&2
exit 1
#else
exec dotnet "@(Product)" "$@"
#endif
