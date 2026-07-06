namespace SangueNoAsfalto.Combat;

/// <summary>
/// Sangramento por tiro — drena HP ao longo do tempo (base para hemorragia por calibre).
/// </summary>
public partial class BleedEffect : Node
{
    [Export]
    public NodePath HealthPath { get; set; } = "../Health";

    private Health? _health;
    private float _remainingDuration;
    private float _damagePerSecond;
    private float _tickAccumulator;

    public override void _Ready()
    {
        _health = GetNodeOrNull<Health>(HealthPath);
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
}
