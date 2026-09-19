@echo off
setlocal
cd /d "%~dp0"

echo.
echo ==========================================
echo        VRCL Installer Build
echo ==========================================
echo.
echo Building VRCL.Installer.exe with the
echo official VRCL Client wolf icon...
echo.

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
echo The EXE contains:
echo  - VRCL wolf application icon
echo  - VRCL logo inside the installer
echo  - self-contained .NET runtime
echo.
echo Public GitHub releases are signed automatically
echo by the release workflow after Azure Artifact
echo Signing is configured.
echo.
pause
