#!/bin/sh
# @(MSG_ORIGIN)
# @(MSG_EDIT_WARNING)
set -e
cd "`dirname "$0"`"

case $1 in
debug)
    echo "Opening Xcode"
#if @(COCOAPODS:defined)
    pod install
    open -aXcode "@(project.name).xcworkspace"
#else
    open -aXcode "@(project.name).xcodeproj"
#endif
    exit $?
    ;;
esac

#if @(IOS_SIMULATOR:defined)

if ! which ios-sim > /dev/null 2>&1; then
    echo "ERROR: Unable to find the 'ios-sim' command." >&2
    echo -e "\nYou can install ios-sim using NPM:" >&2
    echo -e "\n    npm install ios-sim -g\n" >&2
    exit 1
fi

if [ ! -f sim-devices.txt ]; then
    ios-sim showdevicetypes > sim-devices.txt
fi

DEVICE="@(config.ios.simulator.device || 'undefined')"

if ! cat sim-devices.txt | grep "$DEVICE" > /dev/null; then
    echo "WARNING: Simulator '$DEVICE' not found; using first iPhone" >&2
    DEVICE="iPhone-"
fi

DEVICE=`cat sim-devices.txt | grep $DEVICE | head -n 1`

echo "Simulator: $DEVICE"

#if @(COCOAPODS:defined)
pod install
ios-sim launch -d"$DEVICE" "build/Build/Products/@(pbxproj.configuration)-iphonesimulator/@(project.name).app" "$@"
#else
ios-sim launch -d"$DEVICE" "build/@(pbxproj.configuration)-iphonesimulator/@(project.name).app" "$@"
#endif

#else // @(IOS_SIMULATOR:defined)

if ! which ios-deploy > /dev/null 2>&1; then
    echo "ERROR: Unable to find the 'ios-deploy' command." >&2
    echo -e "\nYou can install ios-deploy using NPM:" >&2
    echo -e "\n    npm install ios-deploy -g\n" >&2
    exit 1
fi

case $1 in
uninstall)
    echo "Uninstalling @(bundleIdentifier)"
    ios-deploy -9 -1 "@(bundleIdentifier)"
    exit $?
    ;;
esac

#if @(COCOAPODS:defined)
pod install
ios-deploy --noninteractive --debug --bundle "build/Build/Products/@(pbxproj.configuration)-iphoneos/@(project.name).app" "$@"
#else
ios-deploy --noninteractive --debug --bundle "build/@(pbxproj.configuration)-iphoneos/@(project.name).app" "$@"
#endif

#endif // @(IOS_SIMULATOR:defined)
