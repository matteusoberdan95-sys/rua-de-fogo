using Godot;
using SangueNoAsfalto.Combat;
using SangueNoAsfalto.Core;

namespace SangueNoAsfalto.Player;

public partial class SideScrollerPlayerController : CharacterBody2D
{
    [Export]
    public float HorizontalSpeed { get; set; } = 260f;

    [Export]
    public float LaneSpeed { get; set; } = 150f;

    [Export]
    public float DashSpeed { get; set; } = 690f;

    [Export]
    public float DashDuration { get; set; } = 0.14f;

    [Export]
    public float AttackDuration { get; set; } = 0.13f;

    [Export]
    public float ComboResetTime { get; set; } = 0.58f;

    [Export]
    public float ShootCooldown { get; set; } = 0.32f;

    [Export]
    public float ShootStaminaCost { get; set; } = 16f;

    [Export]
    public float MaxStamina { get; set; } = 100f;

    [Export]
    public float DashStaminaCost { get; set; } = 28f;

    [Export]
    public float StaminaRegenPerSecond { get; set; } = 32f;

    [Export]
    public float MinX { get; set; } = -1060f;

    [Export]
    public float MaxX { get; set; } = 1060f;

    [Export]
    public float MinLaneY { get; set; } = 260f;

    [Export]
    public float MaxLaneY { get; set; } = 470f;

    [Export]
    public PackedScene? ProjectileScene { get; set; }

    public float CurrentStamina { get; private set; }

    public Vector2 LastMovementInput { get; private set; }

    public int FacingSign { get; private set; } = 1;

    private readonly KeyPressLatch _attackLatch = new(Key.J);
    private readonly KeyPressLatch _dashLatch = new(Key.K, Key.Space);
    private readonly KeyPressLatch _shootLatch = new(Key.L);
    private Hitbox? _attackArea;
    private CollisionShape2D? _attackCollision;
    private Health? _health;
    private float _dashTimeRemaining;
    private float _attackTimeRemaining;
    private float _comboResetRemaining;
    private float _shootCooldownRemaining;
    private int _comboIndex;

    public override void _Ready()
    {
        AddToGroup("player");
        AddToGroup("side_player");
        CurrentStamina = MaxStamina;

        _attackArea = GetNodeOrNull<Hitbox>("AttackArea");
        _attackCollision = GetNodeOrNull<CollisionShape2D>("AttackArea/CollisionShape2D");

        if (_attackArea is not null)
        {
            _attackArea.Source = this;
            _attackArea.Monitoring = false;
        }

        SetAttackCollision(false);

        _health = GetNodeOrNull<Health>("Health");
        if (_health is not null)
        {
            _health.Died += OnDied;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;
        Vector2 input = ReadMovementInput();
        LastMovementInput = input;

        if (Mathf.Abs(input.X) > 0.01f)
        {
            FacingSign = input.X > 0f ? 1 : -1;
        }

        TickAttack(dt);
        TickDash(dt, input);
        TickCombo(dt);
        TickShoot(dt);
        RegenerateStamina(dt);
        UpdateAttackArc();
        UpdateFacingVisual();
        UpdateInvulnerabilityVisual();

        if (_dashTimeRemaining <= 0f)
        {
            Velocity = new Vector2(input.X * HorizontalSpeed, input.Y * LaneSpeed);
        }

        MoveAndSlide();
        ClampToPlayableArea();

        if (Input.IsActionJustPressed("attack") || _attackLatch.ConsumeJustPressed())
        {
            StartAttack();
        }

        if (Input.IsActionJustPressed("dash") || _dashLatch.ConsumeJustPressed())
        {
            StartDash(input);
        }

        if (Input.IsActionJustPressed("shoot") || _shootLatch.ConsumeJustPressed())
        {
            Shoot();
        }
    }

    private static Vector2 ReadMovementInput()
    {
        Vector2 input = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        if (input.LengthSquared() > 0.01f)
        {
            return input.LengthSquared() > 1f ? input.Normalized() : input;
        }

        float x = 0f;
        float y = 0f;

        if (IsKeyDown(Key.A) || IsKeyDown(Key.Left))
        {
            x -= 1f;
        }

        if (IsKeyDown(Key.D) || IsKeyDown(Key.Right))
        {
            x += 1f;
        }

        if (IsKeyDown(Key.W) || IsKeyDown(Key.Up))
        {
            y -= 1f;
        }

        if (IsKeyDown(Key.S) || IsKeyDown(Key.Down))
        {
            y += 1f;
        }

        input = new Vector2(x, y);
        return input.LengthSquared() > 1f ? input.Normalized() : input;
    }

    private static bool IsKeyDown(Key key)
    {
        return Input.IsKeyPressed(key) || Input.IsPhysicalKeyPressed(key);
    }

    private void TickDash(float dt, Vector2 input)
    {
        if (_dashTimeRemaining <= 0f)
        {
            return;
        }

        _dashTimeRemaining -= dt;
        float direction = Mathf.Abs(input.X) > 0.01f ? Mathf.Sign(input.X) : FacingSign;
        Velocity = new Vector2(direction * DashSpeed, input.Y * LaneSpeed * 0.4f);
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

    private void TickCombo(float dt)
    {
        if (_comboResetRemaining <= 0f)
        {
            _comboIndex = 0;
            return;
        }

        _comboResetRemaining -= dt;
    }

    private void TickShoot(float dt)
    {
        if (_shootCooldownRemaining > 0f)
        {
            _shootCooldownRemaining = Mathf.Max(_shootCooldownRemaining - dt, 0f);
        }
    }

    private void RegenerateStamina(float dt)
    {
        if (_dashTimeRemaining > 0f)
        {
            return;
        }

        CurrentStamina = Mathf.Min(CurrentStamina + StaminaRegenPerSecond * dt, MaxStamina);
    }

    private void StartAttack()
    {
        if (_attackTimeRemaining > 0f)
        {
            return;
        }

        _attackTimeRemaining = AttackDuration;
        _comboIndex = _comboResetRemaining > 0f ? (_comboIndex + 1) % 3 : 0;
        _comboResetRemaining = ComboResetTime;

        if (_attackArea is not null)
        {
            _attackArea.Damage = _comboIndex switch
            {
                1 => 30,
                2 => 50,
                _ => 24,
            };

            _attackArea.KnockbackForce = _comboIndex == 2 ? 520f : 350f;
        }

        SetAttackCollision(true);
    }

    private void StartDash(Vector2 input)
    {
        if (_dashTimeRemaining > 0f || CurrentStamina < DashStaminaCost)
        {
            return;
        }

        CurrentStamina -= DashStaminaCost;
        _dashTimeRemaining = DashDuration;
        _health?.MakeInvulnerable(DashDuration + 0.08f);
        float direction = Mathf.Abs(input.X) > 0.01f ? Mathf.Sign(input.X) : FacingSign;
        Velocity = new Vector2(direction * DashSpeed, input.Y * LaneSpeed * 0.45f);
    }

    private void Shoot()
    {
        if (ProjectileScene is null || _shootCooldownRemaining > 0f || CurrentStamina < ShootStaminaCost)
        {
            return;
        }

        CurrentStamina -= ShootStaminaCost;
        _shootCooldownRemaining = ShootCooldown;

        Projectile projectile = ProjectileScene.Instantiate<Projectile>();
        projectile.Source = this;
        projectile.Direction = new Vector2(FacingSign, 0f);
        projectile.GlobalPosition = GlobalPosition + new Vector2(FacingSign * 58f, -8f);
        projectile.Rotation = FacingSign > 0 ? 0f : Mathf.Pi;

        GetTree().CurrentScene?.AddChild(projectile);
    }

    private void UpdateAttackArc()
    {
        if (_attackArea is null)
        {
            return;
        }

        _attackArea.Position = new Vector2(FacingSign * 45f, -6f);
        _attackArea.Rotation = FacingSign > 0 ? 0f : Mathf.Pi;
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

    private void ClampToPlayableArea()
    {
        GlobalPosition = new Vector2(
            Mathf.Clamp(GlobalPosition.X, MinX, MaxX),
            Mathf.Clamp(GlobalPosition.Y, MinLaneY, MaxLaneY));
    }

    private void UpdateFacingVisual()
    {
        foreach (Node child in GetChildren())
        {
            if (child is Polygon2D polygon && child.Name != "LaneShadow")
            {
                polygon.Scale = new Vector2(FacingSign, 1f);
            }
        }
    }

    private void UpdateInvulnerabilityVisual()
    {
        if (_health is not null && _health.IsInvulnerable)
        {
            Modulate = new Color(0.78f, 0.9f, 1f, 0.72f);
            return;
        }

        Modulate = Colors.White;
    }

    private void OnDied()
    {
        SetPhysicsProcess(false);
        Modulate = Colors.DarkRed;
    }

    private sealed class KeyPressLatch
    {
        private readonly Key[] _keys;
        private bool _wasDown;

        public KeyPressLatch(params Key[] keys)
        {
            _keys = keys;
        }

        public bool ConsumeJustPressed()
        {
            bool isDown = false;
            foreach (Key key in _keys)
            {
                isDown = isDown || Input.IsKeyPressed(key) || Input.IsPhysicalKeyPressed(key);
            }

            bool justPressed = isDown && !_wasDown;
            _wasDown = isDown;
            return justPressed;
        }
    }
}
