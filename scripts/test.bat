@echo off

%~dp0\..\bin\uno test %~dp0\..\lib
%~dp0\..\bin\uno test-gen %~dp0\..\lib %TEMP%\PackageCompilationTest
%~dp0\..\bin\uno build --target=dotnetexe --no-strip --clean %TEMP%\PackageCompilationTest
