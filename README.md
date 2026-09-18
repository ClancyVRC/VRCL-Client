# VRCL Client

<p align="center">
  <img src="assets/vrcl-logo.svg" width="110" alt="VRCL Client wolf logo">
</p>

<h1 align="center">VRCL Client</h1>

<p align="center">
  A custom Windows launcher and control client built around VRChat, SteamVR, and Meta PCVR.
</p>

<p align="center">
  <a href="https://github.com/ClancyVRC/VRCL-Client/releases/latest/download/VRCL.Installer.exe">
    <strong>⬇️ Download VRCL Client Installer</strong>
  </a>
</p>

<p align="center">
  <a href="https://github.com/ClancyVRC/VRCL-Client/releases"><img src="https://img.shields.io/github/v/release/ClancyVRC/VRCL-Client?include_prereleases&label=Latest%20Beta" alt="Latest Beta"></a>
  <img src="https://img.shields.io/badge/Windows-x64-2ea44f" alt="Windows x64">
  <img src="https://img.shields.io/badge/.NET-10-512bd4" alt=".NET 10">
  <img src="https://img.shields.io/badge/WPF-Desktop-0078d4" alt="WPF Desktop">
</p>

---

## What is VRCL Client?

**VRCL Client** is a dedicated Windows application for managing and launching a VRChat PCVR setup without turning the process into a pile of separate shortcuts, scripts, and setup steps.

It brings VRChat launching, SteamVR handling, Meta/Quest PCVR options, settings, keybinds, tray controls, themes, and update support together in one place.

The goal is simple: **install it, set it up once, and use VRCL Client as your everyday VRChat launch hub.**

## ✦ Features

### 🎮 VRChat Launching
Launch your VR setup from one application with support for:

- Steam
- SteamVR
- VRChat
- Meta / Quest PCVR workflows
- First-run PC detection and setup

### 🥽 Oculus Startup System
The optional **Oculus Startup System (OSS)** is designed for Meta/Quest PCVR users who want the required Oculus/Meta startup flow handled before SteamVR and VRChat are launched.

OSS is intended to detect relevant Meta/Oculus components locally and provide the appropriate setup guidance when needed.

### ⚙️ Settings & Controls

VRCL Client includes a centralized settings experience with:

- Keybinds
- Tray controls
- Launch preferences
- PC/VR setup detection
- Command output
- Portable application behavior
- Theme and background support

### 🎨 Themes & Visuals

The client supports custom visual themes and backgrounds.

Theme packages are being designed so they can be delivered separately through the update system instead of requiring every future full application package to permanently contain every theme.

**Crystal Shards** is planned as one of the future theme packages.

---

## 🔄 Updates Without Manual Builds

> **You do not need to build VRCL Client yourself to use a released version.**

VRCL Client releases are intended to provide **pre-built Windows files** ready for normal users.

When a new full beta is published on GitHub, the release package can contain the complete pre-built application for that version. The application also contains its own update-checking foundation so it can check the project's GitHub releases for newer beta versions.

The intended update flow is:

**GitHub Release → VRCL Client detects update → updater obtains the release package → application files are replaced → user data remains protected → VRCL Client launches again**

The updater is designed around replacing application files rather than treating the user's personal configuration as disposable.

### 📦 Protected User Data

VRCL Client keeps user data in a separate:

`Data/`

directory.

The planned updater architecture is specifically designed to avoid deleting or overwriting that user data when application files are updated.

---

## 🧰 Installer

The **VRCL Installer** is a separate application from the main VRCL Client.

Its job is to handle installation/update operations using the pre-built files published through the project's GitHub Releases.

The intended installer architecture allows the installer to:

- Obtain the appropriate release files from GitHub
- Install VRCL Client into its application directory
- Replace/update its own installed files when a newer installer is published
- Keep the main application and installer responsibilities separate

**Installer downloads are provided from the GitHub Releases system rather than requiring users to manually build the project.**

---

## 📦 How Releases Work

VRCL Client uses GitHub Releases as the distribution point for finished beta builds.

A full beta release is intended to contain the **pre-built application package** for that version.

Example:

`v1.9.95-beta`

with a package such as:

`VRCL_Client_1.9.95-beta.zip`

Future releases can also contain the standalone installer and other release components as the distribution system is expanded.

### Release channels

| Channel | Purpose |
|---|---|
| **Beta** | Active development builds published for testing and early use |
| **Stable** | Future production-ready releases |

---

## 🐺 Built For Windows

VRCL Client is currently developed for:

- **Windows x64**
- **.NET 10 Windows Desktop**
- **WPF**
- **C#**

The repository contains the project's source code and release/update infrastructure. **End users should use the pre-built installer or release package instead of manually compiling the project.**

---

## 📥 Getting VRCL Client

### Recommended

Use the installer button at the top of this page to obtain the latest available installer.

### Releases

All published builds and release packages are available through the project's GitHub Releases page.

**Each full beta is intended to be a ready-to-use pre-built version.**

---

## 🚧 Development Status

VRCL Client is actively being developed.

Current development includes the core launcher, setup flow, settings, themes, GitHub update checking, and the foundation for the standalone installer/updater system.

The release and update infrastructure is being built so future versions can be distributed cleanly without requiring users to open the source code or compile anything themselves.

---

<p align="center">
  <strong>VRCL Client</strong><br>
  Built for a smoother VRChat PCVR setup.
</p>
