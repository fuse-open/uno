:: @(MSG_ORIGIN)
:: @(MSG_EDIT_WARNING)
@echo off

where cmake 1>NUL 2>NUL
if not %ERRORLEVEL% == 0 (
    echo ERROR: Unable to find the 'cmake' command. Make sure CMake is installed and added to PATH. >&2
    echo. >&2
    echo On Windows, you can install CMake using Chocolatey: >&2
    echo. >&2
    echo     choco install cmake >&2
    echo. >&2
    exit /b 1
)

pushd "%~dp0"

cmake -G"Visual Studio 17 2022" .
if not %ERRORLEVEL% == 0 (popd && exit /b %ERRORLEVEL%)

cmake --build . -- /p:Configuration=@(native.configuration) /m
popd && exit /b %ERRORLEVEL%
