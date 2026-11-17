# VRCL Client

<p align="center">
  <img src=".github/branding/github-banner.svg" width="100%" alt="VRCL Client — manage your VRChat sessions">
</p>

<p align="center">
  <strong>Get in & out of VRChat, Fast.</strong><br>
  Control VR and Desktop sessions, manage PCVR startup, monitor activity, and customize your VRChat workflow from one Windows app.
</p>

---

## Patch Notes & Update Information

<p align="center">
  <strong>Current Full Release: v1.0.3</strong><br>
  <sub>RELEASE VALIDATION COMPLETED · NOVEMBER 17, 2026 · 12:05 AM CST (UTC−06:00)</sub>
</p>

### v1.0.3 Full — Session Management & UI Polish

A more complete VRChat session-management experience with a refreshed purple Default theme, smoother controls, and clearer status information.

#### Changes
- Refined the Main Menu and Settings layout while restoring the Default theme’s deep-purple background, violet header and footer, and matching darker session cards.
- Improved separate VR and Desktop session controls, including their launch, restart, close, and keyboard shortcut workflows.
- Expanded session awareness with clearer VRCL, SteamVR, and VRChat activity, plus Discord Activity Status when enabled.
- Polished Settings controls with consistent sliding toggles, clearer experimental options, a Hide Status control, and optional font customization.
- Unified Settings and Patch Notes scrolling behavior and improved theme/color consistency across menus and controls.
- Kept the Data folder visible while retaining protected user-data handling during supported updates.
- Added the Rollback Version control beneath Restart App.
- Added a GitHub Page button beside Give Feedback in App Information.

#### Fixes
- Corrected visual balance in the Main Menu, including the center-card colors and the Default theme background.
- Improved Settings header alignment and centered the status text inside its footer widget.
- Prevented the font selector from cycling through choices when scrolling over the closed dropdown.
- Improved status-widget visibility so it appears only on the Main Menu while session and Discord status continue updating elsewhere.
- Added clear separators between each version in Patch Notes and refined spacing and readability.
- Expanded update discovery to support published Beta and Full releases together.
- Added one-release rollback selection; at the oldest published release, Rollback Version reinstalls that same release.
- Updated the bundled updater to run outside the install folder, fully mirror the selected release, remove obsolete application files, preserve Data, and replace its own updater safely.
- Strengthened uninstallation with external cleanup, installation-specific Oculus CLI process matching, managed desktop-entry cleanup, and complete installation-folder removal.

#### Additions
- Added a short explanation under Experimental Options and grouped its available controls directly beneath the setting.
- Added a divider between Hide Status and Fonts to make experimental settings easier to scan.

---

## What is VRCL Client?

**VRCL** stands for **VRChat Launcher**. VRCL Client is a Windows companion for starting and managing VRChat sessions. It provides separate controls for VR and Desktop play, brings common PCVR startup options into one place, and keeps session tools and app settings easy to reach.

## Current Features

### VRChat Session Controls
- **Start VRChat (VR):** Starts SteamVR, waits for it to initialize, and then launches VRChat in VR mode.
- **Launch VRChat (Desktop):** Starts VRChat through Steam in Desktop mode without unnecessarily launching SteamVR.
- **Restart sessions:** Restart the active VR or Desktop session from the Main Menu.
- **Close VRChat:** Close VRChat and the related active VR session processes.
- **Live session status:** View VRCL activity and the current session state from the Main Menu.

### PCVR Startup & Detection
- **Automatic application scanning:** Scan for Steam, SteamVR, VRChat, Meta PCVR, and supported cache locations.
- **Saved application paths:** Review, reset, or adjust detected application paths in Settings.
- **Meta / Quest PCVR startup:** Use the optional Oculus Startup System for supported Meta PCVR setups.
- **ASW control:** Manage Oculus Asynchronous Spacewarp through VRCL on supported installations.

### Settings & Personalization
- **Themes:** Choose from Default, Dark, Light, and available themed backgrounds.
- **Colors:** Customize the app’s text, accents, and visual details with the available color choices.
- **Animated appearance options:** Use supported animated color effects, including Coat of Color and developer-only color options.
- **Fonts:** Choose an optional interface font through Experimental Options.
- **Status visibility:** Hide the app status from Discord Activity when preferred.

### Discord Activity
- **Rich Presence:** Share VRCL’s current activity through Discord when enabled.
- **Session-aware status:** Keep Discord Activity updated as VRCL changes between the Main Menu and session actions.
- **Visibility control:** Choose whether app status is shown in Discord.

### Keybinds & Quick Access
- **Custom keybinds:** Assign shortcuts to supported VRCL actions.
- **Input modes:** Choose single-key or key-combination shortcuts.
- **System tray menu:** Open VRCL, start or restart VR, close a session, clear supported caches, or exit from the Windows tray.

### Updates, Rollback & App Management
- **GitHub update checks:** Check published VRCL releases and download available updates.
- **Beta and Full release support:** Detect and handle both published release types.
- **Auto Update:** Enable automatic update behavior in Settings.
- **Rollback Version:** Return to the immediately previous published VRCL release. At the oldest available release, the control reinstalls that same release.
- **Protected user data:** Supported updates preserve the VRCL Data folder while replacing application files.
- **Installer and uninstaller:** Install VRCL directly and remove the VRCL installation and its managed app entries when uninstalling.

### Setup & App Information
- **First-launch setup:** Complete the initial VRCL setup before entering the Main Menu.
- **Feedback:** Open the VRCL feedback form from App Information.
- **GitHub Page:** Open the VRCL repository directly from Settings.

---

## Your VRCL Data

Your personal VRCL settings and application data are stored in the app's Data folder and account-specific locations.

Back up your VRCL data before making manual changes to application files. The supported updater is designed to preserve the Data folder when updating application files.

For details about local files, GitHub update checks, and optional Discord Activity, see the [Privacy Statement](PRIVACY.md).

---

## Project Information

<p align="center">
  <a href="PRIVACY.md">🔒 Privacy Statement</a>
  &nbsp;•&nbsp;
  <a href="LICENSE.md">📜 License</a>
  &nbsp;•&nbsp;
  <a href="https://github.com/ClancyVRC/VRCL-Client/releases">📦 Releases</a>
</p>

---

<p align="center">
  <strong>VRCL Client</strong><br>
  <em>Get in & out of VRChat, Fast.</em>
</p>
