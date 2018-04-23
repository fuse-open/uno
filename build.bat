@echo off
:: Please see Makefile for more build options. They also work on Windows. Usually.
pushd "%~dp0"
set MSBUILD=%PROGRAMFILES(X86)%\MSBuild\14.0\bin\MSBuild
set UNO=prebuilt\uno

echo.
echo WARNING: This script is deprecated, please run 'make' or 'bash scripts/build.sh' instead
echo.

:: Do required things || goto ERROR
"%MSBUILD%" /m uno-win32.sln || goto MSBUILD_ERROR
%UNO% doctor -e || goto ERROR
popd && exit /b 0

:MSBUILD_ERROR
echo Note: If a command was not found, please make sure you have Microsoft Build Tools 2015 installed (C# 6/.NET 4.5 support)

:ERROR
echo BUILD FAILED!
popd && pause && exit /b 1
