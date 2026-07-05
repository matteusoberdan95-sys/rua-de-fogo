namespace SangueNoAsfalto.Player;

public partial class PlayerController : CharacterBody2D
{
    [Export]
    public float MoveSpeed { get; set; } = 230f;

    [Export]
    public float DashSpeed { get; set; } = 620f;

    [Export]
    public float DashDuration { get; set; } = 0.14f;

    [Export]
    public float AttackDuration { get; set; } = 0.12f;

    [Export]
    public float ComboResetTime { get; set; } = 0.55f;

    [Export]
    public float ShootCooldown { get; set; } = 0.28f;

    [Export]
    public float ShootStaminaCost { get; set; } = 14f;

    [Export]
    public float MaxStamina { get; set; } = 100f;

    [Export]
    public float DashStaminaCost { get; set; } = 28f;

    [Export]
    public float StaminaRegenPerSecond { get; set; } = 34f;

    [Export]
    public PackedScene? ProjectileScene { get; set; }

    public float CurrentStamina { get; private set; }

    public Vector2 LastMovementInput { get; private set; }

    private Vector2 _facing = Vector2.Right;
    private float _dashTimeRemaining;
    private float _attackTimeRemaining;
    private float _comboResetRemaining;
    private float _shootCooldownRemaining;
    private int _comboIndex;
    private Hitbox? _attackArea;
    private CollisionShape2D? _attackCollision;
    private Health? _health;

    public override void _Ready()
    {
        AddToGroup("player");
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

        if (input.LengthSquared() > 0.01f)
        {
            _facing = input.Normalized();
        }

        TickAttack(dt);
        TickDash(dt, input);
        TickCombo(dt);
        TickShoot(dt);
        RegenerateStamina(dt);
        UpdateInvulnerabilityVisual();

        if (_dashTimeRemaining <= 0f)
        {
            Velocity = input * MoveSpeed;
        }

        MoveAndSlide();
        UpdateAttackArc();

        if (Input.IsActionJustPressed("attack") || IsKeyJustPressed(Key.J))
        {
            StartAttack();
        }

        if (Input.IsActionJustPressed("dash") || IsKeyJustPressed(Key.K) || IsKeyJustPressed(Key.Space))
        {
            StartDash(input);
        }

        if (Input.IsActionJustPressed("shoot") || IsKeyJustPressed(Key.L))
        {
            Shoot();
        }
    }

    private static Vector2 ReadMovementInput()
    {
        Vector2 input = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        if (input.LengthSquared() > 0.01f)
        {
            return input;
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

    private static bool IsKeyJustPressed(Key key)
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
        Velocity = (input.LengthSquared() > 0.01f ? input.Normalized() : _facing) * DashSpeed;
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
                1 => 28,
                2 => 46,
                _ => 24,
            };

            _attackArea.KnockbackForce = _comboIndex == 2 ? 460f : 320f;
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
        Velocity = (input.LengthSquared() > 0.01f ? input.Normalized() : _facing) * DashSpeed;
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
        projectile.Direction = _facing;
        projectile.GlobalPosition = GlobalPosition + _facing * 54f;
        projectile.Rotation = _facing.Angle();

        GetTree().CurrentScene?.AddChild(projectile);
    }

    private void UpdateAttackArc()
    {
        if (_attackArea is null)
        {
            return;
        }

        _attackArea.Position = _facing * 42f;
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

    private void UpdateInvulnerabilityVisual()
    {
        if (_health is not null && _health.IsInvulnerable)
        {
            Modulate = new Color(0.75f, 0.88f, 1f, 0.72f);
            return;
        }

        Modulate = Colors.White;
    }

    private void OnDied()
    {
        SetPhysicsProcess(false);
        Modulate = Colors.DarkRed;
    }
}
