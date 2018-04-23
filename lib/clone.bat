@echo off
::SETLOCAL EnableDelayedExpansion

echo "WARNING: This script is deprecated, see https://github.com/fusetools/uno#standard-library-dev"

SET FUSETOOLS=https://github.com/fusetools
IF "%1"=="--ssh" (
    SET FUSETOOLS=git@github.com:fusetools
)

ECHO %FUSETOOLS%
ECHO %REPOS%

call git clone --recursive %FUSETOOLS%%/fuselibs-public fuselibs
IF %ERRORLEVEL% NEQ 0 (
	EXIT /B %ERRORLEVEL%
	GOTO:eos
)

call git clone --recursive %FUSETOOLS%%/premiumlibs premiumlibs
IF %ERRORLEVEL% NEQ 0 (
	EXIT /B %ERRORLEVEL%
	GOTO:eos
)

:eos
ENDLOCAL
