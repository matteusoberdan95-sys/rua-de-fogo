namespace SangueNoAsfalto.Combat;

public partial class Hitbox : Area2D
{
    [Export]
    public int Damage { get; set; } = 20;

    [Export]
    public float KnockbackForce { get; set; } = 240f;

    [Export]
    public float HitStunDuration { get; set; } = 0.14f;

    public Node? Source { get; set; }
}

public interface ICombatKnockbackReceiver
{
    void ReceiveKnockback(Vector2 impulse, float duration);
}
