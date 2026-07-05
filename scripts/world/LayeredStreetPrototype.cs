namespace SangueNoAsfalto.World;

/// <summary>
/// Rua da Vila Esperanca montada em camadas nativas do Godot. E um prototipo vivo:
/// sem bitmap de referencia ativo, mas com elementos separados para animar e substituir por arte final.
/// </summary>
public partial class LayeredStreetPrototype : Node2D
{
    private readonly List<CanvasItem> _neonItems = [];
    private readonly List<Node2D> _windItems = [];
    private readonly List<CanvasItem> _wetHighlights = [];
    private float _time;

    public override void _Ready()
    {
        BuildStreet();
    }

    public override void _Process(double delta)
    {
        _time += (float)delta;
        float neonPulse = 0.55f + Mathf.Sin(_time * 7.5f) * 0.28f + Mathf.Sin(_time * 17.0f) * 0.08f;
        float wind = Mathf.Sin(_time * 2.4f) * 0.8f;
        float water = 0.58f + Mathf.Sin(_time * 2.0f) * 0.18f;

        foreach (CanvasItem item in _neonItems)
        {
            item.Modulate = new Color(1f, 0.56f, 0.24f, Mathf.Clamp(neonPulse, 0.18f, 1f));
        }

        for (int i = 0; i < _windItems.Count; i++)
        {
            _windItems[i].Rotation = wind * (0.012f + i * 0.002f);
        }

        foreach (CanvasItem item in _wetHighlights)
        {
            Color color = item.Modulate;
            color.A = water;
            item.Modulate = color;
        }
    }

    private void BuildStreet()
    {
        AddSky();

        Node2D far = AddLayer("FarFavela", new Vector2(0f, 10f), -30);
        Node2D farRoot = AddNode(far, "FarRoot", Vector2.Zero, 0);
        AddFarFavela(farRoot);

        Node2D mid = AddLayer("MidStreet", new Vector2(0f, 28f), -20);
        Node2D midRoot = AddNode(mid, "MidRoot", Vector2.Zero, 0);
        AddMidStreet(midRoot);

        Node2D near = AddLayer("NearStreet", new Vector2(0f, 44f), -10);
        Node2D nearRoot = AddNode(near, "NearRoot", Vector2.Zero, 0);
        AddNearStreet(nearRoot);

        AddVignette();
    }

    private void AddSky()
    {
        ColorRect sky = new()
        {
            Name = "SkyWash",
            OffsetLeft = -1600f,
            OffsetTop = -40f,
            OffsetRight = 1600f,
            OffsetBottom = 720f,
            Color = new Color(0.018f, 0.023f, 0.03f)
        };
        AddChild(sky);

        AddPoly(this, "DistantGlow", new Color(0.65f, 0.23f, 0.12f, 0.12f), [
            new Vector2(-1600f, 120f), new Vector2(1600f, 88f), new Vector2(1600f, 250f), new Vector2(-1600f, 280f)
        ], 0);
    }

    private void AddFarFavela(Node2D root)
    {
        AddPoly(root, "HillMass", new Color(0.025f, 0.03f, 0.034f), [
            new Vector2(-1500f, 255f), new Vector2(-1220f, 175f), new Vector2(-980f, 235f), new Vector2(-760f, 160f),
            new Vector2(-420f, 230f), new Vector2(-120f, 150f), new Vector2(180f, 220f), new Vector2(520f, 140f),
            new Vector2(900f, 230f), new Vector2(1280f, 155f), new Vector2(1500f, 210f), new Vector2(1500f, 330f), new Vector2(-1500f, 330f)
        ], 0);

        for (int i = 0; i < 18; i++)
        {
            float x = -1180f + i * 145f;
            float y = 142f + (i % 4) * 18f;
            float h = 78f + (i % 3) * 22f;
            AddRect(root, $"FarHouse{i}", new Vector2(x, y), new Vector2(92f, h), new Color(0.045f, 0.05f, 0.055f), 1);
            AddWindowCluster(root, x + 12f, y + 12f, i);
        }

        AddWire(root, "FarWireA", -1500f, 150f, 1500f, 112f, 0.018f, 4);
        AddWire(root, "FarWireB", -1500f, 188f, 1500f, 174f, 0.014f, 4);
    }

    private void AddMidStreet(Node2D root)
    {
        AddRect(root, "WallBase", new Vector2(-1500f, 170f), new Vector2(3000f, 150f), new Color(0.08f, 0.085f, 0.08f), 0);
        AddRect(root, "SidewalkBack", new Vector2(-1500f, 318f), new Vector2(3000f, 54f), new Color(0.18f, 0.18f, 0.16f), 3);
        AddRect(root, "SidewalkFront", new Vector2(-1500f, 366f), new Vector2(3000f, 20f), new Color(0.10f, 0.105f, 0.10f), 4);

        AddBoteco(root, -1120f, 122f);
        AddFence(root, -450f, 178f, 850f);
        AddForSaleSign(root, 760f, 172f);
        AddPosterWall(root, 70f, 185f);
        AddStreetPole(root, -730f, 120f, true);
        AddStreetPole(root, 360f, 126f, false);
        AddStreetPole(root, 1110f, 116f, true);
        AddWire(root, "MidWireA", -1400f, 100f, 1400f, 88f, 0.028f, 8);
        AddWire(root, "MidWireB", -1400f, 134f, 1400f, 151f, 0.024f, 8);
    }

    private void AddNearStreet(Node2D root)
    {
        AddRect(root, "WetAsphaltBack", new Vector2(-1500f, 385f), new Vector2(3000f, 115f), new Color(0.038f, 0.047f, 0.048f), 0);
        AddRect(root, "WetAsphaltFront", new Vector2(-1500f, 500f), new Vector2(3000f, 220f), new Color(0.024f, 0.028f, 0.03f), 0);

        for (int i = 0; i < 12; i++)
        {
            float x = -1320f + i * 240f;
            AddPoly(root, $"LaneScratch{i}", new Color(0.54f, 0.45f, 0.27f, 0.20f), [
                new Vector2(x, 574f), new Vector2(x + 145f, 570f), new Vector2(x + 155f, 578f), new Vector2(x + 12f, 584f)
            ], 2);
        }

        AddPuddle(root, -950f, 448f, 180f, 25f, new Color(0.35f, 0.52f, 0.58f, 0.18f));
        AddPuddle(root, -280f, 610f, 270f, 36f, new Color(0.55f, 0.40f, 0.22f, 0.18f));
        AddPuddle(root, 430f, 470f, 220f, 28f, new Color(0.42f, 0.55f, 0.60f, 0.20f));
        AddPuddle(root, 980f, 650f, 300f, 42f, new Color(0.45f, 0.22f, 0.13f, 0.20f));
        AddBlood(root, -1050f, 624f);
        AddBlood(root, 700f, 580f);
        AddTrash(root, -520f, 526f);
        AddTrash(root, 1180f, 520f);
        AddDrain(root, 120f, 560f);
    }

    private Node2D AddLayer(string name, Vector2 position, int z)
    {
        Node2D layer = new()
        {
            Name = name,
            Position = position,
            ZIndex = z
        };
        AddChild(layer);
        return layer;
    }

    private static Node2D AddNode(Node parent, string name, Vector2 position, int z)
    {
        Node2D node = new() { Name = name, Position = position, ZIndex = z };
        parent.AddChild(node);
        return node;
    }

    private void AddBoteco(Node2D root, float x, float y)
    {
        AddRect(root, "BotecoWall", new Vector2(x, y), new Vector2(440f, 196f), new Color(0.105f, 0.095f, 0.075f), 2);
        AddRect(root, "BotecoDoor", new Vector2(x + 150f, y + 68f), new Vector2(150f, 128f), new Color(0.035f, 0.036f, 0.035f), 4);
        for (int i = 0; i < 6; i++)
        {
            AddRect(root, $"BotecoDoorLine{i}", new Vector2(x + 150f, y + 82f + i * 18f), new Vector2(150f, 4f), new Color(0.32f, 0.29f, 0.21f, 0.45f), 5);
        }

        AddPoly(root, "BotecoAwning", new Color(0.60f, 0.04f, 0.025f), [
            new Vector2(x - 20f, y + 44f), new Vector2(x + 468f, y + 44f), new Vector2(x + 435f, y + 84f), new Vector2(x + 8f, y + 76f)
        ], 7);
        Label neon = AddLabel(root, "BotecoNeon", "BOTECO DO ZE", new Vector2(x + 42f, y + 52f), 24, new Color(1f, 0.48f, 0.16f), 8);
        _neonItems.Add(neon);
        AddRect(root, "BottleGlow", new Vector2(x + 30f, y + 114f), new Vector2(92f, 28f), new Color(0.9f, 0.55f, 0.16f, 0.16f), 8);
        AddPoly(root, "WallGraffiti", new Color(0.56f, 0.03f, 0.035f, 0.75f), [
            new Vector2(x + 18f, y + 154f), new Vector2(x + 86f, y + 132f), new Vector2(x + 138f, y + 162f), new Vector2(x + 94f, y + 176f)
        ], 8);
    }

    private void AddFence(Node2D root, float x, float y, float width)
    {
        AddRect(root, "FenceBack", new Vector2(x, y + 38f), new Vector2(width, 94f), new Color(0.025f, 0.03f, 0.028f), 2);
        for (int i = 0; i < 18; i++)
        {
            float px = x + i * (width / 18f);
            Node2D plank = AddNode(root, $"FencePlank{i}", new Vector2(px, y + (i % 3) * 8f), 6);
            AddPoly(plank, "Wood", new Color(0.19f, 0.14f, 0.095f), [
                new Vector2(-8f, 0f), new Vector2(9f, -6f), new Vector2(12f, 132f), new Vector2(-10f, 136f)
            ], 0);
            _windItems.Add(plank);
        }
    }

    private void AddForSaleSign(Node2D root, float x, float y)
    {
        AddRect(root, "SignBoard", new Vector2(x, y), new Vector2(175f, 72f), new Color(0.56f, 0.52f, 0.42f), 7);
        AddRect(root, "SignRed", new Vector2(x + 8f, y + 8f), new Vector2(159f, 22f), new Color(0.42f, 0.045f, 0.035f), 8);
        AddLabel(root, "SignText", "VENDE-SE", new Vector2(x + 18f, y + 9f), 18, new Color(0.84f, 0.74f, 0.55f), 9);
        AddLabel(root, "SignPhone", "9 9123-4567", new Vector2(x + 17f, y + 40f), 14, new Color(0.17f, 0.11f, 0.08f), 9);
    }

    private void AddPosterWall(Node2D root, float x, float y)
    {
        AddRect(root, "PosterPaper", new Vector2(x, y), new Vector2(74f, 104f), new Color(0.68f, 0.62f, 0.48f), 7);
        AddPoly(root, "PosterBlood", new Color(0.42f, 0.025f, 0.025f, 0.85f), [
            new Vector2(x + 22f, y + 34f), new Vector2(x + 50f, y + 28f), new Vector2(x + 58f, y + 62f), new Vector2(x + 30f, y + 76f)
        ], 8);
    }

    private void AddStreetPole(Node2D root, float x, float y, bool lit)
    {
        AddRect(root, $"Pole{x}", new Vector2(x, y), new Vector2(14f, 230f), new Color(0.10f, 0.105f, 0.095f), 8);
        AddPoly(root, $"Lamp{x}", lit ? new Color(1f, 0.60f, 0.16f, 0.56f) : new Color(0.42f, 0.36f, 0.24f, 0.22f), [
            new Vector2(x - 8f, y + 10f), new Vector2(x + 74f, y + 3f), new Vector2(x + 104f, y + 38f), new Vector2(x + 44f, y + 66f), new Vector2(x - 20f, y + 42f)
        ], 5);
    }

    private void AddWindowCluster(Node2D root, float x, float y, int seed)
    {
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                if ((row + col + seed) % 3 == 0)
                {
                    AddRect(root, $"Window{seed}_{row}_{col}", new Vector2(x + col * 22f, y + row * 18f), new Vector2(9f, 9f), new Color(0.88f, 0.55f, 0.25f, 0.65f), 2);
                }
            }
        }
    }

    private void AddWire(Node2D root, string name, float x1, float y1, float x2, float y2, float thickness, int z)
    {
        AddPoly(root, name, new Color(0.005f, 0.006f, 0.006f, 0.92f), [
            new Vector2(x1, y1), new Vector2(x2, y2), new Vector2(x2, y2 + thickness * 120f), new Vector2(x1, y1 + thickness * 120f)
        ], z);
    }

    private void AddPuddle(Node2D root, float x, float y, float width, float height, Color color)
    {
        Polygon2D puddle = AddPoly(root, $"Puddle{x}", color, [
            new Vector2(x, y), new Vector2(x + width * 0.35f, y - height), new Vector2(x + width, y - height * 0.25f), new Vector2(x + width * 0.78f, y + height * 0.65f), new Vector2(x + width * 0.12f, y + height * 0.5f)
        ], 3);
        _wetHighlights.Add(puddle);
    }

    private void AddBlood(Node2D root, float x, float y)
    {
        AddPoly(root, $"Blood{x}", new Color(0.30f, 0.004f, 0.008f, 0.78f), [
            new Vector2(x, y), new Vector2(x + 78f, y - 20f), new Vector2(x + 165f, y - 5f), new Vector2(x + 110f, y + 28f), new Vector2(x + 18f, y + 22f)
        ], 4);
    }

    private void AddTrash(Node2D root, float x, float y)
    {
        AddPoly(root, $"TrashBag{x}", new Color(0.008f, 0.012f, 0.013f), [
            new Vector2(x, y), new Vector2(x + 18f, y - 28f), new Vector2(x + 48f, y - 18f), new Vector2(x + 65f, y + 16f), new Vector2(x + 28f, y + 24f)
        ], 8);
        AddPoly(root, $"TrashHi{x}", new Color(0.18f, 0.22f, 0.22f, 0.45f), [
            new Vector2(x + 14f, y - 2f), new Vector2(x + 32f, y - 14f), new Vector2(x + 51f, y + 4f), new Vector2(x + 28f, y + 9f)
        ], 9);
    }

    private void AddDrain(Node2D root, float x, float y)
    {
        AddPoly(root, "Drain", new Color(0.004f, 0.005f, 0.005f), [
            new Vector2(x, y), new Vector2(x + 82f, y - 12f), new Vector2(x + 110f, y + 7f), new Vector2(x + 82f, y + 24f), new Vector2(x + 4f, y + 24f), new Vector2(x - 20f, y + 8f)
        ], 5);
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
            Size = new Vector2(260f, 40f)
        };
        label.AddThemeFontSizeOverride("font_size", fontSize);
        label.AddThemeColorOverride("font_color", color);
        parent.AddChild(label);
        return label;
    }

    private void AddVignette()
    {
        ColorRect vignette = new()
        {
            Name = "Vignette",
            OffsetLeft = -1600f,
            OffsetTop = 0f,
            OffsetRight = 1600f,
            OffsetBottom = 720f,
            MouseFilter = Control.MouseFilterEnum.Ignore,
            Color = new Color(0f, 0f, 0f, 0.20f),
            ZIndex = 100
        };
        AddChild(vignette);
    }
}
