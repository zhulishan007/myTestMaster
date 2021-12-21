@echo off

MarsReport -E DEV   -T excel -O D:\Shraddha\Shraddha\MARS_WEB_PROJECT\Project\MARS_Web\MARSReport\MarsAutomation\ReportOutput\Reports  -F LAM_68 -R FLEX -DATA_TEMPLATE LAM_681.xlsx >  D:\Shraddha\Shraddha\MARS_WEB_PROJECT\Project\MARS_Web\MARSReport\MarsAutomation\ReportOutput\Logs\LAM_68.log


if errorlevel 1 (
   echo Failure Reason Given is %errorlevel%
   
)


if errorlevel 0 (
   echo "Success: " %errorlevel%
   
) 
