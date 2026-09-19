@echo off
setlocal
cd /d "%~dp0"
dotnet publish "VRCL Installer.csproj" -c Release -r win-x64 --self-contained true -o "..\VRCL Installer Built"
if errorlevel 1 (
  echo Build failed.
  pause
  exit /b 1
)
echo.
echo Fixed installer built at:
echo %~dp0..\VRCL Installer Built\VRCL.Installer.exe
pause
