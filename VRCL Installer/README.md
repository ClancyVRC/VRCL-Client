# VRCL Installer

This directory contains the standalone VRCL Installer source and build process used for public VRCL releases.

## Build

Run:

`BUILD_FIXED_INSTALLER.cmd`

The build automatically restores the official VRCL wolf icon from `assets/vrcl-logo.svg`.

The icon is embedded into `VRCL.Installer.exe`, and the installer window reads the logo back from the EXE itself. No separate logo file is required beside the finished installer.

## Public release signing

Public release installers are built and signed automatically by:

`.github/workflows/release-installer.yml`

When a GitHub Release is published, the workflow:

1. Checks out that release tag.
2. Builds the self-contained Windows x64 installer.
3. Signs `VRCL.Installer.exe` with Microsoft Artifact Signing.
4. Verifies the Authenticode signature.
5. Uploads the signed `VRCL.Installer.exe` to that release.

The signing workflow uses GitHub OIDC with Azure Artifact Signing so the signing certificate/private key is not stored in the repository. Microsoft recommends Artifact Signing for non-Store Windows distribution, while SmartScreen reputation still builds over time for new files.

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

- The installer is self-contained for Windows x64. The finished `VRCL.Installer.exe` is standalone and carries its application icon with it.
- Local installer builds are not automatically trusted by Windows unless they are signed.
- The public release workflow is the authoritative signed installer build.
