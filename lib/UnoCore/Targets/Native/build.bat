:: @(MSG_ORIGIN)
:: @(MSG_EDIT_WARNING)
@echo off
pushd "%~dp0"

@(CMake) -G"@(CMake.Generator)" .
if not %ERRORLEVEL% == 0 (popd && exit /b %ERRORLEVEL%)

@(CMake) --build . -- /p:Configuration=@(Native.Configuration) /m
popd && exit /b %ERRORLEVEL%
