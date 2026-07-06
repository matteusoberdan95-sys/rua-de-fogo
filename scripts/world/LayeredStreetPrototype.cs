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

        AddExtendedStreetSegments();
        AddStageOneProductionDress();
        AddImmersionAlongRoute();
        AddDestructibles();
        AddStageExitGate();
        AddForegroundAtmosphere();
        AddVignette();
    }

    private void AddExtendedStreetSegments()
    {
        Node2D? nearRoot = GetNodeOrNull<Node2D>("NearStreet/NearRoot");
        Node2D? midRoot = GetNodeOrNull<Node2D>("MidStreet/MidRoot");
        if (nearRoot is null || midRoot is null)
        {
            return;
        }

        float[] segmentStarts = [1560f, 2160f, 2760f];
        foreach (float start in segmentStarts)
        {
            AddRect(midRoot, $"ExtWall{start}", new Vector2(start - 120f, 170f), new Vector2(620f, 150f), new Color(0.08f, 0.085f, 0.08f), 0);
            AddRect(midRoot, $"ExtWalk{start}", new Vector2(start - 120f, 318f), new Vector2(620f, 54f), new Color(0.18f, 0.18f, 0.16f), 3);
            AddRect(nearRoot, $"ExtAsphalt{start}", new Vector2(start - 120f, 385f), new Vector2(620f, 115f), new Color(0.038f, 0.047f, 0.048f), 0);
            AddRect(nearRoot, $"ExtLane{start}", new Vector2(start - 120f, 500f), new Vector2(620f, 220f), new Color(0.024f, 0.028f, 0.03f), 0);
            AddStreetPole(midRoot, start + 40f, 126f, start % 320f < 160f);
            AddPuddle(nearRoot, start + 80f, 470f, 180f, 24f, new Color(0.42f, 0.55f, 0.60f, 0.18f));
            AddTrash(nearRoot, start + 220f, 520f);
        }
    }

    private void AddStageExitGate()
    {
        Node2D root = AddNode(this, "StageExitGate", new Vector2(3090f, 0f), 15);
        AddRect(root, "ExitArch", new Vector2(-70f, 120f), new Vector2(140f, 210f), new Color(0.12f, 0.11f, 0.09f), 0);
        AddRect(root, "ExitGlow", new Vector2(-52f, 138f), new Vector2(104f, 170f), new Color(0.22f, 0.62f, 0.38f, 0.35f), 1);
        Label exit = AddLabel(root, "ExitLabel", "SAIDA", new Vector2(-34f, 152f), 20, new Color(0.92f, 0.88f, 0.55f), 2);
        _neonItems.Add(exit);
        AddPoly(root, "ExitArrow", new Color(0.95f, 0.78f, 0.22f, 0.85f), [
            new Vector2(0f, 250f), new Vector2(18f, 280f), new Vector2(-18f, 280f)
        ], 3);
    }

    private void AddSky()
    {
        ColorRect sky = new()
        {
            Name = "SkyWash",
            OffsetLeft = -1100f,
            OffsetTop = -40f,
            OffsetRight = 3900f,
            OffsetBottom = 720f,
            Color = new Color(0.018f, 0.023f, 0.03f)
        };
        AddChild(sky);

        AddPoly(this, "DistantGlow", new Color(0.65f, 0.23f, 0.12f, 0.12f), [
            new Vector2(-1100f, 120f), new Vector2(3900f, 88f), new Vector2(3900f, 250f), new Vector2(-1100f, 280f)
        ], 0);
    }

    private void AddFarFavela(Node2D root)
    {
        AddPoly(root, "HillMass", new Color(0.025f, 0.03f, 0.034f), [
            new Vector2(-1100f, 255f), new Vector2(-820f, 175f), new Vector2(-580f, 235f), new Vector2(-360f, 160f),
            new Vector2(-20f, 230f), new Vector2(320f, 150f), new Vector2(680f, 220f), new Vector2(1040f, 140f),
            new Vector2(1400f, 210f), new Vector2(1760f, 155f), new Vector2(2120f, 225f), new Vector2(2480f, 150f),
            new Vector2(2840f, 210f), new Vector2(3200f, 160f), new Vector2(3560f, 230f), new Vector2(3900f, 190f),
            new Vector2(3900f, 330f), new Vector2(-1100f, 330f)
        ], 0);

        for (int i = 0; i < 32; i++)
        {
            float x = -980f + i * 145f;
            float y = 142f + (i % 4) * 18f;
            float h = 78f + (i % 3) * 22f;
            AddRect(root, $"FarHouse{i}", new Vector2(x, y), new Vector2(92f, h), new Color(0.045f, 0.05f, 0.055f), 1);
            AddWindowCluster(root, x + 12f, y + 12f, i);
        }

        AddWire(root, "FarWireA", -1100f, 150f, 3900f, 112f, 0.018f, 4);
        AddWire(root, "FarWireB", -1100f, 188f, 3900f, 174f, 0.014f, 4);
    }

    private void AddMidStreet(Node2D root)
    {
        AddRect(root, "WallBase", new Vector2(-1100f, 170f), new Vector2(5000f, 150f), new Color(0.08f, 0.085f, 0.08f), 0);
        AddRect(root, "SidewalkBack", new Vector2(-1100f, 318f), new Vector2(5000f, 54f), new Color(0.18f, 0.18f, 0.16f), 3);
        AddRect(root, "SidewalkFront", new Vector2(-1100f, 366f), new Vector2(5000f, 20f), new Color(0.10f, 0.105f, 0.10f), 4);

        AddBoteco(root, -1120f, 122f);
        AddFence(root, -450f, 178f, 850f);
        AddForSaleSign(root, 760f, 172f);
        AddPosterWall(root, 70f, 185f);
        AddStreetPole(root, -730f, 120f, true);
        AddStreetPole(root, 360f, 126f, false);
        AddStreetPole(root, 1110f, 116f, true);
        AddWire(root, "MidWireA", -1000f, 100f, 3800f, 88f, 0.028f, 8);
        AddWire(root, "MidWireB", -1000f, 134f, 3800f, 151f, 0.024f, 8);
    }

    private void AddNearStreet(Node2D root)
    {
        AddRect(root, "WetAsphaltBack", new Vector2(-1100f, 385f), new Vector2(5000f, 115f), new Color(0.038f, 0.047f, 0.048f), 0);
        AddRect(root, "WetAsphaltFront", new Vector2(-1100f, 500f), new Vector2(5000f, 220f), new Color(0.024f, 0.028f, 0.03f), 0);

        for (int i = 0; i < 22; i++)
        {
            float x = -1020f + i * 240f;
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

    private void AddImmersionAlongRoute()
    {
        Node2D? midRoot = GetNodeOrNull<Node2D>("MidStreet/MidRoot");
        Node2D? nearRoot = GetNodeOrNull<Node2D>("NearStreet/NearRoot");
        if (midRoot is null || nearRoot is null)
        {
            return;
        }

        AddRoadsideAltar(midRoot, 470f, 318f);
        AddBusShelter(midRoot, 1180f, 308f);
        AddBrokenCar(nearRoot, 2050f, 478f);
        AddPoliceSilhouette(nearRoot, 890f, 488f);
        AddLabel(midRoot, "ZoneLabelA", "BARRACO DO MARTINS", new Vector2(1080f, 148f), 16, new Color(0.88f, 0.62f, 0.22f), 9);
        AddLabel(midRoot, "ZoneLabelB", "FIM DA VILA", new Vector2(2480f, 148f), 16, new Color(0.75f, 0.58f, 0.22f), 9);
    }

    private void AddStageOneProductionDress()
    {
        Node2D? midRoot = GetNodeOrNull<Node2D>("MidStreet/MidRoot");
        Node2D? nearRoot = GetNodeOrNull<Node2D>("NearStreet/NearRoot");
        if (midRoot is null || nearRoot is null)
        {
            return;
        }

        AddSidewalkTiles(midRoot, -1080f, 320f, 4550f);
        AddAsphaltTextureRun(nearRoot, -1080f, 430f, 4550f);

        AddStorefront(midRoot, -920f, 145f, "BAR DO ZE", new Color(0.12f, 0.07f, 0.045f), true);
        AddStorefront(midRoot, -120f, 155f, "MERCADINHO", new Color(0.085f, 0.10f, 0.08f), false);
        AddStorefront(midRoot, 1540f, 155f, "OFICINA", new Color(0.10f, 0.088f, 0.075f), false);
        AddStorefront(midRoot, 2220f, 158f, "ACAI E LANCHES", new Color(0.075f, 0.07f, 0.09f), true);

        AddWallGraffiti(midRoot, -550f, 210f, "A RUA NAO ESQUECE", new Color(0.52f, 0.04f, 0.035f, 0.80f));
        AddWallGraffiti(midRoot, 520f, 208f, "JUSTICA PRA TODOS", new Color(0.62f, 0.10f, 0.05f, 0.78f));
        AddWallGraffiti(midRoot, 1880f, 208f, "NINGUEM SOME", new Color(0.70f, 0.58f, 0.22f, 0.70f));
        AddWallGraffiti(midRoot, 2580f, 210f, "SAIDA?", new Color(0.56f, 0.05f, 0.06f, 0.80f));

        AddDumpster(nearRoot, -690f, 500f);
        AddDumpster(nearRoot, 1710f, 506f);
        AddElectricalBox(midRoot, 1350f, 248f);
        AddElectricalBox(midRoot, 2660f, 250f);
        AddHangingClothes(midRoot, 950f, 150f);
        AddHangingClothes(midRoot, 2380f, 142f);
        AddCableBundle(midRoot, 280f, 112f);
        AddCableBundle(midRoot, 1820f, 118f);

        AddRoadPaint(nearRoot, -840f, 585f, "PARE");
        AddRoadPaint(nearRoot, 1420f, 584f, "VILA");
        AddRoadPaint(nearRoot, 2860f, 584f, "SAIDA");

        AddPothole(nearRoot, -180f, 622f, 1.0f);
        AddPothole(nearRoot, 1180f, 618f, 0.8f);
        AddPothole(nearRoot, 2440f, 626f, 1.1f);

        for (int i = 0; i < 28; i++)
        {
            float x = -980f + i * 165f;
            float y = 492f + (i % 5) * 34f;
            AddLoosePaper(nearRoot, x, y, i);
            if (i % 3 == 0)
            {
                AddBottleShard(nearRoot, x + 74f, y + 24f, i);
            }
        }
    }

    private void AddSidewalkTiles(Node2D root, float startX, float y, float width)
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
        }
    }

    private void AddAsphaltTextureRun(Node2D root, float startX, float y, float width)
    {
        int count = Mathf.CeilToInt(width / 130f);
        for (int i = 0; i < count; i++)
        {
            float x = startX + i * 130f;
            AddPoly(root, $"AsphaltPatch{i}", new Color(0.065f, 0.07f, 0.067f, 0.28f), [
                new Vector2(x, y + (i % 4) * 18f),
                new Vector2(x + 80f, y - 6f + (i % 3) * 15f),
                new Vector2(x + 126f, y + 8f + (i % 5) * 10f),
                new Vector2(x + 92f, y + 24f + (i % 4) * 12f),
                new Vector2(x + 10f, y + 20f + (i % 2) * 9f)
            ], 1);
            AddRect(root, $"AsphaltThinCrack{i}", new Vector2(x + 22f, y + 50f + (i % 5) * 12f), new Vector2(92f, 2f), new Color(0.005f, 0.006f, 0.006f, 0.65f), 2);
        }
    }

    private void AddStorefront(Node2D root, float x, float y, string title, Color wall, bool neon)
    {
        AddRect(root, $"StoreWall{title}", new Vector2(x, y), new Vector2(360f, 168f), wall, 4);
        AddRect(root, $"StoreShutter{title}", new Vector2(x + 90f, y + 62f), new Vector2(190f, 106f), new Color(0.085f, 0.087f, 0.08f), 6);
        for (int i = 0; i < 6; i++)
        {
            AddRect(root, $"StoreShutterLine{title}{i}", new Vector2(x + 90f, y + 72f + i * 16f), new Vector2(190f, 3f), new Color(0.34f, 0.31f, 0.22f, 0.35f), 7);
        }

        AddPoly(root, $"StoreAwning{title}", new Color(0.48f, 0.04f, 0.035f, 0.95f), [
            new Vector2(x - 14f, y + 38f), new Vector2(x + 376f, y + 38f), new Vector2(x + 348f, y + 70f), new Vector2(x + 8f, y + 68f)
        ], 8);
        Label label = AddLabel(root, $"StoreLabel{title}", title, new Vector2(x + 42f, y + 42f), 20, new Color(0.92f, 0.70f, 0.25f), 9);
        if (neon)
        {
            _neonItems.Add(label);
            AddRect(root, $"NeonBleed{title}", new Vector2(x + 32f, y + 76f), new Vector2(230f, 28f), new Color(0.9f, 0.34f, 0.10f, 0.12f), 8);
        }
    }

    private void AddWallGraffiti(Node2D root, float x, float y, string text, Color color)
    {
        AddPoly(root, $"GraffitiSplash{x}", color, [
            new Vector2(x, y + 12f), new Vector2(x + 74f, y - 8f), new Vector2(x + 150f, y + 12f), new Vector2(x + 218f, y - 4f),
            new Vector2(x + 260f, y + 18f), new Vector2(x + 190f, y + 34f), new Vector2(x + 120f, y + 24f), new Vector2(x + 42f, y + 38f)
        ], 7);
        AddLabel(root, $"GraffitiText{x}", text, new Vector2(x + 18f, y - 2f), 15, new Color(0.86f, 0.74f, 0.48f, 0.88f), 8);
    }

    private void AddDumpster(Node2D root, float x, float y)
    {
        AddPoly(root, $"Dumpster{x}", new Color(0.075f, 0.14f, 0.12f), [
            new Vector2(x, y), new Vector2(x + 112f, y - 8f), new Vector2(x + 126f, y + 42f), new Vector2(x + 16f, y + 54f)
        ], 8);
        AddRect(root, $"DumpsterLid{x}", new Vector2(x + 6f, y - 14f), new Vector2(118f, 12f), new Color(0.05f, 0.09f, 0.08f), 9);
        AddRect(root, $"DumpsterRust{x}", new Vector2(x + 26f, y + 18f), new Vector2(34f, 8f), new Color(0.45f, 0.15f, 0.05f, 0.62f), 10);
    }

    private void AddElectricalBox(Node2D root, float x, float y)
    {
        AddRect(root, $"ElectricBox{x}", new Vector2(x, y), new Vector2(54f, 70f), new Color(0.18f, 0.18f, 0.15f), 8);
        AddPoly(root, $"ElectricWarn{x}", new Color(0.82f, 0.58f, 0.13f, 0.95f), [
            new Vector2(x + 27f, y + 12f), new Vector2(x + 40f, y + 40f), new Vector2(x + 14f, y + 40f)
        ], 9);
        AddRect(root, $"ElectricSpark{x}", new Vector2(x + 48f, y + 10f), new Vector2(42f, 3f), new Color(0.52f, 0.72f, 1f, 0.45f), 10);
    }

    private void AddHangingClothes(Node2D root, float x, float y)
    {
        AddWire(root, $"ClothesLine{x}", x - 120f, y, x + 140f, y + 8f, 0.012f, 9);
        Color[] colors = [
            new Color(0.50f, 0.04f, 0.04f, 0.90f),
            new Color(0.72f, 0.66f, 0.48f, 0.88f),
            new Color(0.08f, 0.12f, 0.13f, 0.90f),
            new Color(0.23f, 0.31f, 0.18f, 0.86f)
        ];
        for (int i = 0; i < 4; i++)
        {
            Node2D cloth = AddNode(root, $"HangingCloth{x}_{i}", new Vector2(x - 78f + i * 52f, y + 10f), 10);
            AddPoly(cloth, "Cloth", colors[i], [
                new Vector2(-16f, 0f), new Vector2(18f, 2f), new Vector2(14f, 48f), new Vector2(-18f, 42f)
            ], 0);
            _windItems.Add(cloth);
        }
    }

    private void AddCableBundle(Node2D root, float x, float y)
    {
        for (int i = 0; i < 4; i++)
        {
            AddWire(root, $"CableBundle{x}_{i}", x - 150f, y + i * 10f, x + 240f, y + 28f + i * 6f, 0.010f, 10);
        }
    }

    private void AddRoadPaint(Node2D root, float x, float y, string text)
    {
        Label label = AddLabel(root, $"RoadPaint{text}{x}", text, new Vector2(x, y), 18, new Color(0.72f, 0.68f, 0.52f, 0.22f), 4);
        label.Rotation = -0.04f;
        label.Scale = new Vector2(1.4f, 0.72f);
    }

    private void AddPothole(Node2D root, float x, float y, float scale)
    {
        AddPoly(root, $"Pothole{x}", new Color(0.005f, 0.007f, 0.007f, 0.90f), [
            new Vector2(x, y), new Vector2(x + 56f * scale, y - 16f * scale), new Vector2(x + 128f * scale, y - 6f * scale),
            new Vector2(x + 152f * scale, y + 18f * scale), new Vector2(x + 96f * scale, y + 34f * scale), new Vector2(x + 20f * scale, y + 28f * scale)
        ], 5);
        AddPoly(root, $"PotholeWet{x}", new Color(0.36f, 0.46f, 0.50f, 0.18f), [
            new Vector2(x + 18f * scale, y + 6f * scale), new Vector2(x + 86f * scale, y - 6f * scale),
            new Vector2(x + 124f * scale, y + 10f * scale), new Vector2(x + 72f * scale, y + 22f * scale)
        ], 6);
    }

    private void AddLoosePaper(Node2D root, float x, float y, int seed)
    {
        Node2D paper = AddNode(root, $"LoosePaper{seed}", new Vector2(x, y), 9);
        paper.Rotation = (seed % 7 - 3) * 0.12f;
        AddPoly(paper, "Paper", new Color(0.56f, 0.52f, 0.40f, 0.58f), [
            new Vector2(-12f, -7f), new Vector2(14f, -5f), new Vector2(10f, 10f), new Vector2(-10f, 8f)
        ], 0);
        _windItems.Add(paper);
    }

    private void AddBottleShard(Node2D root, float x, float y, int seed)
    {
        AddPoly(root, $"BottleShard{seed}", new Color(0.44f, 0.62f, 0.36f, 0.42f), [
            new Vector2(x, y), new Vector2(x + 18f, y - 6f), new Vector2(x + 30f, y + 6f), new Vector2(x + 10f, y + 12f)
        ], 9);
    }

    private void AddForegroundAtmosphere()
    {
        Node2D fg = AddLayer("ForegroundAtmosphere", Vector2.Zero, 90);
        for (int i = 0; i < 16; i++)
        {
            float x = -980f + i * 300f;
            AddRect(fg, $"RainSplash{i}", new Vector2(x, 646f + (i % 3) * 18f), new Vector2(42f, 2f), new Color(0.62f, 0.78f, 0.85f, 0.16f), 0);
        }
    }

    private void AddRoadsideAltar(Node2D root, float x, float y)
    {
        AddRect(root, "AltarBase", new Vector2(x - 30f, y + 6f), new Vector2(60f, 24f), new Color(0.12f, 0.1f, 0.075f), 6);
        AddPoly(root, "AltarStone", new Color(0.16f, 0.14f, 0.11f), [
            new Vector2(x - 22f, y - 6f), new Vector2(x + 22f, y - 6f), new Vector2(x + 14f, y - 24f), new Vector2(x - 14f, y - 24f)
        ], 7);
        AddRect(root, "AltarCloth", new Vector2(x - 19f, y - 3f), new Vector2(38f, 15f), new Color(0.45f, 0.02f, 0.035f, 0.88f), 8);
        AddPoly(root, "AltarCandle", new Color(1f, 0.58f, 0.14f, 0.58f), [
            new Vector2(x + 4f, y - 30f), new Vector2(x + 14f, y - 14f), new Vector2(x + 4f, y - 1f), new Vector2(x - 6f, y - 14f)
        ], 9);
    }

    private void AddBusShelter(Node2D root, float x, float y)
    {
        AddRect(root, "BusRoof", new Vector2(x - 70f, y - 48f), new Vector2(140f, 12f), new Color(0.14f, 0.13f, 0.11f), 6);
        AddRect(root, "BusBack", new Vector2(x - 62f, y - 36f), new Vector2(124f, 86f), new Color(0.08f, 0.085f, 0.08f, 0.85f), 5);
        AddRect(root, "BusBench", new Vector2(x - 48f, y + 18f), new Vector2(96f, 10f), new Color(0.22f, 0.18f, 0.14f), 7);
        AddLabel(root, "BusSign", "ONIBUS", new Vector2(x - 34f, y - 38f), 14, new Color(0.82f, 0.72f, 0.28f), 8);
    }

    private void AddBrokenCar(Node2D root, float x, float y)
    {
        AddPoly(root, "CarBody", new Color(0.11f, 0.115f, 0.12f), [
            new Vector2(x - 90f, y + 8f), new Vector2(x + 70f, y + 4f), new Vector2(x + 96f, y - 18f), new Vector2(x + 40f, y - 34f),
            new Vector2(x - 40f, y - 30f), new Vector2(x - 98f, y - 8f)
        ], 8);
        AddPoly(root, "CarWindow", new Color(0.04f, 0.05f, 0.06f, 0.9f), [
            new Vector2(x - 18f, y - 26f), new Vector2(x + 34f, y - 28f), new Vector2(x + 22f, y - 12f), new Vector2(x - 10f, y - 10f)
        ], 9);
        AddPoly(root, "CarDent", new Color(0.28f, 0.03f, 0.03f, 0.55f), [
            new Vector2(x + 48f, y - 8f), new Vector2(x + 72f, y - 4f), new Vector2(x + 64f, y + 10f), new Vector2(x + 44f, y + 6f)
        ], 10);
    }

    private void AddPoliceSilhouette(Node2D root, float x, float y)
    {
        AddPoly(root, "PoliceCar", new Color(0.09f, 0.11f, 0.14f), [
            new Vector2(x - 72f, y + 6f), new Vector2(x + 58f, y + 4f), new Vector2(x + 78f, y - 12f), new Vector2(x + 10f, y - 22f), new Vector2(x - 58f, y - 10f)
        ], 8);
        AddRect(root, "PoliceLightL", new Vector2(x - 8f, y - 26f), new Vector2(12f, 6f), new Color(0.9f, 0.15f, 0.12f, 0.75f), 9);
        AddRect(root, "PoliceLightR", new Vector2(x + 8f, y - 26f), new Vector2(12f, 6f), new Color(0.15f, 0.35f, 0.95f, 0.75f), 9);
    }

    private void AddDestructibles()
    {
        Node2D? nearRoot = GetNodeOrNull<Node2D>("NearStreet/NearRoot");
        if (nearRoot is null)
        {
            return;
        }

        SpawnBreakable(nearRoot, new Vector2(-820f, 520f), BreakableStageProp.PropKind.Crate);
        SpawnBreakable(nearRoot, new Vector2(-420f, 518f), BreakableStageProp.PropKind.TrashBag);
        SpawnBreakable(nearRoot, new Vector2(180f, 512f), BreakableStageProp.PropKind.FencePlank);
        SpawnBreakable(nearRoot, new Vector2(620f, 508f), BreakableStageProp.PropKind.KioskBottle);
        SpawnBreakable(nearRoot, new Vector2(980f, 510f), BreakableStageProp.PropKind.Crate);
        SpawnBreakable(nearRoot, new Vector2(1420f, 514f), BreakableStageProp.PropKind.StreetSign);
        SpawnBreakable(nearRoot, new Vector2(1880f, 516f), BreakableStageProp.PropKind.TrashBag);
        SpawnBreakable(nearRoot, new Vector2(2340f, 512f), BreakableStageProp.PropKind.FencePlank);
        SpawnBreakable(nearRoot, new Vector2(2780f, 508f), BreakableStageProp.PropKind.Crate);
    }

    private static void SpawnBreakable(Node parent, Vector2 position, BreakableStageProp.PropKind kind)
    {
        BreakableStageProp prop = new()
        {
            Name = $"Breakable_{kind}_{(int)position.X}",
            Position = position,
            Kind = kind,
        };
        parent.AddChild(prop);
    }

    private void AddVignette()
    {
        ColorRect vignette = new()
        {
            Name = "Vignette",
            OffsetLeft = -1100f,
            OffsetTop = 0f,
            OffsetRight = 3900f,
            OffsetBottom = 720f,
            MouseFilter = Control.MouseFilterEnum.Ignore,
            Color = new Color(0f, 0f, 0f, 0.20f),
            ZIndex = 100
        };
        AddChild(vignette);
    }
}
