namespace SangueNoAsfalto.Enemies;

public partial class EnemyGruntController : CharacterBody2D
{
    [Export]
    public float MoveSpeed { get; set; } = 130f;

    [Export]
    public float AggroRange { get; set; } = 520f;

    [Export]
    public float AttackRange { get; set; } = 58f;

    [Export]
    public float AttackCooldown { get; set; } = 0.8f;

    [Export]
    public float AttackDuration { get; set; } = 0.1f;

    private Node2D? _target;
    private Hitbox? _attackArea;
    private CollisionShape2D? _attackCollision;
    private float _cooldownRemaining;
    private float _attackTimeRemaining;
    private Vector2 _facing = Vector2.Left;

    public override void _Ready()
    {
        AddToGroup("enemy");

        _attackArea = GetNodeOrNull<Hitbox>("AttackArea");
        _attackCollision = GetNodeOrNull<CollisionShape2D>("AttackArea/CollisionShape2D");

        if (_attackArea is not null)
        {
            _attackArea.Source = this;
            _attackArea.Monitoring = false;
        }

        SetAttackCollision(false);

        Health? health = GetNodeOrNull<Health>("Health");
        if (health is not null)
        {
            health.Died += OnDied;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;
        _cooldownRemaining = Mathf.Max(_cooldownRemaining - dt, 0f);
        TickAttack(dt);

        _target ??= GetTree().GetFirstNodeInGroup("player") as Node2D;
        if (_target is null)
        {
            return;
        }

        float distance = GlobalPosition.DistanceTo(_target.GlobalPosition);
        if (distance > AggroRange)
        {
            Velocity = Vector2.Zero;
            MoveAndSlide();
            return;
        }

        Vector2 direction = GlobalPosition.DirectionTo(_target.GlobalPosition);
        _facing = direction;
        UpdateAttackArc();

        if (distance <= AttackRange)
        {
            Velocity = Vector2.Zero;
            TryAttack();
        }
        else
        {
            Velocity = direction * MoveSpeed;
        }

        MoveAndSlide();
    }

    private void TryAttack()
    {
        if (_cooldownRemaining > 0f || _attackTimeRemaining > 0f)
        {
            return;
        }

        _cooldownRemaining = AttackCooldown;
        _attackTimeRemaining = AttackDuration;
        SetAttackCollision(true);
    }

    private void TickAttack(float dt)
    {
        if (_attackTimeRemaining <= 0f)
        {
            return;
        }

        _attackTimeRemaining -= dt;
        if (_attackTimeRemaining <= 0f)
        {
            SetAttackCollision(false);
        }
    }

    private void UpdateAttackArc()
    {
        if (_attackArea is null)
        {
            return;
        }

        _attackArea.Position = _facing * 38f;
        _attackArea.Rotation = _facing.Angle();
    }

    private void SetAttackCollision(bool enabled)
    {
        if (_attackArea is not null)
        {
            _attackArea.Monitoring = enabled;
        }

        if (_attackCollision is not null)
        {
            _attackCollision.Disabled = !enabled;
        }
    }

    private void OnDied()
    {
        QueueFree();
    }
}
