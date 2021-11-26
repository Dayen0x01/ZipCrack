@echo off

title Wifi Panel Finder
for /f "tokens=2,3 delims={,}" %%a in ('"WMIC NICConfig where IPEnabled="True" get DefaultIPGateway /value | find "I" "') do set c=%%~a %%~b

echo IP Address is: %c%
set a=http://
set b=%a%%c%
start %b%

pause