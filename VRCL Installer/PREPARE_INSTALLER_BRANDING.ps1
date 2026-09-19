$ErrorActionPreference = "Stop"
$projectRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Split-Path -Parent $projectRoot
$masterLogo = Join-Path $repoRoot "assets\vrcl-app-icon.png"
$embeddedLogo = Join-Path $projectRoot "vrcl_app_icon.png"
$installerIcon = Join-Path $projectRoot "vrcl_installer.ico"
if (-not (Test-Path -LiteralPath $masterLogo)) { throw "The 1024x1024 VRCL master artwork was not found: $masterLogo" }
if (-not (Test-Path -LiteralPath $embeddedLogo)) { throw "The embedded high-resolution logo was not found: $embeddedLogo" }
if (-not (Test-Path -LiteralPath $installerIcon)) { throw "The multi-resolution VRCL installer icon was not found: $installerIcon" }
$pngBytes = [IO.File]::ReadAllBytes($masterLogo)
if ($pngBytes.Length -lt 24) { throw "The master artwork is too small to be a valid PNG." }
$pngSignature = @(137,80,78,71,13,10,26,10)
for ($i = 0; $i -lt 8; $i++) { if ($pngBytes[$i] -ne $pngSignature[$i]) { throw "The master artwork is not a valid PNG file." } }
$ihdr = [Text.Encoding]::ASCII.GetString($pngBytes, 12, 4)
if ($ihdr -ne "IHDR") { throw "The master artwork is missing the PNG IHDR header." }
$width = ([uint32]$pngBytes[16] -shl 24) -bor ([uint32]$pngBytes[17] -shl 16) -bor ([uint32]$pngBytes[18] -shl 8) -bor [uint32]$pngBytes[19]
$height = ([uint32]$pngBytes[20] -shl 24) -bor ([uint32]$pngBytes[21] -shl 16) -bor ([uint32]$pngBytes[22] -shl 8) -bor [uint32]$pngBytes[23]
if ($width -ne 1024 -or $height -ne 1024) { throw "The master artwork must be exactly 1024x1024. Found ${width}x${height}." }
$iconBytes = [IO.File]::ReadAllBytes($installerIcon)
$header = [BitConverter]::ToUInt16($iconBytes, 0)
$type = [BitConverter]::ToUInt16($iconBytes, 2)
$count = [BitConverter]::ToUInt16($iconBytes, 4)
if ($header -ne 0 -or $type -ne 1 -or $count -lt 10) { throw "vrcl_installer.ico is not a valid multi-resolution Windows ICO." }
Write-Host "VRCL high-resolution branding verified."
Write-Host "  Master artwork: $masterLogo"
Write-Host "  Embedded header artwork: $embeddedLogo"
Write-Host "  Installer icon: $installerIcon"
Write-Host "  ICO image entries: $count"
