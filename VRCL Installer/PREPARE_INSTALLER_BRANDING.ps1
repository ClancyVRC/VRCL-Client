$ErrorActionPreference = "Stop"

$projectRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Split-Path -Parent $projectRoot
$svgPath = Join-Path $repoRoot "assets\vrcl-logo.svg"
$iconPath = Join-Path $projectRoot "vrcl_installer.ico"

if (-not (Test-Path -LiteralPath $svgPath)) {
    throw "VRCL logo source not found: $svgPath"
}

# The official VRCL SVG contains the 96x96 PNG wolf logo as an embedded
# base64 image. Extract that PNG and wrap it in a standard ICO container.
$svg = Get-Content -LiteralPath $svgPath -Raw
$match = [regex]::Match($svg, 'data:image/png;base64,([^"''>]+)')
if (-not $match.Success) {
    throw "Could not find the embedded VRCL PNG inside vrcl-logo.svg."
}

$png = [Convert]::FromBase64String($match.Groups[1].Value)

# ICO header: reserved=0, type=1 (icon), count=1.
$bytes = [System.Collections.Generic.List[byte]]::new()
$bytes.AddRange([BitConverter]::GetBytes([UInt16]0))
$bytes.AddRange([BitConverter]::GetBytes([UInt16]1))
$bytes.AddRange([BitConverter]::GetBytes([UInt16]1))

# One 96x96 PNG-backed icon entry.
$bytes.Add([byte]96)
$bytes.Add([byte]96)
$bytes.Add([byte]0)
$bytes.Add([byte]0)
$bytes.AddRange([BitConverter]::GetBytes([UInt16]1))
$bytes.AddRange([BitConverter]::GetBytes([UInt16]32))
$bytes.AddRange([BitConverter]::GetBytes([UInt32]$png.Length))
$bytes.AddRange([BitConverter]::GetBytes([UInt32]22))
$bytes.AddRange($png)

[IO.File]::WriteAllBytes($iconPath, $bytes.ToArray())

Write-Host "VRCL installer icon prepared:"
Write-Host "  $iconPath"
