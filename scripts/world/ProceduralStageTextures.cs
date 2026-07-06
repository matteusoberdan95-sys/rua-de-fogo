namespace SangueNoAsfalto.World;

/// <summary>
/// Padrões visuais procedurais para o blocking da Vila Esperança — tijolo, telha, asfalto, janelas.
/// Tudo em Polygon2D/ColorRect; sem bitmap de referência no runtime.
/// </summary>
public static class ProceduralStageTextures
{
    public static void AddBrickWall(Node2D root, float x, float y, float width, float height, int seed, int z = 3)
    {
        Color mortar = new(0.12f, 0.10f, 0.085f);
        Color brickA = new(0.28f + (seed % 3) * 0.02f, 0.11f, 0.07f);
        Color brickB = brickA.Darkened(0.08f);

        AddRect(root, $"BrickMortar{seed}", new Vector2(x, y), new Vector2(width, height), mortar, z);

        float rowH = 11f;
        float brickW = 26f;
        int rows = Mathf.CeilToInt(height / rowH);
        int cols = Mathf.CeilToInt(width / brickW);

        for (int row = 0; row < rows; row++)
        {
            float offset = row % 2 == 0 ? 0f : brickW * 0.5f;
            for (int col = 0; col < cols + 1; col++)
            {
                float bx = x + col * brickW + offset;
                float by = y + row * rowH + 1f;
                if (bx >= x + width - 2f || by >= y + height - 2f)
                {
                    continue;
                }

                Color c = (row + col + seed) % 2 == 0 ? brickA : brickB;
                float bw = Mathf.Min(brickW - 3f, x + width - bx - 1f);
                float bh = Mathf.Min(rowH - 2f, y + height - by - 1f);
                if (bw <= 2f || bh <= 2f)
                {
                    continue;
                }

                AddRect(root, $"Brick{seed}_{row}_{col}", new Vector2(bx, by), new Vector2(bw, bh), c, z + 1);
            }
        }

        for (int i = 0; i < 4; i++)
        {
            float sx = x + 8f + (seed * 17 + i * 41) % (int)(width - 20f);
            float sy = y + 6f + (seed * 11 + i * 29) % (int)(height - 14f);
            AddRect(root, $"BrickStain{seed}_{i}", new Vector2(sx, sy), new Vector2(14f + i * 4f, 3f), new Color(0f, 0f, 0f, 0.12f), z + 2);
        }
    }

    public static void AddCorrugatedRoof(Node2D root, float x, float y, float width, Color baseColor, int seed, int z = 4)
    {
        AddPoly(root, $"RoofBase{seed}", baseColor.Darkened(0.12f), [
            new Vector2(x, y + 10f), new Vector2(x + width, y + 10f), new Vector2(x + width - 8f, y), new Vector2(x + 8f, y)
        ], z);

        int ridges = Mathf.CeilToInt(width / 14f);
        for (int i = 0; i < ridges; i++)
        {
            float rx = x + i * 14f;
            Color ridge = i % 2 == 0 ? baseColor : baseColor.Lightened(0.06f);
            AddPoly(root, $"RoofRidge{seed}_{i}", ridge, [
                new Vector2(rx, y + 2f), new Vector2(rx + 8f, y), new Vector2(rx + 14f, y + 2f), new Vector2(rx + 6f, y + 10f)
            ], z + 1);
        }
    }

    public static void AddLitWindow(Node2D root, StageAssetContext ctx, float x, float y, float w, float h, int seed, bool lit = true, int z = 5)
    {
        AddRect(root, $"WinFrame{seed}", new Vector2(x, y), new Vector2(w, h), new Color(0.06f, 0.065f, 0.07f), z);
        Color glow = lit ? new Color(0.92f, 0.58f, 0.18f, 0.55f) : new Color(0.04f, 0.045f, 0.05f, 0.9f);
        ColorRect pane = AddRect(root, $"WinPane{seed}", new Vector2(x + 3f, y + 3f), new Vector2(w - 6f, h - 6f), glow, z + 1);
        if (lit)
        {
            ctx.NeonItems.Add(pane);
            ColorRect bleed = AddRect(root, $"WinBleed{seed}", new Vector2(x - 4f, y - 2f), new Vector2(w + 8f, h + 4f), new Color(0.9f, 0.45f, 0.12f, 0.08f), z);
            ctx.NeonItems.Add(bleed);
        }

        AddRect(root, $"WinBarV{seed}", new Vector2(x + w * 0.5f - 1f, y + 2f), new Vector2(2f, h - 4f), new Color(0.08f, 0.08f, 0.075f, 0.7f), z + 2);
        AddRect(root, $"WinBarH{seed}", new Vector2(x + 2f, y + h * 0.5f - 1f), new Vector2(w - 4f, 2f), new Color(0.08f, 0.08f, 0.075f, 0.7f), z + 2);
    }

    public static void BuildFavelaHouse(Node2D root, StageAssetContext ctx, float x, float y, float width, float height, int seed)
    {
        bool brick = seed % 3 != 1;
        if (brick)
        {
            AddBrickWall(root, x, y + 14f, width, height - 14f, seed, 1);
        }
        else
        {
            AddRect(root, $"Concrete{seed}", new Vector2(x, y + 14f), new Vector2(width, height - 14f), new Color(0.14f, 0.13f, 0.12f), 1);
            for (int i = 0; i < 5; i++)
            {
                AddRect(root, $"ConcreteBand{seed}_{i}", new Vector2(x, y + 20f + i * 18f), new Vector2(width, 2f), new Color(0.10f, 0.095f, 0.09f, 0.5f), 2);
            }
        }

        Color roofColor = seed % 2 == 0 ? new Color(0.22f, 0.10f, 0.06f) : new Color(0.18f, 0.17f, 0.15f);
        AddCorrugatedRoof(root, x - 4f, y, width + 8f, roofColor, seed, 3);

        int winCount = width > 70f ? 2 : 1;
        for (int w = 0; w < winCount; w++)
        {
            bool lit = (seed + w) % 3 != 0;
            AddLitWindow(root, ctx, x + 10f + w * 34f, y + 28f, 18f, 22f, seed * 10 + w, lit, 4);
        }

        if (seed % 4 == 0)
        {
            AddPoly(root, $"WaterTank{seed}", new Color(0.20f, 0.19f, 0.18f), [
                new Vector2(x + width - 8f, y - 18f), new Vector2(x + width + 18f, y - 18f),
                new Vector2(x + width + 16f, y - 4f), new Vector2(x + width - 6f, y - 4f)
            ], 5);
        }
    }

    public static void AddWetAsphaltReflections(Node2D root, StageAssetContext ctx, float startX, float y, float width)
    {
        int bands = Mathf.CeilToInt(width / 180f);
        for (int i = 0; i < bands; i++)
        {
            float x = startX + i * 180f + 20f;
            Polygon2D streak = AddPoly(root, $"AsphaltReflect{i}", new Color(0.82f, 0.55f, 0.18f, 0.07f), [
                new Vector2(x, y + 8f), new Vector2(x + 120f, y + 4f), new Vector2(x + 108f, y + 22f), new Vector2(x - 8f, y + 26f)
            ], 2);
            ctx.WetHighlights.Add(streak);
        }

        for (int i = 0; i < bands / 2; i++)
        {
            float x = startX + i * 260f + 80f;
            Polygon2D blue = AddPoly(root, $"AsphaltPolice{i}", new Color(0.18f, 0.32f, 0.82f, 0.04f), [
                new Vector2(x, y + 40f), new Vector2(x + 90f, y + 36f), new Vector2(x + 82f, y + 52f), new Vector2(x - 6f, y + 56f)
            ], 2);
            ctx.WetHighlights.Add(blue);
        }
    }

    public static void AddRainLayer(Node2D root, StageAssetContext ctx, float startX, float width)
    {
        for (int i = 0; i < 48; i++)
        {
            float x = startX + (i * 97f) % width;
            float y = 80f + (i % 11) * 52f;
            AddRect(root, $"Rain{i}", new Vector2(x, y), new Vector2(1.5f, 14f + (i % 3) * 6f), new Color(0.72f, 0.80f, 0.88f, 0.10f + (i % 4) * 0.02f), 0);
        }
    }

    public static void AddFilmGrain(Node2D root, float startX, float width, int count = 80)
    {
        RandomNumberGenerator rng = new();
        rng.Seed = 42;
        for (int i = 0; i < count; i++)
        {
            float gx = startX + rng.Randf() * width;
            float gy = 120f + rng.Randf() * 520f;
            float a = 0.04f + rng.Randf() * 0.06f;
            AddRect(root, $"Grain{i}", new Vector2(gx, gy), new Vector2(2f, 2f), new Color(1f, 1f, 1f, a), 0);
        }
    }

    public static void AddAsphaltGrime(Node2D root, float startX, float y, float width, int z = 1)
    {
        RandomNumberGenerator rng = new();
        rng.Seed = 91;
        for (int i = 0; i < 72; i++)
        {
            float x = startX + rng.Randf() * width;
            float gy = y + rng.Randf() * 95f;
            float w = 2f + rng.Randf() * 10f;
            float a = 0.08f + rng.Randf() * 0.22f;
            AddRect(root, $"AsphaltGrime{i}", new Vector2(x, gy), new Vector2(w, 1.5f + rng.Randf() * 3f), new Color(0.02f, 0.024f, 0.028f, a), z);
        }

        for (int i = 0; i < 18; i++)
        {
            float x = startX + rng.Randf() * width;
            float gy = y + 20f + rng.Randf() * 70f;
            AddPoly(root, $"AsphaltCrack{i}", new Color(0.03f, 0.035f, 0.038f, 0.35f), [
                new Vector2(x, gy), new Vector2(x + 18f + rng.Randf() * 40f, gy - 2f),
                new Vector2(x + 16f + rng.Randf() * 38f, gy + 4f), new Vector2(x - 2f, gy + 3f)
            ], z + 1);
        }
    }

    public static void AddWallGrunge(Node2D root, float x, float y, float width, float height, int seed, int z = 2)
    {
        RandomNumberGenerator rng = new();
        rng.Seed = (uint)seed;
        for (int i = 0; i < 14; i++)
        {
            float sx = x + rng.Randf() * (width - 24f);
            float sy = y + rng.Randf() * (height - 12f);
            AddRect(root, $"WallStain{seed}_{i}", new Vector2(sx, sy), new Vector2(18f + rng.Randf() * 40f, 4f + rng.Randf() * 8f), new Color(0f, 0f, 0f, 0.06f + rng.Randf() * 0.12f), z);
        }

        for (int i = 0; i < 6; i++)
        {
            float sx = x + rng.Randf() * width;
            float sy = y + rng.Randf() * height;
            AddPoly(root, $"WallDrip{seed}_{i}", new Color(0.08f, 0.06f, 0.05f, 0.18f), [
                new Vector2(sx, sy), new Vector2(sx + 6f, sy), new Vector2(sx + 4f, sy + 22f + rng.Randf() * 18f), new Vector2(sx - 2f, sy + 20f)
            ], z + 1);
        }
    }

    public static void AddDepthFog(Node2D root, float startX, float width, float y, int z = 1)
    {
        AddPoly(root, "DepthFogBand", new Color(0.18f, 0.12f, 0.08f, 0.12f), [
            new Vector2(startX, y), new Vector2(startX + width, y - 18f),
            new Vector2(startX + width, y + 42f), new Vector2(startX, y + 58f)
        ], z);
    }

    private static ColorRect AddRect(Node parent, string name, Vector2 position, Vector2 size, Color color, int z)
    {
        ColorRect rect = new()
        {
            Name = name,
            Position = position,
            Size = size,
            Color = color,
            ZIndex = z
        };
        parent.AddChild(rect);
        return rect;
    }

    private static Polygon2D AddPoly(Node parent, string name, Color color, Vector2[] points, int z)
    {
        Polygon2D poly = new()
        {
            Name = name,
            Color = color,
            Polygon = points,
            ZIndex = z
        };
        parent.AddChild(poly);
        return poly;
    }
}
