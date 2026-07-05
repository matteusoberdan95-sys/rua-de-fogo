namespace SangueNoAsfalto.World;

public partial class TimeOfDayController : Node
{
    public enum TimeOfDayState
    {
        Dawn,
        Morning,
        Afternoon,
        Sunset,
        Night
    }

    [Export]
    public TimeOfDayState InitialState { get; set; } = TimeOfDayState.Dawn;

    [Export]
    public bool AutoCycle { get; set; } = true;

    [Export]
    public float SecondsPerState { get; set; } = 18f;

    [Export]
    public NodePath SkyPath { get; set; } = "../Sky";

    [Export]
    public NodePath DawnGlowPath { get; set; } = "../DawnGlow";

    [Export]
    public NodePath NightVignettePath { get; set; } = "../NightVignette";

    [Export]
    public NodePath WetRoadSheenPath { get; set; } = "../WetRoadSheen";

    [Export]
    public NodePath[] LightNodePaths { get; set; } = [];

    public TimeOfDayState CurrentState { get; private set; }

    private Polygon2D? _sky;
    private Polygon2D? _dawnGlow;
    private Polygon2D? _nightVignette;
    private Polygon2D? _wetRoadSheen;
    private Polygon2D[] _lightNodes = [];
    private float _stateTimer;

    public override void _Ready()
    {
        AddToGroup("time_of_day");
        _sky = GetNodeOrNull<Polygon2D>(SkyPath);
        _dawnGlow = GetNodeOrNull<Polygon2D>(DawnGlowPath);
        _nightVignette = GetNodeOrNull<Polygon2D>(NightVignettePath);
        _wetRoadSheen = GetNodeOrNull<Polygon2D>(WetRoadSheenPath);
        _lightNodes = ResolveLightNodes();
        SetState(InitialState, instant: true);
    }

    public override void _Process(double delta)
    {
        if (!AutoCycle)
        {
            return;
        }

        _stateTimer += (float)delta;
        if (_stateTimer < SecondsPerState)
        {
            return;
        }

        _stateTimer = 0f;
        SetState(NextState(CurrentState));
    }

    public void SetState(TimeOfDayState state, bool instant = false)
    {
        CurrentState = state;
        TimeVisuals visuals = GetVisuals(state);
        float duration = instant ? 0.01f : 1.15f;

        Tween tween = CreateTween();
        TweenColor(tween, _sky, visuals.Sky, duration);
        TweenColor(tween, _dawnGlow, visuals.DawnGlow, duration);
        TweenColor(tween, _nightVignette, visuals.NightVignette, duration);
        TweenColor(tween, _wetRoadSheen, visuals.WetRoadSheen, duration);

        foreach (Polygon2D light in _lightNodes)
        {
            Color targetColor = light.Color;
            targetColor.A = visuals.LightAlpha;
            TweenColor(tween, light, targetColor, duration);
        }
    }

    private static TimeOfDayState NextState(TimeOfDayState state)
    {
        return state switch
        {
            TimeOfDayState.Dawn => TimeOfDayState.Morning,
            TimeOfDayState.Morning => TimeOfDayState.Afternoon,
            TimeOfDayState.Afternoon => TimeOfDayState.Sunset,
            TimeOfDayState.Sunset => TimeOfDayState.Night,
            _ => TimeOfDayState.Dawn,
        };
    }

    private static TimeVisuals GetVisuals(TimeOfDayState state)
    {
        return state switch
        {
            TimeOfDayState.Morning => new TimeVisuals(
                new Color(0.18f, 0.22f, 0.22f, 1f),
                new Color(0.94f, 0.56f, 0.24f, 0.1f),
                new Color(0f, 0f, 0f, 0.08f),
                new Color(0.42f, 0.46f, 0.42f, 0.09f),
                0.18f),
            TimeOfDayState.Afternoon => new TimeVisuals(
                new Color(0.2f, 0.19f, 0.16f, 1f),
                new Color(0.9f, 0.48f, 0.16f, 0.08f),
                new Color(0f, 0f, 0f, 0.12f),
                new Color(0.36f, 0.32f, 0.24f, 0.08f),
                0.1f),
            TimeOfDayState.Sunset => new TimeVisuals(
                new Color(0.13f, 0.09f, 0.095f, 1f),
                new Color(0.9f, 0.22f, 0.08f, 0.28f),
                new Color(0f, 0f, 0f, 0.22f),
                new Color(0.58f, 0.31f, 0.16f, 0.14f),
                0.38f),
            TimeOfDayState.Night => new TimeVisuals(
                new Color(0.035f, 0.04f, 0.052f, 1f),
                new Color(0.25f, 0.08f, 0.12f, 0.06f),
                new Color(0f, 0f, 0f, 0.38f),
                new Color(0.32f, 0.38f, 0.36f, 0.15f),
                0.62f),
            _ => new TimeVisuals(
                new Color(0.05f, 0.055f, 0.065f, 1f),
                new Color(0.72f, 0.29f, 0.12f, 0.18f),
                new Color(0f, 0f, 0f, 0.26f),
                new Color(0.29f, 0.32f, 0.31f, 0.11f),
                0.42f),
        };
    }

    private Polygon2D[] ResolveLightNodes()
    {
        if (LightNodePaths.Length == 0)
        {
            string[] defaultNames =
            [
                "PostALight",
                "PostBLight",
                "PostCLight",
                "PostALightPool",
                "PostBLightPool",
                "PostCLightPool",
                "CandleGlow"
            ];

            System.Collections.Generic.List<Polygon2D> defaults = [];
            foreach (string nodeName in defaultNames)
            {
                if (FindPolygonByName(GetTree().CurrentScene, nodeName) is { } polygon)
                {
                    defaults.Add(polygon);
                }
            }

            return defaults.ToArray();
        }

        Polygon2D[] nodes = new Polygon2D[LightNodePaths.Length];
        int count = 0;
        foreach (NodePath path in LightNodePaths)
        {
            if (GetNodeOrNull<Polygon2D>(path) is { } light)
            {
                nodes[count++] = light;
            }
        }

        if (count == nodes.Length)
        {
            return nodes;
        }

        System.Array.Resize(ref nodes, count);
        return nodes;
    }

    private static Polygon2D? FindPolygonByName(Node? root, string nodeName)
    {
        if (root is null)
        {
            return null;
        }

        if (root is Polygon2D polygon && root.Name == nodeName)
        {
            return polygon;
        }

        foreach (Node child in root.GetChildren())
        {
            if (FindPolygonByName(child, nodeName) is { } found)
            {
                return found;
            }
        }

        return null;
    }

    private static void TweenColor(Tween tween, Polygon2D? polygon, Color color, float duration)
    {
        if (polygon is not null)
        {
            tween.Parallel().TweenProperty(polygon, "color", color, duration);
        }
    }

    private readonly record struct TimeVisuals(
        Color Sky,
        Color DawnGlow,
        Color NightVignette,
        Color WetRoadSheen,
        float LightAlpha);
}
