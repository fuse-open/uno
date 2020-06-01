:: @(MSG_ORIGIN)
:: @(MSG_EDIT_WARNING)
@echo off

if "%1" == "debug" (
    where cmake 1>NUL 2>NUL
    if not %ERRORLEVEL% == 0 (
        popd
        echo ERROR: Unable to find the 'cmake' command. Make sure CMake is installed and added to PATH. >&2
        echo. >&2
        echo On Windows, you can install CMake using Chocolatey: >&2
        echo. >&2
        echo     choco install cmake >&2
        echo. >&2
        exit /b 1
    )

    pushd "%~dp0"

    cmake -G"Visual Studio 16 2019" .
    if not %ERRORLEVEL% == 0 (popd && exit /b %ERRORLEVEL%)

    echo Opening Visual Studio 2019
    @(uno) open -a"Visual Studio 2019" -t"@(Project.Name) - Microsoft Visual Studio" "@(Project.Name).sln"
    popd && exit /b %ERRORLEVEL%
)

#if @(LIBRARY:Defined)
echo ERROR: @(Product) is a library and cannot be run directly.
exit /b 1
#else
"%~dp0@(Product:Replace('/', '\\'))" %*
exit /b %ERRORLEVEL%
#endif
