# VRCL Client

<p align="center">
  <img src="assets/vrcl-app-icon.webp" width="110" alt="VRCL Client wolf logo">
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
  <strong>Current installer build: v1.0.0-release-beta</strong>
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

### Current installer: v1.0.0-release-beta

The installer checks the project's published GitHub Releases **every time it starts**. It does not rely on an old bundled VRCL Client version.

The installer is intended to:

- Find the newest published release with a matching `VRCL_Client_<version>.zip`
- Download the matching release package
- Verify its SHA-256 digest when GitHub provides one
- Ask where the VRCL Client main-files folder should be installed
- Install the selected release
- Preserve the user's `Data/` directory
- Allow the user to launch VRCL Client after installation

Use **Download VRCL Client Installer** at the top of this page to obtain the installer once its release asset has been published.

---

## 📦 How Releases Work

VRCL Client uses GitHub Releases as the distribution point for finished beta builds.

A full beta release is intended to contain the **pre-built application package** for that version.

Example:

`v1.0.0-release-beta`

with a package such as:

`VRCL_Client_1.0.0-release-beta.zip`

The standalone installer is distributed as `VRCL.Installer.exe`. The installer then retrieves the matching/latest client package from GitHub Releases instead of shipping a permanently bundled client copy.

### Release channels

| Channel | Purpose |
|---|---|
| **Beta** | Active development builds published for testing and early use |
| **Stable** | Future production-ready releases |

---
---

## 📥 Getting VRCL Client

### Recommended

Use the **Download VRCL Client Installer** button at the top of this page to obtain the installer. The installer checks GitHub for the latest published VRCL Client release when it starts.

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
