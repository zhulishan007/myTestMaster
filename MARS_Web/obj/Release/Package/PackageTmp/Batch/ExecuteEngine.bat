@echo off 

Taskkill /F /IM TestFrameMonitor.exe 
 


 Taskkill /F /IM Mars.AutoTestingDriver32.exe 


  
 start "" "C:\automationTest\Automation Workbooks\bin32_test\TestFrameMonitor" admin 
 
 start "" "C:\automationTest\Automation Workbooks\bin32_test\Mars.AutoTestingDriver32" admin -S ASSET_TEST 149351 -App 2 -Mode Base -Continue True -IgnoreError True 
 
 
pause
