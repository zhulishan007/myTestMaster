@echo off

MarsReport -E DEV   -T excel -O Path1  -F Path2 -R FLEX -DATA_TEMPLATE Path3 >  Path4


if errorlevel 1 (
   echo Failure Reason Given is %errorlevel%
   
)


if errorlevel 0 (
   echo "Success: " %errorlevel%
   
)