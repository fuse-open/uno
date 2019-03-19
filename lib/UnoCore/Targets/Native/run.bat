:: @(MSG_ORIGIN)
:: @(MSG_EDIT_WARNING)
@echo off

if "%1" == "debug" (
    pushd "%~dp0"

    @(CMake) -G"@(CMake.Generator)" .
    if not %ERRORLEVEL% == 0 (popd && exit /b %ERRORLEVEL%)

    echo Opening Visual Studio 2017
    @(Uno) open -a"Visual Studio 2017" -t"@(Project.Name) - Microsoft Visual Studio" "@(Project.Name).sln"
    popd && exit /b %ERRORLEVEL%
)

"%~dp0@(Product:Replace('/', '\\'))" %*
exit /b %ERRORLEVEL%
