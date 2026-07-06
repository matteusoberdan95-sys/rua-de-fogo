namespace SangueNoAsfalto.World;

using SangueNoAsfalto.Core;
using SangueNoAsfalto.Player;

/// <summary>
/// Poca de lama (slow) ou agua eletrificada (dano no raio) — Sprint 33.
/// </summary>
public partial class WeatherHazardZone : Node2D
{
    public enum HazardKind
    {
        MudSlow,
        ElectricPuddle,
    }

    [Export]
    public HazardKind Kind { get; set; } = HazardKind.MudSlow;

    [Export]
    public float Radius { get; set; } = 88f;

    [Export]
    public float SlowMultiplier { get; set; } = 0.74f;

    [Export]
    public int LightningDamage { get; set; } = 9;

    private WeatherController? _weather;
    private Polygon2D? _visual;
    private float _pulse;

    public override void _Ready()
    {
        AddToGroup("weather_hazards");
        _weather = GetTree().GetFirstNodeInGroup("weather") as WeatherController;
        if (_weather is not null)
        {
            _weather.LightningStruck += OnLightningStruck;
        }

        BuildVisual();
    }

    public override void _ExitTree()
    {
        if (_weather is not null)
        {
            _weather.LightningStruck -= OnLightningStruck;
        }
    }

    public override void _Process(double delta)
    {
        _pulse += (float)delta;
        if (_visual is null)
        {
            return;
        }

        bool active = IsHazardActive();
        Color color = _visual.Color;
        color.A = active
            ? 0.22f + Mathf.Sin(_pulse * 5f) * 0.06f
            : 0.08f;
        _visual.Color = color;
    }

    public float GetSpeedMultiplierFor(SideScrollerPlayerController player)
    {
        if (Kind != HazardKind.MudSlow || !IsHazardActive() || !ContainsPlayer(player))
        {
            return 1f;
        }

        return SlowMultiplier;
    }

    public bool ContainsPlayer(SideScrollerPlayerController player)
        => player.GlobalPosition.DistanceTo(GlobalPosition) <= Radius;

    private bool IsHazardActive()
    {
        if (_weather is null)
        {
            return Kind == HazardKind.MudSlow;
        }

        return Kind switch
        {
            HazardKind.MudSlow => _weather.CurrentState is WeatherController.WeatherState.HeavyRain
                or WeatherController.WeatherState.Thunderstorm
                or WeatherController.WeatherState.Drizzle,
            HazardKind.ElectricPuddle => _weather.CurrentState == WeatherController.WeatherState.Thunderstorm,
            _ => false,
        };
    }

    private void OnLightningStruck()
    {
        if (Kind != HazardKind.ElectricPuddle || !IsHazardActive())
        {
            return;
        }

        SideScrollerPlayerController? player = GetTree().GetFirstNodeInGroup("side_player") as SideScrollerPlayerController;
        if (player is null || !ContainsPlayer(player))
        {
            return;
        }

        Health? health = player.GetNodeOrNull<Health>("Health");
        if (health is not null)
        {
            health.Damage(LightningDamage);
        }

        CombatFeedback.PlayHit(player, this, LightningDamage);

        if (_visual is not null)
        {
            _visual.Color = new Color(0.55f, 0.82f, 1f, 0.65f);
        }
    }

    private void BuildVisual()
    {
        Color baseColor = Kind switch
        {
            HazardKind.ElectricPuddle => new Color(0.38f, 0.62f, 0.92f, 0.18f),
            _ => new Color(0.42f, 0.32f, 0.14f, 0.20f),
        };

        _visual = new Polygon2D
        {
            Name = "HazardVisual",
            Color = baseColor,
            Polygon =
            [
                new Vector2(-Radius * 0.9f, 8f),
                new Vector2(-Radius * 0.2f, -Radius * 0.35f),
                new Vector2(Radius * 0.85f, -Radius * 0.1f),
                new Vector2(Radius, Radius * 0.25f),
                new Vector2(Radius * 0.15f, Radius * 0.45f),
                new Vector2(-Radius * 0.75f, Radius * 0.35f),
            ],
            ZIndex = 12,
        };
        AddChild(_visual);

        if (Kind == HazardKind.ElectricPuddle)
        {
            Label warn = new()
            {
                Text = "! RAIO !",
                Position = new Vector2(-34f, -28f),
                ZIndex = 13,
            };
            warn.AddThemeFontSizeOverride("font_size", 11);
            warn.AddThemeColorOverride("font_color", new Color(0.72f, 0.88f, 1f, 0.75f));
            AddChild(warn);
        }
    }
}
