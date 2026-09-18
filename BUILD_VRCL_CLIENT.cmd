@echo on
setlocal EnableExtensions
title VRCL Client Build

cd /d "%~dp0"

echo.
echo ============================================================
echo                  VRCL Client Build
echo ============================================================
echo.
echo This uses the normal VRCL dotnet publish process.
echo The CMD window will stay open so build errors are visible.
echo.

echo Checking for dotnet...
where dotnet.exe
if errorlevel 1 (
    echo.
    echo [ERROR] dotnet.exe was not found.
    echo Install the .NET 10 SDK, then run this file again.
    echo.
    pause
    exit /b 1
)

echo.
echo Running:
echo dotnet publish "VRCL Client\VRCL Client.csproj" -c Release -r win-x64 --self-contained false -o "..\VRCL Client"
echo.

dotnet publish "VRCL Client\VRCL Client.csproj" -c Release -r win-x64 --self-contained false -o "..\VRCL Client"
set "BUILD_EXIT=%errorlevel%"

echo.
echo Build exit code: %BUILD_EXIT%
echo.

if not "%BUILD_EXIT%"=="0" (
    echo [ERROR] VRCL Client build failed.
    pause
    exit /b %BUILD_EXIT%
)

if not exist "..\VRCL Client\VRCL Client.exe" (
    echo [ERROR] VRCL Client.exe was not found after publish.
    pause
    exit /b 1
)

echo [OK] VRCL Client.exe was created successfully.
echo Build complete.
pause
exit /b 0