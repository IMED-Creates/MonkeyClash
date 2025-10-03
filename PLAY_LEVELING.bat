@echo off
echo Starting Monkey Clash - Banana Edition...
echo (Use bananas to deploy and level up your monkeys!)

:: Try multiple Python paths
if exist "C:\Python311\python.exe" (
  "C:\Python311\python.exe" MonkeyClash_Leveling.py
) else (
  echo ERROR: Python 3.11 not found
  echo Please run INSTALL.bat first
)

pause
