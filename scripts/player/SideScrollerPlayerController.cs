namespace SangueNoAsfalto.Player;

public partial class SideScrollerPlayerController : CharacterBody2D, ICombatKnockbackReceiver
{
    private const int SidearmMaxAmmoDefault = 7;
    private const float DoubleTapWindowSec = 0.28f;
    private const float RunDurationSec = 2.4f;

    [Export]
    public float HorizontalSpeed { get; set; } = 260f;

    [Export]
    public float RunSpeed { get; set; } = 390f;

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

    public string CombatStyleName => "Rua";

    public string WeaponName => _hasImprovisedWeapon ? "Vergalhao" : "Punhos";

    public int SidearmAmmo => _sidearmAmmo;

    public int SidearmMaxAmmo => SidearmMaxAmmoDefault;

    public bool IsRunning => _isRunning;

    public int WeaponDurability => _weaponDurability;

    public int Continues => _continues;

    public int ComboHitCount { get; private set; }

    public int BestCombo { get; private set; }

    public float Fury { get; private set; }

    public string ComboCalloutText => ComboHitCount switch
    {
        >= 48 => "RUTHLESS!",
        >= 28 => "VICIOUS!",
        >= 12 => "BRUTAL!",
        >= 8 => "HEAVY HITS",
        _ => string.Empty,
    };

    public bool ShowComboCallout => _comboCalloutRemaining > 0f && ComboHitCount >= 8;

    public int Level { get; private set; } = 1;

    public float Experience { get; private set; }

    public float ExperienceToNext { get; private set; } = 100f;

    private readonly KeyPressLatch _attackLatch = new(Key.J);
    private readonly KeyPressLatch _dashLatch = new(Key.K);
    private readonly KeyPressLatch _jumpLatch = new(Key.Space);
    private readonly KeyPressLatch _shootLatch = new(Key.L);
    private CharacterSpriteVisual? _spriteVisual;
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
    private float _comboChainRemaining;
    private float _comboCalloutRemaining;
    private float _runTimeRemaining;
    private double _lastMoveLeftTapMs;
    private double _lastMoveRightTapMs;
    private bool _isRunning;
    private int _sidearmAmmo = SidearmMaxAmmoDefault;

    public override void _Ready()
    {
        AddToGroup("player");
        AddToGroup("side_player");
        CurrentStamina = MaxStamina;
        ApplySavedState();

        _spriteVisual = GetNodeOrNull<CharacterSpriteVisual>("SpriteVisual");
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
            _health.Changed += OnPlayerHealthChanged;
            OnPlayerHealthChanged(_health.CurrentHealth, _health.MaxHealth);
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
        TickCombatStats(dt);
        TickShoot(dt);
        TickJump(dt);
        TickRun(dt, input);
        RegenerateStamina(dt);
        UpdateAttackArc();
        UpdateFacingVisual();
        UpdateJumpVisual();
        UpdateLocomotionVisual();
        UpdateInvulnerabilityVisual();

        if (TickHitStun(dt))
        {
            MoveAndSlide();
            ClampToPlayableArea();
            return;
        }

        if (_dashTimeRemaining <= 0f)
        {
            float moveSpeed = _isRunning ? RunSpeed : HorizontalSpeed;
            Velocity = new Vector2(input.X * moveSpeed, input.Y * LaneSpeed);
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

    private void TickCombatStats(float dt)
    {
        if (_comboChainRemaining > 0f)
        {
            _comboChainRemaining -= dt;
            if (_comboChainRemaining <= 0f)
            {
                ComboHitCount = 0;
            }
        }

        if (_comboCalloutRemaining > 0f)
        {
            _comboCalloutRemaining -= dt;
        }

        Fury = Math.Max(0f, Fury - 10f * dt);
    }

    public void RegisterCombatHit(int damage)
    {
        ComboHitCount++;
        BestCombo = Math.Max(BestCombo, ComboHitCount);
        _comboChainRemaining = ComboResetTime * 1.35f;
        _comboCalloutRemaining = 1.8f;
        Fury = Math.Min(100f, Fury + 6f + damage * 0.12f);
        AddExperience(4f + damage * 0.35f);
    }

    public void RegisterEnemyDefeat()
    {
        AddExperience(28f);
    }

    public void AddExperience(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        Experience += amount;
        while (Experience >= ExperienceToNext)
        {
            Experience -= ExperienceToNext;
            Level++;
            ExperienceToNext = MathF.Round(ExperienceToNext * 1.18f);
        }
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

        bool airKick = _jumpTimeRemaining > 0f;
        if (airKick)
        {
            _comboIndex = 2;
        }
        else
        {
            _comboIndex = _comboResetRemaining > 0f ? (_comboIndex + 1) % 3 : 0;
            if (_isRunning && _comboIndex == 2)
            {
                _comboIndex = 1;
            }
        }

        _comboResetRemaining = ComboResetTime;

        _attackTimeRemaining = _comboIndex switch
        {
            2 => airKick ? 0.24f : 0.22f,
            1 => _isRunning ? 0.13f : 0.17f,
            _ => _isRunning ? 0.11f : 0.12f,
        };

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
        _spriteVisual?.SetAttackCombo(_comboIndex);
        UpdateAttackArc();
        SpawnStrikeEffect(_comboIndex);
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

        if (_sidearmAmmo <= 0)
        {
            return;
        }

        CurrentStamina -= ShootStaminaCost;
        _shootCooldownRemaining = ShootCooldown;
        _sidearmAmmo--;

        _spriteVisual?.PlayShoot();

        Projectile projectile = ProjectileScene.Instantiate<Projectile>();
        projectile.Source = this;
        projectile.Direction = new Vector2(FacingSign, 0f);
        projectile.GlobalPosition = GlobalPosition + new Vector2(FacingSign * 34f, -22f);
        projectile.Rotation = FacingSign > 0 ? 0f : Mathf.Pi;
        projectile.ApplyBleedOnHit = true;
        projectile.BleedDuration = 3.5f;
        projectile.BleedDamagePerSecond = 5f;

        GetTree().CurrentScene?.AddChild(projectile);
    }

    private void TickRun(float dt, Vector2 input)
    {
        double nowMs = Time.GetTicksMsec();

        if (Input.IsActionJustPressed("move_left"))
        {
            if ((nowMs - _lastMoveLeftTapMs) / 1000.0 <= DoubleTapWindowSec)
            {
                _runTimeRemaining = RunDurationSec;
            }

            _lastMoveLeftTapMs = nowMs;
        }

        if (Input.IsActionJustPressed("move_right"))
        {
            if ((nowMs - _lastMoveRightTapMs) / 1000.0 <= DoubleTapWindowSec)
            {
                _runTimeRemaining = RunDurationSec;
            }

            _lastMoveRightTapMs = nowMs;
        }

        if (_runTimeRemaining > 0f)
        {
            _runTimeRemaining -= dt;
        }

        _isRunning = _runTimeRemaining > 0f && Mathf.Abs(input.X) > 0.01f && _hitStunRemaining <= 0f;
    }

    private void OnPlayerHealthChanged(int current, int maximum)
    {
        _spriteVisual?.SetDamageVisualTier(EnemyDamageState.FromHealth(current, maximum));
    }

    private void UpdateAttackArc()
    {
        if (_attackArea is null)
        {
            return;
        }

        _attackArea.Position = _comboIndex switch
        {
            2 => new Vector2(FacingSign * 46f, -20f),
            1 => new Vector2(FacingSign * 40f, 10f),
            _ => new Vector2(FacingSign * 38f, -8f),
        };
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

    private void SpawnStrikeEffect(int comboIndex)
    {
        Node? parent = GetParent();
        if (parent is null)
        {
            return;
        }

        Color color = comboIndex switch
        {
            2 => new Color(1f, 0.52f, 0.24f, 0.82f),
            1 => new Color(1f, 0.84f, 0.42f, 0.72f),
            _ => new Color(1f, 0.94f, 0.66f, 0.68f),
        };

        float radius = comboIndex switch
        {
            2 => 16f,
            1 => 13f,
            _ => 10f,
        };

        Vector2 offset = comboIndex switch
        {
            2 => new Vector2(FacingSign * 52f, -18f),
            1 => new Vector2(FacingSign * 44f, 12f),
            _ => new Vector2(FacingSign * 40f, -6f),
        };

        Polygon2D burst = new()
        {
            Color = color,
            Polygon = MakeBurstPolygon(radius),
            ZIndex = 12,
        };

        parent.AddChild(burst);
        burst.GlobalPosition = GlobalPosition + offset;

        Tween tween = burst.CreateTween();
        tween.TweenProperty(burst, "scale", Vector2.One * 1.7f, 0.05f);
        tween.Parallel().TweenProperty(burst, "modulate:a", 0f, 0.08f);
        tween.TweenCallback(Callable.From(burst.QueueFree));
    }

    private static Vector2[] MakeBurstPolygon(float radius)
    {
        return
        [
            new Vector2(-radius, -radius * 0.65f),
            new Vector2(radius, -radius * 0.65f),
            new Vector2(radius * 1.1f, 0f),
            new Vector2(radius, radius * 0.65f),
            new Vector2(-radius, radius * 0.65f),
            new Vector2(-radius * 1.1f, 0f),
        ];
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

    private void UpdateJumpVisual()
    {
        float heightOffset = 0f;
        if (_jumpTimeRemaining > 0f && JumpDuration > 0f)
        {
            float progress = 1f - _jumpTimeRemaining / JumpDuration;
            heightOffset = Mathf.Sin(progress * Mathf.Pi) * JumpHeight;
        }

        _spriteVisual?.SetJumpOffset(heightOffset);
    }

    private void UpdateLocomotionVisual()
    {
        if (_spriteVisual is null)
        {
            return;
        }

        bool moving = Velocity.LengthSquared() > 900f
            && _dashTimeRemaining <= 0f
            && _hitStunRemaining <= 0f
            && _jumpTimeRemaining <= 0f;
        _spriteVisual.UpdateLocomotion(
            moving,
            _attackTimeRemaining > 0f,
            _dashTimeRemaining > 0f,
            _hitStunRemaining > 0f,
            false,
            _isRunning);
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
        _spriteVisual?.SetFacing(FacingSign);
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
