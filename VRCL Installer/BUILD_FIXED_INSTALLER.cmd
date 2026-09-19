@echo off
setlocal
cd /d "%~dp0"

echo.
echo Preparing the VRCL installer logo and Windows icon...
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0PREPARE_INSTALLER_BRANDING.ps1"
if errorlevel 1 (
  echo.
  echo Installer branding preparation failed.
  pause
  exit /b 1
)

echo.
echo Building VRCL.Installer.exe...
dotnet publish "VRCL Installer.csproj" -c Release -r win-x64 --self-contained true -o "..\VRCL Installer Built"
if errorlevel 1 (
  echo.
  echo Build failed.
  pause
  exit /b 1
)

echo.
echo Installer build complete:
echo %~dp0..\VRCL Installer Built\VRCL.Installer.exe
echo.
echo Public releases are signed automatically by GitHub Actions before the installer is uploaded.
echo This local build is for testing unless you sign it separately.
echo.
pause
