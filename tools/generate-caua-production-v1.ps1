param(
    [string]$Root = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
)

$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.Drawing

$outDir = Join-Path $Root "art\production\characters\caua\sprites"
New-Item -ItemType Directory -Force -Path $outDir | Out-Null

$W = 112
$H = 144
$FootY = 132

Add-Type -ReferencedAssemblies System.Drawing -TypeDefinition @"
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

public static class CauaSpritePainter
{
    static readonly Color Skin = Color.FromArgb(255, 148, 87, 48);
    static readonly Color SkinDark = Color.FromArgb(255, 118, 68, 36);
    static readonly Color Shirt = Color.FromArgb(255, 168, 8, 8);
    static readonly Color ShirtDark = Color.FromArgb(255, 120, 4, 4);
    static readonly Color Pants = Color.FromArgb(255, 16, 18, 14);
    static readonly Color Shoe = Color.FromArgb(255, 158, 10, 8);
    static readonly Color Hair = Color.FromArgb(255, 9, 6, 5);
    static readonly Color Outline = Color.FromArgb(255, 18, 12, 10);

    public static void SaveFrame(string path, float walkPhase, float armSwing, float legSwing, float torsoLean, bool jab, bool cross, bool hurt, bool death)
    {
        using (var bmp = new Bitmap($W, $H, PixelFormat.Format32bppArgb))
        using (var g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = SmoothingMode.None;
            g.Clear(Color.Transparent);
            int cx = $W / 2;
            float bob = (float)Math.Sin(walkPhase * Math.PI * 2) * 2f;
            if (death) bob = 8f;

            float lean = torsoLean + (hurt ? 0.12f : 0f) + (death ? 0.35f : 0f);
            g.TranslateTransform(cx, $FootY + bob);
            g.RotateTransform(lean * 28f);
            g.TranslateTransform(-cx, -($FootY + bob));

            int hipY = $FootY - 44;
            int torsoY = hipY - 38;
            int headY = torsoY - 28;

            if (death)
            {
                DrawLeg(g, cx - 10, hipY, 18, 42, 0.5f, Pants, Outline);
                DrawLeg(g, cx + 8, hipY, -12, 36, -0.2f, Pants, Outline);
                DrawTorso(g, cx, torsoY + 8, 30, 40, 0.4f, Shirt, ShirtDark, Outline);
                DrawHead(g, cx + 12, headY + 16, 0.55f, hurt, death, Outline);
                DrawArm(g, cx - 8, torsoY + 10, 28, 8, 0.8f, Skin, Outline);
                DrawArm(g, cx + 10, torsoY + 14, 22, 6, -0.3f, Skin, Outline);
            }
            else
            {
                float lLeg = legSwing;
                float rLeg = -legSwing;
                DrawLeg(g, cx - 8, hipY, (int)(10 + lLeg * 14), 40, lLeg * 0.35f, Pants, Outline);
                DrawLeg(g, cx + 6, hipY, (int)(-8 + rLeg * 14), 40, rLeg * 0.35f, Pants, Outline);
                DrawTorso(g, cx, torsoY, 28, 38, lean * 0.15f, Shirt, ShirtDark, Outline);

                if (jab)
                {
                    DrawArm(g, cx + 6, torsoY + 6, 34, 10, -0.05f, Skin, Outline);
                    DrawArm(g, cx - 10, torsoY + 8, 16, 12, 0.35f, Skin, Outline);
                }
                else if (cross)
                {
                    DrawArm(g, cx + 2, torsoY + 4, 38, 14, 0.25f, Skin, Outline);
                    DrawArm(g, cx - 12, torsoY + 10, 14, 10, 0.45f, Skin, Outline);
                }
                else
                {
                    DrawArm(g, cx - 12, torsoY + 6, 18, (int)(10 + armSwing * 8), 0.25f + armSwing * 0.2f, Skin, Outline);
                    DrawArm(g, cx + 10, torsoY + 8, 18, (int)(10 - armSwing * 8), -0.15f - armSwing * 0.2f, Skin, Outline);
                }

                DrawHead(g, cx + (hurt ? 4 : 0), headY, lean * 0.2f, hurt, death, Outline);
            }

            DrawShoe(g, cx - 14, (int)($FootY - 6 + bob), Shoe, Outline);
            DrawShoe(g, cx + 2, (int)($FootY - 6 + bob), Shoe, Outline);
            bmp.Save(path, ImageFormat.Png);
        }
    }

    static void DrawHead(Graphics g, int x, int y, float tilt, bool hurt, bool death, Color outline)
    {
        g.TranslateTransform(x, y);
        g.RotateTransform(tilt * 18f);
        FillPoly(g, outline, new[] { new Point(-14, -8), new Point(8, -12), new Point(12, 4), new Point(6, 16), new Point(-10, 14), new Point(-16, 2) });
        FillPoly(g, hurt ? SkinDark : Skin, new[] { new Point(-12, -6), new Point(6, -10), new Point(10, 2), new Point(4, 14), new Point(-8, 12), new Point(-14, 2) });
        FillPoly(g, Hair, new[] { new Point(-12, -8), new Point(8, -12), new Point(6, -2), new Point(-10, 0) });
        FillRect(g, Color.FromArgb(240, 225, 210), -2, -2, 8, 4);
        FillRect(g, Color.FromArgb(20, 14, 10), 2, -1, 3, 3);
        if (hurt) FillRect(g, Color.FromArgb(180, 20, 20), 4, 4, 6, 3);
        g.ResetTransform();
    }

    static void DrawTorso(Graphics g, int x, int y, int w, int h, float lean, Color fill, Color shadow, Color outline)
    {
        g.TranslateTransform(x, y);
        g.RotateTransform(lean * 12f);
        FillPoly(g, outline, new[] { new Point(-w/2 - 2, -4), new Point(w/2 + 2, -6), new Point(w/2, h), new Point(-w/2, h + 2) });
        FillPoly(g, fill, new[] { new Point(-w/2, -2), new Point(w/2, -4), new Point(w/2 - 2, h - 2), new Point(-w/2 + 2, h) });
        FillPoly(g, shadow, new[] { new Point(-w/2 + 2, 8), new Point(-4, 6), new Point(-2, h - 4), new Point(-w/2 + 4, h - 2) });
        g.ResetTransform();
    }

    static void DrawArm(Graphics g, int x, int y, int len, int lift, float angle, Color fill, Color outline)
    {
        g.TranslateTransform(x, y);
        g.RotateTransform(angle * 55f);
        FillPoly(g, outline, new[] { new Point(-4, -2), new Point(len + 4, 0), new Point(len + 2, 8), new Point(-2, 6) });
        FillPoly(g, fill, new[] { new Point(-3, 0), new Point(len, 2), new Point(len - 1, 7), new Point(-1, 5) });
        g.ResetTransform();
    }

    static void DrawLeg(Graphics g, int x, int y, int extend, int len, float angle, Color fill, Color outline)
    {
        g.TranslateTransform(x, y);
        g.RotateTransform(angle * 35f);
        FillPoly(g, outline, new[] { new Point(-6, 0), new Point(6, 0), new Point(4 + extend/4, len), new Point(-4 + extend/6, len) });
        FillPoly(g, fill, new[] { new Point(-5, 2), new Point(5, 2), new Point(3 + extend/4, len - 2), new Point(-3 + extend/6, len - 2) });
        g.ResetTransform();
    }

    static void DrawShoe(Graphics g, int x, int y, Color fill, Color outline)
    {
        FillPoly(g, outline, new[] { new Point(x - 8, y), new Point(x + 14, y - 2), new Point(x + 16, y + 6), new Point(x - 6, y + 8) });
        FillPoly(g, fill, new[] { new Point(x - 6, y + 1), new Point(x + 12, y - 1), new Point(x + 14, y + 5), new Point(x - 4, y + 6) });
    }

    static void FillRect(Graphics g, Color c, int x, int y, int w, int h)
    {
        g.FillRectangle(new SolidBrush(c), x, y, w, h);
    }

    static void FillPoly(Graphics g, Color c, Point[] pts)
    {
        g.FillPolygon(new SolidBrush(c), pts);
    }
}
"@

function Write-Anim($name, $count, $scriptBlock) {
    for ($i = 1; $i -le $count; $i++) {
        $phase = if ($count -gt 1) { ($i - 1) / ($count - 1) } else { 0 }
        $path = Join-Path $outDir ("caua_{0}_{1:D2}.png" -f $name, $i)
        & $scriptBlock $phase $path
        Write-Host "  $path"
    }
}

Write-Host "Gerando sprites Caua v1 em $outDir"

Write-Anim "idle" 4 {
    param($phase, $path)
    [CauaSpritePainter]::SaveFrame($path, $phase, 0, 0, 0, $false, $false, $false, $false)
}

Write-Anim "walk" 6 {
    param($phase, $path)
    [CauaSpritePainter]::SaveFrame($path, $phase, [Math]::Sin($phase * [Math]::PI * 2), [Math]::Sin($phase * [Math]::PI * 2), 0.02, $false, $false, $false, $false)
}

Write-Anim "run" 4 {
    param($phase, $path)
    [CauaSpritePainter]::SaveFrame($path, $phase, [Math]::Sin($phase * [Math]::PI * 2) * 1.2, [Math]::Sin($phase * [Math]::PI * 2) * 1.3, 0.08, $false, $false, $false, $false)
}

Write-Anim "jab" 3 {
    param($phase, $path)
    [CauaSpritePainter]::SaveFrame($path, 0, 0, 0, 0.04 * $phase, $true, $false, $false, $false)
}

Write-Anim "cross" 3 {
    param($phase, $path)
    [CauaSpritePainter]::SaveFrame($path, 0, 0, 0, 0.06 * $phase, $false, $true, $false, $false)
}

Write-Anim "kick" 3 {
    param($phase, $path)
    [CauaSpritePainter]::SaveFrame($path, 0, 0, 0.5 + $phase, 0.08, $false, $false, $false, $false)
}

Write-Anim "hurt" 2 {
    param($phase, $path)
    [CauaSpritePainter]::SaveFrame($path, 0, 0, 0, -0.08, $false, $false, $true, $false)
}

Write-Anim "death" 1 {
    param($phase, $path)
    [CauaSpritePainter]::SaveFrame($path, 0, 0, 0, 0, $false, $false, $false, $true)
}

Write-Host "Concluido. Reimporte no Godot ou rode dotnet build."
