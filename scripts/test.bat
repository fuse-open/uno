@echo off

%~dp0\..\prebuilt\uno test %~dp0\..\Library
%~dp0\..\prebuilt\uno test-gen %~dp0\..\Library\Core %TEMP%\PackageCompilationTest
%~dp0\..\prebuilt\uno build --target=dotnetexe --no-strip --clean %TEMP%\PackageCompilationTest
