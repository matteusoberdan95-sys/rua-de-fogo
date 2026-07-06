namespace SangueNoAsfalto.World;

public partial class WeatherController : Node
{
    public enum WeatherState
    {
        Dry,
        Drizzle,
        HeavyRain,
        Thunderstorm
    }

    [Signal]
    public delegate void LightningStruckEventHandler();

    [Export]
    public WeatherState InitialState { get; set; } = WeatherState.Drizzle;

    [Export]
    public bool AutoCycle { get; set; } = true;

    [Export]
    public float SecondsPerState { get; set; } = 22f;

    [Export]
    public NodePath RainLayerPath { get; set; } = "../WeatherLayer/RainLayer";

    [Export]
    public NodePath FogLayerPath { get; set; } = "../WeatherLayer/FogLayer";

    [Export]
    public NodePath LightningFlashPath { get; set; } = "../WeatherLayer/LightningFlash";

    [Export]
    public NodePath MudPatchPath { get; set; } = "../MudPatch";

    public WeatherState CurrentState { get; private set; }

    public float WindStrength { get; private set; } = 0.4f;

    private Node2D? _rainLayer;
    private Polygon2D? _fogLayer;
    private Polygon2D? _lightningFlash;
    private Polygon2D? _mudPatch;
    private readonly System.Collections.Generic.List<Line2D> _rainStreaks = [];
    private float _stateTimer;
    private float _lightningTimer;
    private float _lightningMin = 3.5f;
    private float _lightningMax = 7.5f;

    public override void _Ready()
    {
        AddToGroup("weather");
        _rainLayer = GetNodeOrNull<Node2D>(RainLayerPath);
        _fogLayer = GetNodeOrNull<Polygon2D>(FogLayerPath);
        _lightningFlash = GetNodeOrNull<Polygon2D>(LightningFlashPath);
        _mudPatch = GetNodeOrNull<Polygon2D>(MudPatchPath);
        BuildRainStreaks();

        SetState(InitialState, instant: true);
        QueueNextLightning();
    }

    public override void _Process(double delta)
    {
        float dt = (float)delta;
        AnimateRain(dt);
        TickLightning(dt);

        if (!AutoCycle)
        {
            return;
        }

        _stateTimer += dt;
        if (_stateTimer < SecondsPerState)
        {
            return;
        }

        _stateTimer = 0f;
        SetState(NextState(CurrentState));
    }

    public void SetWindStrength(float strength)
    {
        WindStrength = Mathf.Clamp(strength, 0f, 1.2f);
    }

    public void SetLightningInterval(float minSeconds, float maxSeconds)
    {
        _lightningMin = Mathf.Max(0.8f, minSeconds);
        _lightningMax = Mathf.Max(_lightningMin + 0.2f, maxSeconds);
        QueueNextLightning();
    }

    public void ResetLightningInterval()
    {
        SetLightningInterval(3.5f, 7.5f);
    }

    public void SetState(WeatherState state, bool instant = false)
    {
        CurrentState = state;
        WeatherVisuals visuals = GetVisuals(state);
        float duration = instant ? 0.01f : 0.85f;

        Tween tween = CreateTween();
        TweenAlpha(tween, _fogLayer, visuals.FogAlpha, duration);
        TweenAlpha(tween, _mudPatch, visuals.MudAlpha, duration);

        if (_rainLayer is not null)
        {
            foreach (Node child in _rainLayer.GetChildren())
            {
                if (child is Polygon2D rainStreak)
                {
                    Color color = rainStreak.Color;
                    color.A = visuals.RainAlpha;
                    tween.Parallel().TweenProperty(rainStreak, "color", color, duration);
                }
            }
        }

        foreach (Line2D streak in _rainStreaks)
        {
            streak.DefaultColor = new Color(0.72f, 0.86f, 0.92f, visuals.RainAlpha);
            streak.Visible = visuals.RainAlpha > 0.01f;
            streak.Width = state == WeatherState.Drizzle ? 1.35f : 1.9f;
        }
    }

    private void AnimateRain(float dt)
    {
        if (_rainLayer is null || CurrentState == WeatherState.Dry)
        {
            return;
        }

        float speed = CurrentState == WeatherState.HeavyRain || CurrentState == WeatherState.Thunderstorm
            ? 560f + WindStrength * 80f
            : 300f + WindStrength * 40f;
        foreach (Line2D streak in _rainStreaks)
        {
            streak.Position += new Vector2(-speed * 0.42f, speed) * dt;
            if (streak.Position.Y > 760f || streak.Position.X < -1380f)
            {
                streak.Position = new Vector2((float)GD.RandRange(-1120.0, 1440.0), (float)GD.RandRange(-260.0, -40.0));
            }
        }
    }

    private void BuildRainStreaks()
    {
        if (_rainLayer is null)
        {
            return;
        }

        foreach (Node child in _rainLayer.GetChildren())
        {
            if (child is Polygon2D polygon)
            {
                polygon.Visible = false;
            }
        }

        const int streakCount = 72;
        for (int i = 0; i < streakCount; i++)
        {
            Line2D streak = new()
            {
                Name = $"RainStreak{i:00}",
                DefaultColor = new Color(0.72f, 0.86f, 0.92f, 0.34f),
                Width = 1.5f,
                ZIndex = 30,
                Antialiased = false
            };

            float length = (float)GD.RandRange(42.0, 84.0);
            streak.AddPoint(Vector2.Zero);
            streak.AddPoint(new Vector2(-length * 0.34f, length));
            streak.Position = new Vector2((float)GD.RandRange(-1280.0, 1440.0), (float)GD.RandRange(-240.0, 720.0));
            _rainLayer.AddChild(streak);
            _rainStreaks.Add(streak);
        }
    }

    private void TickLightning(float dt)
    {
        if (CurrentState != WeatherState.Thunderstorm || _lightningFlash is null)
        {
            return;
        }

        _lightningTimer -= dt;
        if (_lightningTimer > 0f)
        {
            return;
        }

        QueueNextLightning();
        _lightningFlash.Color = new Color(0.75f, 0.86f, 1f, 0.38f + WindStrength * 0.12f);
        Tween tween = CreateTween();
        tween.TweenProperty(_lightningFlash, "color:a", 0f, 0.18f);
        EmitSignal(SignalName.LightningStruck);
        WeatherAmbience.PlayThunder(this);
    }

    private void QueueNextLightning()
    {
        _lightningTimer = (float)GD.RandRange(_lightningMin, _lightningMax);
    }

    private static WeatherState NextState(WeatherState state)
    {
        return state switch
        {
            WeatherState.Dry => WeatherState.Drizzle,
            WeatherState.Drizzle => WeatherState.HeavyRain,
            WeatherState.HeavyRain => WeatherState.Thunderstorm,
            _ => WeatherState.Dry,
        };
    }

    private static WeatherVisuals GetVisuals(WeatherState state)
    {
        return state switch
        {
            WeatherState.Dry => new WeatherVisuals(0f, 0.06f, 0.15f),
            WeatherState.HeavyRain => new WeatherVisuals(0.46f, 0.20f, 0.45f),
            WeatherState.Thunderstorm => new WeatherVisuals(0.56f, 0.26f, 0.56f),
            _ => new WeatherVisuals(0.30f, 0.13f, 0.32f),
        };
    }

    private static void TweenAlpha(Tween tween, Polygon2D? polygon, float alpha, float duration)
    {
        if (polygon is null)
        {
            return;
        }

        Color color = polygon.Color;
        color.A = alpha;
        tween.Parallel().TweenProperty(polygon, "color", color, duration);
    }

    private readonly record struct WeatherVisuals(float RainAlpha, float FogAlpha, float MudAlpha);
}
