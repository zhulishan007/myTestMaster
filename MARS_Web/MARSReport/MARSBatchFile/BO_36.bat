@echo off

MarsReport -E DEV   -T excel -O D:\Shraddha\Shraddha\MARS_WEB_PROJECT\Project\MARS_Web\MARSReport\MarsAutomation\ReportOutput\Reports  -F BO_36 -R FLEX -DATA_TEMPLATE BO_36_20211220_183644.xlsx >  D:\Shraddha\Shraddha\MARS_WEB_PROJECT\Project\MARS_Web\MARSReport\MarsAutomation\ReportOutput\Logs\BO_36.log


if errorlevel 1 (
   echo Failure Reason Given is %errorlevel%
   
)


if errorlevel 0 (
   echo "Success: " %errorlevel%
   
)