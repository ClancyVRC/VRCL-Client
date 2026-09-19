$ErrorActionPreference = "Stop"

$projectRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$logoSource = Join-Path $projectRoot "..\assets\vrcl-app-icon.webp"
$cachedLogo = Join-Path $projectRoot "vrcl-app-icon.webp"
$logoPng = Join-Path $projectRoot "vrcl_installer_logo.png"
$iconFile = Join-Path $projectRoot "vrcl_installer.ico"
$logoUrl = "https://raw.githubusercontent.com/ClancyVRC/VRCL-Client/main/assets/vrcl-app-icon.webp"

if (-not (Test-Path -LiteralPath $logoSource)) {
    if (-not (Test-Path -LiteralPath $cachedLogo)) {
        Write-Host "Downloading the current VRCL logo..."
        Invoke-WebRequest -Uri $logoUrl -OutFile $cachedLogo -UseBasicParsing
    }
    $logoSource = $cachedLogo
}

$magick = Get-Command magick -ErrorAction SilentlyContinue
if (-not $magick) {
    throw "ImageMagick (magick.exe) is required to prepare the VRCL installer icon. Install ImageMagick, then run the installer build again."
}

Write-Host "Preparing VRCL installer branding..."
& $magick.Source $logoSource -background none -alpha on -resize "256x256" $logoPng
if ($LASTEXITCODE -ne 0) { throw "ImageMagick failed while creating vrcl_installer_logo.png." }

& $magick.Source $logoPng -background none -define "icon:auto-resize=256,128,96,64,48,32,16" $iconFile
if ($LASTEXITCODE -ne 0) { throw "ImageMagick failed while creating vrcl_installer.ico." }

Write-Host "Installer logo ready:"
Write-Host "  $logoPng"
Write-Host "  $iconFile"
