using SangueNoAsfalto.Combat;
using SangueNoAsfalto.Core;

namespace SangueNoAsfalto.Player;

public partial class SideScrollerPlayerController : CharacterBody2D, ICombatKnockbackReceiver
{
    private const int SidearmMaxAmmoDefault = 7;
    private const float DoubleTapWindowSec = 0.28f;
    private const float RunDurationSec = 2.4f;

    [Export]
    public float HorizontalSpeed { get; set; } = 220f;

    [Export]
    public float RunSpeed { get; set; } = 330f;

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
    public float ComboResetTime { get; set; } = 1.35f;

    [Export]
    public float ShootCooldown { get; set; } = 0.32f;

    [Export]
    public float ShootStaminaCost { get; set; } = 16f;

    [Export]
    public float MaxStamina { get; set; } = 100f;

    [Export]
    public float DashStaminaCost { get; set; } = 28f;

    [Export]
    public float StaminaRegenPerSecond { get; set; } = 24f;

    [Export]
    public float AttackStaminaMin { get; set; } = 8f;

    [Export]
    public float MinX { get; set; } = -900f;

    [Export]
    public float MaxX { get; set; } = 3350f;

    [Export]
    public float MinLaneY { get; set; } = 260f;

    [Export]
    public float MaxLaneY { get; set; } = 470f;

    [Export]
    public PackedScene? ProjectileScene { get; set; }

    public float CurrentStamina { get; private set; }

    public Vector2 LastMovementInput { get; private set; }

    public int FacingSign { get; private set; } = 1;

    public string CombatStyleName => CombatStyleCatalog.GetDisplayName(ActiveCombatStyle);

    public string ActiveTechniqueDeck => MoveCatalog.GetTechniqueDeckText(ActiveCombatStyle);

    public string LastMoveDisplayName { get; private set; } = string.Empty;

    public CombatStyleKind ActiveCombatStyle => CombatStyleCatalog.GetActiveStyle(Level);

    public StyleUnlockInfo? NextStyleUnlock => CombatStyleCatalog.GetNextUnlock(Level);

    public bool IsExhausted => CurrentStamina <= MaxStamina * 0.28f;

    public event Action<StyleUnlockInfo>? StyleUnlocked;

    public string WeaponName => ImprovisedWeaponCatalog.GetDisplayName(_weaponKind);

    public ImprovisedWeaponKind EquippedWeaponKind => _weaponKind;

    public bool HasImprovisedWeapon => _weaponKind != ImprovisedWeaponKind.None && _weaponDurability > 0;

    public bool IsParrying => _parryPerfectWindow > 0f;

    public bool IsGuarding => _guardActive;

    public bool IsPostureStaggered => Posture?.IsBroken == true || _postureStaggerRemaining > 0f;

    public bool IsReloading => _reloadTimeRemaining > 0f;

    public PostureComponent? Posture { get; private set; }

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

    public float ExperienceToNext { get; private set; } = 48f;

    public int ComboChainSlot { get; private set; }

    private readonly KeyPressLatch _attackLatch = new(Key.J);
    private readonly KeyPressLatch _dashLatch = new(Key.K);
    private readonly KeyPressLatch _jumpLatch = new(Key.Space);
    private readonly KeyPressLatch _shootLatch = new(Key.L);
    private readonly KeyPressLatch _parryLatch = new(Key.Q);
    private readonly KeyPressLatch _reloadLatch = new(Key.E);
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
    private ImprovisedWeaponKind _weaponKind;
    private int _weaponDurability;
    private float _reloadTimeRemaining;
    private float _parryPerfectWindow;
    private bool _guardActive;
    private float _qHoldTime;
    private int _consecutiveBlocks;
    private float _blockChainTimer;
    private float _blockImpactFlash;
    private float _currentMoveDuration;
    private bool _attackBuffered;
    private float _postureStaggerRemaining;
    private float _parryRiposteDelay;
    private float _combatLockRemaining;
    private Node2D? _parryRiposteTarget;
    private bool _isFinisherAttack;
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
        Posture = GetNodeOrNull<PostureComponent>("Posture");
        if (Posture is not null)
        {
            Posture.MaxPosture = 85f;
            Posture.RegenPerSecond = 16f;
            Posture.BrokenDuration = 1.0f;
            Posture.PostureBroken += OnPlayerPostureBroken;
        }
        if (_health is not null)
        {
            _health.Died += OnDied;
            _health.Changed += OnPlayerHealthChanged;
            OnPlayerHealthChanged(_health.CurrentHealth, _health.MaxHealth);
        }

        SyncWeaponVisual();
        _spriteVisual?.SetCombatStyle(ActiveCombatStyle);
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
        TickReload(dt);
        TickPostureStagger(dt);
        TickGuard(dt);
        TickParryRiposte(dt);
        TickCombatLock(dt);
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

        if (_combatLockRemaining > 0f)
        {
            Velocity = Velocity.MoveToward(Vector2.Zero, 2400f * dt);
            MoveAndSlide();
            ClampToPlayableArea();
            return;
        }

        if (_postureStaggerRemaining > 0f)
        {
            Velocity = new Vector2(input.X * HorizontalSpeed * 0.55f, input.Y * LaneSpeed * 0.65f);
        }
        else if (_guardActive)
        {
            Velocity = new Vector2(input.X * HorizontalSpeed * 0.42f, input.Y * LaneSpeed * 0.55f);
        }
        else if (_dashTimeRemaining <= 0f)
        {
            float moveSpeed = _isRunning ? RunSpeed : HorizontalSpeed;
            Velocity = new Vector2(input.X * moveSpeed, input.Y * LaneSpeed);
        }

        MoveAndSlide();
        ClampToPlayableArea();

        if (_postureStaggerRemaining > 0f)
        {
            return;
        }

        if (Input.IsActionJustPressed("attack") || _attackLatch.ConsumeJustPressed())
        {
            if (_dashTimeRemaining > 0f)
            {
                StartDashAttack();
            }
            else
            {
                StartAttack();
            }
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

        if (Input.IsActionJustPressed("reload") || _reloadLatch.ConsumeJustPressed())
        {
            TryStartReload();
        }
    }

    private void TickPostureStagger(float dt)
    {
        if (Posture?.IsBroken == true)
        {
            _postureStaggerRemaining = Mathf.Max(_postureStaggerRemaining, Posture.BrokenTimeRemaining);
        }

        if (_postureStaggerRemaining > 0f)
        {
            _postureStaggerRemaining = Mathf.Max(_postureStaggerRemaining - dt, 0f);
            _guardActive = false;
            _parryPerfectWindow = 0f;
            _spriteVisual?.PlayPostureStagger();
        }
    }

    private void OnPlayerPostureBroken()
    {
        _postureStaggerRemaining = 1.0f;
        _guardActive = false;
        _parryPerfectWindow = 0f;
        _attackTimeRemaining = 0f;
        SetAttackCollision(false);
        CombatFeedback.PlayPostureBreak(this);
        _spriteVisual?.PlayPostureStagger();
    }

    private const float GuardHoldThreshold = 0.10f;
    private const float ParryWindowDuration = 0.36f;

    private void TickGuard(float dt)
    {
        if (_blockChainTimer > 0f)
        {
            _blockChainTimer = Mathf.Max(_blockChainTimer - dt, 0f);
            if (_blockChainTimer <= 0f)
            {
                _consecutiveBlocks = 0;
            }
        }

        if (_postureStaggerRemaining > 0f || Posture?.IsBroken == true)
        {
            _guardActive = false;
            if (Posture is not null)
            {
                Posture.RegenPaused = false;
            }

            return;
        }

        bool qPressed = Input.IsActionJustPressed("parry") || _parryLatch.ConsumeJustPressed();
        bool qHeld = Input.IsActionPressed("parry") || IsKeyDown(Key.Q);

        bool canGuard = _attackTimeRemaining <= 0f
            && _hitStunRemaining <= 0f
            && _combatLockRemaining <= 0f
            && _reloadTimeRemaining <= 0f;

        // Toque em Q abre janela de parry; só vira guarda sustentada após ~0,1s segurando.
        if (qPressed)
        {
            _qHoldTime = 0f;
            _parryPerfectWindow = ParryWindowDuration;
            _spriteVisual?.PlayParryWindup();
        }

        if (qHeld)
        {
            _qHoldTime += dt;
            if (_qHoldTime >= GuardHoldThreshold)
            {
                _parryPerfectWindow = 0f;
                _guardActive = canGuard;
            }
            else
            {
                _guardActive = false;
                CheckTelegraphParry();
            }
        }
        else
        {
            _qHoldTime = 0f;
            _guardActive = false;
            if (_parryPerfectWindow > 0f)
            {
                _parryPerfectWindow = Mathf.Max(_parryPerfectWindow - dt, 0f);
                CheckTelegraphParry();
            }
        }

        if (Posture is not null)
        {
            Posture.RegenPaused = _guardActive || _blockImpactFlash > 0.05f;
        }

        if (_guardActive)
        {
            CurrentStamina = Mathf.Max(0f, CurrentStamina - 14f * dt);
            _spriteVisual?.PlayGuard();
        }

        if (_blockImpactFlash > 0f)
        {
            _blockImpactFlash = Mathf.Max(_blockImpactFlash - dt, 0f);
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
        SyncPlayerAttackHitWindow();

        if (_attackTimeRemaining <= 0f)
        {
            SetAttackCollision(false);
            _isFinisherAttack = false;
            if (_attackArea is not null)
            {
                _attackArea.IsFinisherHit = false;
                _attackArea.IsPostureKill = false;
                _attackArea.IsParryRiposte = false;
                _attackArea.ApplyBleedOnHit = false;
            }

            if (_attackBuffered && !_guardActive)
            {
                _attackBuffered = false;
                StartAttack();
            }
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
        AddExperience(8f + damage * 0.45f);
    }

    public void RegisterEnemyDefeat()
    {
        AddExperience(38f);
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
            ExperienceToNext = MathF.Round(ExperienceToNext * 1.14f);
            if (CombatStyleCatalog.GetUnlockAtLevel(Level) is StyleUnlockInfo unlock && unlock.Level > 1)
            {
                StyleUnlocked?.Invoke(unlock);
                CombatFeedback.PlayStyleUnlock(this, unlock);
            }

            _spriteVisual?.SetCombatStyle(ActiveCombatStyle);
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

        float regen = StaminaRegenPerSecond;
        if (_attackTimeRemaining > 0f)
        {
            regen *= 0.45f;
        }

        CurrentStamina = Mathf.Min(CurrentStamina + regen * dt, MaxStamina);
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
        if (_reloadTimeRemaining > 0f || _guardActive)
        {
            return;
        }

        bool airKick = _jumpTimeRemaining > 0f;
        if (!airKick && FindPostureKillTarget() is Node2D postureTarget)
        {
            StartPostureKill(postureTarget);
            return;
        }

        if (!airKick && HasImprovisedWeapon && FindFinisherTarget() is Node2D finisherTarget)
        {
            StartFinisher(finisherTarget);
            return;
        }

        if (HasImprovisedWeapon && !airKick)
        {
            StartWeaponAttack();
            return;
        }

        bool chaining = _comboResetRemaining > 0f && !airKick && !_isRunning;
        bool canChainCancel = _attackTimeRemaining > 0f
            && chaining
            && _currentMoveDuration > 0f
            && _attackTimeRemaining <= _currentMoveDuration * 0.48f;

        if (_attackTimeRemaining > 0f)
        {
            if (canChainCancel)
            {
                SetAttackCollision(false);
                _attackTimeRemaining = 0f;
            }
            else if (chaining)
            {
                _attackBuffered = true;
                return;
            }
            else
            {
                return;
            }
        }

        int chainLength = MoveCatalog.GetComboLength(ActiveCombatStyle);
        int nextComboIndex = airKick ? chainLength - 1 : chaining ? (_comboIndex + 1) % chainLength : 0;

        MartialMoveDefinition move = MoveCatalog.ResolveMove(
            ActiveCombatStyle,
            nextComboIndex,
            _isRunning && !airKick,
            airKick);

        float staminaCost = MoveCatalog.GetStaminaCost(move, ActiveCombatStyle, _isRunning && !airKick);
        if (CurrentStamina < staminaCost)
        {
            return;
        }

        CurrentStamina -= staminaCost;
        _comboIndex = move.ImpactComboIndex;
        if (!airKick && !_isRunning)
        {
            _comboIndex = nextComboIndex;
        }

        ComboChainSlot = nextComboIndex;

        _comboResetRemaining = ComboResetTime;
        _isFinisherAttack = false;
        float duration = CombatPacing.ScalePlayerMoveDuration(move.Duration);
        _attackTimeRemaining = duration;
        _currentMoveDuration = duration;
        _attackBuffered = false;
        LastMoveDisplayName = move.DisplayName;

        if (_attackArea is not null)
        {
            _attackArea.Damage = move.BaseDamage;
            _attackArea.KnockbackForce = move.Knockback;
            _attackArea.ApplyBleedOnHit = false;
            _attackArea.IsFinisherHit = false;
            _attackArea.PostureDamage = move.PostureDamage;
        }

        SetAttackCollision(false);
        _spriteVisual?.SetAttackMove(move.Anim, move.ImpactComboIndex, ActiveCombatStyle, duration);
        CombatFeedback.SpawnMoveCallout(this, move.DisplayName, ActiveCombatStyle);
        UpdateAttackArc();
        SpawnStrikeEffect(move.ImpactComboIndex);
    }

    private void SyncPlayerAttackHitWindow()
    {
        if (_isFinisherAttack || _currentMoveDuration <= 0f)
        {
            return;
        }

        if (_attackArea is not null && (_attackArea.IsPostureKill || _attackArea.IsParryRiposte || _attackArea.IsFinisherHit))
        {
            return;
        }

        float elapsed = _currentMoveDuration - _attackTimeRemaining;
        float progress = elapsed / _currentMoveDuration;
        bool inWindow = CombatPacing.IsInHitWindow(
            progress,
            CombatPacing.PlayerHitWindowStart,
            CombatPacing.PlayerHitWindowEnd);
        SetAttackCollision(inWindow);
    }

    private void StartWeaponAttack()
    {
        float staminaCost = _weaponKind == ImprovisedWeaponKind.Hammer ? 20f : 14f;
        if (CurrentStamina < staminaCost)
        {
            return;
        }

        CurrentStamina -= staminaCost;
        _comboIndex = 0;
        _comboResetRemaining = ComboResetTime;
        _isFinisherAttack = false;
        _attackTimeRemaining = _weaponKind == ImprovisedWeaponKind.Hammer ? 0.28f : 0.24f;

        if (_attackArea is not null)
        {
            int bonus = ImprovisedWeaponCatalog.GetBasicDamageBonus(_weaponKind);
            _attackArea.Damage = 28 + bonus;
            _attackArea.KnockbackForce = 380f + ImprovisedWeaponCatalog.GetKnockbackBonus(_weaponKind);
            _attackArea.ApplyBleedOnHit = ImprovisedWeaponCatalog.AppliesBleed(_weaponKind);
            _attackArea.BleedDuration = ImprovisedWeaponCatalog.BleedDuration;
            _attackArea.BleedDamagePerSecond = ImprovisedWeaponCatalog.BleedDamagePerSecond;
            _attackArea.IsFinisherHit = false;
        }

        SetAttackCollision(true);
        _spriteVisual?.PlayWeaponAttack(_weaponKind);
        UpdateWeaponAttackArc();
        ConsumeWeaponDurability();
    }

    private void StartFinisher(Node2D target)
    {
        if (Mathf.Sign(target.GlobalPosition.X - GlobalPosition.X) != 0)
        {
            FacingSign = target.GlobalPosition.X >= GlobalPosition.X ? 1 : -1;
            UpdateFacingVisual();
        }

        _isFinisherAttack = true;
        _attackTimeRemaining = 0.58f;
        _comboIndex = 2;

        if (_attackArea is not null)
        {
            _attackArea.Damage = ImprovisedWeaponCatalog.GetFinisherDamage(_weaponKind);
            _attackArea.KnockbackForce = 720f;
            _attackArea.ApplyBleedOnHit = ImprovisedWeaponCatalog.AppliesBleed(_weaponKind);
            _attackArea.BleedDuration = ImprovisedWeaponCatalog.BleedDuration * 1.4f;
            _attackArea.BleedDamagePerSecond = ImprovisedWeaponCatalog.BleedDamagePerSecond * 1.35f;
            _attackArea.IsFinisherHit = true;
            _attackArea.HitStunDuration = 0.32f;
        }

        SetAttackCollision(true);
        _spriteVisual?.PlayFinisherAttack(_weaponKind);
        UpdateFinisherAttackArc(target);

        _weaponDurability = Mathf.Max(_weaponDurability - 2, 0);
        if (_weaponDurability <= 0)
        {
            ClearWeapon();
        }
        else
        {
            SavePlayerState();
        }
    }

    private Node2D? FindFinisherTarget()
    {
        const float rangeX = 82f;
        const float rangeY = 52f;
        Node2D? bestTarget = null;
        float bestDistance = float.MaxValue;

        foreach (Node node in GetTree().GetNodesInGroup("side_enemy"))
        {
            if (node is not Node2D enemy)
            {
                continue;
            }

            Health? health = enemy.GetNodeOrNull<Health>("Health");
            if (health is null || health.CurrentHealth <= 0)
            {
                continue;
            }

            if (EnemyDamageState.FromHealth(health.CurrentHealth, health.MaxHealth) != EnemyDamageVisualTier.Critical)
            {
                continue;
            }

            Vector2 delta = enemy.GlobalPosition - GlobalPosition;
            if (Mathf.Abs(delta.X) > rangeX || Mathf.Abs(delta.Y) > rangeY)
            {
                continue;
            }

            float distance = delta.LengthSquared();
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestTarget = enemy;
            }
        }

        return bestTarget;
    }

    public bool TryParry(Hitbox enemyHitbox)
    {
        if (_parryPerfectWindow <= 0f || enemyHitbox.Source is not Node2D enemySource)
        {
            return false;
        }

        ResolveParrySuccess(enemySource);
        return true;
    }

    public bool TryBlock(Hitbox enemyHitbox)
    {
        if (!_guardActive || _parryPerfectWindow > 0f || Posture?.IsBroken == true)
        {
            return false;
        }

        Node2D? attacker = enemyHitbox.Source as Node2D;
        int chipDamage = Mathf.Max(1, enemyHitbox.Damage / 6);
        _health?.Damage(chipDamage);

        _consecutiveBlocks++;
        _blockChainTimer = 1.6f;
        float chainMul = 1f + Mathf.Min((_consecutiveBlocks - 1) * 0.22f, 0.66f);
        float postureGain = Mathf.Max(36f, enemyHitbox.PostureDamage * 1.28f) * chainMul;
        Posture?.AddPosture(postureGain);

        CurrentStamina = Mathf.Max(0f, CurrentStamina - 10f);
        _blockImpactFlash = 0.18f;

        if (attacker is not null)
        {
            Vector2 knockDir = GlobalPosition.DirectionTo(attacker.GlobalPosition) * -1f;
            if (knockDir.LengthSquared() < 0.01f)
            {
                knockDir = new Vector2(-FacingSign, 0f);
            }

            float force = CombatImpactFeel.ScaleKnockback(enemyHitbox.Damage, enemyHitbox.KnockbackForce);
            float blockPush = Mathf.Clamp(force * 0.48f, 110f, 380f);
            float stun = Mathf.Clamp(0.055f + enemyHitbox.Damage * 0.004f, 0.06f, 0.14f);
            ReceiveKnockback(knockDir * blockPush, stun);
        }

        CombatFeedback.PlayBlock(this, attacker);
        _spriteVisual?.PlayBlockImpact();
        return true;
    }

    private void CheckTelegraphParry()
    {
        if (_parryPerfectWindow <= 0f || _combatLockRemaining > 0f)
        {
            return;
        }

        const float rangeX = 86f;
        const float rangeY = 56f;

        foreach (Node node in GetTree().GetNodesInGroup("side_enemy"))
        {
            if (node is not SideScrollerEnemyController enemy || !enemy.IsTelegraphing || enemy.TelegraphUrgency < 0.38f)
            {
                continue;
            }

            Vector2 delta = enemy.GlobalPosition - GlobalPosition;
            if (Mathf.Abs(delta.X) > rangeX || Mathf.Abs(delta.Y) > rangeY)
            {
                continue;
            }

            ResolveParrySuccess(enemy);
            return;
        }
    }

    private void ResolveParrySuccess(Node2D enemySource)
    {
        _parryPerfectWindow = 0f;
        _guardActive = false;
        PostureComponent? enemyPosture = enemySource.GetNodeOrNull<PostureComponent>("Posture");
        enemyPosture?.BreakPosture();

        if (enemySource is SideScrollerEnemyController enemyController)
        {
            enemyController.OnParried(this);
        }

        _parryRiposteTarget = enemySource;
        _parryRiposteDelay = 0.2f;
        _combatLockRemaining = 0.82f;
        CombatFeedback.PlayParry(this, enemySource);
        _spriteVisual?.PlayParrySuccessMatrix();
    }

    private Node2D? FindPostureKillTarget()
    {
        const float rangeX = 88f;
        const float rangeY = 54f;
        Node2D? bestTarget = null;
        float bestDistance = float.MaxValue;

        foreach (Node node in GetTree().GetNodesInGroup("side_enemy"))
        {
            if (node is not Node2D enemy)
            {
                continue;
            }

            PostureComponent? posture = enemy.GetNodeOrNull<PostureComponent>("Posture");
            if (posture is null || !posture.IsBroken)
            {
                continue;
            }

            Health? health = enemy.GetNodeOrNull<Health>("Health");
            if (health is null || health.CurrentHealth <= 0)
            {
                continue;
            }

            Vector2 delta = enemy.GlobalPosition - GlobalPosition;
            if (Mathf.Abs(delta.X) > rangeX || Mathf.Abs(delta.Y) > rangeY)
            {
                continue;
            }

            float distance = delta.LengthSquared();
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestTarget = enemy;
            }
        }

        return bestTarget;
    }

    private void StartPostureKill(Node2D target)
    {
        if (Mathf.Sign(target.GlobalPosition.X - GlobalPosition.X) != 0)
        {
            FacingSign = target.GlobalPosition.X >= GlobalPosition.X ? 1 : -1;
            UpdateFacingVisual();
        }

        _isFinisherAttack = true;
        _attackTimeRemaining = 0.52f;
        _comboIndex = 2;

        if (_attackArea is not null)
        {
            _attackArea.Damage = 999;
            _attackArea.KnockbackForce = 840f;
            _attackArea.ApplyBleedOnHit = false;
            _attackArea.IsFinisherHit = true;
            _attackArea.IsPostureKill = true;
            _attackArea.HitStunDuration = 0.36f;
        }

        SetAttackCollision(true);
        _spriteVisual?.PlayPostureKill();
        UpdateFinisherAttackArc(target);
    }

    private void StartDashAttack()
    {
        if (_attackTimeRemaining > 0f || _guardActive || _reloadTimeRemaining > 0f)
        {
            return;
        }

        MartialMoveDefinition move = MoveCatalog.GetRunningMove(ActiveCombatStyle);
        float staminaCost = MoveCatalog.GetStaminaCost(move, ActiveCombatStyle, true);
        if (CurrentStamina < staminaCost)
        {
            return;
        }

        CurrentStamina -= staminaCost;
        _comboIndex = 0;
        _comboResetRemaining = ComboResetTime;
        float duration = CombatPacing.ScalePlayerMoveDuration(move.Duration);
        _attackTimeRemaining = duration;
        _currentMoveDuration = duration;
        LastMoveDisplayName = move.DisplayName;

        if (_attackArea is not null)
        {
            _attackArea.Damage = move.BaseDamage + 4;
            _attackArea.KnockbackForce = move.Knockback + 40f;
            _attackArea.PostureDamage = move.PostureDamage;
            _attackArea.ApplyBleedOnHit = false;
            _attackArea.IsFinisherHit = false;
        }

        SetAttackCollision(false);
        _spriteVisual?.SetAttackMove(move.Anim, move.ImpactComboIndex, ActiveCombatStyle, duration);
        CombatFeedback.SpawnMoveCallout(this, move.DisplayName, ActiveCombatStyle);
        UpdateAttackArc();
    }

    private void TickParryRiposte(float dt)
    {
        if (_parryRiposteDelay <= 0f)
        {
            return;
        }

        _parryRiposteDelay -= dt;
        if (_parryRiposteDelay > 0f)
        {
            return;
        }

        if (_parryRiposteTarget is Node2D target && GodotObject.IsInstanceValid(target))
        {
            ExecuteParryRiposte(target);
        }

        _parryRiposteTarget = null;
    }

    private void TickCombatLock(float dt)
    {
        if (_combatLockRemaining <= 0f)
        {
            return;
        }

        _combatLockRemaining = Mathf.Max(_combatLockRemaining - dt, 0f);
    }

    private void ExecuteParryRiposte(Node2D target)
    {
        if (Mathf.Sign(target.GlobalPosition.X - GlobalPosition.X) != 0)
        {
            FacingSign = target.GlobalPosition.X >= GlobalPosition.X ? 1 : -1;
            UpdateFacingVisual();
        }

        _isFinisherAttack = true;
        _attackTimeRemaining = 0.5f;
        _comboIndex = 2;

        if (_attackArea is not null)
        {
            _attackArea.Damage = 999;
            _attackArea.KnockbackForce = 960f;
            _attackArea.ApplyBleedOnHit = false;
            _attackArea.IsFinisherHit = true;
            _attackArea.IsPostureKill = false;
            _attackArea.IsParryRiposte = true;
            _attackArea.HitStunDuration = 0.42f;
        }

        SetAttackCollision(true);
        _spriteVisual?.PlayParryRiposte();
        UpdateFinisherAttackArc(target);
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
        if (ProjectileScene is null || _shootCooldownRemaining > 0f || _reloadTimeRemaining > 0f || CurrentStamina < ShootStaminaCost)
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

    private void TickReload(float dt)
    {
        if (_reloadTimeRemaining <= 0f)
        {
            return;
        }

        _reloadTimeRemaining -= dt;
        Velocity = Velocity.MoveToward(Vector2.Zero, 900f * dt);
        if (_reloadTimeRemaining <= 0f)
        {
            _sidearmAmmo = SidearmMaxAmmoDefault;
            _spriteVisual?.EndReload();
        }
    }

    private void TryStartReload()
    {
        if (_reloadTimeRemaining > 0f || _sidearmAmmo >= SidearmMaxAmmoDefault || _attackTimeRemaining > 0f)
        {
            return;
        }

        _reloadTimeRemaining = 1.25f;
        _dashTimeRemaining = 0f;
        _spriteVisual?.PlayReload();
    }

    public void AddSidearmAmmo(int amount)
    {
        _sidearmAmmo = Mathf.Clamp(_sidearmAmmo + amount, 0, SidearmMaxAmmoDefault);
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
            3 => new Vector2(FacingSign * 50f, -24f),
            2 => new Vector2(FacingSign * 46f, -12f),
            1 => new Vector2(FacingSign * 40f, 10f),
            _ => new Vector2(FacingSign * 38f, -8f),
        };
        _attackArea.Rotation = FacingSign > 0 ? 0f : Mathf.Pi;
    }

    private void UpdateWeaponAttackArc()
    {
        if (_attackArea is null)
        {
            return;
        }

        _attackArea.Position = _weaponKind switch
        {
            ImprovisedWeaponKind.Hammer => new Vector2(FacingSign * 34f, -28f),
            ImprovisedWeaponKind.Knife => new Vector2(FacingSign * 42f, -10f),
            _ => new Vector2(FacingSign * 46f, -12f),
        };
        _attackArea.Rotation = FacingSign > 0 ? 0f : Mathf.Pi;
    }

    private void UpdateFinisherAttackArc(Node2D target)
    {
        if (_attackArea is null)
        {
            return;
        }

        Vector2 delta = target.GlobalPosition - GlobalPosition;
        _attackArea.Position = new Vector2(Mathf.Sign(delta.X == 0f ? FacingSign : delta.X) * 44f, -14f);
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

    public void EquipImprovisedWeapon(ImprovisedWeaponKind kind, int durability)
    {
        _weaponKind = kind;
        _weaponDurability = durability > 0 ? durability : ImprovisedWeaponCatalog.GetDefaultDurability(kind);
        SyncWeaponVisual();
        SavePlayerState();
    }

    private void ClearWeapon()
    {
        _weaponKind = ImprovisedWeaponKind.None;
        _weaponDurability = 0;
        SyncWeaponVisual();
        SavePlayerState();
    }

    private void SyncWeaponVisual()
    {
        _spriteVisual?.SetEquippedWeapon(_weaponKind);
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
        SaveManager.Current.HasImprovisedWeapon = HasImprovisedWeapon;
        SaveManager.Current.WeaponKind = (int)_weaponKind;
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
            _isRunning,
            _isFinisherAttack,
            _reloadTimeRemaining > 0f,
            _guardActive,
            _parryPerfectWindow > 0f,
            IsPostureStaggered,
            IsExhausted);
    }

    private void ApplySavedState()
    {
        SaveManager.Load();
        if (SaveManager.Current.HasImprovisedWeapon && SaveManager.Current.WeaponDurability > 0)
        {
            _weaponKind = (ImprovisedWeaponKind)Mathf.Clamp(SaveManager.Current.WeaponKind, 1, 3);
            _weaponDurability = SaveManager.Current.WeaponDurability;
        }
        else
        {
            _weaponKind = ImprovisedWeaponKind.None;
            _weaponDurability = 0;
        }

        _continues = Mathf.Clamp(SaveManager.Current.Continues, 0, 1);
    }

    private void ConsumeWeaponDurability()
    {
        if (!HasImprovisedWeapon)
        {
            return;
        }

        _weaponDurability--;
        if (_weaponDurability <= 0)
        {
            ClearWeapon();
            return;
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
