using Godot;
using SangueNoAsfalto.Core;

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

        _health?.Damage(hitbox.Damage);
        ApplyKnockback(hitbox);
    }

    private void ApplyKnockback(Hitbox hitbox)
    {
        if (Owner is not CharacterBody2D body || hitbox.Source is not Node2D source)
        {
            return;
        }

        Vector2 direction = body.GlobalPosition.DirectionTo(source.GlobalPosition) * -1f;
        body.Velocity += direction * hitbox.KnockbackForce;
    }
}
