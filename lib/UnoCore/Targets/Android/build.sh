#!/bin/sh
# @(MSG_ORIGIN)
# @(MSG_EDIT_WARNING)
set -e
cd "`dirname "$0"`"

#if @(JDK.Directory:IsSet)
export JAVA_HOME="@(JDK.Directory)"
#endif

./gradlew @(Gradle.Task) "$@"

#if !@(LIBRARY:Defined)
ln -sf @(APK.BuildName:QuoteSpace) @(Product:QuoteSpace)
#endif
