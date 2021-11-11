#!/bin/sh
# @(MSG_ORIGIN)
# @(MSG_EDIT_WARNING)
set -e
cd "`dirname "$0"`"

case $1 in
debug)
    echo "Opening Xcode"
#if @(COCOAPODS:Defined)
    pod install
    open -aXcode "@(Project.Name).xcworkspace"
#else
    open -aXcode "@(Project.Name).xcodeproj"
#endif
    exit $?
    ;;
esac

#if @(IOS_SIMULATOR:Defined)

if ! which ios-sim > /dev/null 2>&1; then
    echo "ERROR: Unable to find the 'ios-sim' command." >&2
    echo -e "\nYou can install ios-sim using NPM:" >&2
    echo -e "\n    npm install ios-sim -g\n" >&2
    exit 1
fi

#if @(COCOAPODS:Defined)
pod install
ios-sim launch -d"@(iOS.Simulator.Device || 'iPhone-12')" "build/Build/Products/@(Pbxproj.Configuration)-iphonesimulator/@(Project.Name).app" "$@"
#else
ios-sim launch -d"@(iOS.Simulator.Device || 'iPhone-12')" "build/@(Pbxproj.Configuration)-iphonesimulator/@(Project.Name).app" "$@"
#endif

#else // @(IOS_SIMULATOR:Defined)

if ! which ios-deploy > /dev/null 2>&1; then
    echo "ERROR: Unable to find the 'ios-deploy' command." >&2
    echo -e "\nYou can install ios-deploy using NPM:" >&2
    echo -e "\n    npm install ios-deploy -g\n" >&2
    exit 1
fi

case $1 in
uninstall)
    echo "Uninstalling @(BundleIdentifier)"
    ios-deploy -9 -1 "@(BundleIdentifier)"
    exit $?
    ;;
esac

#if @(COCOAPODS:Defined)
pod install
ios-deploy --noninteractive --debug --bundle "build/Build/Products/@(Pbxproj.Configuration)-iphoneos/@(Project.Name).app" "$@"
#else
ios-deploy --noninteractive --debug --bundle "build/@(Pbxproj.Configuration)-iphoneos/@(Project.Name).app" "$@"
#endif

#endif // @(IOS_SIMULATOR:Defined)
