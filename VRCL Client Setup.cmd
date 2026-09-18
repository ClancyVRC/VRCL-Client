@echo on
setlocal EnableExtensions
title VRCL Client Setup

cd /d "%~dp0"

echo.
echo ============================================================
echo                  VRCL Client Setup
echo ============================================================
echo.

where dotnet.exe
if errorlevel 1 goto RuntimeMissing

dotnet.exe --list-runtimes 2>nul | findstr /r /c:"Microsoft.WindowsDesktop.App 10\." >nul
if errorlevel 1 goto RuntimeMissing

goto LaunchVRCL

:RuntimeMissing
echo.
echo .NET 10 Windows Desktop Runtime was not found.
echo Install it with Windows Package Manager (winget), then retry.
echo.

where winget.exe
if errorlevel 1 (
    echo [ERROR] winget.exe was not found.
    pause
    exit /b 1
)

winget.exe install --id Microsoft.DotNet.DesktopRuntime.10 --exact --source winget --accept-package-agreements --accept-source-agreements
if errorlevel 1 (
    echo [ERROR] Runtime installation failed.
    pause
    exit /b 1
)

dotnet.exe --list-runtimes 2>nul | findstr /r /c:"Microsoft.WindowsDesktop.App 10\." >nul
if errorlevel 1 (
    echo [ERROR] Microsoft.WindowsDesktop.App 10.x was not detected.
    pause
    exit /b 1
)

:LaunchVRCL
if not exist "%~dp0..\VRCL Client\VRCL Client.exe" (
    echo [ERROR] VRCL Client.exe was not found.
    echo Build VRCL first with BUILD_VRCL_CLIENT.cmd.
    pause
    exit /b 1
)

"%~dp0..\VRCL Client\VRCL Client.exe"
set "VRCL_EXIT=%errorlevel%"
pause
exit /b %VRCL_EXIT%