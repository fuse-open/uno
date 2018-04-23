:: @(MSG_ORIGIN)
:: @(MSG_EDIT_WARNING)
@echo off

if "%1" == "debug" (
    pushd "%~dp0"
    @(CMake) -G"@(CMake.Generator)" . || popd && exit /b %ERRORLEVEL%
    echo Opening Visual Studio
    @(Uno) open -a"Visual Studio 2015" "@(Project.Name).sln"
    popd && exit /b %ERRORLEVEL%
)

"%~dp0@(Product:Replace('/', '\\'))" %*
exit /b %ERRORLEVEL%
