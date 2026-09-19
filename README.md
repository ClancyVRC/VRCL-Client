# VRCL Client

<p align="center">
  <img src="assets/vrcl-app-icon.webp" width="110" alt="VRCL Client wolf logo">
</p>

<h1 align="center">VRCL Client</h1>

<p align="center">
  A simple launcher for VRChat PCVR.
</p>

<p align="center">
  <a href="https://github.com/ClancyVRC/VRCL-Client/releases/download/v1.0.0-release-beta/VRCL.Installer.exe">
    <img src="https://img.shields.io/badge/%E2%86%93%20DOWNLOAD%20VRCL%20INSTALLER-6C3CE9?style=for-the-badge" alt="Download VRCL Installer">
  </a>
  <br>
  <sub>VRCL.Installer.exe · Windows x64</sub>
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

## 🚀 Getting Started

### What you need

- 64-bit Windows
- VRChat
- SteamVR and/or Meta/Quest PCVR
- An internet connection for installing and updating

You **do not** need Visual Studio, the .NET SDK, or any developer tools to use VRCL Client.

### Install

1. Download **VRCL.Installer.exe** using the button above.
2. Run the installer.
3. Pick where you want VRCL Client installed.
4. Let the installer download the current release.
5. Launch **VRCL Client.exe**.

On first launch, VRCL Client can help find the relevant VRChat, SteamVR, Steam, and Meta/Quest locations on your PC.

---

## 🎮 Features

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

---

## 📦 Current Release

### VRCL Client v1.0.0-release-beta

**Beta release**

- [Release page](https://github.com/ClancyVRC/VRCL-Client/releases/tag/v1.0.0-release-beta)
- [Download Installer](https://github.com/ClancyVRC/VRCL-Client/releases/download/v1.0.0-release-beta/VRCL.Installer.exe)
- [Download Client ZIP](https://github.com/ClancyVRC/VRCL-Client/releases/download/v1.0.0-release-beta/VRCL_Client_v1.0.0-release-beta.zip)

The normal release package format is:

`VRCL_Client_v<version>.zip`

The standalone installer is:

`VRCL.Installer.exe`

---

## 🎨 Themes

VRCL Client supports custom themes and backgrounds.

More themes can be added separately as the project grows.

---

## 🛠️ Developers

VRCL Client is a **.NET 10 / Windows x64** project.

If you just want to use VRCL Client, download the installer above. You do not need to build it yourself.

---

## 📌 Notes

- Windows only
- Keep the `Data/` folder if you want to keep your settings and personal app data.
- Don't mix files from different releases.
- Use the installer for a new installation.
- Use the built-in updater for supported app updates.

---

<p align="center">
  <strong>VRCL Client</strong><br>
  Simple VRChat PCVR launching.
</p>
