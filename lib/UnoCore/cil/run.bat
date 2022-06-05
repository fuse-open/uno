:: @(MSG_ORIGIN)
:: @(MSG_EDIT_WARNING)
@echo off

#if @(LIBRARY:Defined)
echo ERROR: @(Product) is a library and cannot be run directly.
exit /b 1
#else
dotnet "%~dp0@(Product:Replace('/', '\\'))" %*
exit /b %ERRORLEVEL%
#endif
