#!/bin/sh
# @(MSG_ORIGIN)
# @(MSG_EDIT_WARNING)
set -e
cd "`dirname "$0"`"

#if !@(SDK.Directory:IsSet)
echo "ERROR: Could not locate the Android SDK." >&2
echo "" >&2
echo "This dependency can be acquired by installing 'android-build-tools':" >&2
echo "" >&2
echo "    npm install android-build-tools@1.x -g" >&2
echo "" >&2
echo "After installing, pass --force to make sure the new configuration is picked up." >&2
echo "" >&2
echo "    uno build android --force" >&2
echo "" >&2
exit 1
#endif

#if @(JDK.Directory:IsSet)
export JAVA_HOME="@(JDK.Directory)"
#endif

./gradlew @(Gradle.AssembleTask) "$@"

#if @(LIBRARY:Defined)
ln -sf @(Outputs.AAR:QuoteSpace) @(Product:QuoteSpace)
#else
ln -sf @(Outputs.APK:QuoteSpace) @(Product:QuoteSpace)
# if !@(DEBUG:Defined)
./gradlew @(Gradle.BundleTask)
ln -sf @(Outputs.Bundle:QuoteSpace) @(Bundle:QuoteSpace)
# endif
#endif
