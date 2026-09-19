# VRCL Installer

Standalone VRCL Client installer for Windows x64.

## What it does

- Checks the official `ClancyVRC/VRCL-Client` GitHub Releases every time the installer starts.
- Scans published releases and selects the newest compatible VRCL Client version.
- Accepts both `VRCL_Client_v<version>.zip` and `VRCL_Client_<version>.zip`.
- Verifies SHA-256 when GitHub provides a digest.
- Preserves the installed `Data` folder during updates.
- Uses the supplied 1024x1024 VRCL artwork for the installer header and multi-size Windows icon.

## Build

Run:

`BUILD_FIXED_INSTALLER.cmd`

The finished `VRCL.Installer.exe` is self-contained for Windows x64.

## Public release signing

Public release installers are built and signed automatically by:

`.github/workflows/release-installer.yml`

When a GitHub Release is published, the workflow:

1. Checks out that release tag.
2. Builds the self-contained Windows x64 installer.
3. Signs `VRCL.Installer.exe` with Microsoft Artifact Signing.
4. Verifies the Authenticode signature.
5. Uploads the signed `VRCL.Installer.exe` to that release.

The signing workflow uses GitHub OIDC with Azure Artifact Signing so the signing certificate/private key is not stored in the repository.

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

## Notes

- The installer is self-contained for Windows x64.
- The installer window uses the embedded high-resolution VRCL artwork.
- Existing VRCL `Data`/settings are preserved when the client is updated.
