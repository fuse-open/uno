:: @(MSG_ORIGIN)
:: @(MSG_EDIT_WARNING)
@echo off
pushd "%~dp0"

if "%1" == "debug" (
#if @(JDK.Directory:IsSet)
    set JAVA_HOME=@(JDK.Directory:NativePath)
#endif
    echo Opening Android Studio
    @(uno) open -a"Android Studio" -t"@(Project.Name)" "%~dp0"
    exit /b %ERRORLEVEL%
)

#if @(LIBRARY:Defined)
echo ERROR: @(Product) is a library and cannot be run directly.
exit /b 1
#else
if "%1" == "uninstall" (
    echo Uninstalling @(Activity.Package)
    @(uno) adb uninstall @(Activity.Package)
    exit /b %ERRORLEVEL%
)

@(uno) launch-apk "%~dp0@(Product)" ^
    --package=@(Activity.Package) ^
    --activity=@(Activity.Name) ^
    --sym-dir="%~dp0src\main\.uno" ^
    %*
exit /b %ERRORLEVEL%
#endif
