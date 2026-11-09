# VRCL Client

<p align="center">
  <img src=".github/branding/github-banner.svg" width="100%" alt="VRCL Client — manage your VRChat sessions">
</p>

<p align="center">
  <strong>Manage your VRChat sessions from one place.</strong><br>
  Launch, monitor, restart, and close VRChat in VR or Desktop without juggling separate shortcuts.
</p>

<p align="center">
  <a href="https://github.com/ClancyVRC/VRCL-Client/releases/download/v1.0.2-release-beta/VRCL.Installer.exe">
    <img src="https://img.shields.io/badge/%E2%AC%87%20DOWNLOAD%20VRCL%20INSTALLER-5B2DE8?style=for-the-badge" alt="Download VRCL Installer">
  </a>
  <br>
  <a href="https://dotnet.microsoft.com/en-us/download">
    <img src="https://img.shields.io/badge/%E2%AC%87%20GET%20.NET%2010-512BD4?style=for-the-badge" alt="Get .NET 10">
  </a>
  <br>
  <sub>Windows x64 · .NET 10 / WPF · Source-available</sub>
</p>

<p align="center">
  <strong>Packaged releases are self-contained.</strong> The .NET 10 SDK is needed when building from source.
</p>

---

## Manage your VRChat sessions

**VRCL** stands for **VRChat Launcher**. VRCL Client has grown beyond a basic launcher: it brings your VRChat session controls, PCVR startup options, and everyday management tools together in one Windows app.

### Session controls

- Start VRChat in **VR** with the SteamVR startup sequence.
- Start VRChat in **Desktop** without unnecessarily starting SteamVR.
- Restart or close VR and Desktop sessions independently.
- Use keybinds and tray actions for common session controls.

### PCVR setup and control

- Detect relevant PC and VR software.
- Support Meta / Quest PCVR startup through the optional **Oculus Startup System (OSS)**.
- Configure available VR and launch options from Settings.
- Use built-in cache and setup tools without having to hunt through folders.

### Status and Discord

- Show what VRCL is currently doing, including launch activity and the active app or VRCL view.
- Use Discord Activity Status / Rich Presence to reflect VRCL activity when enabled.
- Control whether VRCL activity is displayed through the app's available settings.

### Personalization and updates

- Customize themes, colors, and backgrounds.
- Use the built-in updater to check for compatible published releases.
- Keep personal settings and app data in the separate `Data/` folder.

---

## Download and install

The newest public release currently listed on GitHub is **v1.0.2-release-beta**.

1. Download **[VRCL.Installer.exe](https://github.com/ClancyVRC/VRCL-Client/releases/download/v1.0.2-release-beta/VRCL.Installer.exe)** from the published release.
2. Run the installer and choose where VRCL Client should be installed.
3. Launch VRCL Client and use its session controls to start VRChat in VR or Desktop.

The installer checks GitHub Releases for compatible published packages, verifies SHA-256 when GitHub provides a digest, and preserves supported existing `Data/` content during installation or updates.

[View the v1.0.2 public beta release and its files](https://github.com/ClancyVRC/VRCL-Client/releases/tag/v1.0.2-release-beta)

> Development builds newer than the latest published release are not public downloads until they are published as a GitHub Release.

---

## Your data

VRCL keeps personal application data in its `Data/` folder. Keep this folder when moving or updating an installation if you want to retain your settings and other VRCL data.

- Do not mix files from different releases.
- Use the installer for a new installation.
- Use the built-in updater for supported updates.
- Back up your `Data/` folder before making manual changes.

See the [Privacy Statement](PRIVACY.md) for more information about local data and network connections.

---

## Developers

VRCL Client is a **Windows x64 WPF application built with .NET 10**.

To build a release from source on Windows:

1. Install the .NET 10 SDK using the **Get .NET 10** link above.
2. Open Command Prompt in the project directory.
3. Run `BUILD_RELEASE.cmd`.

The repository includes the client, updater, installer project files, build scripts, and supporting assets.

---

## Project information

<p align="center">
  <a href="PRIVACY.md">🔒 Privacy Statement</a>
  &nbsp;•&nbsp;
  <a href="LICENSE.md">📜 Source-Available License</a>
  &nbsp;•&nbsp;
  <a href="https://github.com/ClancyVRC/VRCL-Client/releases">📦 Releases</a>
</p>

---

<p align="center">
  <strong>VRCL Client</strong><br>
  Manage your VRChat sessions.
</p>
