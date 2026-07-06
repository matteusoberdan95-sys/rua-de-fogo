namespace SangueNoAsfalto.Combat;

public partial class Projectile : Hitbox
{
    [Export]
    public float Speed { get; set; } = 520f;

    [Export]
    public float Lifetime { get; set; } = 1.2f;

    public Vector2 Direction { get; set; } = Vector2.Right;

    public override void _Ready()
    {
        Monitoring = true;
        AreaEntered += OnAreaEntered;
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;
        Position += Direction.Normalized() * Speed * dt;

        Lifetime -= dt;
        if (Lifetime <= 0f)
        {
            QueueFree();
        }
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area.Owner == Source)
        {
            return;
        }

        QueueFree();
    }
}
