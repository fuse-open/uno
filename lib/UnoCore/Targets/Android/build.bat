:: @(MSG_ORIGIN)
:: @(MSG_EDIT_WARNING)
@echo off
pushd "%~dp0"

#if @(JDK.Directory:IsSet)
set JAVA_HOME=@(JDK.Directory:NativePath)
#endif

call gradlew @(Gradle.Task) %* || goto ERROR
copy /Y @(APK.BuildName:QuoteSpace:Replace('/', '\\')) @(Product:QuoteSpace) || goto ERROR
popd && exit /b 0

:ERROR
popd && exit /b 1
