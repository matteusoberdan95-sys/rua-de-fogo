namespace SangueNoAsfalto.Combat;

public partial class Hitbox : Area2D
{
    [Export]
    public int Damage { get; set; } = 20;

    [Export]
    public float KnockbackForce { get; set; } = 240f;

    [Export]
    public float HitStunDuration { get; set; } = 0.14f;

    public bool ApplyBleedOnHit { get; set; }

    public float BleedDuration { get; set; } = 3f;

    public float BleedDamagePerSecond { get; set; } = 4f;

    [Export]
    public float PostureDamage { get; set; } = 18f;

    public bool IsFinisherHit { get; set; }

    public bool IsPostureKill { get; set; }

    public Node? Source { get; set; }
}

public interface ICombatKnockbackReceiver
{
    void ReceiveKnockback(Vector2 impulse, float duration);
}
