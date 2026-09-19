@echo off
setlocal
cd /d "%~dp0"

echo.
echo ==========================================
echo        VRCL Installer Build
echo ==========================================
echo.
echo Preparing the official VRCL wolf icon...
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0PREPARE_INSTALLER_BRANDING.ps1"
if errorlevel 1 (
  echo.
  echo ICON PREPARATION FAILED
  pause
  exit /b 1
)

set "OUT=%~dp0..\VRCL Installer Built"

echo.
echo Cleaning previous installer build...
if exist "%OUT%" rmdir /s /q "%OUT%"
if exist "%OUT%" (
  echo.
  echo Could not remove the previous build folder.
  echo Close any running VRCL.Installer.exe and try again.
  pause
  exit /b 1
)

echo.
echo Building new VRCL.Installer.exe...
dotnet publish "VRCL Installer.csproj" -c Release -r win-x64 --self-contained true -o "%OUT%"
if errorlevel 1 (
  echo.
  echo ==========================================
  echo BUILD FAILED
  echo ==========================================
  echo.
  pause
  exit /b 1
)

if not exist "%OUT%\VRCL.Installer.exe" (
  echo.
  echo ==========================================
  echo BUILD FAILED - EXE NOT FOUND
  echo ==========================================
  echo.
  pause
  exit /b 1
)

echo.
echo ==========================================
echo BUILD COMPLETE
echo ==========================================
echo.
echo New installer:
echo %OUT%\VRCL.Installer.exe
echo.
echo Features:
echo - Latest VRCL release installation
echo - Manual version selection
echo - Version refresh from GitHub
echo - SHA-256 verification when provided
echo - Versioned ZIP payload detection
echo - Protected Data folder preservation
echo - Program Files\VRCL Client default
echo - Official VRCL wolf branding
echo.
echo The standalone EXE contains the installer branding.
echo.
pause
