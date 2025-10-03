@echo off
echo Starting Monkey Clash - Banana Edition
echo (Now with proper banana resource display!)

:: Try Python paths
if exist "C:\Python311\python.exe" (
  "C:\Python311\python.exe" MonkeyClash_BananaEdition.py
) else (
  echo ERROR: Python 3.11 not found
  echo Please run INSTALL.bat first
)

pause
