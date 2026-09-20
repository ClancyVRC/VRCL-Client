# VRCL Client

<p align="center">
  <img src="assets/github-banner.svg" width="100%" alt="VRCL Client banner">
</p>

<p align="center">
  A simple launcher for VRChat PCVR.
</p>

<p align="center">
  <a href="https://github.com/ClancyVRC/VRCL-Client/releases/download/v1.0.0-release-beta/VRCL.Installer.exe">
    <img src="https://img.shields.io/badge/%E2%AC%87%20GET%20VRCL%20INSTALLER-5B2DE8?style=for-the-badge" alt="VRCL Installer">
  </a>
  &nbsp;
  <a href="https://dotnet.microsoft.com/en-us/download/dotnet/10.0">
    <img src="https://img.shields.io/badge/%E2%AC%87%20GET%20.NET%20(REQUIRED)-512BD4?style=for-the-badge" alt=".NET 10 (required)">
  </a>
  <br>
  <a href="https://www.virustotal.com/gui/file/a346bf17a0951f168f8ce5ffd63d36f7a99edf61386eddfb49ec1385c560b259?nocache=1">
    <img src="https://img.shields.io/badge/%E2%9F%A9%20SCAN%20AT%20VIRUSTOTAL-00A9E0?style=for-the-badge" alt="Scan VRCL Installer at VirusTotal">
  </a>
  <br>
  <sub>VRCL.Installer.exe · Windows x64</sub>
</p>

<p align="center">
  <strong>Required:</strong> .NET 10 is required to run VRCL Client.
</p>

---

## 🐺 What is VRCL Client?

**VRCL** stands for **"VRChat Launcher"**.

VRCL Client is a Windows app made to make starting and managing VRChat PCVR a little easier.

It puts the things you use most in one place:

- VRChat and SteamVR launching
- Meta / Quest PCVR options
- PC setup detection
- Keybinds and settings
- Tray controls
- Themes and backgrounds
- Built-in update support

---

### 🎮 Features

### VRChat Launching

Launch your VR setup from one app, including:

- Steam
- SteamVR
- VRChat
- Meta / Quest PCVR

### Oculus Startup System

The optional **Oculus Startup System (OSS)** is available for Meta/Quest PCVR setups that need the Oculus/Meta startup process handled before VRChat.

### Settings

VRCL Client includes settings for:

- Keybinds
- Launch options
- Tray controls
- PC/VR setup
- Themes and backgrounds

### Updates

VRCL Client can check GitHub Releases for updates and use the VRCL Updater to update the app.

Your personal `Data/` folder is kept separate from normal application updates.

---

## 🧰 VRCL Installer

The installer is separate from the main VRCL Client.

**Current installer:** `v1.0.0-release-beta`

It:

- Checks GitHub Releases every time it starts
- Selects the newest compatible published VRCL Client release
- Accepts the standard `VRCL_Client_v<version>.zip` package format
- Also accepts the older `VRCL_Client_<version>.zip` format
- Verifies SHA-256 when GitHub provides a digest
- Installs it where you choose
- Preserves existing `Data/` content
- Lets you launch VRCL Client after installation

The installer on the **v1.0.0 GitHub release has been replaced with the current fixed installer build.**

### 🔎 VirusTotal

Public VirusTotal reports for the current published builds:

- [VRCL Installer — VirusTotal](https://www.virustotal.com/gui/file/a346bf17a0951f168f8ce5ffd63d36f7a99edf61386eddfb49ec1385c560b259?nocache=1)
- [VRCL Client v1.0.0 — VirusTotal](https://www.virustotal.com/gui/file/a49316acd16403eab6826a975c71c946626dfd613ab29571357edf4895841b10)

---

## 🎨 Themes

VRCL Client supports custom themes and backgrounds.

More themes can be added separately as the project grows.

---

## 🛠️ Developers

VRCL Client is a **.NET 10 / Windows x64** project.

**.NET 10 is required to run VRCL Client.** For manual builds, use the **Get .NET (required)** button at the top.

---

## 📌 Notes

- Windows only
- Keep the `Data/` folder if you want to keep your settings and personal app data.
- Don't mix files from different releases.
- Use the installer for a new installation.
- Use the built-in updater for supported app updates.

---

## 📄 Project Information

<p align="center">
  <a href="PRIVACY.md">🔒 Privacy Statement</a>
  &nbsp;•&nbsp;
  <a href="LICENSE.md">📜 License</a>
</p>

---

<p align="center">
  <strong>VRCL Client</strong><br>
  Simple VRChat PCVR launching.
</p>
