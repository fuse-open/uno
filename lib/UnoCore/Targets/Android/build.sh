#!/bin/sh
# @(MSG_ORIGIN)
# @(MSG_EDIT_WARNING)
set -e
cd "`dirname "$0"`"

#if !@(SDK.Directory:IsSet) || !@(NDK.Directory:IsSet)
echo "ERROR: Could not locate the Android SDK or NDK." >&2
echo "" >&2
echo "These dependencies can be acquired by installing 'android-build-tools':" >&2
echo "" >&2
echo "    npm install android-build-tools -g" >&2
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

./gradlew @(APK.Gradle.Task) "$@"

#if @(LIBRARY:Defined)
ln -sf @(AAR.BuildName:QuoteSpace) @(Product:QuoteSpace)
#else
ln -sf @(APK.BuildName:QuoteSpace) @(Product:QuoteSpace)
# if !@(DEBUG:Defined)
./gradlew @(Bundle.Gradle.Task)
ln -sf @(Bundle.BuildName:QuoteSpace) @(Bundle:QuoteSpace)
# endif
#endif
