namespace SangueNoAsfalto.World;

/// <summary>
/// Landmarks por ato da Fase 1 — evita repeticao visual e guia o olhar do jogador.
/// Trechos conforme docs/STAGE_01_VILA_ESPERANCA.md.
/// </summary>
public static class StageActLandmarks
{
    public static void BuildAll(Node2D midRoot, Node2D nearRoot, StageAssetContext ctx)
    {
        BuildActEntrada(midRoot, nearRoot, ctx);
        BuildActBarracoMartins(midRoot, nearRoot, ctx);
        BuildActRuaCentral(midRoot, nearRoot, ctx);
        BuildActVielaEstreita(midRoot, nearRoot, ctx);
        BuildActPortaoSaida(midRoot, nearRoot, ctx);
    }

    private static void BuildActEntrada(Node2D mid, Node2D near, StageAssetContext ctx)
    {
        StageAssetLibrary.BuildEntranceArch(mid, ctx, -180f, 118f);
        StageAssetLibrary.BuildBotecoDoZe(mid, ctx, -1120f, 118f);
        StageAssetLibrary.BuildTrashCluster(near, ctx, -560f, 524f, 0);
        StageAssetLibrary.BuildTrashCluster(near, ctx, -320f, 518f, 1);
        AddZoneBanner(mid, "Act1Banner", "ENTRADA DA VILA", -60f, 138f, new Color(0.82f, 0.58f, 0.22f));
        AddHintLabel(mid, "Act1Hint", "Boteco fechado · chuva · lixo", -40f, 162f, 12, new Color(0.55f, 0.52f, 0.44f, 0.75f));
    }

    private static void BuildActBarracoMartins(Node2D mid, Node2D near, StageAssetContext ctx)
    {
        AddZoneBanner(mid, "Act2Banner", "BARRACO DO MARTINS", 920f, 138f, new Color(0.88f, 0.62f, 0.22f));
        BuildCheckpointMarker(mid, 480f, 300f, ctx);
        StageAssetLibrary.BuildMercadinho(mid, ctx, 520f, 152f, "CASA MARTINS", new Color(0.09f, 0.085f, 0.07f), false);
        StageAssetLibrary.BuildBusShelter(mid, 1180f, 306f);
        StageAssetLibrary.BuildStreetPole(mid, ctx, 640f, 118f, lit: true);
        StageAssetLibrary.BuildPuddle(near, ctx, 720f, 458f, 240f, 30f, new Color(0.38f, 0.50f, 0.56f, 0.2f));
        StageAssetLibrary.BuildTrashCluster(near, ctx, 880f, 516f, 2);
    }

    private static void BuildActRuaCentral(Node2D mid, Node2D near, StageAssetContext ctx)
    {
        AddZoneBanner(mid, "Act3Banner", "RUA CENTRAL", 1680f, 136f, new Color(0.78f, 0.55f, 0.24f));
        StageAssetLibrary.BuildStreetPole(mid, ctx, 1280f, 114f, lit: true);
        StageAssetLibrary.BuildStreetPole(mid, ctx, 1920f, 116f, lit: true, flicker: true);
        StageAssetLibrary.BuildMercadinho(mid, ctx, 1480f, 150f, "OFICINA", new Color(0.10f, 0.088f, 0.075f), false);
        BuildPoliceDetail(near, 890f, 482f);
        BuildBrokenCarDetail(near, 2050f, 474f);
        StageAssetLibrary.BuildPuddle(near, ctx, 1580f, 612f, 320f, 38f, new Color(0.48f, 0.24f, 0.14f, 0.22f));
    }

    private static void BuildActVielaEstreita(Node2D mid, Node2D near, StageAssetContext ctx)
    {
        AddZoneBanner(mid, "Act4Banner", "VIELA ESTREITA", 2280f, 134f, new Color(0.62f, 0.48f, 0.22f));
        StageAssetLibrary.BuildAlleyCompression(mid, 2100f, 148f, 520f);
        StageAssetLibrary.BuildFenceSection(mid, ctx, 2180f, 172f, 420f, damaged: true);
        StageAssetLibrary.BuildTirePile(near, 2240f, 508f);
        StageAssetLibrary.BuildTirePile(near, 2520f, 512f);
        StageAssetLibrary.BuildTrashCluster(near, ctx, 2420f, 520f, 3);
        AddHintLabel(mid, "Act4Hint", "Cerca quebrada · infectados", 2300f, 158f, 12, new Color(0.52f, 0.48f, 0.40f, 0.72f));
    }

    private static void BuildActPortaoSaida(Node2D mid, Node2D near, StageAssetContext ctx)
    {
        AddZoneBanner(mid, "Act5Banner", "PORTAO SAIDA", 2860f, 130f, new Color(0.72f, 0.82f, 0.38f));
        StageAssetLibrary.BuildExitFogBand(mid, 2720f, 120f, 480f);
        BuildBossAltar(mid, 2920f, 292f, ctx);
        AddWallGraffiti(mid, 2680f, 206f, "ELE VOLTA", new Color(0.56f, 0.05f, 0.06f, 0.82f));
        StageAssetLibrary.BuildStreetPole(mid, ctx, 2820f, 112f, lit: true, flicker: true);
    }

    private static void BuildCheckpointMarker(Node2D root, float x, float y, StageAssetContext ctx)
    {
        AddRect(root, "CheckpointPillar", new Vector2(x - 8f, y - 40f), new Vector2(16f, 64f), new Color(0.18f, 0.62f, 0.38f, 0.85f), 8);
        Label cp = AddLabel(root, "CheckpointLabel", "CHECKPOINT", new Vector2(x - 42f, y - 72f), 14, new Color(0.78f, 0.92f, 0.55f), 9);
        ctx.NeonItems.Add(cp);
    }

    private static void BuildPoliceDetail(Node2D root, float x, float y)
    {
        AddPoly(root, "PoliceCarBody", new Color(0.09f, 0.11f, 0.14f), [
            new Vector2(x - 88f, y + 8f), new Vector2(x + 68f, y + 4f), new Vector2(x + 92f, y - 16f), new Vector2(x + 14f, y - 28f), new Vector2(x - 72f, y - 14f)
        ], 8);
        AddRect(root, "PoliceLightL", new Vector2(x - 12f, y - 32f), new Vector2(14f, 7f), new Color(0.9f, 0.15f, 0.12f, 0.8f), 9);
        AddRect(root, "PoliceLightR", new Vector2(x + 10f, y - 32f), new Vector2(14f, 7f), new Color(0.15f, 0.35f, 0.95f, 0.8f), 9);
        AddLabel(root, "PoliceMark", "190", new Vector2(x - 18f, y + 2f), 12, new Color(0.72f, 0.68f, 0.55f, 0.55f), 9);
    }

    private static void BuildBrokenCarDetail(Node2D root, float x, float y)
    {
        AddPoly(root, "CarBody", new Color(0.11f, 0.115f, 0.12f), [
            new Vector2(x - 98f, y + 10f), new Vector2(x + 78f, y + 6f), new Vector2(x + 104f, y - 20f), new Vector2(x + 44f, y - 38f),
            new Vector2(x - 44f, y - 34f), new Vector2(x - 106f, y - 10f)
        ], 8);
        AddPoly(root, "CarWindow", new Color(0.04f, 0.05f, 0.06f, 0.9f), [
            new Vector2(x - 22f, y - 30f), new Vector2(x + 38f, y - 32f), new Vector2(x + 26f, y - 14f), new Vector2(x - 12f, y - 12f)
        ], 9);
        AddPoly(root, "CarDent", new Color(0.28f, 0.03f, 0.03f, 0.58f), [
            new Vector2(x + 52f, y - 10f), new Vector2(x + 78f, y - 6f), new Vector2(x + 70f, y + 12f), new Vector2(x + 48f, y + 8f)
        ], 10);
        AddPoly(root, "CarBlood", new Color(0.35f, 0.02f, 0.04f, 0.72f), [
            new Vector2(x + 8f, y + 14f), new Vector2(x + 48f, y + 10f), new Vector2(x + 42f, y + 28f), new Vector2(x + 4f, y + 26f)
        ], 11);
    }

    private static void BuildBossAltar(Node2D root, float x, float y, StageAssetContext ctx)
    {
        AddRect(root, "BossAltarBase", new Vector2(x - 48f, y + 4f), new Vector2(96f, 28f), new Color(0.10f, 0.085f, 0.065f), 6);
        AddPoly(root, "BossAltarStone", new Color(0.16f, 0.14f, 0.11f), [
            new Vector2(x - 34f, y - 10f), new Vector2(x + 34f, y - 10f), new Vector2(x + 24f, y - 38f), new Vector2(x - 24f, y - 38f)
        ], 7);
        AddRect(root, "BossAltarCloth", new Vector2(x - 28f, y - 6f), new Vector2(56f, 20f), new Color(0.45f, 0.02f, 0.035f, 0.9f), 8);
        Polygon2D flame = AddPoly(root, "BossAltarFlame", new Color(1f, 0.48f, 0.12f, 0.65f), [
            new Vector2(x + 6f, y - 48f), new Vector2(x + 20f, y - 22f), new Vector2(x + 6f, y - 4f), new Vector2(x - 8f, y - 22f)
        ], 9);
        ctx.NeonItems.Add(flame);
    }

    private static void AddZoneBanner(Node2D root, string name, string text, float x, float y, Color color)
    {
        AddRect(root, $"{name}Bg", new Vector2(x - 8f, y - 6f), new Vector2(text.Length * 9.5f + 24f, 28f), new Color(0f, 0f, 0f, 0.42f), 7);
        AddLabel(root, name, text, new Vector2(x, y), 16, color, 8);
    }

    private static void AddWallGraffiti(Node2D root, float x, float y, string text, Color color)
    {
        AddPoly(root, $"GraffitiSplash{x}", color, [
            new Vector2(x, y + 12f), new Vector2(x + 74f, y - 8f), new Vector2(x + 150f, y + 12f), new Vector2(x + 218f, y - 4f),
            new Vector2(x + 260f, y + 18f), new Vector2(x + 190f, y + 34f), new Vector2(x + 120f, y + 24f), new Vector2(x + 42f, y + 38f)
        ], 7);
        AddLabel(root, $"GraffitiText{x}", text, new Vector2(x + 18f, y - 2f), 15, new Color(0.86f, 0.74f, 0.48f, 0.88f), 8);
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

    private static void AddHintLabel(Node parent, string name, string text, float x, float y, int fontSize, Color color)
    {
        Label label = new()
        {
            Name = name,
            Text = text,
            Position = new Vector2(x, y),
            ZIndex = 8,
            Size = new Vector2(280f, 40f)
        };
        label.AddThemeFontSizeOverride("font_size", fontSize);
        label.AddThemeColorOverride("font_color", color);
        parent.AddChild(label);
    }

    private static Label AddLabel(Node parent, string name, string text, Vector2 position, int fontSize, Color color, int z = 8)
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
}
