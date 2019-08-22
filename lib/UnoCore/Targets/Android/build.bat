:: @(MSG_ORIGIN)
:: @(MSG_EDIT_WARNING)
@echo off
pushd "%~dp0"

#if !@(SDK.Directory:IsSet) || !@(NDK.Directory:IsSet)
echo ERROR: Could not locate the Android SDK or NDK. >&2
echo. >&2
echo These dependencies can be acquired by installing 'android-build-tools': >&2
echo. >&2
echo     npm install android-build-tools -g >&2
echo. >&2
echo After installing, pass --force to make sure the new configuration is picked up. >&2
echo. >&2
echo     uno build android --force >&2
echo. >&2
goto ERROR
#endif

#if @(JDK.Directory:IsSet)
set JAVA_HOME=@(JDK.Directory:NativePath)
#endif

call gradlew @(Gradle.Task) %* || goto ERROR
copy /Y @(APK.BuildName:QuoteSpace:Replace('/', '\\')) @(Product:QuoteSpace) || goto ERROR
popd && exit /b 0

:ERROR
popd && exit /b 1
