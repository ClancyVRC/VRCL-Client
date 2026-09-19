@echo off
setlocal
cd /d "%~dp0"

echo.
echo ==========================================
echo        VRCL Installer Build
echo ==========================================
echo.
echo Restoring the official VRCL wolf icon...
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0PREPARE_INSTALLER_BRANDING.ps1"
if errorlevel 1 (
  echo.
  echo ICON PREPARATION FAILED
  pause
  exit /b 1
)

echo.
echo Building VRCL.Installer.exe...
dotnet publish "VRCL Installer.csproj" -c Release -r win-x64 --self-contained true -o "..\VRCL Installer Built"
if errorlevel 1 (
  echo.
  echo ==========================================
  echo BUILD FAILED
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
echo %~dp0..\VRCL Installer Built\VRCL.Installer.exe
echo.
echo The standalone EXE contains the VRCL wolf icon.
echo The installer window uses the same embedded icon,
echo so no separate logo file is required beside the EXE.
echo.
echo Public GitHub releases are signed automatically
echo by the release workflow after Artifact Signing
echo is configured.
echo.
pause
