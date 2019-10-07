:: @(MSG_ORIGIN)
:: @(MSG_EDIT_WARNING)
@echo off
pushd "%~dp0"

cmake -G"@(CMake.Generator)" .
if not %ERRORLEVEL% == 0 (popd && exit /b %ERRORLEVEL%)

cmake --build . -- /p:Configuration=@(Native.Configuration) /m
popd && exit /b %ERRORLEVEL%
