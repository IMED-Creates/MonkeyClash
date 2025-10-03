@echo off
echo Starting Monkey Clash...

:: Try multiple Python paths
if exist "C:\Python311\python.exe" (
  "C:\Python311\python.exe" monkey_clash.py
) else if exist "%LOCALAPPDATA%\Programs\Python\Python311\python.exe" (
  "%LOCALAPPDATA%\Programs\Python\Python311\python.exe" monkey_clash.py
) else (
  echo ERROR: Python 3.11 not found
  echo Please run INSTALL.bat first
)

pause
