# VRCL Installer

This directory documents the standalone VRCL Installer build used for the public release.

## Release build

- Installer: `VRCL.Installer.exe`
- Release: `v1.0.0-release-beta`
- Client package: `VRCL_Client_1.0.0-release-beta.zip`
- Installer build is self-contained for Windows x64.

## Build package contents

The installer build package supplied for release contains:

```text
VRCL Installer/
├── Themes/
│   ├── build_defaults.json
│   └── vrcl_version.json
├── VRCL.Installer.exe
├── VRCL.Installer.pdb
└── wolf_logo.png
```

The compiled installer files are intentionally documented here rather than modified. The repository does not contain the installer source project in this change.

## Distribution

For normal users, download `VRCL.Installer.exe` from the GitHub Release rather than the repository source tree.

The installer checks the GitHub Releases API at startup and selects the newest published VRCL client release that has the expected `VRCL_Client_<version>.zip` asset.
