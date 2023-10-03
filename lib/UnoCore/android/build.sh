#!/bin/sh
# @(MSG_ORIGIN)
# @(MSG_EDIT_WARNING)
set -e
cd "`dirname "$0"`"

#if !@(sdk.directory:isSet)
echo "ERROR: Could not locate the Android SDK." >&2
echo "" >&2
echo "This dependency can be acquired by installing 'android-build-tools':" >&2
echo "" >&2
echo "    npm install android-build-tools@2.x -g" >&2
echo "" >&2
echo "After installing, pass --force to make sure the new configuration is picked up." >&2
echo "" >&2
echo "    uno build android --force" >&2
echo "" >&2
exit 1
#endif

#if @(jdk.directory:isSet)
export JAVA_HOME="@(jdk.directory)"
#endif

./gradlew @(gradle.assembleTask) "$@"

#if @(LIBRARY:defined)
ln -sf @(outputs.aar:quoteSpace) @(product:quoteSpace)
#else
ln -sf @(outputs.apk:quoteSpace) @(product:quoteSpace)
# if !@(DEBUG:defined)
./gradlew @(gradle.bundleTask)
ln -sf @(outputs.bundle:quoteSpace) @(bundle:quoteSpace)
# endif
#endif
