# VRCL Client

<p align="center">
  <img src="assets/vrcl-app-icon.webp" width="110" alt="VRCL Client wolf logo">
</p>

<h1 align="center">VRCL Client</h1>

<p align="center">
  A Windows launcher and control client for VRChat PCVR setups.
</p>

<p align="center">
  <a href="https://github.com/ClancyVRC/VRCL-Client/releases/download/v1.0.0-release-beta/VRCL.Installer.exe">
    <strong>⬇️ DOWNLOAD VRCL CLIENT INSTALLER</strong>
  </a>
</p>

---

## 🚀 Start Here

### What you need

- A **64-bit Windows PC**.
- An internet connection for the first installation and for downloading releases/updates.
- The **VRCL Installer** from the button above.
- VRChat and the VR platform/software you want VRCL Client to launch, such as SteamVR or Meta/Quest PCVR, depending on your setup.

**You do not need to download the source code, open Visual Studio, install a compiler, or build VRCL Client yourself.**

### 1. Download the installer

Click **Download VRCL Client Installer** at the top of this page.

The installer is the normal starting point for a new user.

### 2. Run the installer

Open:

`VRCL.Installer.exe`

The installer checks the project's GitHub Releases when it starts and looks for the newest published VRCL Client release with its matching:

`VRCL_Client_<version>.zip`

It then downloads that release package rather than relying on an old bundled client copy.

### 3. Choose where VRCL Client goes

The installer lets you choose the folder for the VRCL Client main files.

A typical folder name is:

`VRCL Client - VRC Client v1.0.0 - (main files)`

The installer preserves the user's `Data/` directory when updating an existing installation.

### 4. Launch VRCL Client

After installation, start:

`VRCL Client.exe`

You can use the desktop shortcut if the installer/build provides one.

### 5. First setup

On first launch, use VRCL Client's setup/settings areas to detect and configure the parts of your PCVR setup that you want VRCL Client to manage.

Depending on your hardware and software, this can include:

- Steam
- SteamVR
- VRChat
- Meta / Quest PCVR
- Oculus Startup System (OSS)
- Keybinds and launch preferences

---

## 🐺 What is VRCL Client?

VRCL Client is designed to be a single Windows launch hub for a VRChat PCVR setup.

It brings together:

- VRChat launching
- Steam and SteamVR handling
- Meta/Quest PCVR options
- PC/setup detection
- Settings and keybinds
- Tray controls
- Themes and backgrounds
- GitHub update checking

The goal is simple: **install it, set it up, and use VRCL Client as your everyday VRChat launch hub.**

---

## 🔄 Updates

VRCL Client uses GitHub Releases for its application updates.

The intended flow is:

**GitHub Release → VRCL Client checks for an update → VRCL Updater downloads the release package → application files are replaced → protected user data remains intact → VRCL Client starts again**

The updater is designed to replace application files without treating the user's personal data as part of the update payload.

### Protected data

The `Data/` directory is treated as protected user data.

Release packages should not contain replacement `Data/` content, and the updater is designed to prevent update manifests from deleting that directory.

---

## 🧰 VRCL Installer

The installer is a separate application from VRCL Client.

**Current installer build:** `v1.0.0-release-beta`

The installer is intended to:

- Check GitHub Releases every time it starts
- Find the newest published release with a matching client ZIP
- Download the matching client package
- Verify the GitHub SHA-256 digest when available
- Ask where to install the VRCL Client main-files folder
- Install the selected release
- Preserve existing `Data/` content
- Let the user launch VRCL Client after installation

The installer is **not** the main VRCL Client. Its job is to obtain and install the latest released client.

---

## 🎮 Main Features

### VRChat Launching

Launch your VR setup from one application with support for:

- Steam
- SteamVR
- VRChat
- Meta / Quest PCVR workflows
- First-run PC detection and setup

### Oculus Startup System

The optional **Oculus Startup System (OSS)** is intended for Meta/Quest PCVR users who want the required Meta/Oculus startup flow handled before SteamVR and VRChat.

### Settings & Controls

VRCL Client includes a centralized settings experience for:

- Keybinds
- Tray controls
- Launch preferences
- PC/VR setup detection
- Command output
- Theme and background support

### Themes

VRCL Client supports custom visual themes and backgrounds.

**Crystal Shards** is planned as a separate theme package so future themes do not have to be permanently baked into every full application release.

---

## 📦 Releases

GitHub Releases are the distribution point for finished VRCL Client builds.

### Current release

**VRCL Client v1.0.0-release-beta** is the current public beta release.

- [Release page](https://github.com/ClancyVRC/VRCL-Client/releases/tag/v1.0.0-release-beta)
- [Download VRCL.Installer.exe](https://github.com/ClancyVRC/VRCL-Client/releases/download/v1.0.0-release-beta/VRCL.Installer.exe)
- [Download VRCL_Client_1.0.0-release-beta.zip](https://github.com/ClancyVRC/VRCL-Client/releases/download/v1.0.0-release-beta/VRCL_Client_1.0.0-release-beta.zip)

The beta release is marked as a pre-release on GitHub. Future stable releases can use the repository's standard latest-release download link once a stable release is published.

A full client release uses:

`v<version>`

and its matching package uses:

`VRCL_Client_<version>.zip`

For example:

`v1.0.0-release-beta`

with:

`VRCL_Client_1.0.0-release-beta.zip`

The standalone installer is:

`VRCL.Installer.exe`

The installer retrieves the client package from GitHub Releases when it runs.

### Release channels

| Channel | Purpose |
|---|---|
| **Beta** | Development/testing builds |
| **Stable** | Future production-ready releases |

---

## 🛠️ For Developers

Normal users should use the installer and published releases.

Developers who want to build VRCL Client from source can use the repository's Windows/.NET build files. The source tree is separate from the pre-built release packages.

The project currently targets **.NET 10 / Windows x64**.

---

## 📌 Important

- VRCL Client is a Windows application.
- The installer requires an internet connection to retrieve the latest published client release.
- Do not delete the `Data/` directory if you want to preserve your VRCL Client data.
- Do not manually mix files from different VRCL Client releases.
- Use the installer for a new installation and VRCL's updater for supported in-app updates.

---

<p align="center">
  <strong>VRCL Client</strong><br>
  Built for a smoother VRChat PCVR setup.
</p>
