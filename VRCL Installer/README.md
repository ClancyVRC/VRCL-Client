# VRCL Installer

This directory contains the standalone VRCL Installer source and build process used for public VRCL releases.

## Build

Run:

`BUILD_FIXED_INSTALLER.cmd`

The build automatically prepares the current VRCL wolf logo as:

- `vrcl_installer_logo.png` for the installer window
- `vrcl_installer.ico` for the Windows EXE/application icon

The branding source is the main VRCL Client logo at `assets/vrcl-app-icon.webp`.

## Public release signing

Public release installers are built and signed automatically by:

`.github/workflows/release-installer.yml`

When a GitHub Release is published, the workflow:

1. Checks out that release tag.
2. Builds the self-contained Windows x64 installer.
3. Signs `VRCL.Installer.exe` with Microsoft Artifact Signing.
4. Verifies the Authenticode signature.
5. Uploads the signed `VRCL.Installer.exe` to that release.

The signing workflow uses GitHub OIDC with Azure Artifact Signing so the signing certificate/private key is not stored in the repository. Microsoft recommends Artifact Signing for non-Store Windows distribution, while SmartScreen reputation still builds over time for new files. citeturn0search1turn4search0

## Required GitHub configuration

Before the first signed release, configure:

### Actions secrets

- `AZURE_CLIENT_ID`
- `AZURE_TENANT_ID`
- `AZURE_SUBSCRIPTION_ID`

### Actions variables

- `AZURE_SIGNING_ENDPOINT`
- `AZURE_SIGNING_ACCOUNT`
- `AZURE_CERTIFICATE_PROFILE`

The Azure identity used by the workflow must have the Artifact Signing Certificate Profile Signer role.

## Release flow

The normal VRCL release flow can stay the same:

1. Build/test VRCL Client.
2. Create the GitHub Release.
3. Publish the release.
4. GitHub Actions builds, signs, verifies, and uploads `VRCL.Installer.exe` automatically.

No manual re-signing step is needed for public releases once the Azure Artifact Signing setup is configured.

## Notes

- The installer is self-contained for Windows x64.
- Local installer builds are not automatically trusted by Windows unless they are signed.
- The public release workflow is the authoritative signed installer build.
