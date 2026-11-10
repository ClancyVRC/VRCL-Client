# VRCL Client

<p align="center">
  <img src=".github/branding/github-banner.svg" width="100%" alt="VRCL Client — manage your VRChat sessions">
</p>

<p align="center">
  <strong>Take control of your VRChat sessions.</strong><br>
  Manage VR and Desktop sessions from one place. Launch VRChat, start SteamVR when needed, monitor your session, and quickly restart or close VRChat whenever you need.
</p>

<p align="center">
  <a href="https://github.com/ClancyVRC/VRCL-Client/releases/download/v1.0.0-release-beta/VRCL.Installer.exe">
    <img src="https://img.shields.io/badge/%E2%AC%87%20DOWNLOAD%20VRCL%20INSTALLER-5B2DE8?style=for-the-badge" alt="Download VRCL Installer">
  </a>
  <br>
  <a href="https://dotnet.microsoft.com/en-us/download">
    <img src="https://img.shields.io/badge/%E2%AC%87%20GET%20.NET%2010-512BD4?style=for-the-badge" alt="Get .NET 10">
  </a>
  <br>
  <sub>Windows x64 · .NET 10 · VRChat session management</sub>
</p>

<p align="center">
  <strong>VRCL Installer is self-contained.</strong> VRCL Client requires the .NET 10 Desktop Runtime.
</p>

---

<div align="center">

## 🛠️ Patch Notes & Update Information

<sub>INSTALLER &amp; UPDATER NOTICE · NOVEMBER 10, 2026</sub>

</div>

> [!IMPORTANT]
> The VRCL Installer and update system have been updated to improve release detection, installation, and version compatibility.

### VRCL Installer
- Fixed release detection so the installer can find and offer the latest compatible published VRCL Client version.
- Fixed the installation flow for installing the latest available release.
- If you have an older copy of `VRCL.Installer.exe`, we recommend deleting that copy from your PC and downloading the current installer using the **Download VRCL Installer** button above.

### Update compatibility
- **v1.0.0-release-beta through v1.0.2-release-beta:** These older builds use a legacy update system and currently update to the latest beta release, rather than moving directly to full releases.
- **v1.0.3-release-beta and newer:** The updated updater supports moving to any compatible published beta or full release.

<sub>Tested successfully on the developer's system. Additional testing on another PC is still pending.</sub>

---

## What is VRCL Client?

**VRCL** stands for **VRChat Launcher**.

VRCL Client is a Windows companion built around one main idea: **make managing your VRChat sessions easier.** Instead of juggling separate shortcuts, startup steps, and settings, VRCL brings your VR and Desktop launch controls and everyday VRChat tools together in one place.

## Main features

### 🎮 VR and Desktop session controls

Manage both ways of playing VRChat without mixing their startup behavior.

- **Start VRChat (VR):** Launches SteamVR, allows time for it to start, and then launches VRChat in VR mode.
- **Start VRChat (Desktop):** Opens VRChat through Steam in Desktop mode without unnecessarily starting SteamVR.
- **Restart sessions:** Restart your VR or Desktop session from VRCL.
- **Close sessions:** Close the relevant VRChat session using the dedicated controls.
- **Tray controls:** Access common session actions from the Windows system tray.
- **Keybinds:** Set keyboard shortcuts for the session actions you use most.

### 🥽 Meta / Quest PCVR support

VRCL includes options to help handle PCVR startup for supported Meta / Quest setups.

- **Oculus Startup System (OSS):** Optional startup sequence for Meta / Quest PCVR.
- **PCVR detection:** Detects relevant VR software and can identify Meta / Quest PCVR setups.
- **ASW controls:** Built-in control for Oculus Asynchronous Spacewarp on supported setups.
- **Launch options:** Keep the VR startup sequence and related options together in Settings.

### 🧰 Setup and PC tools

Keep common setup and maintenance tasks close to your session controls.

- First-run setup and PC detection.
- VRChat and SteamVR setup checks.
- Built-in VRChat cache clearing.
- Quick access to common session and application actions.
- Portable operation and application shortcuts.

### 💬 Status and Discord Activity

Keep track of what VRCL is doing and optionally show activity through Discord.

- Built-in Discord Activity Status / Rich Presence.
- App and session activity information.
- Discord activity can be managed through VRCL's available settings.
- Status features can reflect whether VRCL is launching or managing a session.

### 🎨 Themes and personalization

Make VRCL feel like your setup.

- Light, Dark, and Default themes.
- Custom colors and backgrounds.
- Additional appearance options and experimental customization.
- Animated color effects, including Coat of Color.
- Settings for tailoring the app's look and behavior.

### 🔄 Updates and data protection

VRCL includes its own update flow so supported updates are easier to install.

- Check for compatible VRCL Client releases.
- Use the bundled VRCL Updater for supported updates.
- Preserve your existing `Data/` folder during supported updates.
- Keep your settings and personal app data when moving to a newer version.

---

## Install VRCL Client

The latest public release currently available is **v1.0.2-release-beta**.

1. Download **[VRCL.Installer.exe](https://github.com/ClancyVRC/VRCL-Client/releases/download/v1.0.0-release-beta/VRCL.Installer.exe)** using the installer button above.
2. Run the installer and choose where you want VRCL Client installed.
3. Let the installer find and install a compatible published version.
4. Open VRCL Client and start managing your VRChat session.

The standalone installer:

- Checks the official GitHub Releases for compatible VRCL Client packages.
- Finds the newest compatible published version.
- Supports the versioned VRCL ZIP package formats.
- Verifies SHA-256 when GitHub provides a digest.
- Installs to the location you choose.
- Preserves supported existing `Data/` content.

[Browse all VRCL Client releases](https://github.com/ClancyVRC/VRCL-Client/releases)

> VRCL Client is still in beta. Features and behavior may change as the project continues to develop. Only versions published on the Releases page are public downloads.

---

## Your VRCL data

Your personal VRCL settings and application data are stored in the `Data/` folder.

**Keep your Data folder when updating or moving VRCL Client** if you want to retain your existing settings and personal app data. Supported installer and updater operations are designed to preserve it.

For details about local files, GitHub update checks, and optional Discord Activity, see the [Privacy Statement](PRIVACY.md).

---

## Project information

<p align="center">
  <a href="PRIVACY.md">🔒 Privacy Statement</a>
  &nbsp;•&nbsp;
  <a href="LICENSE.md">📜 License</a>
  &nbsp;•&nbsp;
  <a href="https://github.com/ClancyVRC/VRCL-Client/releases">📦 Releases</a>
</p>

---

## 🛡️ Security Scan & Download Reputation

VRCL Client and its installer are relatively new, so Windows may display a Microsoft Defender SmartScreen message stating that the installer is **not commonly downloaded** or that its publisher is unknown. This is a reputation warning and does not, by itself, mean the file has been identified as malware.

A submission for `VRCL.Installer.exe` has been successfully received by Microsoft for review. The review is pending; this does not mean Microsoft has approved or certified the installer.

The submitted installer was also scanned with VirusTotal. **The shared report showed no antivirus detections at the time it was checked.** You can view the report and its current results here:

<p align="center">
  <a href="https://www.virustotal.com/gui/file/26d59c00e35af47f6ba34929be1d665c6a1f08f6533c33772c69c340d833eabf">
    <img src="https://img.shields.io/badge/VIEW%20VIRUSTOTAL%20SCAN-394EFF?style=for-the-badge" alt="View VRCL Installer VirusTotal report">
  </a>
</p>

> VirusTotal results can change as security vendors update their detections. A clean scan is reassuring, but it is not an absolute guarantee that a file is harmless. Always download VRCL Installer from this official repository.

---

<p align="center">
  <strong>VRCL Client</strong><br>
  <em>Manage your VRChat sessions.</em>
</p>
