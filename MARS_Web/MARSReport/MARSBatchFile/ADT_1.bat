@echo off

MarsReport -E DEV   -T excel -O D:\Shraddha\Shraddha\MARS_WEB_PROJECT\Project\MARS_Web\MARSReport\MarsAutomation\ReportOutput\Reports  -F ADT_1 -R FLEX -DATA_TEMPLATE ADT_1_20211220_181018.xlsx >  D:\Shraddha\Shraddha\MARS_WEB_PROJECT\Project\MARS_Web\MARSReport\MarsAutomation\ReportOutput\Logs\ADT_1.log


if errorlevel 1 (
   echo Failure Reason Given is %errorlevel%
   
)


if errorlevel 0 (
   echo "Success: " %errorlevel%
   
)