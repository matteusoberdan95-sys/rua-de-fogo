param(
    [string]$Root = (Resolve-Path ".").Path
)

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

public static class SpriteNormalizer
{
    public static void ConvertIdleFrame(string sourcePath, string outputPath, int targetBodyHeight)
    {
        using (var source = new Bitmap(sourcePath))
        using (var cutout = RemoveDarkBackground(source, 42))
        using (var frame = NewNormalizedFrame(cutout, 384, 1024, targetBodyHeight, 915))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            frame.Save(outputPath, ImageFormat.Png);
        }
    }

    public static void ConvertWalkSheet(string sourcePath, string outputPath, int frameCount, int targetBodyHeight)
    {
        using (var source = new Bitmap(sourcePath))
        using (var sheet = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb))
        using (var graphics = Graphics.FromImage(sheet))
        {
            int frameWidth = source.Width / frameCount;
            int frameHeight = source.Height;
            graphics.Clear(Color.FromArgb(0, 0, 0, 0));
            graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            graphics.PixelOffsetMode = PixelOffsetMode.Half;

            for (int i = 0; i < frameCount; i++)
            {
                using (var slice = new Bitmap(frameWidth, frameHeight, PixelFormat.Format32bppArgb))
                using (var sliceGraphics = Graphics.FromImage(slice))
                {
                    sliceGraphics.DrawImage(
                        source,
                        new Rectangle(0, 0, frameWidth, frameHeight),
                        new Rectangle(i * frameWidth, 0, frameWidth, frameHeight),
                        GraphicsUnit.Pixel);

                    using (var cutout = RemoveDarkBackground(slice, 42))
                    using (var normalized = NewNormalizedFrame(cutout, frameWidth, frameHeight, targetBodyHeight, 915))
                    {
                        graphics.DrawImageUnscaled(normalized, i * frameWidth, 0);
                    }
                }
            }

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            sheet.Save(outputPath, ImageFormat.Png);
        }
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

        Action<int, int> enqueue = (x, y) =>
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
            {
                return;
            }

            int visitIndex = y * width + x;
            if (visited[visitIndex])
            {
                return;
            }

            int index = y * data.Stride + x * 4;
            int maxChannel = Math.Max(bytes[index + 2], Math.Max(bytes[index + 1], bytes[index]));
            if (maxChannel <= threshold)
            {
                visited[visitIndex] = true;
                queue.Enqueue(visitIndex);
            }
        };

        for (int x = 0; x < width; x++)
        {
            enqueue(x, 0);
            enqueue(x, height - 1);
        }

        for (int y = 0; y < height; y++)
        {
            enqueue(0, y);
            enqueue(width - 1, y);
        }

        while (queue.Count > 0)
        {
            int visitIndex = queue.Dequeue();
            int x = visitIndex % width;
            int y = visitIndex / width;
            int index = y * data.Stride + x * 4;
            bytes[index + 3] = 0;

            enqueue(x + 1, y);
            enqueue(x - 1, y);
            enqueue(x, y + 1);
            enqueue(x, y - 1);
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

        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                int index = y * data.Stride + x * 4;
                if (bytes[index + 3] > 12)
                {
                    minX = Math.Min(minX, x);
                    minY = Math.Min(minY, y);
                    maxX = Math.Max(maxX, x);
                    maxY = Math.Max(maxY, y);
                }
            }
        }

        image.UnlockBits(data);
        if (maxX < 0)
        {
            return new Rectangle(0, 0, image.Width, image.Height);
        }

        int pad = 8;
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

$playerDir = Join-Path $Root "art\sprites\player"
$enemyDir = Join-Path $Root "art\sprites\enemies"

[SpriteNormalizer]::ConvertIdleFrame(
    (Join-Path $playerDir "caua_idle.png"),
    (Join-Path $playerDir "caua_idle_game.png"),
    790)

[SpriteNormalizer]::ConvertWalkSheet(
    (Join-Path $playerDir "caua_walk_sheet.png"),
    (Join-Path $playerDir "caua_walk_sheet_game.png"),
    4,
    790)

[SpriteNormalizer]::ConvertIdleFrame(
    (Join-Path $enemyDir "grunt_idle.png"),
    (Join-Path $enemyDir "grunt_idle_game.png"),
    760)

Write-Host "Sprites normalizados em art/sprites/*/*_game.png"
