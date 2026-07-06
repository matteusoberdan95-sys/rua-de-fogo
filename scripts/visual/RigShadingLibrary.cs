namespace SangueNoAsfalto.Visual;

/// <summary>
/// Cel-shading e props de combate para rigs Polygon2D — estilo arcade/pixel legível, não "boneco de cera".
/// </summary>
public static class RigShadingLibrary
{
    public static void AddCelShape(Node parent, string name, Color baseColor, Vector2[] points, int z)
    {
        Color outline = baseColor.Darkened(0.48f);
        Color shadow = baseColor.Darkened(0.22f);
        Color highlight = baseColor.Lightened(0.14f);

        AddPoly(parent, $"{name}Outline", outline, points, z - 2);
        AddPoly(parent, name, baseColor, points, z);

        Vector2[] shadowBand = OffsetPointsDown(points, 3f);
        AddPoly(parent, $"{name}Shadow", new Color(shadow.R, shadow.G, shadow.B, 0.72f), shadowBand, z + 1);

        if (points.Length >= 4)
        {
            AddPoly(parent, $"{name}Hi", new Color(highlight.R, highlight.G, highlight.B, 0.55f), [
                points[0], points[1], Lerp(points[1], points[2], 0.35f), Lerp(points[0], points[3], 0.35f)
            ], z + 2);
        }
    }

    public static void AddTorsoShade(Node torso, Color shirt, Color shadowTint)
    {
        AddPoly(torso, "TorsoShadeL", shadowTint, [
            new Vector2(-18f, -8f), new Vector2(-4f, -12f), new Vector2(-2f, 28f), new Vector2(-16f, 32f)
        ], 3);
        AddPoly(torso, "TorsoHi", shirt.Lightened(0.12f), [
            new Vector2(4f, -14f), new Vector2(16f, -10f), new Vector2(14f, 8f), new Vector2(2f, 4f)
        ], 4);
        AddPoly(torso, "AbsLine", shirt.Darkened(0.18f), [
            new Vector2(-2f, 4f), new Vector2(4f, 3f), new Vector2(3f, 22f), new Vector2(-3f, 23f)
        ], 4);
    }

    public static void BuildCaseiraKnife(Node parent, out CanvasItem blade, out CanvasItem blood)
    {
        Color steel = new(0.78f, 0.80f, 0.84f);
        Color edge = new(0.92f, 0.94f, 0.96f);
        Color handle = new(0.22f, 0.14f, 0.08f);
        Color wrap = new(0.38f, 0.32f, 0.24f);

        AddPoly(parent, "KnifeHandle", handle, [
            new Vector2(4f, 14f), new Vector2(14f, 12f), new Vector2(16f, 22f), new Vector2(6f, 24f)
        ], 6);
        AddPoly(parent, "KnifeWrap", wrap, [
            new Vector2(6f, 15f), new Vector2(12f, 14f), new Vector2(13f, 20f), new Vector2(7f, 21f)
        ], 7);
        blade = AddPoly(parent, "KnifeBlade", steel, [
            new Vector2(10f, 10f), new Vector2(44f, 6f), new Vector2(48f, 18f), new Vector2(12f, 22f)
        ], 8);
        AddPoly(parent, "KnifeEdge", edge, [
            new Vector2(14f, 9f), new Vector2(42f, 6f), new Vector2(44f, 11f), new Vector2(16f, 14f)
        ], 9);
        blood = AddPoly(parent, "KnifeBlood", new Color(0.52f, 0.02f, 0.03f, 0.85f), [
            new Vector2(18f, 12f), new Vector2(38f, 10f), new Vector2(36f, 18f), new Vector2(16f, 20f)
        ], 10);
    }

    public static void BuildIronPipe(Node parent, float x0, float y0, float length, float thickness)
    {
        Color metal = new(0.48f, 0.46f, 0.44f);
        Color dark = metal.Darkened(0.2f);
        Color shine = metal.Lightened(0.22f);

        AddPoly(parent, "PipeBody", dark, [
            new Vector2(x0, y0), new Vector2(x0 + length, y0 - 4f),
            new Vector2(x0 + length, y0 + thickness), new Vector2(x0, y0 + thickness + 4f)
        ], 0);
        AddPoly(parent, "PipeTop", metal, [
            new Vector2(x0 + 4f, y0 + 2f), new Vector2(x0 + length - 2f, y0 - 1f),
            new Vector2(x0 + length - 2f, y0 + thickness * 0.45f), new Vector2(x0 + 4f, y0 + thickness * 0.45f + 2f)
        ], 1);
        AddPoly(parent, "PipeShine", shine, [
            new Vector2(x0 + 8f, y0 + 3f), new Vector2(x0 + length * 0.7f, y0), new Vector2(x0 + length * 0.68f, y0 + 4f), new Vector2(x0 + 8f, y0 + 6f)
        ], 2);
        AddPoly(parent, "PipeRust", new Color(0.42f, 0.18f, 0.06f, 0.55f), [
            new Vector2(x0 + length * 0.4f, y0 + thickness * 0.5f), new Vector2(x0 + length * 0.55f, y0 + thickness * 0.45f),
            new Vector2(x0 + length * 0.52f, y0 + thickness + 2f), new Vector2(x0 + length * 0.38f, y0 + thickness + 3f)
        ], 3);
    }

    public static void BuildButcherCleaver(Node parent, float x0, float y0)
    {
        Color blade = new(0.72f, 0.74f, 0.78f);
        AddPoly(parent, "CleaverBlade", blade.Darkened(0.15f), [
            new Vector2(x0, y0 + 8f), new Vector2(x0 + 52f, y0), new Vector2(x0 + 56f, y0 + 28f), new Vector2(x0 + 4f, y0 + 34f)
        ], 0);
        AddPoly(parent, "CleaverFace", blade, [
            new Vector2(x0 + 6f, y0 + 10f), new Vector2(x0 + 48f, y0 + 4f), new Vector2(x0 + 50f, y0 + 24f), new Vector2(x0 + 8f, y0 + 28f)
        ], 1);
        AddPoly(parent, "CleaverEdge", new Color(0.9f, 0.92f, 0.94f), [
            new Vector2(x0 + 8f, y0 + 8f), new Vector2(x0 + 50f, y0 + 2f), new Vector2(x0 + 52f, y0 + 8f), new Vector2(x0 + 10f, y0 + 14f)
        ], 2);
        AddPoly(parent, "CleaverBlood", new Color(0.55f, 0.02f, 0.03f, 0.8f), [
            new Vector2(x0 + 20f, y0 + 12f), new Vector2(x0 + 42f, y0 + 8f), new Vector2(x0 + 40f, y0 + 22f), new Vector2(x0 + 18f, y0 + 26f)
        ], 3);
        AddPoly(parent, "CleaverHandle", new Color(0.18f, 0.1f, 0.06f), [
            new Vector2(x0 - 18f, y0 + 12f), new Vector2(x0 + 2f, y0 + 10f), new Vector2(x0 + 4f, y0 + 26f), new Vector2(x0 - 16f, y0 + 28f)
        ], 0);
    }

    public static void BuildBaton(Node parent, float x0, float y0)
    {
        Color grip = new(0.12f, 0.1f, 0.09f);
        Color shaft = new(0.32f, 0.3f, 0.28f);
        AddPoly(parent, "BatonGrip", grip, [
            new Vector2(x0, y0 + 4f), new Vector2(x0 + 16f, y0 + 2f), new Vector2(x0 + 18f, y0 + 14f), new Vector2(x0 + 2f, y0 + 16f)
        ], 0);
        AddPoly(parent, "BatonShaft", shaft, [
            new Vector2(x0 + 14f, y0 + 3f), new Vector2(x0 + 58f, y0 - 2f), new Vector2(x0 + 60f, y0 + 8f), new Vector2(x0 + 16f, y0 + 12f)
        ], 1);
        AddPoly(parent, "BatonCap", new Color(0.72f, 0.68f, 0.22f), [
            new Vector2(x0 + 54f, y0 - 1f), new Vector2(x0 + 62f, y0 - 3f), new Vector2(x0 + 64f, y0 + 6f), new Vector2(x0 + 56f, y0 + 8f)
        ], 2);
    }

    public static void AddLeopardPrint(Node torso, int spots = 6)
    {
        Color spot = new(0.22f, 0.12f, 0.03f, 0.75f);
        Vector2[] positions =
        [
            new(-8f, -4f), new(8f, 2f), new(-4f, 14f), new(12f, 18f), new(-10f, 22f), new(6f, -8f)
        ];
        for (int i = 0; i < spots && i < positions.Length; i++)
        {
            Vector2 p = positions[i];
            AddPoly(torso, $"Spot{i}", spot, [
                new Vector2(p.X - 4f, p.Y - 3f), new Vector2(p.X + 5f, p.Y - 4f),
                new Vector2(p.X + 6f, p.Y + 4f), new Vector2(p.X - 3f, p.Y + 5f)
            ], 7);
        }
    }

    public static void AddApronDetail(Node torso)
    {
        AddPoly(torso, "ApronFold", new Color(0.72f, 0.7f, 0.66f), [
            new Vector2(-4f, 8f), new Vector2(8f, 6f), new Vector2(10f, 28f), new Vector2(-2f, 30f)
        ], 7);
        AddPoly(torso, "ApronStrap", new Color(0.68f, 0.66f, 0.62f), [
            new Vector2(-14f, -6f), new Vector2(-8f, -8f), new Vector2(-6f, 4f), new Vector2(-12f, 6f)
        ], 7);
        for (int i = 0; i < 4; i++)
        {
            AddPoly(torso, $"BloodSpl{i}", new Color(0.5f + i * 0.02f, 0.02f, 0.03f, 0.65f), [
                new Vector2(-8f + i * 6f, 10f + i * 5f), new Vector2(2f + i * 6f, 8f + i * 4f),
                new Vector2(4f + i * 5f, 18f + i * 4f), new Vector2(-6f + i * 5f, 20f + i * 5f)
            ], 8);
        }
    }

    public static void AddSuitLapels(Node torso)
    {
        AddPoly(torso, "LapelL", new Color(0.1f, 0.09f, 0.1f), [
            new Vector2(-14f, -12f), new Vector2(-4f, -8f), new Vector2(-6f, 18f), new Vector2(-16f, 14f)
        ], 6);
        AddPoly(torso, "LapelR", new Color(0.11f, 0.1f, 0.11f), [
            new Vector2(6f, -10f), new Vector2(14f, -12f), new Vector2(16f, 12f), new Vector2(8f, 16f)
        ], 6);
        AddPoly(torso, "ShirtV", new Color(0.88f, 0.86f, 0.84f), [
            new Vector2(-2f, -8f), new Vector2(4f, -8f), new Vector2(2f, 12f), new Vector2(0f, 12f)
        ], 7);
    }

    private static Vector2 Lerp(Vector2 a, Vector2 b, float t) => a.Lerp(b, t);

    private static Vector2[] OffsetPointsDown(Vector2[] points, float amount)
    {
        Vector2[] result = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            result[i] = points[i] + new Vector2(0f, amount * 0.35f);
        }

        return result;
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
