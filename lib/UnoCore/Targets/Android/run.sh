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
    @(Uno) adb uninstall "@(Activity.Package)"
    exit $?
    ;;
esac

@(Uno) launch-apk "@(Product)" \
    --package=@(Activity.Package) \
    --activity=@(Activity.Name) \
    --sym-dir="app/src/main/.uno" \
    "$@"
