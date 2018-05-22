:: @(MSG_ORIGIN)
:: @(MSG_EDIT_WARNING)
@echo off
pushd "%~dp0"

if "%1" == "debug" (
    echo Opening Android Studio
    call gradlew --recompile-scripts
    @(Uno) open -a"Android Studio" -t"@(Project.Name)" "%~dp0app/app.iml"
    exit /b %ERRORLEVEL%
)

if "%1" == "uninstall" (
    echo Uninstalling @(Activity.Package)
    @(Uno) adb uninstall @(Activity.Package)
    exit /b %ERRORLEVEL%
)

@(Uno) launch-apk "%~dp0@(Product)" ^
    --package=@(Activity.Package) ^
    --activity=@(Activity.Name) ^
    --sym-dir="%~dp0src\main\.uno" ^
    %*
exit /b %ERRORLEVEL%
