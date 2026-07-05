namespace SangueNoAsfalto.Combat;

public partial class Hurtbox : Area2D
{
    [Export]
    public NodePath HealthPath { get; set; } = "../Health";

    private Health? _health;

    public override void _Ready()
    {
        _health = GetNodeOrNull<Health>(HealthPath);
        AreaEntered += OnAreaEntered;
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area is not Hitbox hitbox || hitbox.Source == Owner)
        {
            return;
        }

        bool damaged = _health?.Damage(hitbox.Damage) == true;
        if (!damaged)
        {
            return;
        }

        if (hitbox.Source is SideScrollerPlayerController player)
        {
            player.RegisterCombatHit(hitbox.Damage);
        }

        ApplyKnockback(hitbox);
        PlayFeedback(hitbox);
    }

    private void ApplyKnockback(Hitbox hitbox)
    {
        if (Owner is not CharacterBody2D body || hitbox.Source is not Node2D source)
        {
            return;
        }

        Vector2 direction = body.GlobalPosition.DirectionTo(source.GlobalPosition) * -1f;
        Vector2 impulse = direction * hitbox.KnockbackForce;
        if (Owner is ICombatKnockbackReceiver receiver)
        {
            receiver.ReceiveKnockback(impulse, hitbox.HitStunDuration);
            return;
        }

        body.Velocity += impulse;
    }

    private void PlayFeedback(Hitbox hitbox)
    {
        if (Owner is not Node2D target || hitbox.Source is not Node2D source)
        {
            return;
        }

        CombatFeedback.PlayHit(target, source, hitbox.Damage);
    }
}
