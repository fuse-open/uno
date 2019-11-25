:: @(MSG_ORIGIN)
:: @(MSG_EDIT_WARNING)
@echo off
pushd "%~dp0"
setlocal

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

:: Make sure ninja.exe and cmake.exe are in PATH.
for /D %%D in ("@(SDK.Directory:NativePath)\cmake\*") do (
    if exist "%%D\bin" (
        echo Using %%D
        set PATH="%%D\bin;%PATH%"
        goto BUILD
    )
)

:BUILD
call gradlew @(Gradle.AssembleTask) %* || goto ERROR

#if @(LIBRARY:Defined)
copy /Y @(Outputs.AAR:QuoteSpace:Replace('/', '\\')) @(Product:QuoteSpace) || goto ERROR
#else
copy /Y @(Outputs.APK:QuoteSpace:Replace('/', '\\')) @(Product:QuoteSpace) || goto ERROR
# if !@(DEBUG:Defined)
call gradlew @(Gradle.BundleTask) %* || goto ERROR
copy /Y @(Outputs.Bundle:QuoteSpace:Replace('/', '\\')) @(Bundle:QuoteSpace) || goto ERROR
# endif
#endif

popd && exit /b 0

:ERROR
popd && exit /b 1
