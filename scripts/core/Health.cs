using Godot;

namespace SangueNoAsfalto.Core;

public partial class Health : Node
{
    [Signal]
    public delegate void DiedEventHandler();

    [Signal]
    public delegate void ChangedEventHandler(int current, int maximum);

    [Export]
    public int MaxHealth { get; set; } = 100;

    [Export]
    public float InvulnerabilityAfterDamage { get; set; }

    public int CurrentHealth { get; private set; }

    public bool IsInvulnerable => _invulnerabilityRemaining > 0f;

    private float _invulnerabilityRemaining;

    public override void _Ready()
    {
        CurrentHealth = MaxHealth;
        EmitSignal(SignalName.Changed, CurrentHealth, MaxHealth);
    }

    public override void _Process(double delta)
    {
        if (_invulnerabilityRemaining > 0f)
        {
            _invulnerabilityRemaining = Mathf.Max(_invulnerabilityRemaining - (float)delta, 0f);
        }
    }

    public void Damage(int amount)
    {
        if (amount <= 0 || CurrentHealth <= 0 || IsInvulnerable)
        {
            return;
        }

        CurrentHealth = Mathf.Max(CurrentHealth - amount, 0);
        EmitSignal(SignalName.Changed, CurrentHealth, MaxHealth);

        if (CurrentHealth > 0f && InvulnerabilityAfterDamage > 0f)
        {
            MakeInvulnerable(InvulnerabilityAfterDamage);
        }

        if (CurrentHealth == 0)
        {
            EmitSignal(SignalName.Died);
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0 || CurrentHealth <= 0)
        {
            return;
        }

        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
        EmitSignal(SignalName.Changed, CurrentHealth, MaxHealth);
    }

    public void MakeInvulnerable(float seconds)
    {
        _invulnerabilityRemaining = Mathf.Max(_invulnerabilityRemaining, seconds);
    }
}
