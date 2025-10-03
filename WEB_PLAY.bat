@echo off
echo Preparing Monkey Clash for browser play...

:: Install pygbag if missing
if exist "C:\Python311\python.exe" (
  "C:\Python311\python.exe" -m pip install pygbag --quiet
) else (
  python -m pip install pygbag --quiet
)

:: Build and serve the game
echo Starting web server...
echo Game will be available at: http://localhost:8000
if exist "C:\Python311\python.exe" (
  "C:\Python311\python.exe" -m pygbag main.py
) else (
  python -m pygbag main.py
)

pause
