@echo off

MarsReport -E DEV   -T excel -O D:\Shraddha\Shraddha\MARS_WEB_PROJECT\Project\MARS_Web\MARSReport\MarsAutomation\ReportOutput\Reports  -F ADT_11 -R FLEX -DATA_TEMPLATE ADT_11_20211216_172256.xlsx >  D:\Shraddha\Shraddha\MARS_WEB_PROJECT\Project\MARS_Web\MARSReport\MarsAutomation\ReportOutput\Logs\ADT_11.log


if errorlevel 1 (
   echo Failure Reason Given is %errorlevel%
   
)


if errorlevel 0 (
   echo "Success: " %errorlevel%
   
)