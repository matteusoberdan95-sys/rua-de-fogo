namespace SangueNoAsfalto.Player;

public partial class SideScrollerPlayerController : CharacterBody2D, ICombatKnockbackReceiver
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
    public float JumpDuration { get; set; } = 0.42f;

    [Export]
    public float JumpHeight { get; set; } = 62f;

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

    public string WeaponName => _hasImprovisedWeapon ? "Vergalhao" : "Facao";

    public int WeaponDurability => _weaponDurability;

    public int Continues => _continues;

    private readonly KeyPressLatch _attackLatch = new(Key.J);
    private readonly KeyPressLatch _dashLatch = new(Key.K);
    private readonly KeyPressLatch _jumpLatch = new(Key.Space);
    private readonly KeyPressLatch _shootLatch = new(Key.L);
    private readonly Dictionary<Polygon2D, Vector2> _jumpVisualOrigins = [];
    private Hitbox? _attackArea;
    private CollisionShape2D? _attackCollision;
    private Health? _health;
    private float _dashTimeRemaining;
    private float _attackTimeRemaining;
    private float _comboResetRemaining;
    private float _shootCooldownRemaining;
    private float _hitStunRemaining;
    private float _jumpTimeRemaining;
    private bool _hasImprovisedWeapon;
    private int _weaponDurability;
    private int _continues;
    private int _comboIndex;

    public override void _Ready()
    {
        AddToGroup("player");
        AddToGroup("side_player");
        CurrentStamina = MaxStamina;
        ApplySavedState();
        CaptureJumpVisuals();

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
        TickJump(dt);
        RegenerateStamina(dt);
        UpdateAttackArc();
        UpdateFacingVisual();
        UpdateJumpVisual();
        UpdateInvulnerabilityVisual();

        if (TickHitStun(dt))
        {
            MoveAndSlide();
            ClampToPlayableArea();
            return;
        }

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

        if (Input.IsActionJustPressed("jump") || _jumpLatch.ConsumeJustPressed())
        {
            StartJump();
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

    private void TickJump(float dt)
    {
        if (_jumpTimeRemaining > 0f)
        {
            _jumpTimeRemaining = Mathf.Max(_jumpTimeRemaining - dt, 0f);
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

    private bool TickHitStun(float dt)
    {
        if (_hitStunRemaining <= 0f)
        {
            return false;
        }

        _hitStunRemaining -= dt;
        Velocity = Velocity.MoveToward(Vector2.Zero, 1300f * dt);
        if (_hitStunRemaining <= 0f)
        {
            Velocity = Vector2.Zero;
        }

        return true;
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
            if (_hasImprovisedWeapon)
            {
                _attackArea.Damage += _comboIndex == 2 ? 18 : 10;
                _attackArea.KnockbackForce += 130f;
            }
        }

        SetAttackCollision(true);
        SpawnSlashEffect(_comboIndex);
        ConsumeWeaponDurability();
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

    private void StartJump()
    {
        if (_jumpTimeRemaining > 0f || _hitStunRemaining > 0f)
        {
            return;
        }

        _jumpTimeRemaining = JumpDuration;
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

    private void SpawnSlashEffect(int comboIndex)
    {
        Node? parent = GetParent();
        if (parent is null)
        {
            return;
        }

        float reach = comboIndex == 2 ? 92f : 72f;
        float height = comboIndex == 2 ? 54f : 40f;
        Polygon2D slash = new()
        {
            Color = comboIndex == 2
                ? new Color(1f, 0.16f, 0.08f, 0.9f)
                : new Color(1f, 0.86f, 0.48f, 0.78f),
            Polygon =
            [
                new Vector2(0f, -height * 0.5f),
                new Vector2(reach, -height * 0.18f),
                new Vector2(reach + 18f, 0f),
                new Vector2(reach, height * 0.18f),
                new Vector2(0f, height * 0.5f),
                new Vector2(24f, 0f)
            ],
            ZIndex = 12
        };

        parent.AddChild(slash);
        slash.GlobalPosition = GlobalPosition + new Vector2(FacingSign * 16f, -8f);
        slash.Scale = new Vector2(FacingSign, 1f);
        slash.Rotation = comboIndex switch
        {
            1 => -0.18f * FacingSign,
            2 => 0.22f * FacingSign,
            _ => 0f,
        };

        Tween tween = slash.CreateTween();
        tween.TweenProperty(slash, "scale", new Vector2(slash.Scale.X * 1.16f, slash.Scale.Y * 1.22f), 0.055f);
        tween.Parallel().TweenProperty(slash, "modulate:a", 0f, 0.09f);
        tween.TweenCallback(Callable.From(slash.QueueFree));
    }

    public void ReceiveKnockback(Vector2 impulse, float duration)
    {
        _dashTimeRemaining = 0f;
        _attackTimeRemaining = 0f;
        _hitStunRemaining = Mathf.Max(_hitStunRemaining, duration);
        _jumpTimeRemaining = 0f;
        Velocity = impulse;
        UpdateJumpVisual();
        SetAttackCollision(false);
    }

    public void Heal(int amount)
    {
        _health?.Heal(amount);
    }

    public void EquipImprovisedWeapon(int durability)
    {
        _hasImprovisedWeapon = true;
        _weaponDurability = Mathf.Max(durability, 1);
        SavePlayerState();
    }

    public void AddContinue()
    {
        _continues = Mathf.Clamp(_continues + 1, 0, 1);
        SavePlayerState();
    }

    public bool TryUseContinue(Vector2 respawnPosition)
    {
        if (_continues <= 0 || _health is null)
        {
            return false;
        }

        _continues--;
        GlobalPosition = respawnPosition;
        Velocity = Vector2.Zero;
        _dashTimeRemaining = 0f;
        _attackTimeRemaining = 0f;
        _hitStunRemaining = 0f;
        _jumpTimeRemaining = 0f;
        _health.Revive(Mathf.CeilToInt(_health.MaxHealth * 0.55f));
        SetPhysicsProcess(true);
        UpdateJumpVisual();
        Modulate = Colors.White;
        SavePlayerState();
        return true;
    }

    public void SavePlayerState()
    {
        SaveManager.Current.HasImprovisedWeapon = _hasImprovisedWeapon;
        SaveManager.Current.WeaponDurability = _weaponDurability;
        SaveManager.Current.Continues = _continues;
        SaveManager.Save();
    }

    private void ClampToPlayableArea()
    {
        GlobalPosition = new Vector2(
            Mathf.Clamp(GlobalPosition.X, MinX, MaxX),
            Mathf.Clamp(GlobalPosition.Y, MinLaneY, MaxLaneY));
    }

    private void CaptureJumpVisuals()
    {
        _jumpVisualOrigins.Clear();
        foreach (Node child in GetChildren())
        {
            if (child is Polygon2D polygon && child.Name != "LaneShadow")
            {
                _jumpVisualOrigins[polygon] = polygon.Position;
            }
        }
    }

    private void UpdateJumpVisual()
    {
        float heightOffset = 0f;
        if (_jumpTimeRemaining > 0f && JumpDuration > 0f)
        {
            float progress = 1f - _jumpTimeRemaining / JumpDuration;
            heightOffset = Mathf.Sin(progress * Mathf.Pi) * JumpHeight;
        }

        foreach ((Polygon2D polygon, Vector2 origin) in _jumpVisualOrigins)
        {
            polygon.Position = origin + new Vector2(0f, -heightOffset);
        }
    }

    private void ApplySavedState()
    {
        SaveManager.Load();
        _hasImprovisedWeapon = SaveManager.Current.HasImprovisedWeapon && SaveManager.Current.WeaponDurability > 0;
        _weaponDurability = _hasImprovisedWeapon ? SaveManager.Current.WeaponDurability : 0;
        _continues = Mathf.Clamp(SaveManager.Current.Continues, 0, 1);
    }

    private void ConsumeWeaponDurability()
    {
        if (!_hasImprovisedWeapon)
        {
            return;
        }

        _weaponDurability--;
        if (_weaponDurability <= 0)
        {
            _hasImprovisedWeapon = false;
            _weaponDurability = 0;
        }

        SavePlayerState();
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
            float pulse = 0.55f + Mathf.Sin(Time.GetTicksMsec() * 0.035f) * 0.25f;
            Modulate = Colors.White.Lerp(new Color(0.28f, 0.86f, 1f, 0.62f), pulse);
            return;
        }

        Modulate = Colors.White;
    }

    private void OnDied()
    {
        SetPhysicsProcess(false);
        _jumpTimeRemaining = 0f;
        UpdateJumpVisual();
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
