@echo off
for /r %~dp0\.. %%x in (*Test.dll) do peverify "%%x" || goto ERROR

exit /b 0

:ERROR
echo. 
echo ERROR: peverify returned non-zero!
exit /b 1
