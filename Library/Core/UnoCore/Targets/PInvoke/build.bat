:: @(MSG_ORIGIN)
:: @(MSG_EDIT_WARNING)
@echo off

pushd "%~dp0"
cmake -G"Visual Studio 15 2017" . || popd && exit /b 1
cmake --build . -- "@(Project.Name).sln" /p:Configuration=@(PInvoke.Configuration) /m
popd && exit /b %ERRORLEVEL%
