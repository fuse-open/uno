#!/bin/bash
# @(MSG_ORIGIN)
# @(MSG_EDIT_WARNING)
set -e
set -o pipefail
cd "`dirname "$0"`"

if ! which xcodebuild > /dev/null 2>&1; then
    echo "ERROR: Unable to find the 'xcodebuild' command." >&2
    echo -e "\nYou can download Xcode Command Line Tools from this page:" >&2
    echo -e "\n    https://developer.apple.com/xcode/\n" >&2
    exit 1
fi

#if @(COCOAPODS:defined)
if ! which pod > /dev/null 2>&1; then
    echo "ERROR: Unable to find the 'pod' command." >&2
    echo -e "\nYou can install Cocoapods using Ruby:" >&2
    echo -e "\n    sudo gem install cocoapods\n" >&2
    exit 1
fi
#endif

mkdir -p data

function openXcode() {
    echo "Error: Xcode couldn't find a provisioning profile. Try building from the Xcode GUI and/or setting the 'iOS.DevelopmentTeam' variable in your '.unoproj' or '~/.unoconfig' to solve this problem. Pass the '--debug' flag to Uno to open the Xcode GUI. Pass the '-v' flag to Uno for more information about the error. Now opening Xcode."
    #if @(COCOAPODS:defined)
        open -aXcode "@(project.name).xcworkspace"
    #else
        open -aXcode "@(project.name).xcodeproj"
    #endif
    exit 1
}

function checkForError() {
    grep "Xcode couldn't find a provisioning profile" | while read -r line; do openXcode; done
}

#if @(IOS_SIMULATOR:defined)
BUILD_ARGS="-sdk iphonesimulator"
#else
BUILD_ARGS=""
#endif

#if @(COCOAPODS:defined)
pod install
xcodebuild $BUILD_ARGS -workspace "@(project.name).xcworkspace" -scheme "@(project.name)" -derivedDataPath build "$@" | tee >(checkForError)
#else
xcodebuild $BUILD_ARGS -project "@(project.name).xcodeproj" "$@" | tee >(checkForError)
#endif
