:: @(MSG_ORIGIN)
:: @(MSG_EDIT_WARNING)
@echo off

#if @(LIBRARY:defined) || @(PREVIEW:defined)
echo ERROR: @(product) is a library and cannot be run directly.
exit /b 1
#else
dotnet "%~dp0@(product:replace('/', '\\'))" %*
exit /b %ERRORLEVEL%
#endif
