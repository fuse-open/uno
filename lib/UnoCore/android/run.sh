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
    echo "Uninstalling @(activity.package)"
    @(uno) adb uninstall "@(activity.package)"
    exit $?
    ;;
esac

#if @(LIBRARY:defined)
echo "ERROR: @(product) is a library and cannot be run directly." >&2
exit 1
#else
@(uno) launch-apk "@(product)" \
    --package=@(activity.package) \
    --activity=@(activity.name) \
    --sym-dir="app/src/main/.uno" \
    @(ANDROID_EMU:defined:test('--emulator', '')) \
    "$@"
#endif
