namespace SangueNoAsfalto.Combat;

/// <summary>
/// Sangramento por tiro ou corte — drena HP ao longo do tempo com gotas visuais.
/// </summary>
public partial class BleedEffect : Node
{
    [Export]
    public NodePath HealthPath { get; set; } = "../Health";

    private Health? _health;
    private Node2D? _ownerBody;
    private float _remainingDuration;
    private float _damagePerSecond;
    private float _tickAccumulator;
    private float _dripAccumulator;

    public bool IsActive => _remainingDuration > 0f;

    public override void _Ready()
    {
        _health = GetNodeOrNull<Health>(HealthPath);
        _ownerBody = Owner as Node2D ?? GetParent() as Node2D;
    }

    public override void _Process(double delta)
    {
        if (_remainingDuration <= 0f || _health is null || _health.CurrentHealth <= 0)
        {
            return;
        }

        float dt = (float)delta;
        _remainingDuration -= dt;
        _tickAccumulator += dt;
        _dripAccumulator += dt;

        if (_dripAccumulator >= 0.42f)
        {
            _dripAccumulator = 0f;
            SpawnBloodDrip();
        }

        if (_tickAccumulator < 0.35f)
        {
            return;
        }

        _tickAccumulator = 0f;
        _health.Damage(Mathf.Max(1, Mathf.RoundToInt(_damagePerSecond * 0.35f)));
    }

    public void Apply(float duration, float damagePerSecond)
    {
        _remainingDuration = Mathf.Max(_remainingDuration, duration);
        _damagePerSecond = Mathf.Max(_damagePerSecond, damagePerSecond);
    }

    private void SpawnBloodDrip()
    {
        if (_ownerBody is null)
        {
            return;
        }

        Node? parent = _ownerBody.GetParent();
        if (parent is null)
        {
            return;
        }

        Polygon2D drip = new()
        {
            Color = new Color(0.62f, 0.01f, 0.02f, 0.92f),
            Polygon =
            [
                new Vector2(-2f, -4f),
                new Vector2(2f, -4f),
                new Vector2(3f, 2f),
                new Vector2(0f, 8f),
                new Vector2(-3f, 2f),
            ],
            Scale = Vector2.One * (0.85f + GD.Randf() * 0.35f),
            ZIndex = 9,
        };

        parent.AddChild(drip);
        float side = GD.Randf() * 14f - 7f;
        drip.GlobalPosition = _ownerBody.GlobalPosition + new Vector2(side, -8f + GD.Randf() * 6f);

        Tween tween = drip.CreateTween();
        tween.TweenProperty(drip, "position", drip.Position + new Vector2(side * 0.2f, 28f + GD.Randf() * 10f), 0.35f);
        tween.Parallel().TweenProperty(drip, "modulate:a", 0f, 0.45f);
        tween.TweenCallback(Callable.From(drip.QueueFree));
    }
}
