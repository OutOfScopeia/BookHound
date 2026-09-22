# Generates Play Store visual assets from the adaptive-icon vector drawables.
# Output goes to docs/playstore/.

Add-Type -AssemblyName System.Drawing

$root       = Split-Path -Parent $PSScriptRoot
$outDir     = Join-Path $root 'docs\playstore'
New-Item -ItemType Directory -Force -Path $outDir | Out-Null

# Palette (must match res/drawable/ic_launcher_*.xml)
$cambridgeBlue   = [System.Drawing.ColorTranslator]::FromHtml('#002147')
$cambridgeShade  = [System.Drawing.ColorTranslator]::FromHtml('#001834')
$leatherTan      = [System.Drawing.ColorTranslator]::FromHtml('#B08D57')
$cambridgeGreen  = [System.Drawing.ColorTranslator]::FromHtml('#4A5D3A')
$cream           = [System.Drawing.ColorTranslator]::FromHtml('#F5F0E1')
$labelYellow     = [System.Drawing.ColorTranslator]::FromHtml('#FFD200')
$viewfinder      = [System.Drawing.ColorTranslator]::FromHtml('#FFE500')

# Rectangles are expressed in the 108-unit vector viewport and scaled at draw time.
$vpSize = 108.0

function New-Rect { param($x, $y, $w, $h) [pscustomobject]@{ X=$x; Y=$y; W=$w; H=$h } }

$backgroundStripes = @(8,22,36,50,64,78,92) | ForEach-Object { New-Rect $_ 0 3 108 }

$leftBook       = New-Rect 20 30 16 62
$leftBookBands  = @( (New-Rect 20 34 16 2), (New-Rect 20 86 16 2) )
$leftSpineGlyphs = @(
    (New-Rect 27 54 3 2),
    (New-Rect 28 57 1 1),
    (New-Rect 28 59 2 2),
    (New-Rect 28 62 1 1),
    (New-Rect 27 64 3 2)
)

$rightBook      = New-Rect 72 30 16 62
$rightBookBands = @( (New-Rect 72 34 16 2), (New-Rect 72 86 16 2) )
$rightSpineGlyphs = @(
    (New-Rect 79 52 2 2),
    (New-Rect 80 55 1 1),
    (New-Rect 80 57 1 2),
    (New-Rect 79 60 3 2),
    (New-Rect 80 63 1 1),
    (New-Rect 79 65 3 2)
)

$centerBook      = New-Rect 40 22 28 70
$centerBookBands = @(
    (New-Rect 40 26 28 3),
    (New-Rect 40 31 28 1),
    (New-Rect 40 84 28 3),
    (New-Rect 40 89 28 1)
)

$label = New-Rect 42 50 24 22
$labelGlyphs = @(
    (New-Rect 52 52 4 2),
    (New-Rect 52 55 3 2),
    (New-Rect 53 58 2 2),
    (New-Rect 54 60 1 1),
    (New-Rect 52 61 3 2),
    (New-Rect 54 63 1 1),
    (New-Rect 54 64 1 2),
    (New-Rect 52 67 3 2),
    (New-Rect 52 70 3 2)
)

# Viewfinder brackets — each L decomposed into a horizontal + vertical bar.
$viewfinderRects = @(
    (New-Rect 32 16 12 3),  # TL horizontal
    (New-Rect 32 16 3 12),  # TL vertical
    (New-Rect 64 16 12 3),  # TR horizontal
    (New-Rect 73 16 3 12),  # TR vertical
    (New-Rect 32 86 3 12),  # BL vertical
    (New-Rect 32 95 12 3),  # BL horizontal
    (New-Rect 64 95 12 3),  # BR horizontal
    (New-Rect 73 86 3 12)   # BR vertical
)

function Invoke-Render {
    param(
        [int]$size,
        [string]$path,
        [switch]$RoundMask
    )

    $bmp = New-Object System.Drawing.Bitmap $size, $size
    $g   = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode     = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $g.PixelOffsetMode   = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic

    $scale = $size / $vpSize

    function DrawRects {
        param($rects, $color)
        $brush = New-Object System.Drawing.SolidBrush $color
        foreach ($r in $rects) {
            $g.FillRectangle(
                $brush,
                [float]($r.X * $scale),
                [float]($r.Y * $scale),
                [float]($r.W * $scale),
                [float]($r.H * $scale)
            )
        }
        $brush.Dispose()
    }

    if ($RoundMask) {
        $g.Clear([System.Drawing.Color]::Transparent)
        $clip = New-Object System.Drawing.Drawing2D.GraphicsPath
        $clip.AddEllipse(0, 0, $size, $size)
        $g.SetClip($clip)
    }

    # Background
    $bg = New-Object System.Drawing.SolidBrush $cambridgeBlue
    $g.FillRectangle($bg, 0, 0, $size, $size)
    $bg.Dispose()

    DrawRects $backgroundStripes $cambridgeShade

    DrawRects @($leftBook)  $leatherTan
    DrawRects $leftBookBands $cambridgeBlue
    DrawRects $leftSpineGlyphs $cambridgeBlue

    DrawRects @($rightBook)  $cambridgeGreen
    DrawRects $rightBookBands $cambridgeBlue
    DrawRects $rightSpineGlyphs $cream

    DrawRects @($centerBook)  $cream
    DrawRects $centerBookBands $cambridgeBlue

    DrawRects @($label) $labelYellow
    $pen = New-Object System.Drawing.Pen($cambridgeBlue, [float]$scale)
    $g.DrawRectangle(
        $pen,
        [float]($label.X * $scale),
        [float]($label.Y * $scale),
        [float]($label.W * $scale),
        [float]($label.H * $scale)
    )
    $pen.Dispose()
    DrawRects $labelGlyphs $cambridgeBlue

    DrawRects $viewfinderRects $viewfinder

    $g.Dispose()
    $bmp.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose()
    Write-Host "  wrote $path"
}

Write-Host 'Rendering Play Store icon (512x512)...'
Invoke-Render -size 512 -path (Join-Path $outDir 'icon-512.png')

Write-Host 'Rendering high-res preview icon (1024x1024)...'
Invoke-Render -size 1024 -path (Join-Path $outDir 'icon-1024.png')

Write-Host 'Rendering round preview (matches most Android launcher masks)...'
Invoke-Render -size 512 -path (Join-Path $outDir 'icon-512-round-preview.png') -RoundMask

# ---------------------------------------------------------------------------
# Feature graphic — Play Store promotional banner (1024x500, PNG/JPEG, <=15MB).
# ---------------------------------------------------------------------------

function Invoke-RenderFeatureGraphic {
    param([int]$width = 1024, [int]$height = 500, [string]$path)

    $bmp = New-Object System.Drawing.Bitmap $width, $height
    $g   = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode     = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $g.PixelOffsetMode   = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::AntiAliasGridFit

    # Background
    $bg = New-Object System.Drawing.SolidBrush $cambridgeBlue
    $g.FillRectangle($bg, 0, 0, $width, $height)
    $bg.Dispose()

    # Vertical library-stripe texture across the whole banner
    $stripe = New-Object System.Drawing.SolidBrush $cambridgeShade
    for ($x = 24; $x -lt $width; $x += 56) {
        $g.FillRectangle($stripe, [float]$x, 0, 6.0, [float]$height)
    }
    $stripe.Dispose()

    # -----------------------------------------------------------------------
    # Left side: oversized book stack echoing the app icon.
    # Coordinates below are in a local 220-wide x 400-tall space, offset into
    # the banner and drawn with plain FillRectangle for a crisp pixel look.
    # -----------------------------------------------------------------------
    $offsetX = 60
    $offsetY = 50
    $unit    = 4  # scale factor for the 108-based icon geometry

    function DrawIconRect {
        param($color, $ix, $iy, $iw, $ih)
        $brush = New-Object System.Drawing.SolidBrush $color
        $g.FillRectangle(
            $brush,
            [float]($offsetX + $ix * $unit),
            [float]($offsetY + $iy * $unit),
            [float]($iw * $unit),
            [float]($ih * $unit)
        )
        $brush.Dispose()
    }

    # Left book (tan)
    DrawIconRect $leatherTan     20 30 16 62
    DrawIconRect $cambridgeBlue  20 34 16 2
    DrawIconRect $cambridgeBlue  20 86 16 2

    # Right book (green)
    DrawIconRect $cambridgeGreen 72 30 16 62
    DrawIconRect $cambridgeBlue  72 34 16 2
    DrawIconRect $cambridgeBlue  72 86 16 2

    # Center book (cream)
    DrawIconRect $cream         40 22 28 70
    DrawIconRect $cambridgeBlue 40 26 28 3
    DrawIconRect $cambridgeBlue 40 31 28 1
    DrawIconRect $cambridgeBlue 40 84 28 3
    DrawIconRect $cambridgeBlue 40 89 28 1

    # Yellow highlight label
    DrawIconRect $labelYellow   42 50 24 22
    $pen = New-Object System.Drawing.Pen($cambridgeBlue, [float]$unit)
    $g.DrawRectangle(
        $pen,
        [float]($offsetX + 42 * $unit),
        [float]($offsetY + 50 * $unit),
        [float](24 * $unit),
        [float](22 * $unit)
    )
    $pen.Dispose()

    # Viewfinder brackets around the center book
    DrawIconRect $viewfinder 32 16 12 3
    DrawIconRect $viewfinder 32 16 3 12
    DrawIconRect $viewfinder 64 16 12 3
    DrawIconRect $viewfinder 73 16 3 12
    DrawIconRect $viewfinder 32 86 3 12
    DrawIconRect $viewfinder 32 95 12 3
    DrawIconRect $viewfinder 64 95 12 3
    DrawIconRect $viewfinder 73 86 3 12

    # -----------------------------------------------------------------------
    # Right side: wordmark + tagline.
    # -----------------------------------------------------------------------
    $textLeft = 500

    # Yellow accent bar under the tagline
    $accent = New-Object System.Drawing.SolidBrush $viewfinder
    $g.FillRectangle($accent, [float]$textLeft, 320.0, 120.0, 6.0)
    $accent.Dispose()

    $titleFont    = New-Object System.Drawing.Font('Georgia', 78, [System.Drawing.FontStyle]::Bold, [System.Drawing.GraphicsUnit]::Pixel)
    $taglineFont  = New-Object System.Drawing.Font('Georgia', 30, [System.Drawing.FontStyle]::Italic, [System.Drawing.GraphicsUnit]::Pixel)
    $subtitleFont = New-Object System.Drawing.Font('Segoe UI', 22, [System.Drawing.FontStyle]::Regular, [System.Drawing.GraphicsUnit]::Pixel)

    $creamBrush  = New-Object System.Drawing.SolidBrush $cream
    $yellowBrush = New-Object System.Drawing.SolidBrush $labelYellow

    $g.DrawString('BookHound',            $titleFont,    $creamBrush,  [float]$textLeft, 150.0)
    $g.DrawString('Find the book.',       $taglineFont,  $yellowBrush, [float]$textLeft, 250.0)
    $g.DrawString('Snap the shelf.',      $taglineFont,  $yellowBrush, [float]$textLeft, 285.0)
    $g.DrawString('Real-time OCR call-number matching.', $subtitleFont, $creamBrush, [float]$textLeft, 340.0)

    $titleFont.Dispose(); $taglineFont.Dispose(); $subtitleFont.Dispose()
    $creamBrush.Dispose(); $yellowBrush.Dispose()

    $g.Dispose()
    $bmp.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose()
    Write-Host "  wrote $path"
}

Write-Host 'Rendering feature graphic (1024x500)...'
Invoke-RenderFeatureGraphic -path (Join-Path $outDir 'feature-graphic-1024x500.png')

Write-Host ''
Write-Host 'Done. Play Console uploads:'
Write-Host '  App icon         -> docs/playstore/icon-512.png'
Write-Host '  Feature graphic  -> docs/playstore/feature-graphic-1024x500.png'
Write-Host '  Screenshots      -> run scripts/CaptureScreenshots.ps1 on a connected device'
