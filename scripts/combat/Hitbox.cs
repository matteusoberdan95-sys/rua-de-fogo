using Godot;

namespace SangueNoAsfalto.Combat;

public partial class Hitbox : Area2D
{
    [Export]
    public int Damage { get; set; } = 20;

    [Export]
    public float KnockbackForce { get; set; } = 240f;

    public Node? Source { get; set; }
}
