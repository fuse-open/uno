:: @(MSG_ORIGIN)
:: @(MSG_EDIT_WARNING)
@echo off
pushd "%~dp0"

if "%1" == "debug" (
#if @(jdk.directory:isSet)
    set JAVA_HOME=@(jdk.directory:nativePath)
#endif
    echo Opening Android Studio
    @(uno) open -a"Android Studio" -t"@(project.name)" "%~dp0"
    exit /b %ERRORLEVEL%
)

#if @(LIBRARY:defined)
echo ERROR: @(product) is a library and cannot be run directly.
exit /b 1
#else
if "%1" == "uninstall" (
    echo Uninstalling @(activity.package)
    @(uno) adb uninstall @(activity.package)
    exit /b %ERRORLEVEL%
)

@(uno) launch-apk "%~dp0@(product)" ^
    --package=@(activity.package) ^
    --activity=@(activity.name) ^
    --sym-dir="%~dp0src\main\.uno" ^
    @(ANDROID_EMU:defined:test('--emulator', '')) ^
    %*
exit /b %ERRORLEVEL%
#endif
