param(
    [string]$Root = (Resolve-Path ".").Path
)

# AVISO: saida deste script e APENAS estudo em art/. NUNCA apontar .tscn ou runtime para estes PNG.
# Gameplay = UseLayeredPrototype (Polygon2D na engine). Ver docs/VISUAL_RULE.md

$ErrorActionPreference = "Stop"

Add-Type -AssemblyName System.Drawing

Add-Type -ReferencedAssemblies System.Drawing -TypeDefinition @"
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

public static class ReferenceAssetExtractor
{
    public static void ExtractFrame(string sourcePath, string outputPath, int x, int y, int width, int height, int targetBodyHeight)
    {
        using (var source = new Bitmap(sourcePath))
        using (var crop = Crop(source, new Rectangle(x, y, width, height)))
        using (var cutout = RemoveDarkBackground(crop, 32))
        using (var frame = NewNormalizedFrame(cutout, 384, 1024, targetBodyHeight, 915))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            frame.Save(outputPath, ImageFormat.Png);
        }
    }

    public static void ExtractWalkSheet(string sourcePath, string outputPath, int targetBodyHeight)
    {
        var crops = new[]
        {
            new Rectangle(382, 154, 88, 126),
            new Rectangle(454, 142, 70, 138),
            new Rectangle(382, 154, 88, 126),
            new Rectangle(454, 142, 70, 138)
        };

        using (var source = new Bitmap(sourcePath))
        using (var sheet = new Bitmap(1536, 1024, PixelFormat.Format32bppArgb))
        using (var graphics = Graphics.FromImage(sheet))
        {
            graphics.Clear(Color.FromArgb(0, 0, 0, 0));
            graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            graphics.PixelOffsetMode = PixelOffsetMode.Half;

            for (int i = 0; i < crops.Length; i++)
            {
                using (var crop = Crop(source, crops[i]))
                using (var cutout = RemoveDarkBackground(crop, 32))
                using (var frame = NewNormalizedFrame(cutout, 384, 1024, targetBodyHeight, 915))
                {
                    graphics.DrawImageUnscaled(frame, i * 384, 0);
                }
            }

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            sheet.Save(outputPath, ImageFormat.Png);
        }
    }

    private static Bitmap Crop(Bitmap source, Rectangle rect)
    {
        var crop = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
        using (var graphics = Graphics.FromImage(crop))
        {
            graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            graphics.PixelOffsetMode = PixelOffsetMode.Half;
            graphics.DrawImage(source, new Rectangle(0, 0, rect.Width, rect.Height), rect, GraphicsUnit.Pixel);
        }

        return crop;
    }

    private static Bitmap RemoveDarkBackground(Bitmap source, int threshold)
    {
        int width = source.Width;
        int height = source.Height;
        var output = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        using (var graphics = Graphics.FromImage(output))
        {
            graphics.DrawImage(source, 0, 0, width, height);
        }

        var rect = new Rectangle(0, 0, width, height);
        var data = output.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        int byteCount = Math.Abs(data.Stride) * height;
        var bytes = new byte[byteCount];
        Marshal.Copy(data.Scan0, bytes, 0, byteCount);

        var visited = new bool[width * height];
        var queue = new Queue<int>(width + height);

        Action<int, int> enqueue = (px, py) =>
        {
            if (px < 0 || px >= width || py < 0 || py >= height)
            {
                return;
            }

            int visitIndex = py * width + px;
            if (visited[visitIndex])
            {
                return;
            }

            int index = py * data.Stride + px * 4;
            int maxChannel = Math.Max(bytes[index + 2], Math.Max(bytes[index + 1], bytes[index]));
            if (maxChannel <= threshold)
            {
                visited[visitIndex] = true;
                queue.Enqueue(visitIndex);
            }
        };

        for (int px = 0; px < width; px++)
        {
            enqueue(px, 0);
            enqueue(px, height - 1);
        }

        for (int py = 0; py < height; py++)
        {
            enqueue(0, py);
            enqueue(width - 1, py);
        }

        while (queue.Count > 0)
        {
            int visitIndex = queue.Dequeue();
            int px = visitIndex % width;
            int py = visitIndex / width;
            int index = py * data.Stride + px * 4;
            bytes[index + 3] = 0;

            enqueue(px + 1, py);
            enqueue(px - 1, py);
            enqueue(px, py + 1);
            enqueue(px, py - 1);
        }

        Marshal.Copy(bytes, 0, data.Scan0, byteCount);
        output.UnlockBits(data);
        return output;
    }

    private static Rectangle GetAlphaBounds(Bitmap image)
    {
        int minX = image.Width;
        int minY = image.Height;
        int maxX = -1;
        int maxY = -1;

        var rect = new Rectangle(0, 0, image.Width, image.Height);
        var data = image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        int byteCount = Math.Abs(data.Stride) * image.Height;
        var bytes = new byte[byteCount];
        Marshal.Copy(data.Scan0, bytes, 0, byteCount);

        for (int py = 0; py < image.Height; py++)
        {
            for (int px = 0; px < image.Width; px++)
            {
                int index = py * data.Stride + px * 4;
                if (bytes[index + 3] > 12)
                {
                    minX = Math.Min(minX, px);
                    minY = Math.Min(minY, py);
                    maxX = Math.Max(maxX, px);
                    maxY = Math.Max(maxY, py);
                }
            }
        }

        image.UnlockBits(data);
        if (maxX < 0)
        {
            return new Rectangle(0, 0, image.Width, image.Height);
        }

        int pad = 4;
        int left = Math.Max(0, minX - pad);
        int top = Math.Max(0, minY - pad);
        int right = Math.Min(image.Width - 1, maxX + pad);
        int bottom = Math.Min(image.Height - 1, maxY + pad);
        return new Rectangle(left, top, right - left + 1, bottom - top + 1);
    }

    private static Bitmap NewNormalizedFrame(Bitmap source, int frameWidth, int frameHeight, int targetBodyHeight, int footLine)
    {
        Rectangle bounds = GetAlphaBounds(source);
        double scale = Math.Min((frameWidth * 0.86) / bounds.Width, (double)targetBodyHeight / bounds.Height);
        int drawWidth = Math.Max(1, (int)Math.Round(bounds.Width * scale));
        int drawHeight = Math.Max(1, (int)Math.Round(bounds.Height * scale));
        int destX = (int)Math.Round((frameWidth - drawWidth) / 2.0);
        int destY = (int)Math.Round((double)(footLine - drawHeight));

        var frame = new Bitmap(frameWidth, frameHeight, PixelFormat.Format32bppArgb);
        using (var graphics = Graphics.FromImage(frame))
        {
            graphics.Clear(Color.FromArgb(0, 0, 0, 0));
            graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            graphics.PixelOffsetMode = PixelOffsetMode.Half;
            graphics.DrawImage(source, new Rectangle(destX, destY, drawWidth, drawHeight), bounds, GraphicsUnit.Pixel);
        }

        return frame;
    }
}
"@

$sheet = Join-Path $Root "references\personagens_ref\ChatGPT Image 5 de jul. de 2026, 18_18_40 (1).png"
$playerDir = Join-Path $Root "art\sprites\player"

[ReferenceAssetExtractor]::ExtractFrame(
    $sheet,
    (Join-Path $playerDir "caua_ref_idle.png"),
    288, 140, 82, 142, 790)

[ReferenceAssetExtractor]::ExtractWalkSheet(
    $sheet,
    (Join-Path $playerDir "caua_ref_walk_sheet.png"),
    790)

[ReferenceAssetExtractor]::ExtractFrame(
    $sheet,
    (Join-Path $playerDir "caua_ref_attack.png"),
    535, 140, 142, 142, 790)

Write-Host "Assets extraidos da prancha de referencia para art/sprites/player/caua_ref_*.png"
