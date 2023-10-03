:: @(MSG_ORIGIN)
:: @(MSG_EDIT_WARNING)
@echo off
pushd "%~dp0"
setlocal

#if !@(sdk.directory:isSet)
echo ERROR: Could not locate the Android SDK. >&2
echo. >&2
echo This dependency can be acquired by installing 'android-build-tools': >&2
echo. >&2
echo     npm install android-build-tools@2.x -g >&2
echo. >&2
echo After installing, pass --force to make sure the new configuration is picked up. >&2
echo. >&2
echo     uno build android --force >&2
echo. >&2
goto ERROR
#endif

#if @(jdk.directory:isSet)
set JAVA_HOME=@(jdk.directory:nativePath)
#endif

:: Make sure ninja.exe and cmake.exe are in PATH.
for /D %%D in ("@(sdk.directory:nativePath)\cmake\*") do (
    if exist "%%D\bin" (
        echo Using %%D
        set PATH="%%D\bin;%PATH%"
        goto BUILD
    )
)

:BUILD
call gradlew @(gradle.assembleTask) %* || goto ERROR

#if @(LIBRARY:defined)
copy /Y @(outputs.aar:quoteSpace:replace('/', '\\')) @(product:quoteSpace) || goto ERROR
#else
copy /Y @(outputs.apk:quoteSpace:replace('/', '\\')) @(product:quoteSpace) || goto ERROR
# if !@(DEBUG:defined)
call gradlew @(gradle.bundleTask) %* || goto ERROR
copy /Y @(outputs.bundle:quoteSpace:replace('/', '\\')) @(bundle:quoteSpace) || goto ERROR
# endif
#endif

popd && exit /b 0

:ERROR
popd && exit /b 1
