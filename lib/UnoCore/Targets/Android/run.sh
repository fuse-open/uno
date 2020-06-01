#!/bin/sh
# @(MSG_ORIGIN)
# @(MSG_EDIT_WARNING)
set -e
cd "`dirname "$0"`"

case $1 in
debug)
    echo "Opening Android Studio"
    open -a"Android Studio" .
    exit $?
    ;;
uninstall)
    echo "Uninstalling @(Activity.Package)"
    @(uno) adb uninstall "@(Activity.Package)"
    exit $?
    ;;
esac

#if @(LIBRARY:Defined)
echo "ERROR: @(Product) is a library and cannot be run directly." >&2
exit 1
#else
@(uno) launch-apk "@(Product)" \
    --package=@(Activity.Package) \
    --activity=@(Activity.Name) \
    --sym-dir="app/src/main/.uno" \
    "$@"
#endif
