@echo off
:: Note to BASH users:
:: Non-cmd.exe users should use 'make unocore' or 'bash build.sh --unocore' instead.
:: This Windows port is only for the weak.
pushd "%~dp0.."
set SRC_DIR=Library\Core\UnoCore
set OUT_DIR=%SRC_DIR%\build\corelib\Debug
set DST_DIR=src\runtime\Uno.Runtime.Core
set MSBUILD=%PROGRAMFILES(X86)%\MSBuild\14.0\bin\MSBuild
set UNO=prebuilt\uno
set STUFF=prebuilt\stuff

:: Build uno.exe
%STUFF% install prebuilt || goto ERROR
"%MSBUILD%" /m /p:Configuration=Debug uno-win32.sln || goto ERROR

:: Build Uno project
%UNO% build corelib %SRC_DIR% || goto ERROR

:: Build VS project
"%MSBUILD%" /m %OUT_DIR%\Uno.Runtime.Core.sln || goto ERROR

:: Replace VS project
rd /s /q %OUT_DIR%\bin
rd /s /q %OUT_DIR%\obj
del /f /s /q %DST_DIR%\*
xcopy /y /s /r /h %OUT_DIR%\* %DST_DIR%\* || goto ERROR
del /f /s /q %DST_DIR%\*.sln

:: Done
popd && exit /b 0

:ERROR
pause
popd && exit /b 1
