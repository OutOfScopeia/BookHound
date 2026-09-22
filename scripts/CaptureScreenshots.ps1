# Pulls Play Store screenshots from a connected Android device via adb.
# Output goes to docs/playstore/screenshots/.
#
# Usage:
#   .\scripts\CaptureScreenshots.ps1              # interactive: press Enter to capture
#   .\scripts\CaptureScreenshots.ps1 -Count 4     # capture 4 shots, prompting between each
#   .\scripts\CaptureScreenshots.ps1 -Serial RFCY11JWE3J

[CmdletBinding()]
param(
    [int]$Count = 0,          # 0 = interactive until user quits
    [string]$Serial = '',
    [string]$AdbPath = "$env:LOCALAPPDATA\Android\Sdk\platform-tools\adb.exe"
)

if (-not (Test-Path $AdbPath)) {
    Write-Error "adb not found at $AdbPath. Install Android platform-tools or pass -AdbPath."
    exit 1
}

$root   = Split-Path -Parent $PSScriptRoot
$outDir = Join-Path $root 'docs\playstore\screenshots'
New-Item -ItemType Directory -Force -Path $outDir | Out-Null

# Resolve target device
$devicesOutput = & $AdbPath devices | Select-Object -Skip 1 | Where-Object { $_ -match '\tdevice$' }
$serials = $devicesOutput | ForEach-Object { ($_ -split '\t')[0] }

if ($serials.Count -eq 0) {
    Write-Error 'No connected devices. Plug in your phone with USB debugging enabled.'
    exit 1
}

if ($Serial -eq '') {
    if ($serials.Count -gt 1) {
        Write-Host 'Multiple devices connected:'
        $serials | ForEach-Object { Write-Host "  $_" }
        Write-Error 'Pass -Serial <id> to pick one.'
        exit 1
    }
    $Serial = $serials[0]
}
Write-Host "Using device: $Serial" -ForegroundColor Cyan

# Determine starting index so re-runs don't overwrite prior shots
$existing = Get-ChildItem $outDir -Filter 'screenshot-*.png' -ErrorAction SilentlyContinue |
    ForEach-Object {
        if ($_.BaseName -match 'screenshot-(\d+)') { [int]$Matches[1] } else { 0 }
    }
$next = if ($existing) { ($existing | Measure-Object -Maximum).Maximum + 1 } else { 1 }

$captured = 0
Write-Host ''
Write-Host 'Point the phone at the shot you want, then press Enter. Type q + Enter to stop.' -ForegroundColor Yellow

while ($true) {
    if ($Count -gt 0 -and $captured -ge $Count) { break }

    $prompt = "Capture #$next (Enter=snap, q=quit)"
    $input  = Read-Host $prompt
    if ($input -eq 'q' -or $input -eq 'Q') { break }

    $outPath = Join-Path $outDir ("screenshot-{0:D2}.png" -f $next)

    # adb exec-out pipes raw PNG bytes; must write via binary file stream (not Out-File / redirection)
    $psi = New-Object System.Diagnostics.ProcessStartInfo
    $psi.FileName               = $AdbPath
    $psi.Arguments              = "-s $Serial exec-out screencap -p"
    $psi.RedirectStandardOutput = $true
    $psi.UseShellExecute        = $false
    $psi.CreateNoWindow         = $true

    $proc = [System.Diagnostics.Process]::Start($psi)
    $fs   = [System.IO.File]::Create($outPath)
    $proc.StandardOutput.BaseStream.CopyTo($fs)
    $fs.Close()
    $proc.WaitForExit()

    $bytes = (Get-Item $outPath).Length
    if ($bytes -lt 1024) {
        Write-Warning "  screencap returned only $bytes bytes — is the screen unlocked?"
        Remove-Item $outPath -Force
        continue
    }

    # Validate resolution (Play requires >= 320 px on the short side; we recommend >= 1080)
    Add-Type -AssemblyName System.Drawing
    $img = [System.Drawing.Image]::FromFile($outPath)
    $shortSide = [Math]::Min($img.Width, $img.Height)
    $img.Dispose()

    Write-Host ("  saved {0}  ({1}x{2}, short side {3}px)" -f $outPath, $img.Width, $img.Height, $shortSide) -ForegroundColor Green
    if ($shortSide -lt 1080) {
        Write-Warning "  short side is only $shortSide px — Play recommends >= 1080. Consider a higher-res device."
    }

    $next++
    $captured++
}

Write-Host ''
Write-Host "Captured $captured screenshot(s). Files in $outDir" -ForegroundColor Cyan
Write-Host 'Play Console requires 2-8 phone screenshots (16:9 or 9:16, short side >= 320 px, up to 8 MB each).'
