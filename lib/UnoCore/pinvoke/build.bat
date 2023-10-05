:: @(MSG_ORIGIN)
:: @(MSG_EDIT_WARNING)
@echo off

pushd "%~dp0"
cmake -G"Visual Studio 15 2017" . || popd && exit /b 1
cmake --build . -- "@(project.name).sln" /p:Configuration=@(pinvoke.configuration) /m
popd && exit /b %ERRORLEVEL%
