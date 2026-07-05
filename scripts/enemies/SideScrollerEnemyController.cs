using Godot;
using SangueNoAsfalto.Combat;
using SangueNoAsfalto.Core;

namespace SangueNoAsfalto.Enemies;

public partial class SideScrollerEnemyController : CharacterBody2D
{
    [Export]
    public float MoveSpeed { get; set; } = 120f;

    [Export]
    public float LaneSpeed { get; set; } = 95f;

    [Export]
    public float AggroRange { get; set; } = 900f;

    [Export]
    public float AttackRangeX { get; set; } = 58f;

    [Export]
    public float AttackRangeY { get; set; } = 34f;

    [Export]
    public float AttackCooldown { get; set; } = 0.95f;

    [Export]
    public float TelegraphDuration { get; set; } = 0.18f;

    [Export]
    public float AttackDuration { get; set; } = 0.11f;

    [Export]
    public float MinLaneY { get; set; } = 260f;

    [Export]
    public float MaxLaneY { get; set; } = 470f;

    private Node2D? _target;
    private Hitbox? _attackArea;
    private CollisionShape2D? _attackCollision;
    private float _cooldownRemaining;
    private float _telegraphRemaining;
    private float _attackTimeRemaining;
    private int _facingSign = -1;

    public override void _Ready()
    {
        AddToGroup("enemy");
        AddToGroup("side_enemy");

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
        TickTelegraph(dt);
        TickAttack(dt);

        _target ??= GetTree().GetFirstNodeInGroup("side_player") as Node2D;
        _target ??= GetTree().GetFirstNodeInGroup("player") as Node2D;
        if (_target is null)
        {
            return;
        }

        Vector2 deltaToTarget = _target.GlobalPosition - GlobalPosition;
        if (deltaToTarget.Length() > AggroRange)
        {
            Velocity = Vector2.Zero;
            MoveAndSlide();
            return;
        }

        if (Mathf.Abs(deltaToTarget.X) > 0.01f)
        {
            _facingSign = deltaToTarget.X > 0f ? 1 : -1;
        }

        UpdateAttackArc();
        UpdateFacingVisual();

        bool alignedForHit = Mathf.Abs(deltaToTarget.X) <= AttackRangeX && Mathf.Abs(deltaToTarget.Y) <= AttackRangeY;
        if (alignedForHit)
        {
            Velocity = Vector2.Zero;
            TryAttack();
        }
        else if (_telegraphRemaining <= 0f && _attackTimeRemaining <= 0f)
        {
            float x = Mathf.Abs(deltaToTarget.X) > AttackRangeX * 0.75f ? Mathf.Sign(deltaToTarget.X) : 0f;
            float y = Mathf.Abs(deltaToTarget.Y) > AttackRangeY * 0.65f ? Mathf.Sign(deltaToTarget.Y) : 0f;
            Velocity = new Vector2(x * MoveSpeed, y * LaneSpeed);
        }

        MoveAndSlide();
        GlobalPosition = new Vector2(GlobalPosition.X, Mathf.Clamp(GlobalPosition.Y, MinLaneY, MaxLaneY));
    }

    private void TryAttack()
    {
        if (_cooldownRemaining > 0f || _telegraphRemaining > 0f || _attackTimeRemaining > 0f)
        {
            return;
        }

        _cooldownRemaining = AttackCooldown;
        _telegraphRemaining = TelegraphDuration;
        Modulate = new Color(1f, 0.58f, 0.42f, 1f);
    }

    private void TickTelegraph(float dt)
    {
        if (_telegraphRemaining <= 0f)
        {
            return;
        }

        _telegraphRemaining -= dt;
        if (_telegraphRemaining <= 0f)
        {
            _attackTimeRemaining = AttackDuration;
            SetAttackCollision(true);
            Modulate = Colors.White;
        }
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

        _attackArea.Position = new Vector2(_facingSign * 38f, -4f);
        _attackArea.Rotation = _facingSign > 0 ? 0f : Mathf.Pi;
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

    private void UpdateFacingVisual()
    {
        foreach (Node child in GetChildren())
        {
            if (child is Polygon2D polygon && child.Name != "LaneShadow")
            {
                polygon.Scale = new Vector2(_facingSign, 1f);
            }
        }
    }

    private void OnDied()
    {
        QueueFree();
    }
}
