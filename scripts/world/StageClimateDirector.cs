namespace SangueNoAsfalto.World;

using SangueNoAsfalto.Core;
using SangueNoAsfalto.Player;

/// <summary>
/// Orquestra clima/horario por ato, vento, apagao e assinatura de chefes (Sprint 33).
/// </summary>
public partial class StageClimateDirector : Node
{
    [Export]
    public NodePath WeatherControllerPath { get; set; } = "../WeatherController";

    [Export]
    public NodePath TimeOfDayControllerPath { get; set; } = "../TimeOfDayController";

    [Export]
    public NodePath PlayerPath { get; set; } = "../SideScrollerPlayer";

    [Export]
    public NodePath DirectorPath { get; set; } = "../SideScrollerDirector";

    [Export]
    public NodePath FogLayerPath { get; set; } = "../WeatherLayer/FogLayer";

    public float CurrentWindStrength { get; private set; }

    private WeatherController? _weather;
    private TimeOfDayController? _timeOfDay;
    private SideScrollerPlayerController? _player;
    private SideScrollerDirector? _director;
    private Polygon2D? _fogLayer;
    private ColorRect? _blackoutOverlay;
    private StageClimateProfile.Act _currentAct;
    private StageClimateProfile.ActClimate _currentClimate;
    private float _windGustTimer;
    private float _rainAudioTimer;
    private float _blackoutRemaining;
    private bool _hintShownForAct;
    private StageScrollSpawns.SpawnKind? _activeBossKind;
    private WeatherController.WeatherState? _appliedWeather;
    private TimeOfDayController.TimeOfDayState? _appliedTime;

    public override void _Ready()
    {
        AddToGroup("climate_director");
        _weather = GetNodeOrNull<WeatherController>(WeatherControllerPath);
        _timeOfDay = GetNodeOrNull<TimeOfDayController>(TimeOfDayControllerPath);
        _player = GetNodeOrNull<SideScrollerPlayerController>(PlayerPath);
        _director = GetNodeOrNull<SideScrollerDirector>(DirectorPath);
        _fogLayer = GetNodeOrNull<Polygon2D>(FogLayerPath);
        _currentAct = StageClimateProfile.Act.Entrada;
        _currentClimate = StageClimateProfile.GetActClimate(_currentAct);
        BuildBlackoutOverlay();
        SpawnHazardZones();
        _windGustTimer = 4f;
        _rainAudioTimer = 0.6f;
        if (_weather is not null)
        {
            _weather.AutoCycle = false;
        }

        if (_timeOfDay is not null)
        {
            _timeOfDay.AutoCycle = false;
        }
    }

    public override void _Process(double delta)
    {
        float dt = (float)delta;
        TickBlackout(dt);
        FindPlayer();

        if (_player is null)
        {
            return;
        }

        float playerX = _player.GlobalPosition.X;
        StageClimateProfile.Act act = StageClimateProfile.ResolveAct(playerX);
        StageClimateProfile.ActClimate climate = StageClimateProfile.GetActClimate(act);
        StageClimateProfile.BossClimate? bossClimate = ResolveBossClimate();

        if (bossClimate is not null)
        {
            ApplyBossClimate(bossClimate.Value);
        }
        else
        {
            ApplyActClimate(climate);
        }

        if (act != _currentAct)
        {
            _currentAct = act;
            _hintShownForAct = false;
        }

        if (!_hintShownForAct && !string.IsNullOrEmpty(climate.Hint))
        {
            _hintShownForAct = true;
            if (_director is not null && bossClimate is null)
            {
                _director.SetClimateHint(climate.Hint);
            }
        }

        CurrentWindStrength = bossClimate?.WindStrength ?? climate.WindStrength;
        TickWindGusts(dt, CurrentWindStrength);
        TickRainAmbience(dt, bossClimate?.Weather ?? climate.Weather);
        ApplyFogBoost(bossClimate?.FogBoost ?? climate.FogBoost);
    }

    public void NotifyBossSpawned(StageScrollSpawns.SpawnKind kind)
    {
        _activeBossKind = kind;
        StageClimateProfile.BossClimate? boss = StageClimateProfile.GetBossClimate(kind, bossAlive: true);
        if (boss is null)
        {
            return;
        }

        if (boss.Value.PulseBlackoutOnStart)
        {
            TriggerBlackout(1.35f);
        }

        if (_director is not null)
        {
            _director.SetClimateHint(boss.Value.Status);
        }

        if (_weather is not null)
        {
            _weather.SetLightningInterval(2.2f, 4.2f);
        }
    }

    private StageClimateProfile.BossClimate? ResolveBossClimate()
    {
        if (_activeBossKind is not StageScrollSpawns.SpawnKind kind)
        {
            return null;
        }

        if (!IsBossAlive(kind))
        {
            _activeBossKind = null;
            _weather?.ResetLightningInterval();
            return null;
        }

        return StageClimateProfile.GetBossClimate(kind, bossAlive: true);
    }

    private bool IsBossAlive(StageScrollSpawns.SpawnKind kind)
    {
        foreach (Node node in GetTree().GetNodesInGroup("enemy"))
        {
            if (node.IsQueuedForDeletion())
            {
                continue;
            }

            if (node.HasMeta("stage_boss_kind") && (int)node.GetMeta("stage_boss_kind") == (int)kind)
            {
                return true;
            }
        }

        return false;
    }

    private void ApplyActClimate(StageClimateProfile.ActClimate climate)
    {
        _currentClimate = climate;
        if (_weather is not null && _appliedWeather != climate.Weather)
        {
            _weather.SetState(climate.Weather);
            _appliedWeather = climate.Weather;
        }

        if (_timeOfDay is not null && _appliedTime != climate.Time)
        {
            _timeOfDay.SetState(climate.Time);
            _appliedTime = climate.Time;
        }

        _weather?.SetWindStrength(climate.WindStrength);
    }

    private void ApplyBossClimate(StageClimateProfile.BossClimate climate)
    {
        if (_weather is not null && _appliedWeather != climate.Weather)
        {
            _weather.SetState(climate.Weather);
            _appliedWeather = climate.Weather;
        }

        if (_timeOfDay is not null && _appliedTime != climate.Time)
        {
            _timeOfDay.SetState(climate.Time);
            _appliedTime = climate.Time;
        }

        _weather?.SetWindStrength(climate.WindStrength);
    }

    private void ApplyFogBoost(float boost)
    {
        if (_fogLayer is null || _weather is null)
        {
            return;
        }

        Color color = _fogLayer.Color;
        float targetAlpha = Mathf.Clamp(GetBaseFogAlpha(_weather.CurrentState) + boost, 0f, 0.62f);
        color.A = Mathf.Lerp(color.A, targetAlpha, 0.04f);
        _fogLayer.Color = color;
    }

    private static float GetBaseFogAlpha(WeatherController.WeatherState state)
    {
        return state switch
        {
            WeatherController.WeatherState.Dry => 0.06f,
            WeatherController.WeatherState.HeavyRain => 0.20f,
            WeatherController.WeatherState.Thunderstorm => 0.26f,
            _ => 0.13f,
        };
    }

    private void TickWindGusts(float dt, float strength)
    {
        if (strength < 0.4f)
        {
            return;
        }

        _windGustTimer -= dt;
        if (_windGustTimer > 0f)
        {
            return;
        }

        _windGustTimer = (float)GD.RandRange(5.5 / strength, 9.0 / strength);
        float push = strength * (float)GD.RandRange(18f, 42f);
        foreach (Node node in GetTree().GetNodesInGroup("wind_prop"))
        {
            if (node is Node2D prop)
            {
                prop.Position += new Vector2(push, (float)GD.RandRange(-4f, 4f));
            }
        }
    }

    private void TickRainAmbience(float dt, WeatherController.WeatherState weather)
    {
        float intensity = weather switch
        {
            WeatherController.WeatherState.Drizzle => 0.35f,
            WeatherController.WeatherState.HeavyRain => 0.72f,
            WeatherController.WeatherState.Thunderstorm => 0.85f,
            _ => 0f,
        };

        if (intensity <= 0f)
        {
            return;
        }

        _rainAudioTimer -= dt;
        if (_rainAudioTimer > 0f)
        {
            return;
        }

        _rainAudioTimer = (float)GD.RandRange(0.35, 0.75) / intensity;
        WeatherAmbience.PlayRainTick(this, intensity);
    }

    public void TriggerBlackout(float seconds)
    {
        _blackoutRemaining = Mathf.Max(_blackoutRemaining, seconds);
        if (_blackoutOverlay is not null)
        {
            _blackoutOverlay.Color = new Color(0f, 0f, 0f, 0.58f);
        }
    }

    private void TickBlackout(float dt)
    {
        if (_blackoutRemaining <= 0f)
        {
            return;
        }

        _blackoutRemaining -= dt;
        if (_blackoutOverlay is null)
        {
            return;
        }

        if (_blackoutRemaining <= 0f)
        {
            _blackoutOverlay.Color = new Color(0f, 0f, 0f, 0f);
            return;
        }

        float alpha = Mathf.Lerp(0.58f, 0f, 1f - _blackoutRemaining / 1.35f);
        _blackoutOverlay.Color = new Color(0f, 0f, 0f, alpha * 0.85f);
    }

    private void BuildBlackoutOverlay()
    {
        _blackoutOverlay = new ColorRect
        {
            Name = "ClimateBlackout",
            OffsetLeft = -1200f,
            OffsetTop = 0f,
            OffsetRight = 4000f,
            OffsetBottom = 720f,
            Color = new Color(0f, 0f, 0f, 0f),
            MouseFilter = Control.MouseFilterEnum.Ignore,
            ZIndex = 120,
        };
        AddChild(_blackoutOverlay);
    }

    private void SpawnHazardZones()
    {
        Node parent = GetParent() ?? this;
        SpawnHazard(parent, new Vector2(-240f, 598f), WeatherHazardZone.HazardKind.MudSlow);
        SpawnHazard(parent, new Vector2(720f, 588f), WeatherHazardZone.HazardKind.ElectricPuddle);
        SpawnHazard(parent, new Vector2(1580f, 612f), WeatherHazardZone.HazardKind.MudSlow);
        SpawnHazard(parent, new Vector2(1620f, 612f), WeatherHazardZone.HazardKind.ElectricPuddle, 72f);
        SpawnHazard(parent, new Vector2(2340f, 578f), WeatherHazardZone.HazardKind.MudSlow, 96f);
        SpawnHazard(parent, new Vector2(2940f, 568f), WeatherHazardZone.HazardKind.ElectricPuddle, 100f);
    }

    private static void SpawnHazard(Node parent, Vector2 position, WeatherHazardZone.HazardKind kind, float radius = 88f)
    {
        WeatherHazardZone zone = new()
        {
            Name = $"ClimateHazard_{kind}_{(int)position.X}",
            Position = position,
            Kind = kind,
            Radius = radius,
        };
        parent.AddChild(zone);
    }

    private void FindPlayer()
    {
        _player ??= GetNodeOrNull<SideScrollerPlayerController>(PlayerPath);
        _player ??= GetTree().GetFirstNodeInGroup("side_player") as SideScrollerPlayerController;
    }
}
