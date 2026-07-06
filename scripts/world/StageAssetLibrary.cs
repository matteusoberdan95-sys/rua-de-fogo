namespace SangueNoAsfalto.World;

/// <summary>
/// Biblioteca de props da Vila Esperanca — blocagem rica pronta para substituicao por sprite/tile final.
/// Cada metodo monta nodes separados (nao bitmap chapado) conforme docs/STAGE_ASSET_PIPELINE.md.
/// </summary>
public static class StageAssetLibrary
{
    // Paleta oficial (VISUAL_BIBLE.md)
    public static readonly Color AsphaltBlack = new(0.043f, 0.051f, 0.055f);
    public static readonly Color ConcreteCold = new(0.169f, 0.188f, 0.2f);
    public static readonly Color PostLight = new(0.839f, 0.631f, 0.227f);
    public static readonly Color BloodDark = new(0.353f, 0.02f, 0.031f, 0.82f);
    public static readonly Color NeonOrange = new(1f, 0.48f, 0.16f);

    public static void BuildBotecoDoZe(Node2D root, StageAssetContext ctx, float x, float y)
    {
        AddRect(root, "BotecoWall", new Vector2(x, y), new Vector2(460f, 204f), new Color(0.105f, 0.095f, 0.075f), 2);
        AddRect(root, "BotecoTrim", new Vector2(x, y + 196f), new Vector2(460f, 8f), new Color(0.06f, 0.055f, 0.045f), 3);

        AddRect(root, "BotecoDoor", new Vector2(x + 158f, y + 72f), new Vector2(156f, 132f), new Color(0.028f, 0.03f, 0.028f), 4);
        for (int i = 0; i < 7; i++)
        {
            AddRect(root, $"BotecoShutterLine{i}", new Vector2(x + 158f, y + 84f + i * 17f), new Vector2(156f, 4f), new Color(0.32f, 0.29f, 0.21f, 0.5f), 5);
        }

        AddPoly(root, "BotecoAwning", new Color(0.60f, 0.04f, 0.025f), [
            new Vector2(x - 24f, y + 40f), new Vector2(x + 488f, y + 40f), new Vector2(x + 452f, y + 88f), new Vector2(x + 4f, y + 78f)
        ], 7);

        Label neon = AddLabel(root, "BotecoNeon", "BOTECO DO ZE", new Vector2(x + 48f, y + 48f), 26, NeonOrange, 8);
        ctx.NeonItems.Add(neon);

        AddRect(root, "SkolSign", new Vector2(x + 318f, y + 88f), new Vector2(72f, 38f), new Color(0.78f, 0.12f, 0.08f), 8);
        AddLabel(root, "SkolText", "SKOL", new Vector2(x + 328f, y + 94f), 16, new Color(0.95f, 0.88f, 0.42f), 9);

        AddRect(root, "BotecoWindow", new Vector2(x + 38f, y + 118f), new Vector2(88f, 52f), new Color(0.04f, 0.045f, 0.05f, 0.92f), 6);
        ColorRect windowGlow = AddRect(root, "WindowGlow", new Vector2(x + 42f, y + 122f), new Vector2(80f, 44f), new Color(0.9f, 0.55f, 0.16f, 0.14f), 7);
        ctx.NeonItems.Add(windowGlow);

        for (int i = 0; i < 3; i++)
        {
            AddRect(root, $"Table{i}", new Vector2(x + 24f + i * 52f, y + 178f), new Vector2(38f, 6f), new Color(0.14f, 0.11f, 0.08f), 9);
            AddPoly(root, $"Bottle{i}", new Color(0.38f, 0.62f, 0.22f, 0.75f), [
                new Vector2(x + 36f + i * 52f, y + 168f), new Vector2(x + 44f + i * 52f, y + 152f), new Vector2(x + 50f + i * 52f, y + 168f)
            ], 10);
        }

        AddPoly(root, "WallGraffiti", new Color(0.56f, 0.03f, 0.035f, 0.78f), [
            new Vector2(x + 22f, y + 158f), new Vector2(x + 96f, y + 134f), new Vector2(x + 152f, y + 166f), new Vector2(x + 98f, y + 182f)
        ], 8);
    }

    public static void BuildMercadinho(Node2D root, StageAssetContext ctx, float x, float y, string title, Color wall, bool neon)
    {
        AddRect(root, $"StoreWall{title}", new Vector2(x, y), new Vector2(380f, 176f), wall, 4);
        AddRect(root, $"StoreRollDoor{title}", new Vector2(x + 96f, y + 64f), new Vector2(200f, 112f), new Color(0.07f, 0.072f, 0.068f), 6);
        for (int i = 0; i < 7; i++)
        {
            AddRect(root, $"StoreRollLine{title}{i}", new Vector2(x + 96f, y + 74f + i * 14f), new Vector2(200f, 3f), new Color(0.38f, 0.34f, 0.24f, 0.42f), 7);
        }

        AddPoly(root, $"StoreAwning{title}", new Color(0.12f, 0.38f, 0.18f, 0.92f), [
            new Vector2(x - 16f, y + 36f), new Vector2(x + 396f, y + 36f), new Vector2(x + 368f, y + 72f), new Vector2(x + 6f, y + 70f)
        ], 8);

        Label label = AddLabel(root, $"StoreLabel{title}", title, new Vector2(x + 48f, y + 40f), 22, new Color(0.92f, 0.70f, 0.25f), 9);
        if (neon)
        {
            ctx.NeonItems.Add(label);
            ColorRect bleed = AddRect(root, $"NeonBleed{title}", new Vector2(x + 36f, y + 74f), new Vector2(250f, 32f), new Color(0.9f, 0.34f, 0.10f, 0.14f), 8);
            ctx.NeonItems.Add(bleed);
        }

        AddRect(root, $"PriceTag{title}", new Vector2(x + 28f, y + 118f), new Vector2(64f, 28f), new Color(0.62f, 0.58f, 0.42f), 9);
        AddLabel(root, $"PriceText{title}", "FECHADO", new Vector2(x + 34f, y + 122f), 12, new Color(0.18f, 0.12f, 0.08f), 10);
    }

    public static void BuildStreetPole(Node2D root, StageAssetContext ctx, float x, float y, bool lit, bool flicker = false)
    {
        Color poleColor = lit ? PostLight * new Color(0.55f, 0.55f, 0.55f, 1f) : new Color(0.14f, 0.13f, 0.11f);
        AddRect(root, $"PoleBody{x}", new Vector2(x, y), new Vector2(16f, 238f), poleColor, 8);
        AddRect(root, $"PoleCap{x}", new Vector2(x - 2f, y - 6f), new Vector2(20f, 8f), new Color(0.08f, 0.08f, 0.07f), 9);

        Color lampColor = lit
            ? new Color(PostLight.R, PostLight.G, PostLight.B, 0.62f)
            : new Color(0.42f, 0.36f, 0.24f, 0.22f);
        Polygon2D lamp = AddPoly(root, $"LampArm{x}", lampColor, [
            new Vector2(x - 10f, y + 8f), new Vector2(x + 82f, y), new Vector2(x + 112f, y + 36f), new Vector2(x + 48f, y + 68f), new Vector2(x - 22f, y + 44f)
        ], 5);

        if (lit)
        {
            ColorRect bulb = AddRect(root, $"Bulb{x}", new Vector2(x + 72f, y + 18f), new Vector2(18f, 12f), new Color(1f, 0.82f, 0.42f, 0.85f), 6);
            ctx.NeonItems.Add(bulb);

            Polygon2D pool = AddPoly(root, $"LightPool{x}", new Color(PostLight.R, PostLight.G, PostLight.B, 0.16f), [
                new Vector2(x - 48f, y + 248f), new Vector2(x + 140f, y + 242f), new Vector2(x + 168f, y + 268f), new Vector2(x - 72f, y + 274f)
            ], 2);
            ctx.WetHighlights.Add(pool);

            if (flicker)
            {
                ctx.FlickerItems.Add(lamp);
                ctx.FlickerItems.Add(bulb);
                ctx.FlickerItems.Add(pool);
            }
        }
    }

    public static void BuildTrashCluster(Node2D root, StageAssetContext ctx, float x, float y, int variant = 0)
    {
        float offset = variant * 18f;
        AddPoly(root, $"TrashBag{x}", new Color(0.008f, 0.012f, 0.013f), [
            new Vector2(x, y), new Vector2(x + 20f, y - 32f), new Vector2(x + 54f, y - 20f), new Vector2(x + 72f, y + 18f), new Vector2(x + 30f, y + 28f)
        ], 8);
        AddPoly(root, $"TrashHi{x}", new Color(0.18f, 0.22f, 0.22f, 0.48f), [
            new Vector2(x + 16f, y - 4f), new Vector2(x + 36f, y - 16f), new Vector2(x + 58f, y + 2f), new Vector2(x + 32f, y + 10f)
        ], 9);

        if (variant % 2 == 0)
        {
            Node2D can = AddNode(root, $"Can{variant}", new Vector2(x + 78f + offset, y + 8f), 8);
            AddPoly(can, "CanBody", new Color(0.22f, 0.24f, 0.22f), [
                new Vector2(-10f, -18f), new Vector2(12f, -20f), new Vector2(14f, 14f), new Vector2(-12f, 16f)
            ], 0);
        }

        AddLoosePaper(root, ctx, x + 42f, y + 22f, variant + 3);
    }

    public static void BuildFenceSection(Node2D root, StageAssetContext ctx, float x, float y, float width, bool damaged = false)
    {
        AddRect(root, "FenceBack", new Vector2(x, y + 38f), new Vector2(width, 98f), new Color(0.025f, 0.03f, 0.028f), 2);
        int planks = Mathf.CeilToInt(width / 48f);
        for (int i = 0; i < planks; i++)
        {
            float px = x + i * (width / planks);
            if (damaged && i == planks / 2)
            {
                continue;
            }

            Node2D plank = AddNode(root, $"FencePlank{i}", new Vector2(px, y + (i % 3) * 8f + (damaged ? 12f : 0f)), 6);
            AddPoly(plank, "Wood", new Color(0.19f, 0.14f, 0.095f), [
                new Vector2(-9f, 0f), new Vector2(10f, -6f), new Vector2(13f, 136f), new Vector2(-11f, 140f)
            ], 0);
            ctx.WindItems.Add(plank);
        }
    }

    public static void BuildPuddle(Node2D root, StageAssetContext ctx, float x, float y, float width, float height, Color baseColor)
    {
        Polygon2D puddle = AddPoly(root, $"Puddle{x}", baseColor, [
            new Vector2(x, y), new Vector2(x + width * 0.35f, y - height), new Vector2(x + width, y - height * 0.25f),
            new Vector2(x + width * 0.78f, y + height * 0.65f), new Vector2(x + width * 0.12f, y + height * 0.5f)
        ], 3);
        ctx.WetHighlights.Add(puddle);

        Polygon2D spec = AddPoly(root, $"PuddleSpec{x}", new Color(0.72f, 0.82f, 0.88f, 0.22f), [
            new Vector2(x + width * 0.22f, y - height * 0.15f), new Vector2(x + width * 0.58f, y - height * 0.35f),
            new Vector2(x + width * 0.72f, y - height * 0.08f), new Vector2(x + width * 0.38f, y + height * 0.05f)
        ], 4);
        ctx.WetHighlights.Add(spec);
    }

    public static void BuildCurbAndLane(Node2D root, float startX, float width)
    {
        AddRect(root, "CurbLine", new Vector2(startX, 382f), new Vector2(width, 6f), new Color(0.22f, 0.21f, 0.18f), 6);
        AddRect(root, "CurbShadow", new Vector2(startX, 388f), new Vector2(width, 3f), new Color(0f, 0f, 0f, 0.28f), 5);

        int dashCount = Mathf.CeilToInt(width / 96f);
        for (int i = 0; i < dashCount; i++)
        {
            float x = startX + i * 96f + 12f;
            AddRect(root, $"LaneDash{i}", new Vector2(x, 548f), new Vector2(52f, 4f), new Color(0.72f, 0.62f, 0.22f, 0.28f), 3);
        }
    }

    public static void BuildAsphaltReadability(Node2D root, StageAssetContext ctx, float startX, float y, float width)
    {
        int count = Mathf.CeilToInt(width / 130f);
        for (int i = 0; i < count; i++)
        {
            float x = startX + i * 130f;
            AddPoly(root, $"AsphaltPatch{i}", new Color(0.065f, 0.07f, 0.067f, 0.32f), [
                new Vector2(x, y + (i % 4) * 18f),
                new Vector2(x + 80f, y - 6f + (i % 3) * 15f),
                new Vector2(x + 126f, y + 8f + (i % 5) * 10f),
                new Vector2(x + 92f, y + 24f + (i % 4) * 12f),
                new Vector2(x + 10f, y + 20f + (i % 2) * 9f)
            ], 1);
            AddRect(root, $"AsphaltCrack{i}", new Vector2(x + 22f, y + 50f + (i % 5) * 12f), new Vector2(92f, 2f), new Color(0.005f, 0.006f, 0.006f, 0.68f), 2);

            if (i % 4 == 1)
            {
                Polygon2D track = AddPoly(root, $"TireTrack{i}", new Color(0.02f, 0.022f, 0.024f, 0.55f), [
                    new Vector2(x + 18f, y + 78f), new Vector2(x + 108f, y + 72f), new Vector2(x + 104f, y + 86f), new Vector2(x + 14f, y + 92f)
                ], 2);
                ctx.WetHighlights.Add(track);
            }
        }
    }

    public static void BuildSidewalkTiles(Node2D root, float startX, float y, float width)
    {
        int count = Mathf.CeilToInt(width / 86f);
        for (int i = 0; i < count; i++)
        {
            float x = startX + i * 86f;
            Color tile = i % 2 == 0
                ? new Color(0.19f, 0.185f, 0.165f, 0.95f)
                : new Color(0.15f, 0.152f, 0.14f, 0.95f);
            AddRect(root, $"SidewalkTile{i}", new Vector2(x, y), new Vector2(84f, 28f), tile, 5);
            AddRect(root, $"SidewalkCrack{i}", new Vector2(x + 8f + (i % 4) * 7f, y + 10f), new Vector2(42f, 2f), new Color(0.035f, 0.038f, 0.036f, 0.72f), 6);
            if (i % 5 == 2)
            {
                AddRect(root, $"SidewalkWeed{i}", new Vector2(x + 28f, y + 18f), new Vector2(18f, 6f), new Color(0.18f, 0.28f, 0.12f, 0.55f), 7);
            }
        }
    }

    public static void BuildEntranceArch(Node2D root, StageAssetContext ctx, float x, float y)
    {
        AddRect(root, "ArchLeft", new Vector2(x - 8f, y), new Vector2(18f, 148f), new Color(0.12f, 0.11f, 0.09f), 6);
        AddRect(root, "ArchRight", new Vector2(x + 132f, y), new Vector2(18f, 148f), new Color(0.12f, 0.11f, 0.09f), 6);
        AddRect(root, "ArchTop", new Vector2(x - 12f, y - 8f), new Vector2(172f, 22f), new Color(0.14f, 0.12f, 0.10f), 7);
        Label sign = AddLabel(root, "EntranceSign", "VILA ESPERANCA", new Vector2(x + 4f, y + 18f), 18, new Color(0.88f, 0.72f, 0.28f), 8);
        ctx.NeonItems.Add(sign);
        AddLabel(root, "EntranceSub", "ENTRADA", new Vector2(x + 38f, y + 48f), 14, new Color(0.62f, 0.58f, 0.48f), 8);
    }

    public static void BuildAlleyCompression(Node2D root, float x, float y, float width)
    {
        AddRect(root, "AlleyWallLeft", new Vector2(x, y), new Vector2(28f, 168f), new Color(0.06f, 0.065f, 0.06f), 3);
        AddRect(root, "AlleyWallRight", new Vector2(x + width - 28f, y), new Vector2(28f, 168f), new Color(0.06f, 0.065f, 0.06f), 3);
        AddPoly(root, "AlleyShadow", new Color(0f, 0f, 0f, 0.22f), [
            new Vector2(x + 20f, y + 160f), new Vector2(x + width - 20f, y + 160f), new Vector2(x + width - 40f, y + 320f), new Vector2(x + 40f, y + 320f)
        ], 1);
    }

    public static void BuildTirePile(Node2D root, float x, float y)
    {
        for (int i = 0; i < 4; i++)
        {
            float ox = i * 28f;
            float oy = (i % 2) * 14f;
            AddPoly(root, $"Tire{i}", new Color(0.04f, 0.042f, 0.04f), [
                new Vector2(x + ox, y + oy), new Vector2(x + 24f + ox, y - 8f + oy), new Vector2(x + 26f + ox, y + 18f + oy), new Vector2(x + 2f + ox, y + 22f + oy)
            ], 8);
            AddPoly(root, $"TireRing{i}", new Color(0.12f, 0.11f, 0.10f, 0.65f), [
                new Vector2(x + 8f + ox, y + 4f + oy), new Vector2(x + 20f + ox, y + 2f + oy), new Vector2(x + 18f + ox, y + 14f + oy), new Vector2(x + 6f + ox, y + 16f + oy)
            ], 9);
        }
    }

    public static void BuildExitFogBand(Node2D root, float x, float y, float width)
    {
        AddPoly(root, "ExitFog", new Color(0.42f, 0.48f, 0.52f, 0.12f), [
            new Vector2(x, y + 80f), new Vector2(x + width, y + 60f), new Vector2(x + width, y + 220f), new Vector2(x, y + 240f)
        ], -65);
    }

    private static void AddLoosePaper(Node2D root, StageAssetContext ctx, float x, float y, int seed)
    {
        Node2D paper = AddNode(root, $"LoosePaper{seed}", new Vector2(x, y), 9);
        paper.Rotation = (seed % 7 - 3) * 0.12f;
        AddPoly(paper, "Paper", new Color(0.56f, 0.52f, 0.40f, 0.58f), [
            new Vector2(-12f, -7f), new Vector2(14f, -5f), new Vector2(10f, 10f), new Vector2(-10f, 8f)
        ], 0);
        ctx.WindItems.Add(paper);
    }

    private static Node2D AddNode(Node parent, string name, Vector2 position, int z)
    {
        Node2D node = new() { Name = name, Position = position, ZIndex = z };
        parent.AddChild(node);
        return node;
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

    private static Label AddLabel(Node parent, string name, string text, Vector2 position, int fontSize, Color color, int z)
    {
        Label label = new()
        {
            Name = name,
            Text = text,
            Position = position,
            ZIndex = z,
            Size = new Vector2(280f, 40f)
        };
        label.AddThemeFontSizeOverride("font_size", fontSize);
        label.AddThemeColorOverride("font_color", color);
        parent.AddChild(label);
        return label;
    }

    public static void BuildBusShelter(Node2D root, float x, float y)
    {
        AddRect(root, "BusRoof", new Vector2(x - 74f, y - 50f), new Vector2(148f, 14f), new Color(0.14f, 0.13f, 0.11f), 6);
        AddRect(root, "BusBack", new Vector2(x - 66f, y - 36f), new Vector2(132f, 90f), new Color(0.08f, 0.085f, 0.08f, 0.88f), 5);
        AddRect(root, "BusBench", new Vector2(x - 52f, y + 16f), new Vector2(104f, 12f), new Color(0.22f, 0.18f, 0.14f), 7);
        AddLabel(root, "BusSign", "ONIBUS", new Vector2(x - 38f, y - 40f), 14, new Color(0.82f, 0.72f, 0.28f), 8);
        AddRect(root, "BusAd", new Vector2(x - 58f, y - 28f), new Vector2(48f, 36f), new Color(0.52f, 0.48f, 0.38f, 0.65f), 6);
    }
}
