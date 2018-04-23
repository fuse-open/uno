:: @(MSG_ORIGIN)
:: @(MSG_EDIT_WARNING)
@echo off

:: Load Visual Studio 2013 environment
set filename="%VS120COMNTOOLS%\vsvars32.bat"
if not exist %filename% (
    echo FATAL ERROR: Visual Studio 2013 was not found. 1>&2
    exit /b 1
)
if not defined VSINSTALLDIR (
    call %filename%
)

pushd "%~dp0"
cmake -G"Visual Studio 12" . || popd && exit /b 1
msbuild "@(Project.Name).sln" /p:Configuration=@(PInvoke.Configuration) /m
popd && exit /b %ERRORLEVEL%
