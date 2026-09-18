# VRCL Client

VRCL Client is a custom C# WPF VRChat launcher and control client for Windows.

## Current development line

**Version:** v1.9.95-beta  
**Target:** Windows x64  
**Framework:** .NET 10 Windows Desktop  
**UI:** WPF  
**Runtime model:** framework-dependent, optimized win-x64 publish

## What is included

The current 1.9.x development line includes:

- First-run setup and PC detection for Steam, SteamVR, VRChat, and relevant Meta/Oculus locations.
- VR launch flow for SteamVR and VRChat.
- Optional Oculus Startup System (OSS) flow for Meta/Quest PCVR.
- Optional ASW disable/enforcement support.
- Settings, keybinds, tray controls, and portable operation.
- Theme/background support.
- Centralized command output in Settings.
- GitHub beta-release update checking.
- Separate application and user-data path handling so the future updater can preserve user data.

The project is still under active development. Installer/updater automation and additional release infrastructure are being developed separately from the protected application data.

## Repository layout

```text
VRCL-Client/
├── VRCL Client/
│   ├── Update/
│   │   ├── GitHubUpdateChecker.cs
│   │   ├── github_release_config.template.json
│   │   └── release_structure.json
│   ├── Themes/
│   ├── *.xaml
│   ├── *.xaml.cs
│   ├── VrclPaths.cs
│   ├── VrclVersion.cs
│   └── VRCL Client.csproj
├── Directory.Build.props
├── Directory.Build.targets
├── BUILD_VRCL_CLIENT.cmd
└── VRCL Client Setup.cmd
```

## Building

A Windows machine with the .NET 10 SDK is required.

From the repository root:

```bat
BUILD_VRCL_CLIENT.cmd
```

The normal publish command is:

```bat
dotnet publish "VRCL Client\VRCL Client.csproj" -c Release -r win-x64 --self-contained false -o "..\VRCL Client"
```

The project is intended to be built on Windows. A successful repository-side edit is not considered a native Windows build verification.

## Release/update structure

Beta releases use tags in the form:

```text
v1.9.95-beta
```

The client package asset uses:

```text
VRCL_Client_1.9.95-beta.zip
```

The updater configuration lives under `VRCL Client/Update/`.

The current GitHub update checker reads published, non-draft beta releases and compares their version tags against the running client version.

## User data

VRCL keeps user data under a separate `Data/` directory. The planned updater is intended to replace application files without deleting or overwriting that user data.

## Development notes

- Make focused changes one at a time.
- Preserve the existing 1.9.x UI/theme behavior unless a change is specifically requested.
- Do not treat the protected application baseline as disposable.
- Do not claim a Windows/native WPF build is compile-verified unless it was actually built on Windows.

## Status

This repository is the public development home for VRCL Client. Release automation, the standalone VRCL Updater, installer, and future uninstaller are being developed incrementally.
