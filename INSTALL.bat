@echo off
echo Installing Monkey Clash...

:: Download Python 3.11 (if missing)
if not exist "C:\Python311\python.exe" (
  echo Downloading Python 3.11...
  curl -L -o python-installer.exe https://www.python.org/ftp/python/3.11.9/python-3.11.9-amd64.exe
  echo Installing Python...
  start /wait python-installer.exe /quiet InstallAllUsers=1 PrependPath=1
  del python-installer.exe
)

:: Install requirements
"C:\Python311\python.exe" -m pip install pygame==2.5.2 numpy==1.24.3

echo Installation complete! Press any key to launch the game...
pause
"C:\Python311\python.exe" monkey_clash.py
