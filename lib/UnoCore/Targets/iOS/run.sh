#!/bin/sh
# @(MSG_ORIGIN)
# @(MSG_EDIT_WARNING)
set -e
cd "`dirname "$0"`"

case $1 in
debug)
    echo "Opening Xcode"
#if @(Cocoapods:Defined)
    pod install
    open -aXcode "@(Project.Name).xcworkspace"
#else
    open -aXcode "@(Project.Name).xcodeproj"
#endif
    exit $?
    ;;
esac

if ! which ios-deploy > /dev/null 2>&1; then
    echo "ERROR: Unable to find the 'ios-deploy' command." >&2
    echo -e "\nYou can install ios-deploy using NPM:" >&2
    echo -e "\n    npm install ios-deploy@beta -g\n" >&2
    exit 1
fi

case $1 in
uninstall)
    echo "Uninstalling @(BundleIdentifier)"
    ios-deploy -9 -1 "@(BundleIdentifier)"
    exit $?
    ;;
esac

#if @(Cocoapods:Defined)
pod install
ios-deploy --noninteractive --debug --bundle "build/Build/Products/@(Pbxproj.Configuration)-iphoneos/@(Project.Name).app" "$@"
#else
ios-deploy --noninteractive --debug --bundle "build/@(Pbxproj.Configuration)-iphoneos/@(Project.Name).app" "$@"
#endif
