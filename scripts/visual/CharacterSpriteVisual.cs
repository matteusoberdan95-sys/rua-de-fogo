namespace SangueNoAsfalto.Visual;

using SangueNoAsfalto.Combat;

public enum LayeredPrototypePreset
{
    Caua,
    QuebraOsso,
    Fast,
    Brute,
    Infected,
    MiniBoss,
}

/// <summary>
/// Rig 2D em camadas — combate desarmado, reacao a hit e machucado cumulativo no inimigo.
/// </summary>
public partial class CharacterSpriteVisual : Node2D, IActorVisual
{
    [Export]
    public NodePath SpritePath { get; set; } = new("AnimatedSprite2D");

    [Export]
    public float DisplayScale { get; set; } = 0.11f;

    [Export]
    public Color SpriteTint { get; set; } = Colors.White;

    [Export]
    public bool SourceFacesRight { get; set; } = true;

    [Export]
    public bool UseLayeredPrototype { get; set; }

    [Export]
    public LayeredPrototypePreset LayeredPreset { get; set; } = LayeredPrototypePreset.Caua;

    private AnimatedSprite2D? _sprite;
    private Node2D? _rig;
    private Node2D? _torso;
    private Node2D? _head;
    private Node2D? _hair;
    private Node2D? _backArm;
    private Node2D? _frontArm;
    private Node2D? _backLeg;
    private Node2D? _frontLeg;
    private Node2D? _backLegShin;
    private Node2D? _frontLegShin;
    private Node2D? _backArmForearm;
    private Node2D? _frontArmForearm;
    private Node2D? _shirtPulse;
    private Node2D? _vestFlap;
    private Node2D? _clothSway;
    private Node2D? _hairTail;
    private Polygon2D? _faceBruise;
    private Polygon2D? _blackEye;
    private Polygon2D? _noseBleed;
    private Polygon2D? _noseBleedDrip;
    private Polygon2D? _bloodSmearFace;
    private Polygon2D? _shirtTear;
    private Polygon2D? _painGrimace;
    private Polygon2D? _torsoBlood;
    private Polygon2D? _criticalLimp;
    private CanvasItem? _caseiraKnife;
    private CanvasItem? _knifeBlood;
    private Vector2 _basePosition;
    private Vector2 _jumpOffset;
    private Vector2 _movementOffset;
    private string _currentAnim = string.Empty;
    private string _layeredState = "idle";
    private float _stateTime;
    private float _flashTime;
    private float _hurtTime;
    private float _hurtStrength = 1f;
    private Vector2 _hurtDirection = Vector2.Left;
    private int _attackComboIndex;
    private MoveAnimProfile _activeMoveAnim = MoveAnimProfile.Jab;
    private float _activeMoveDuration = 0.32f;
    private CombatStyleKind _combatStyle = CombatStyleKind.Rua;
    private bool _impactSpawned;
    private bool _isRunning;
    private bool _isExhausted;
    private float _shootAnimTime;
    private Node2D? _sidearm;
    private Node2D? _improvisedWeapon;
    private ImprovisedWeaponKind _equippedWeaponKind;
    private Node2D? _variantAccentRoot;
    private bool _finisherAttack;
    private float _reloadAnimTime;
    private float _stateLockRemaining;
    private EnemyDamageVisualTier _damageTier = EnemyDamageVisualTier.Intact;
    private int _facingSign = 1;
    private float _styleAttackFlash;
    private float _headAnchorY = -96f;
    private Vector2 _baseRigScale = Vector2.One;
    private float _idlePersonalityOffset;
    private bool _isDying;

    public bool IsProductionArtActive => false;

    public override void _Ready()
    {
        _basePosition = Position;
        _sprite = GetNodeOrNull<AnimatedSprite2D>(SpritePath);

        if (UseLayeredPrototype)
        {
            if (_sprite is not null)
            {
                _sprite.Visible = false;
            }

            BuildLayeredPrototype();
            _idlePersonalityOffset = (GetInstanceId() % 997) * 0.01f;
            return;
        }

        if (_sprite is null)
        {
            return;
        }

        _sprite.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
        _sprite.Scale = Vector2.One * DisplayScale;
        _sprite.Modulate = SpriteTint;
    }

    public override void _Process(double delta)
    {
        float dt = (float)delta;

        if (!UseLayeredPrototype)
        {
            TickSpritePresentation(dt);
            return;
        }

        if (_rig is null)
        {
            return;
        }

        _stateTime += dt;
        if (_hurtTime > 0f)
        {
            _hurtTime = Mathf.Max(_hurtTime - dt, 0f);
            if (_hurtTime <= 0f && _painGrimace is not null)
            {
                _painGrimace.Visible = false;
            }
        }

        if (_flashTime > 0f && !_isDying)
        {
            _flashTime = Mathf.Max(_flashTime - dt, 0f);
            _rig.Modulate = _flashTime > 0f ? new Color(1f, 0.34f, 0.32f) : Colors.White;
        }

        AnimateLayeredPrototype();
    }

    /// <summary>
    /// Garante rig em camadas (inimigos que nascem sem SpriteVisual ou cena corrigida em runtime).
    /// </summary>
    public void EnsureLayeredRig(LayeredPrototypePreset preset)
    {
        LayeredPreset = preset;
        UseLayeredPrototype = true;

        if (_sprite is not null)
        {
            _sprite.Visible = false;
        }

        if (_rig is not null && IsInstanceValid(_rig))
        {
            return;
        }

        _rig?.QueueFree();
        _rig = null;
        BuildLayeredPrototype();
        _idlePersonalityOffset = (GetInstanceId() % 997) * 0.01f;
    }

    public void SetFacing(int sign)
    {
        if (sign == 0)
        {
            return;
        }

        _facingSign = sign > 0 ? 1 : -1;
        if (UseLayeredPrototype)
        {
            if (_rig is not null)
            {
                _rig.Scale = new Vector2(_baseRigScale.X * _facingSign, _baseRigScale.Y);
            }

            return;
        }

        if (_sprite is null)
        {
            return;
        }

        float magnitude = Mathf.Abs(_sprite.Scale.X) > 0.001f ? Mathf.Abs(_sprite.Scale.X) : DisplayScale;
        float direction = SourceFacesRight ? Mathf.Sign(sign) : -Mathf.Sign(sign);
        _sprite.Scale = new Vector2(direction * magnitude, magnitude);
    }

    public void SetJumpOffset(float heightOffset)
    {
        _jumpOffset = new Vector2(0f, -heightOffset);
        ApplyVisualOffset();
    }

    public void SetDamageVisualTier(EnemyDamageVisualTier tier)
    {
        _damageTier = tier;
        if (_faceBruise is not null)
        {
            _faceBruise.Visible = tier >= EnemyDamageVisualTier.Hurt;
        }

        if (_blackEye is not null)
        {
            _blackEye.Visible = tier >= EnemyDamageVisualTier.Hurt;
        }

        if (_noseBleed is not null)
        {
            _noseBleed.Visible = tier >= EnemyDamageVisualTier.Hurt;
        }

        if (_bloodSmearFace is not null)
        {
            _bloodSmearFace.Visible = tier >= EnemyDamageVisualTier.Critical;
        }

        if (_shirtTear is not null)
        {
            _shirtTear.Visible = tier >= EnemyDamageVisualTier.Hurt;
        }

        if (_torsoBlood is not null)
        {
            _torsoBlood.Visible = tier >= EnemyDamageVisualTier.Hurt;
            _torsoBlood.Modulate = tier >= EnemyDamageVisualTier.Critical
                ? new Color(0.62f, 0.01f, 0.02f, 0.95f)
                : new Color(0.52f, 0.01f, 0.02f, 0.72f);
        }

        if (_criticalLimp is not null)
        {
            _criticalLimp.Visible = tier >= EnemyDamageVisualTier.Critical;
        }

        if (UseLayeredPrototype && _rig is not null)
        {
            _rig.Modulate = tier switch
            {
                EnemyDamageVisualTier.Critical => new Color(0.92f, 0.72f, 0.72f),
                EnemyDamageVisualTier.Hurt => new Color(0.96f, 0.86f, 0.84f),
                _ => Colors.White,
            };
        }
    }

    public void PlayDeath()
    {
        if (!UseLayeredPrototype || _isDying)
        {
            return;
        }

        _isDying = true;
        _stateLockRemaining = CombatPacing.DeathBodySeconds;
        _layeredState = "death";
        _stateTime = 0f;
        _hurtTime = 0f;
    }

    public void PlayParryStagger()
    {
        if (!UseLayeredPrototype)
        {
            return;
        }

        _flashTime = 0.22f;
        _stateLockRemaining = 0.58f;
        SetLayeredState("parry_stagger");
    }

    public bool IsDeathPlaying => _isDying;

    private void TickSpritePresentation(float dt)
    {
        _stateTime += dt;

        if (_styleAttackFlash > 0f)
        {
            _styleAttackFlash = Mathf.Max(_styleAttackFlash - dt, 0f);
        }

        if (_flashTime > 0f)
        {
            _flashTime = Mathf.Max(_flashTime - dt, 0f);
        }

        if (_sprite is null)
        {
            return;
        }

        Color baseTint = SpriteTint;
        if (_damageTier >= EnemyDamageVisualTier.Critical)
        {
            baseTint = baseTint.Lerp(new Color(0.82f, 0.38f, 0.38f), 0.5f);
        }
        else if (_damageTier >= EnemyDamageVisualTier.Hurt)
        {
            baseTint = baseTint.Lerp(new Color(1f, 0.68f, 0.62f), 0.28f);
        }

        if (_styleAttackFlash > 0f)
        {
            Color styleFlash = _combatStyle switch
            {
                CombatStyleKind.Boxe => new Color(1f, 0.55f, 0.25f),
                CombatStyleKind.MuayThai => new Color(1f, 0.9f, 0.25f),
                CombatStyleKind.Capoeira => new Color(0.5f, 1f, 0.45f),
                _ => new Color(1f, 0.92f, 0.65f),
            };
            float t = _styleAttackFlash / 0.22f;
            _sprite.Modulate = baseTint.Lerp(styleFlash, t * 0.55f);
            _sprite.Scale = new Vector2(_sprite.Scale.X, Mathf.Abs(_sprite.Scale.Y) * (1f + t * 0.06f));
        }
        else if (_flashTime > 0f)
        {
            _sprite.Modulate = baseTint.Lerp(new Color(1f, 0.35f, 0.32f), _flashTime / 0.14f);
        }
        else
        {
            _sprite.Modulate = baseTint;
        }

        float breath = Mathf.Sin(_stateTime * 4.2f) * 1.4f;
        _movementOffset = _currentAnim is "idle" or "walk"
            ? new Vector2(0f, breath)
            : Vector2.Zero;
        ApplyVisualOffset();
    }

    public void PlayPostureStagger()
    {
        if (!UseLayeredPrototype)
        {
            return;
        }

        SetLayeredState("posture_stagger");
    }

    public void PlayGuard()
    {
        if (!UseLayeredPrototype)
        {
            return;
        }

        SetLayeredState("guard");
    }

    public void PlayBlockImpact()
    {
        if (!UseLayeredPrototype)
        {
            return;
        }

        _flashTime = 0.16f;
        _stateLockRemaining = 0.1f;
        SetLayeredState("guard");
    }

    public void PlayShoot()
    {
        if (!UseLayeredPrototype)
        {
            return;
        }

        _shootAnimTime = 0.22f;
        SetLayeredState("shoot");
    }

    public void UpdateLocomotion(
        bool isMoving,
        bool isAttacking,
        bool isDashing,
        bool isHurt = false,
        bool isTelegraphing = false,
        bool isRunning = false,
        bool isFinisherAttack = false,
        bool isReloading = false,
        bool isGuarding = false,
        bool isParrying = false,
        bool isPostureStaggered = false,
        bool isExhausted = false)
    {
        _isRunning = isRunning;
        _isExhausted = isExhausted;
        _finisherAttack = isFinisherAttack;
        if (_shootAnimTime > 0f)
        {
            _shootAnimTime = Mathf.Max(_shootAnimTime - (float)GetProcessDeltaTime(), 0f);
            if (_shootAnimTime <= 0f && _layeredState == "shoot")
            {
                SetLayeredState("idle");
            }
        }

        if (UseLayeredPrototype)
        {
            float bobSpeed = _isRunning ? 0.028f : _isExhausted ? 0.032f : 0.018f;
            float bobAmount = _isRunning ? 2.4f : _isExhausted ? 3.4f : 1.8f;
            if (_damageTier == EnemyDamageVisualTier.Critical)
            {
                bobAmount = 2.6f;
            }

            _movementOffset = isMoving && !isAttacking && !isDashing && !isHurt && !isTelegraphing && _shootAnimTime <= 0f && !_isDying
                ? new Vector2(0f, Mathf.Sin(Time.GetTicksMsec() * bobSpeed) * bobAmount)
                : Vector2.Zero;
            ApplyVisualOffset();

            if (_isDying || _layeredState == "death")
            {
                return;
            }

            if (_shootAnimTime > 0f)
            {
                return;
            }

            if (_reloadAnimTime > 0f)
            {
                _reloadAnimTime = Mathf.Max(_reloadAnimTime - (float)GetProcessDeltaTime(), 0f);
            }

            if (_stateLockRemaining > 0f)
            {
                _stateLockRemaining = Mathf.Max(_stateLockRemaining - (float)GetProcessDeltaTime(), 0f);
                if (_isDying || _layeredState is "death" or "parry_stagger")
                {
                    ApplyVisualOffset();
                }

                return;
            }

            if (_isDying)
            {
                ApplyVisualOffset();
                return;
            }

            string nextState = isHurt || _hurtTime > 0f ? "hurt"
                : isPostureStaggered || _layeredState == "posture_stagger" ? "posture_stagger"
                : _layeredState == "parry_stagger" ? "parry_stagger"
                : _layeredState == "death" ? "death"
                : _layeredState == "parry_success" ? "parry_success"
                : isGuarding || _layeredState == "guard" ? "guard"
                : isParrying || _layeredState == "parry" ? "parry"
                : isReloading || _reloadAnimTime > 0f ? "reload"
                : isTelegraphing ? "telegraph"
                : isFinisherAttack ? "finisher"
                : isAttacking ? "attack"
                : isDashing ? "dash"
                : isMoving ? "walk"
                : "idle";
            SetLayeredState(nextState);
            return;
        }

        if (isAttacking || isDashing)
        {
            _movementOffset = Vector2.Zero;
            ApplyVisualOffset();
            Play("attack");
            return;
        }

        bool hasWalk = _sprite?.SpriteFrames?.HasAnimation("walk") == true;
        if (isMoving && !hasWalk)
        {
            float pulse = Mathf.Sin(Time.GetTicksMsec() * 0.018f);
            _movementOffset = new Vector2(0f, pulse * 2.5f);
        }
        else
        {
            _movementOffset = Vector2.Zero;
        }

        ApplyVisualOffset();
        Play(isMoving ? "walk" : "idle");
    }

    public void Play(string animationName)
    {
        if (UseLayeredPrototype)
        {
            SetLayeredState(animationName);
            return;
        }

        if (_sprite?.SpriteFrames is null)
        {
            return;
        }

        if (!_sprite.SpriteFrames.HasAnimation(animationName))
        {
            animationName = "idle";
        }

        if (_currentAnim == animationName && _sprite.IsPlaying())
        {
            return;
        }

        _currentAnim = animationName;
        _sprite.Play(animationName);
    }

    public void SetEquippedWeapon(ImprovisedWeaponKind kind)
    {
        _equippedWeaponKind = kind;
        if (!UseLayeredPrototype || _frontArm is null)
        {
            return;
        }

        _improvisedWeapon?.QueueFree();
        _improvisedWeapon = null;

        if (kind == ImprovisedWeaponKind.None)
        {
            return;
        }

        _improvisedWeapon = AddJoint(_frontArm, "ImprovisedWeapon", new Vector2(8f, 28f), 5);
        BuildImprovisedWeaponMesh(kind);
    }

    public void PlayWeaponAttack(ImprovisedWeaponKind kind)
    {
        _equippedWeaponKind = kind;
        SetLayeredState("attack");
    }

    public void PlayFinisherAttack(ImprovisedWeaponKind kind)
    {
        _equippedWeaponKind = kind;
        _finisherAttack = true;
        SetLayeredState("finisher");
    }

    public void PlayReload()
    {
        if (!UseLayeredPrototype)
        {
            return;
        }

        _reloadAnimTime = 1.25f;
        SetLayeredState("reload");
    }

    public void EndReload()
    {
        _reloadAnimTime = 0f;
        if (_layeredState == "reload")
        {
            SetLayeredState("idle");
        }
    }

    private void BuildImprovisedWeaponMesh(ImprovisedWeaponKind kind)
    {
        if (_improvisedWeapon is null)
        {
            return;
        }

        switch (kind)
        {
            case ImprovisedWeaponKind.Hammer:
                AddPolygon(_improvisedWeapon, "Handle", new Color(0.22f, 0.12f, 0.06f), new[]
                {
                    new Vector2(-3f, -4f), new Vector2(4f, -4f), new Vector2(4f, 24f), new Vector2(-3f, 24f)
                }, 0);
                AddPolygon(_improvisedWeapon, "Head", new Color(0.28f, 0.28f, 0.3f), new[]
                {
                    new Vector2(-12f, -16f), new Vector2(12f, -16f), new Vector2(14f, 2f), new Vector2(-14f, 2f)
                }, 1);
                break;
            case ImprovisedWeaponKind.Knife:
                AddPolygon(_improvisedWeapon, "Grip", new Color(0.12f, 0.08f, 0.05f), new[]
                {
                    new Vector2(-2f, -2f), new Vector2(5f, -2f), new Vector2(5f, 10f), new Vector2(-2f, 10f)
                }, 0);
                AddPolygon(_improvisedWeapon, "Blade", new Color(0.72f, 0.74f, 0.78f), new[]
                {
                    new Vector2(-3f, 10f), new Vector2(4f, 10f), new Vector2(2f, 28f), new Vector2(-1f, 28f)
                }, 1);
                AddPolygon(_improvisedWeapon, "Edge", new Color(0.9f, 0.92f, 0.95f, 0.85f), new[]
                {
                    new Vector2(2f, 12f), new Vector2(4f, 10f), new Vector2(2f, 28f)
                }, 2);
                break;
            default:
                AddPolygon(_improvisedWeapon, "Handle", new Color(0.18f, 0.09f, 0.05f), new[]
                {
                    new Vector2(-3f, -2f), new Vector2(4f, -2f), new Vector2(4f, 14f), new Vector2(-3f, 14f)
                }, 0);
                AddPolygon(_improvisedWeapon, "Bar", new Color(0.78f, 0.72f, 0.52f), new[]
                {
                    new Vector2(-26f, -5f), new Vector2(22f, -8f), new Vector2(34f, 0f), new Vector2(22f, 8f), new Vector2(-26f, 5f)
                }, 1);
                break;
        }
    }

    public void PlayParryWindup()
    {
        if (!UseLayeredPrototype)
        {
            _sprite!.Modulate = SpriteTint.Lerp(new Color(0.45f, 0.72f, 1f), 0.45f);
            return;
        }

        SetLayeredState("parry");
    }

    public void PlayParry()
    {
        _flashTime = 0.14f;
        if (!UseLayeredPrototype)
        {
            return;
        }

        SetLayeredState("parry");
    }

    public void PlayParrySuccessMatrix()
    {
        _flashTime = 0.22f;
        if (!UseLayeredPrototype)
        {
            Play("attack");
            return;
        }

        SetLayeredState("parry_success");
        _stateLockRemaining = 0.22f;
    }

    public void PlayParryRiposte()
    {
        _finisherAttack = true;
        if (!UseLayeredPrototype)
        {
            _styleAttackFlash = 0.35f;
            Play("attack");
            return;
        }

        SetLayeredState("finisher");
        _stateLockRemaining = 0.48f;
    }

    public void PlayPostureKill()
    {
        _finisherAttack = true;
        SetLayeredState("finisher");
    }

    public void BeginEnemyStrike(float duration, int patternIndex, MoveAnimProfile anim)
    {
        _activeMoveDuration = Mathf.Max(duration, 0.18f);
        _attackComboIndex = patternIndex;
        _activeMoveAnim = anim;
        _layeredState = "attack";
        _stateTime = 0f;
        _impactSpawned = false;
    }

    public void BeginEnemyStrike(float duration, int patternIndex)
    {
        BeginEnemyStrike(duration, patternIndex, patternIndex == 1 ? MoveAnimProfile.SideKick : MoveAnimProfile.Jab);
    }

    public void SetAttackCombo(int comboIndex)
    {
        _attackComboIndex = Mathf.Clamp(comboIndex, 0, 2);
        if (UseLayeredPrototype && _layeredState == "attack")
        {
            _stateTime = 0f;
            _impactSpawned = false;
        }
    }

    public void SetCombatStyle(CombatStyleKind style)
    {
        _combatStyle = style;
    }

    public void SetAttackMove(MoveAnimProfile anim, int impactComboIndex, CombatStyleKind style, float duration = 0.32f)
    {
        _combatStyle = style;
        _activeMoveAnim = anim;
        _activeMoveDuration = Mathf.Max(duration, 0.14f);
        _attackComboIndex = Mathf.Clamp(impactComboIndex, 0, 2);
        if (UseLayeredPrototype)
        {
            _layeredState = "attack";
            _stateTime = 0f;
            _impactSpawned = false;
            return;
        }

        _styleAttackFlash = 0.22f;
        _stateTime = 0f;
        Play("attack");
    }

    public void PlayHitReaction(Vector2 direction, float severity = 1f)
    {
        if (_isDying)
        {
            return;
        }

        _flashTime = 0.16f;
        float addedHurt = Mathf.Clamp(0.22f + severity * 0.12f, 0.22f, 0.62f);
        _hurtTime = Mathf.Min(_hurtTime + addedHurt, 0.72f);
        _hurtStrength = Mathf.Clamp(_hurtStrength + severity * 0.35f, 0.5f, 3.5f);
        if (direction.LengthSquared() > 0.01f)
        {
            _hurtDirection = direction.Normalized();
        }

        if (_painGrimace is not null)
        {
            _painGrimace.Visible = true;
        }

        if (UseLayeredPrototype)
        {
            _layeredState = "hurt";
            _stateTime = 0f;
            _impactSpawned = false;
            return;
        }

        PlayHitFlash();
    }

    public void PlayHitFlash()
    {
        if (UseLayeredPrototype)
        {
            _flashTime = 0.1f;
            return;
        }

        if (_sprite is null)
        {
            return;
        }

        _sprite.Modulate = new Color(1f, 0.35f, 0.35f);
        GetTree().CreateTimer(0.08).Timeout += () =>
        {
            if (IsInstanceValid(_sprite))
            {
                _sprite.Modulate = Colors.White;
            }
        };
    }

    private void ApplyVisualOffset()
    {
        Position = _basePosition + _jumpOffset + _movementOffset;
    }

    private void SetLayeredState(string nextState)
    {
        if (_isDying && nextState != "death")
        {
            return;
        }

        if (_layeredState == nextState)
        {
            return;
        }

        if (_layeredState == "finisher")
        {
            _finisherAttack = false;
        }

        _layeredState = nextState;
        _stateTime = 0f;
        _impactSpawned = false;
    }

    private void BuildLayeredPrototype()
    {
        PresetPalette palette = ResolvePresetPalette();
        _baseRigScale = palette.RigScale;
        _headAnchorY = palette.HeadYOffset;
        _rig = new Node2D { Name = $"Layered{LayeredPreset}Rig", YSortEnabled = true };
        AddChild(_rig);
        _rig.Scale = new Vector2(_baseRigScale.X * _facingSign, _baseRigScale.Y);

        Color skin = palette.Skin;
        Color backSkin = skin.Darkened(0.08f);
        Color pants = palette.Pants;
        Color backPants = pants.Darkened(0.12f);
        Color shirt = palette.Shirt;
        Color vest = palette.Vest;
        Color shoeAccent = palette.ShoeAccent;
        AddReadabilitySilhouette(_rig, palette);

        Node2D backLayer = AddJoint(_rig, "BackLayer", Vector2.Zero, 0);
        _backLeg = AddLegRig(backLayer, "BackLeg", new Vector2(-8f, -12f), backPants, shoeAccent, z: 0, out _backLegShin);
        _backArm = AddArmRig(backLayer, "BackArm", new Vector2(-15f, -58f), backSkin, z: 1, knuckles: palette.ShowKnuckles, out _backArmForearm);

        _torso = AddJoint(_rig, "Torso", new Vector2(0f, -56f), 4);
        AddPolygon(_torso, "BackVest", vest, new[]
        {
            new Vector2(-18f, -20f), new Vector2(14f, -22f), new Vector2(20f, 4f), new Vector2(14f, 30f),
            new Vector2(0f, 34f), new Vector2(-15f, 28f), new Vector2(-22f, 6f)
        }, 0);
        _shirtPulse = AddJoint(_torso, "ShirtPulse", Vector2.Zero, 2);
        RigShadingLibrary.AddCelShape(_shirtPulse, "TorsoCloth", shirt, MakeTorsoSilhouette(), 0);
        RigShadingLibrary.AddTorsoShade(_shirtPulse, shirt, shirt.Darkened(0.28f));
        AddPolygon(_shirtPulse, "ChestMark", palette.ChestMark, new[]
        {
            new Vector2(-11f, -2f), new Vector2(9f, -3f), new Vector2(11f, 4f), new Vector2(-11f, 5f)
        }, 1);
        AddPolygon(_torso, "RaggedHem", palette.HemColor, new[]
        {
            new Vector2(-17f, 28f), new Vector2(-6f, 34f), new Vector2(2f, 29f), new Vector2(14f, 35f), new Vector2(12f, 42f), new Vector2(-14f, 39f)
        }, 3);
        _vestFlap = AddJoint(_torso, "VestFlap", new Vector2(-18f, 10f), 4);
        AddPolygon(_vestFlap, "Flap", vest.Darkened(0.08f), new[]
        {
            new Vector2(0f, 0f), new Vector2(14f, -2f), new Vector2(18f, 16f), new Vector2(4f, 20f), new Vector2(-2f, 8f)
        }, 0);
        _torsoBlood = AddPolygon(_torso, "TorsoBlood", new Color(0.52f, 0.01f, 0.02f, 0.82f), new[]
        {
            new Vector2(-8f, 4f), new Vector2(10f, 2f), new Vector2(14f, 16f), new Vector2(-4f, 20f), new Vector2(-12f, 12f)
        }, 6);
        _torsoBlood.Visible = false;

        if (LayeredPreset == LayeredPrototypePreset.Caua)
        {
            AddPolygon(_torso, "TankTopL", shirt, new[]
            {
                new Vector2(-17f, -20f), new Vector2(-8f, -22f), new Vector2(-6f, 32f), new Vector2(-16f, 34f)
            }, 5);
            AddPolygon(_torso, "TankTopR", shirt, new[]
            {
                new Vector2(8f, -21f), new Vector2(17f, -19f), new Vector2(16f, 33f), new Vector2(6f, 32f)
            }, 5);
            AddPolygon(_torso, "TankNeck", shirt, new[]
            {
                new Vector2(-8f, -20f), new Vector2(8f, -21f), new Vector2(6f, -10f), new Vector2(-6f, -9f)
            }, 6);
            AddPolygon(_torso, "CargoPocket", new Color(0.05f, 0.055f, 0.048f), new[]
            {
                new Vector2(6f, 18f), new Vector2(16f, 17f), new Vector2(17f, 26f), new Vector2(5f, 27f)
            }, 4);
            AddPolygon(_torso, "CargoHem", pants, new[]
            {
                new Vector2(-15f, 28f), new Vector2(14f, 30f), new Vector2(12f, 36f), new Vector2(-13f, 35f)
            }, 4);
        }
        else if (LayeredPreset == LayeredPrototypePreset.Brute)
        {
            _torso.Scale = new Vector2(1.22f, 1.14f);
            AddPolygon(_torso, "ShoulderL", palette.Shirt.Darkened(0.1f), new[]
            {
                new Vector2(-24f, -18f), new Vector2(-10f, -22f), new Vector2(-6f, -8f), new Vector2(-20f, -4f)
            }, 5);
            AddPolygon(_torso, "ShoulderR", palette.Shirt.Darkened(0.08f), new[]
            {
                new Vector2(10f, -21f), new Vector2(24f, -17f), new Vector2(20f, -3f), new Vector2(6f, -7f)
            }, 5);
        }
        else if (LayeredPreset == LayeredPrototypePreset.QuebraOsso)
        {
            AddPolygon(_torso, "TankTopL", shirt, new[]
            {
                new Vector2(-17f, -18f), new Vector2(-7f, -20f), new Vector2(-5f, 30f), new Vector2(-15f, 32f)
            }, 5);
            AddPolygon(_torso, "TankTopR", shirt, new[]
            {
                new Vector2(7f, -19f), new Vector2(17f, -17f), new Vector2(15f, 31f), new Vector2(5f, 30f)
            }, 5);
        }
        else if (LayeredPreset == LayeredPrototypePreset.Infected)
        {
            AddPolygon(_torso, "ButcherShirt", new Color(0.82f, 0.80f, 0.76f), new[]
            {
                new Vector2(-14f, -14f), new Vector2(12f, -16f), new Vector2(14f, 18f), new Vector2(-12f, 20f)
            }, 5);
        }
        else if (LayeredPreset == LayeredPrototypePreset.Fast)
        {
            _torso.Scale = new Vector2(0.92f, 0.96f);
        }

        _frontLeg = AddLegRig(_rig, "FrontLeg", new Vector2(8f, -12f), pants, shoeAccent, z: 6, out _frontLegShin);
        if (LayeredPreset == LayeredPrototypePreset.Caua)
        {
            ApplyCauaSneakers(_frontLegShin, shoeAccent);
            ApplyCauaSneakers(_backLegShin, shoeAccent);
        }

        _head = AddJoint(_rig, "Head", new Vector2(0f, _headAnchorY), 8);
        BuildProfileHead(_head, skin, palette);
        _blackEye = AddPolygon(_head, "BlackEye", new Color(0.32f, 0.1f, 0.48f, 0.92f), new[]
        {
            new Vector2(4f, -4f), new Vector2(12f, -6f), new Vector2(11f, 1f), new Vector2(3f, 2f)
        }, 9);
        _blackEye.Visible = false;
        _noseBleed = AddPolygon(_head, "NoseBleed", new Color(0.48f, 0.01f, 0.02f, 0.88f), new[]
        {
            new Vector2(12f, 5f), new Vector2(15f, 4f), new Vector2(14f, 10f), new Vector2(11f, 11f)
        }, 9);
        _noseBleed.Visible = false;
        _noseBleedDrip = AddPolygon(_head, "NoseBleedDrip", new Color(0.55f, 0.01f, 0.02f, 0.75f), new[]
        {
            new Vector2(13f, 11f), new Vector2(15f, 11f), new Vector2(14f, 17f), new Vector2(12f, 17f)
        }, 10);
        _noseBleedDrip.Visible = false;
        _bloodSmearFace = AddPolygon(_head, "BloodSmearFace", new Color(0.42f, 0.01f, 0.02f, 0.72f), new[]
        {
            new Vector2(6f, 2f), new Vector2(16f, 0f), new Vector2(17f, 10f), new Vector2(5f, 12f)
        }, 9);
        _bloodSmearFace.Visible = false;
        _shirtTear = AddPolygon(_torso, "ShirtTear", new Color(0.04f, 0.04f, 0.045f, 0.85f), new[]
        {
            new Vector2(-6f, 2f), new Vector2(8f, -2f), new Vector2(12f, 14f), new Vector2(-2f, 18f), new Vector2(-10f, 10f)
        }, 7);
        _shirtTear.Visible = false;
        _faceBruise = AddPolygon(_head, "FaceBruise", new Color(0.38f, 0.08f, 0.12f, 0.88f), new[]
        {
            new Vector2(-4f, -2f), new Vector2(6f, -4f), new Vector2(5f, 4f), new Vector2(-5f, 5f)
        }, 7);
        _faceBruise.Visible = false;
        _painGrimace = AddPolygon(_head, "PainGrimace", new Color(0.42f, 0.06f, 0.08f, 0.92f), new[]
        {
            new Vector2(2f, 12f), new Vector2(12f, 11f), new Vector2(10f, 16f), new Vector2(6f, 15f), new Vector2(2f, 16f)
        }, 8);
        _painGrimace.Visible = false;
        _hair = AddJoint(_head, "Hair", new Vector2(-1f, -12f), 6);
        switch (LayeredPreset)
        {
            case LayeredPrototypePreset.Caua:
                AddPolygon(_hair, "BuzzCut", palette.HairColor, MakeEllipse(0f, -8f, 13f, 5f, 10), 0);
                break;
            case LayeredPrototypePreset.QuebraOsso:
            case LayeredPrototypePreset.Brute:
                AddPolygon(_hair, "BaldScalp", skin.Darkened(0.06f), MakeEllipse(0f, -6f, palette.FaceRx - 1f, 4f, 10), 0);
                break;
            case LayeredPrototypePreset.MiniBoss:
                AddPolygon(_hair, "GreyHair", palette.HairColor, MakeEllipse(0f, -7f, 12f, 4.5f, 10), 0);
                AddPolygon(_head, "Mustache", new Color(0.14f, 0.11f, 0.09f), new[]
                {
                    new Vector2(-8f, 9f), new Vector2(9f, 8f), new Vector2(7f, 13f), new Vector2(0f, 14f), new Vector2(-7f, 13f)
                }, 7);
                break;
            case LayeredPrototypePreset.Infected:
                AddPolygon(_hair, "ShortHair", palette.HairColor, MakeEllipse(0f, -6f, 11f, 4f, 10), 0);
                break;
            default:
                AddPolygon(_hair, "HairSpikes", palette.HairColor, palette.HairShape, 0);
                _hairTail = AddJoint(_hair, "HairTail", new Vector2(6f, 6f), 1);
                AddPolygon(_hairTail, "TailStrand", palette.HairTailColor, new[]
                {
                    new Vector2(-3f, 0f), new Vector2(8f, -2f), new Vector2(12f, 10f), new Vector2(0f, 12f)
                }, 0);
                break;
        }

        if (LayeredPreset == LayeredPrototypePreset.QuebraOsso || LayeredPreset == LayeredPrototypePreset.Fast)
        {
            AddPolygon(_head, "Goatee", new Color(0.06f, 0.04f, 0.035f), new[]
            {
                new Vector2(-5f, 12f), new Vector2(6f, 11f), new Vector2(4f, 20f), new Vector2(0f, 22f), new Vector2(-4f, 20f)
            }, 7);
        }

        _frontArm = AddArmRig(_rig, "FrontArm", new Vector2(15f, -58f), skin.Lightened(0.04f), z: 10, knuckles: palette.ShowKnuckles, out _frontArmForearm);
        if (LayeredPreset == LayeredPrototypePreset.Caua)
        {
            AddPolygon(_frontArmForearm, "ArmBandage", new Color(0.82f, 0.78f, 0.68f), new[]
            {
                new Vector2(-9f, -4f), new Vector2(9f, -5f), new Vector2(9f, 4f), new Vector2(-8f, 5f)
            }, 5);
            AddPolygon(_frontArmForearm, "BandageStain", new Color(0.42f, 0.02f, 0.03f, 0.65f), new[]
            {
                new Vector2(-4f, -2f), new Vector2(5f, -3f), new Vector2(4f, 2f), new Vector2(-3f, 3f)
            }, 6);
            Node2D knifeRoot = AddJoint(_frontArmForearm, "CaseiraKnifeRoot", Vector2.Zero, 7);
            RigShadingLibrary.BuildCaseiraKnife(knifeRoot, out _, out CanvasItem? blood);
            _caseiraKnife = knifeRoot;
            _knifeBlood = blood;
            SetCaseiraKnifeVisible(false);
        }

        _sidearm = AddJoint(_frontArmForearm, "Sidearm", new Vector2(14f, 8f), 9);
        AddPolygon(_sidearm, "Grip", new Color(0.08f, 0.06f, 0.05f), new[]
        {
            new Vector2(-3f, -2f), new Vector2(10f, -2f), new Vector2(10f, 3f), new Vector2(-3f, 3f)
        }, 0);
        AddPolygon(_sidearm, "Slide", new Color(0.18f, 0.18f, 0.2f), new[]
        {
            new Vector2(8f, -3f), new Vector2(22f, -3f), new Vector2(22f, 2f), new Vector2(8f, 2f)
        }, 1);
        _sidearm.Visible = false;
        _criticalLimp = AddPolygon(_frontLeg, "CriticalLimp", new Color(0.42f, 0.05f, 0.06f, 0.75f), new[]
        {
            new Vector2(-4f, -8f), new Vector2(8f, -10f), new Vector2(6f, 6f), new Vector2(-6f, 8f)
        }, 4);
        _criticalLimp.Visible = false;

        AddPresetAccents(palette);
        ApplyPresetPosture();
    }

    private void ApplyPresetPosture()
    {
        switch (LayeredPreset)
        {
            case LayeredPrototypePreset.QuebraOsso:
                if (_head is not null)
                {
                    _head.Position = new Vector2(0f, _headAnchorY);
                }

                break;
            case LayeredPrototypePreset.Infected:
                if (_torso is not null)
                {
                    _torso.Scale = new Vector2(1.1f, 1.08f);
                }

                break;
            case LayeredPrototypePreset.MiniBoss:
                if (_torso is not null)
                {
                    _torso.Position = new Vector2(0f, -58f);
                }

                if (_frontLeg is not null && _backLeg is not null)
                {
                    _frontLeg.Position = new Vector2(10f, -12f);
                    _backLeg.Position = new Vector2(-10f, -12f);
                }

                break;
        }
    }

    private readonly struct PresetPalette
    {
        public Color Skin { get; init; }
        public Color Pants { get; init; }
        public Color Shirt { get; init; }
        public Color Vest { get; init; }
        public Color ShoeAccent { get; init; }
        public Color ChestMark { get; init; }
        public Color HemColor { get; init; }
        public Color HairColor { get; init; }
        public Color HairTailColor { get; init; }
        public Color EyeGlow { get; init; }
        public Vector2[] HairShape { get; init; }
        public Vector2 RigScale { get; init; }
        public float HeadYOffset { get; init; }
        public float FaceRx { get; init; }
        public float FaceRy { get; init; }
        public float HeadScale { get; init; }
        public bool ShowKnuckles { get; init; }
        public bool CorruptedEyes { get; init; }
    }

    private PresetPalette ResolvePresetPalette()
    {
        return LayeredPreset switch
        {
            LayeredPrototypePreset.Caua => new PresetPalette
            {
                Skin = new Color(0.58f, 0.34f, 0.19f),
                Pants = new Color(0.065f, 0.075f, 0.06f),
                Shirt = new Color(0.66f, 0.035f, 0.03f),
                Vest = new Color(0.13f, 0.09f, 0.075f),
                ShoeAccent = new Color(0.62f, 0.04f, 0.03f),
                ChestMark = new Color(0.88f, 0.77f, 0.58f, 0.85f),
                HemColor = new Color(0.34f, 0.015f, 0.012f),
                HairColor = new Color(0.035f, 0.025f, 0.02f),
                HairTailColor = new Color(0.05f, 0.035f, 0.028f),
                EyeGlow = Colors.Transparent,
                HairShape = MakeEllipse(0f, -6f, 12f, 5f, 10),
                RigScale = new Vector2(0.94f, 1.08f),
                HeadYOffset = -104f,
                FaceRx = 10f,
                FaceRy = 12f,
                HeadScale = 0.82f,
                ShowKnuckles = true,
                CorruptedEyes = false,
            },
            LayeredPrototypePreset.Fast => new PresetPalette
            {
                Skin = new Color(0.50f, 0.29f, 0.17f),
                Pants = new Color(0.14f, 0.16f, 0.08f),
                Shirt = new Color(0.24f, 0.22f, 0.1f),
                Vest = new Color(0.2f, 0.18f, 0.08f),
                ShoeAccent = new Color(0.18f, 0.14f, 0.1f),
                ChestMark = new Color(0.82f, 0.78f, 0.66f, 0.7f),
                HemColor = new Color(0.14f, 0.012f, 0.018f),
                HairColor = new Color(0.03f, 0.02f, 0.018f),
                HairTailColor = new Color(0.03f, 0.02f, 0.018f),
                EyeGlow = Colors.Transparent,
                HairShape = MakeEllipse(0f, -4f, 10f, 4f, 10),
                RigScale = new Vector2(0.84f, 1.06f),
                HeadYOffset = -101f,
                FaceRx = 9.5f,
                FaceRy = 11.5f,
                HeadScale = 0.80f,
                ShowKnuckles = true,
                CorruptedEyes = false,
            },
            LayeredPrototypePreset.QuebraOsso => new PresetPalette
            {
                Skin = new Color(0.48f, 0.28f, 0.16f),
                Pants = new Color(0.12f, 0.17f, 0.07f),
                Shirt = new Color(0.30f, 0.34f, 0.14f),
                Vest = new Color(0.26f, 0.30f, 0.12f),
                ShoeAccent = new Color(0.22f, 0.14f, 0.08f),
                ChestMark = new Color(0.78f, 0.74f, 0.62f, 0.65f),
                HemColor = new Color(0.10f, 0.14f, 0.06f),
                HairColor = new Color(0.04f, 0.03f, 0.025f),
                HairTailColor = new Color(0.04f, 0.03f, 0.025f),
                EyeGlow = Colors.Transparent,
                HairShape = MakeEllipse(0f, -3f, 12f, 3.5f, 10),
                RigScale = new Vector2(1.0f, 1.08f),
                HeadYOffset = -102f,
                FaceRx = 10.5f,
                FaceRy = 12.5f,
                HeadScale = 0.84f,
                ShowKnuckles = true,
                CorruptedEyes = false,
            },
            LayeredPrototypePreset.Brute => new PresetPalette
            {
                Skin = new Color(0.42f, 0.26f, 0.16f),
                Pants = new Color(0.72f, 0.68f, 0.58f),
                Shirt = new Color(0.82f, 0.66f, 0.14f),
                Vest = new Color(0.76f, 0.58f, 0.12f),
                ShoeAccent = new Color(0.22f, 0.16f, 0.1f),
                ChestMark = new Color(0.38f, 0.22f, 0.06f, 0.55f),
                HemColor = new Color(0.62f, 0.48f, 0.1f),
                HairColor = new Color(0.02f, 0.015f, 0.012f),
                HairTailColor = new Color(0.025f, 0.018f, 0.015f),
                EyeGlow = Colors.Transparent,
                HairShape = MakeEllipse(0f, -2f, 12f, 6f, 10),
                RigScale = new Vector2(1.18f, 1.22f),
                HeadYOffset = -108f,
                FaceRx = 12f,
                FaceRy = 14f,
                HeadScale = 0.90f,
                ShowKnuckles = true,
                CorruptedEyes = false,
            },
            LayeredPrototypePreset.Infected => new PresetPalette
            {
                Skin = new Color(0.52f, 0.31f, 0.2f),
                Pants = new Color(0.22f, 0.12f, 0.06f),
                Shirt = new Color(0.78f, 0.76f, 0.72f),
                Vest = new Color(0.86f, 0.84f, 0.8f),
                ShoeAccent = new Color(0.2f, 0.14f, 0.1f),
                ChestMark = new Color(0.52f, 0.03f, 0.04f, 0.75f),
                HemColor = new Color(0.72f, 0.7f, 0.66f),
                HairColor = new Color(0.04f, 0.03f, 0.028f),
                HairTailColor = new Color(0.04f, 0.03f, 0.028f),
                EyeGlow = Colors.Transparent,
                HairShape = MakeEllipse(0f, -4f, 11f, 4f, 10),
                RigScale = new Vector2(1.08f, 1.16f),
                HeadYOffset = -104f,
                FaceRx = 10.5f,
                FaceRy = 13f,
                HeadScale = 0.88f,
                ShowKnuckles = false,
                CorruptedEyes = false,
            },
            LayeredPrototypePreset.MiniBoss => new PresetPalette
            {
                Skin = new Color(0.46f, 0.28f, 0.18f),
                Pants = new Color(0.08f, 0.07f, 0.075f),
                Shirt = new Color(0.16f, 0.15f, 0.17f),
                Vest = new Color(0.2f, 0.19f, 0.21f),
                ShoeAccent = new Color(0.16f, 0.12f, 0.1f),
                ChestMark = new Color(0.12f, 0.42f, 0.16f, 0.9f),
                HemColor = new Color(0.14f, 0.13f, 0.15f),
                HairColor = new Color(0.12f, 0.1f, 0.09f),
                HairTailColor = new Color(0.12f, 0.1f, 0.09f),
                EyeGlow = Colors.Transparent,
                HairShape = MakeEllipse(0f, -2f, 12f, 5f, 10),
                RigScale = new Vector2(1.08f, 1.16f),
                HeadYOffset = -104f,
                FaceRx = 10.5f,
                FaceRy = 12.5f,
                HeadScale = 0.86f,
                ShowKnuckles = false,
                CorruptedEyes = false,
            },
            _ => new PresetPalette
            {
                Skin = new Color(0.50f, 0.29f, 0.17f),
                Pants = new Color(0.14f, 0.16f, 0.08f),
                Shirt = new Color(0.24f, 0.22f, 0.1f),
                Vest = new Color(0.2f, 0.18f, 0.08f),
                ShoeAccent = new Color(0.18f, 0.14f, 0.1f),
                ChestMark = new Color(0.82f, 0.78f, 0.66f, 0.7f),
                HemColor = new Color(0.14f, 0.012f, 0.018f),
                HairColor = new Color(0.03f, 0.02f, 0.018f),
                HairTailColor = new Color(0.04f, 0.03f, 0.025f),
                EyeGlow = Colors.Transparent,
                HairShape = MakeEllipse(0f, -4f, 10f, 4f, 10),
                RigScale = new Vector2(0.96f, 1.06f),
                HeadYOffset = -101f,
                FaceRx = 9.5f,
                FaceRy = 11.5f,
                HeadScale = 0.82f,
                ShowKnuckles = true,
                CorruptedEyes = false,
            },
        };
    }

    private void AddPresetAccents(PresetPalette palette)
    {
        _variantAccentRoot?.QueueFree();
        _variantAccentRoot = AddJoint(_rig!, "VariantAccents", Vector2.Zero, 12);

        switch (LayeredPreset)
        {
            case LayeredPrototypePreset.QuebraOsso:
                RigShadingLibrary.BuildIronPipe(_variantAccentRoot, 24f, -74f, 76f, 12f);
                break;
            case LayeredPrototypePreset.Fast:
                RigShadingLibrary.BuildIronPipe(_variantAccentRoot, 20f, -66f, 64f, 10f);
                break;
            case LayeredPrototypePreset.Brute:
                RigShadingLibrary.AddLeopardPrint(_torso!);
                break;
            case LayeredPrototypePreset.Infected:
                AddPolygon(_torso!, "ButcherApron", new Color(0.84f, 0.82f, 0.78f), new[]
                {
                    new Vector2(-18f, -8f), new Vector2(16f, -10f), new Vector2(20f, 34f), new Vector2(-16f, 36f)
                }, 6);
                RigShadingLibrary.AddApronDetail(_torso!);
                RigShadingLibrary.BuildButcherCleaver(_variantAccentRoot, 42f, -38f);
                break;
            case LayeredPrototypePreset.MiniBoss:
                RigShadingLibrary.AddSuitLapels(_torso!);
                AddPolygon(_torso!, "OfficialSash", new Color(0.12f, 0.48f, 0.16f), new[]
                {
                    new Vector2(-16f, -6f), new Vector2(14f, -8f), new Vector2(18f, 28f), new Vector2(-12f, 30f)
                }, 7);
                AddPolygon(_torso!, "SashStripe", new Color(0.82f, 0.68f, 0.08f), new[]
                {
                    new Vector2(-4f, -4f), new Vector2(2f, -5f), new Vector2(6f, 26f), new Vector2(0f, 27f)
                }, 8);
                RigShadingLibrary.BuildBaton(_variantAccentRoot, 34f, -42f);
                break;
        }
    }

    private void BuildProfileHead(Node2D head, Color skin, PresetPalette palette)
    {
        float s = palette.HeadScale;
        Color shadow = skin.Darkened(0.22f);
        Color deep = skin.Darkened(0.32f);
        Color highlight = skin.Lightened(0.1f);

        AddPolygon(head, "Neck", shadow, new[]
        {
            new Vector2(-5f * s, 14f * s), new Vector2(7f * s, 14f * s), new Vector2(5f * s, 24f * s), new Vector2(-6f * s, 23f * s)
        }, 0);
        AddPolygon(head, "SkullOutline", deep, new[]
        {
            new Vector2(-10f * s, -13f * s), new Vector2(5f * s, -16f * s), new Vector2(14f * s, -9f * s),
            new Vector2(16f * s, 3f * s), new Vector2(12f * s, 15f * s), new Vector2(5f * s, 21f * s),
            new Vector2(-3f * s, 23f * s), new Vector2(-11f * s, 18f * s), new Vector2(-13f * s, 6f * s),
            new Vector2(-12f * s, -4f * s)
        }, 1);
        AddPolygon(head, "SkullProfile", skin, new[]
        {
            new Vector2(-9f * s, -12f * s), new Vector2(4f * s, -15f * s), new Vector2(13f * s, -8f * s),
            new Vector2(15f * s, 2f * s), new Vector2(12f * s, 13f * s), new Vector2(5f * s, 20f * s),
            new Vector2(-2f * s, 22f * s), new Vector2(-10f * s, 17f * s), new Vector2(-11f * s, 5f * s),
            new Vector2(-10f * s, -3f * s)
        }, 2);
        AddPolygon(head, "SkullBackShade", shadow, new[]
        {
            new Vector2(-11f * s, -8f * s), new Vector2(-3f * s, -10f * s), new Vector2(-2f * s, 18f * s), new Vector2(-11f * s, 16f * s)
        }, 3);
        AddPolygon(head, "SkullHighlight", new Color(highlight.R, highlight.G, highlight.B, 0.65f), new[]
        {
            new Vector2(2f * s, -10f * s), new Vector2(11f * s, -7f * s), new Vector2(10f * s, 2f * s), new Vector2(1f * s, 0f)
        }, 4);
        AddPolygon(head, "BrowRidge", new Color(0.05f, 0.03f, 0.025f), new[]
        {
            new Vector2(3f * s, -7f * s), new Vector2(14f * s, -9f * s), new Vector2(13f * s, -4f * s), new Vector2(2f * s, -2f * s)
        }, 5);
        AddPolygon(head, "EyeWhite", new Color(0.92f, 0.88f, 0.80f), new[]
        {
            new Vector2(6f * s, -4f * s), new Vector2(12f * s, -5f * s), new Vector2(11f * s, -1f * s), new Vector2(5f * s, 0f)
        }, 6);
        if (palette.CorruptedEyes)
        {
            AddPolygon(head, "EyeGlow", palette.EyeGlow, new[]
            {
                new Vector2(6f * s, -4f * s), new Vector2(12f * s, -5f * s), new Vector2(11f * s, -1f * s), new Vector2(5f * s, 0f)
            }, 7);
        }
        else
        {
            AddPolygon(head, "Pupil", new Color(0.05f, 0.035f, 0.03f), new[]
            {
                new Vector2(8f * s, -3.5f * s), new Vector2(11f * s, -4f * s), new Vector2(10f * s, -1.5f * s), new Vector2(7f * s, -1f * s)
            }, 7);
        }

        AddPolygon(head, "NoseBridge", deep, new[]
        {
            new Vector2(12f * s, 0f), new Vector2(14f * s, 4f * s), new Vector2(12f * s, 7f * s), new Vector2(10f * s, 5f * s)
        }, 5);
        AddPolygon(head, "EarBack", shadow, new[]
        {
            new Vector2(-11f * s, 0f), new Vector2(-8f * s, 3f * s), new Vector2(-9f * s, 8f * s), new Vector2(-12f * s, 6f * s)
        }, 1);
        AddPolygon(head, "Mouth", new Color(0.22f, 0.06f, 0.05f), new[]
        {
            new Vector2(5f * s, 12f * s), new Vector2(11f * s, 11f * s), new Vector2(10f * s, 14f * s), new Vector2(4f * s, 15f * s)
        }, 7);
        AddPolygon(head, "JawLine", deep, new[]
        {
            new Vector2(-1f * s, 17f * s), new Vector2(9f * s, 16f * s), new Vector2(7f * s, 20f * s), new Vector2(-3f * s, 21f * s)
        }, 4);
    }

    private void AddReadabilitySilhouette(Node2D parent, PresetPalette palette)
    {
        Color halo = LayeredPreset == LayeredPrototypePreset.Caua
            ? new Color(1f, 0.62f, 0.20f, 0.12f)
            : new Color(0.76f, 0.04f, 0.04f, 0.12f);
        Color rim = palette.ShoeAccent.Lightened(0.18f);
        rim.A = 0.16f;

        AddPolygon(parent, "ReadabilityHalo", halo, new[]
        {
            new Vector2(-42f, -132f), new Vector2(34f, -130f), new Vector2(48f, -86f),
            new Vector2(44f, -28f), new Vector2(24f, 10f), new Vector2(-26f, 12f),
            new Vector2(-46f, -28f), new Vector2(-50f, -88f),
        }, -3);

        AddPolygon(parent, "ContactShadow", new Color(0f, 0f, 0f, 0.34f), new[]
        {
            new Vector2(-42f, 7f), new Vector2(36f, 5f), new Vector2(54f, 14f),
            new Vector2(34f, 23f), new Vector2(-36f, 23f), new Vector2(-56f, 14f),
        }, -2);

        AddPolygon(parent, "RimSide", rim, new[]
        {
            new Vector2(30f, -118f), new Vector2(38f, -98f), new Vector2(40f, -34f),
            new Vector2(30f, 2f), new Vector2(22f, -2f), new Vector2(30f, -62f),
        }, -1);
    }

    private static Node2D AddJoint(Node parent, string name, Vector2 position, int z)
    {
        Node2D node = new() { Name = name, Position = position, ZIndex = z };
        parent.AddChild(node);
        return node;
    }

    private static Vector2[] MakeTorsoSilhouette()
    {
        return
        [
            new Vector2(-16f, -24f), new Vector2(-3f, -27f), new Vector2(11f, -25f), new Vector2(18f, -16f),
            new Vector2(20f, 5f), new Vector2(16f, 27f), new Vector2(7f, 43f), new Vector2(-4f, 44f),
            new Vector2(-16f, 36f), new Vector2(-21f, 14f), new Vector2(-20f, -4f)
        ];
    }

    private static Vector2[] MakeThighSilhouette()
    {
        return
        [
            new Vector2(-8f, -38f), new Vector2(6f, -40f), new Vector2(12f, -22f), new Vector2(10f, 14f),
            new Vector2(5f, 23f), new Vector2(-6f, 21f), new Vector2(-12f, 4f), new Vector2(-12f, -26f)
        ];
    }

    private static Vector2[] MakeShinSilhouette()
    {
        return
        [
            new Vector2(-6f, -12f), new Vector2(6f, -13f), new Vector2(8f, 4f), new Vector2(6f, 22f),
            new Vector2(1f, 28f), new Vector2(-5f, 25f), new Vector2(-8f, 2f)
        ];
    }

    private static Vector2[] MakeUpperArmSilhouette()
    {
        return
        [
            new Vector2(-6f, -11f), new Vector2(7f, -12f), new Vector2(10f, 2f), new Vector2(8f, 21f),
            new Vector2(2f, 25f), new Vector2(-5f, 22f), new Vector2(-8f, 4f)
        ];
    }

    private static Vector2[] MakeForearmSilhouette()
    {
        return
        [
            new Vector2(-7f, -8f), new Vector2(8f, -9f), new Vector2(10f, 10f), new Vector2(7f, 27f),
            new Vector2(0f, 30f), new Vector2(-6f, 27f), new Vector2(-8f, 5f)
        ];
    }

    private static void AddSoftPolygon(Node parent, string name, Color color, Vector2[] points, int z)
    {
        RigShadingLibrary.AddCelShape(parent, name, color, points, z);
    }

    private static void ApplyCauaSneakers(Node2D shin, Color sneakerRed)
    {
        AddPolygon(shin, "SneakerBody", sneakerRed, new[]
        {
            new Vector2(-12f, 22f), new Vector2(5f, 21f), new Vector2(20f, 24f), new Vector2(21f, 31f),
            new Vector2(6f, 34f), new Vector2(-11f, 32f), new Vector2(-14f, 26f)
        }, 2);
        AddPolygon(shin, "SoleWhite", new Color(0.88f, 0.86f, 0.82f), new[]
        {
            new Vector2(-13f, 30f), new Vector2(20f, 31f), new Vector2(19f, 34f), new Vector2(-12f, 33f)
        }, 3);
    }

    private static Node2D AddLegRig(
        Node parent,
        string name,
        Vector2 position,
        Color cloth,
        Color accent,
        int z,
        out Node2D shinJoint)
    {
        Node2D hip = AddJoint(parent, name, position, z);
        AddSoftPolygon(hip, "Thigh", cloth, MakeThighSilhouette(), 0);
        shinJoint = AddJoint(hip, "ShinJoint", new Vector2(0f, 24f), 1);
        AddSoftPolygon(shinJoint, "Shin", cloth.Darkened(0.15f), MakeShinSilhouette(), 0);
        AddSoftPolygon(shinJoint, "Boot", new Color(0.09f, 0.06f, 0.045f), new[]
        {
            new Vector2(-12f, 22f), new Vector2(5f, 21f), new Vector2(20f, 24f), new Vector2(21f, 31f),
            new Vector2(7f, 34f), new Vector2(-11f, 32f), new Vector2(-14f, 26f)
        }, 1);
        AddPolygon(shinJoint, "ShoeAccent", accent, new[]
        {
            new Vector2(-5f, 20f), new Vector2(8f, 21f), new Vector2(8f, 25f), new Vector2(-6f, 25f)
        }, 2);
        return hip;
    }

    private static Node2D AddArmRig(
        Node parent,
        string name,
        Vector2 position,
        Color skin,
        int z,
        bool knuckles,
        out Node2D forearmJoint)
    {
        Node2D shoulder = AddJoint(parent, name, position, z);
        AddSoftPolygon(shoulder, "UpperArm", skin, MakeUpperArmSilhouette(), 0);
        forearmJoint = AddJoint(shoulder, "ForearmJoint", new Vector2(0f, 30f), 1);
        AddSoftPolygon(forearmJoint, "Forearm", skin.Lightened(0.04f), MakeForearmSilhouette(), 0);
        AddPolygon(forearmJoint, "Wrap", new Color(0.85f, 0.78f, 0.62f), new[]
        {
            new Vector2(-9f, 7f), new Vector2(9f, 6f), new Vector2(9f, 12f), new Vector2(-8f, 13f)
        }, 1);
        float fistRx = knuckles ? 9f : 8f;
        AddPolygon(forearmJoint, "Fist", skin.Darkened(0.08f), MakeEllipse(1f, 31f, fistRx, 7f, 8), 2);
        if (knuckles)
        {
            AddPolygon(forearmJoint, "Knuckles", new Color(0.78f, 0.72f, 0.58f), new[]
            {
                new Vector2(-4f, 27f), new Vector2(6f, 26f), new Vector2(5f, 31f), new Vector2(-3f, 32f)
            }, 3);
        }

        return shoulder;
    }

    private static Node2D AddLimb(Node parent, string name, Vector2 position, Color cloth, Color accent, int z)
    {
        Node2D leg = AddJoint(parent, name, position, z);
        AddPolygon(leg, "Thigh", cloth, new[]
        {
            new Vector2(-6f, -35f), new Vector2(8f, -35f), new Vector2(7f, -10f), new Vector2(-8f, -9f)
        }, 0);
        AddPolygon(leg, "Shin", cloth.Darkened(0.15f), new[]
        {
            new Vector2(-7f, -11f), new Vector2(7f, -10f), new Vector2(5f, 14f), new Vector2(-8f, 14f)
        }, 1);
        AddPolygon(leg, "Boot", new Color(0.09f, 0.06f, 0.045f), new[]
        {
            new Vector2(-10f, 11f), new Vector2(8f, 11f), new Vector2(17f, 17f), new Vector2(-12f, 18f)
        }, 2);
        AddPolygon(leg, "ShoeAccent", accent, new[]
        {
            new Vector2(-5f, 7f), new Vector2(7f, 8f), new Vector2(7f, 12f), new Vector2(-6f, 12f)
        }, 3);
        return leg;
    }

    private static Node2D AddArm(Node parent, string name, Vector2 position, Color skin, int z, bool knuckles = false)
    {
        Node2D arm = AddJoint(parent, name, position, z);
        AddPolygon(arm, "UpperArm", skin, new[]
        {
            new Vector2(-6f, -8f), new Vector2(7f, -9f), new Vector2(9f, 14f), new Vector2(-8f, 15f)
        }, 0);
        AddPolygon(arm, "Forearm", skin.Lightened(0.04f), new[]
        {
            new Vector2(-8f, 12f), new Vector2(9f, 12f), new Vector2(8f, 32f), new Vector2(-7f, 33f)
        }, 1);
        AddPolygon(arm, "Wrap", new Color(0.85f, 0.78f, 0.62f), new[]
        {
            new Vector2(-9f, 18f), new Vector2(9f, 17f), new Vector2(9f, 23f), new Vector2(-8f, 24f)
        }, 2);
        float fistRx = knuckles ? 9f : 8f;
        AddPolygon(arm, "Fist", skin.Darkened(0.08f), MakeEllipse(1f, 36f, fistRx, 7f, 8), 3);
        if (knuckles)
        {
            AddPolygon(arm, "Knuckles", new Color(0.78f, 0.72f, 0.58f), new[]
            {
                new Vector2(-4f, 32f), new Vector2(6f, 31f), new Vector2(5f, 36f), new Vector2(-3f, 37f)
            }, 4);
        }

        return arm;
    }

    private static Polygon2D AddPolygon(Node parent, string name, Color color, Vector2[] points, int z)
    {
        Polygon2D polygon = new()
        {
            Name = name,
            Color = color,
            Polygon = points,
            ZIndex = z
        };
        parent.AddChild(polygon);
        return polygon;
    }

    private static Vector2[] MakeEllipse(float cx, float cy, float rx, float ry, int points)
    {
        Vector2[] vertices = new Vector2[points];
        for (int i = 0; i < points; i++)
        {
            float angle = Mathf.Tau * i / points;
            vertices[i] = new Vector2(cx + Mathf.Cos(angle) * rx, cy + Mathf.Sin(angle) * ry);
        }

        return vertices;
    }

    private void AnimateLayeredPrototype()
    {
        float idle = _isExhausted
            ? Mathf.Sin(_stateTime * 7.6f) * 1.35f
            : Mathf.Sin(_stateTime * 4.2f);
        float heartbeat = _isExhausted
            ? Mathf.Max(0f, Mathf.Sin(_stateTime * 11.5f))
            : Mathf.Max(0f, Mathf.Sin(_stateTime * 8.5f));
        float walkSpeed = _damageTier == EnemyDamageVisualTier.Critical && !_isRunning
            ? 8.2f
            : _isExhausted && !_isRunning
                ? 7.4f
                : _isRunning
                    ? 14.5f
                    : 10.5f;
        float walk = Mathf.Sin(_stateTime * walkSpeed);
        float walkOpposite = Mathf.Cos(_stateTime * walkSpeed);
        float limpFactor = _damageTier == EnemyDamageVisualTier.Critical ? 0.55f : 1f;

        ResetLayeredPose();

        if (_torso is not null)
        {
            _torso.Position = new Vector2(0f, -56f + idle * 1.2f);
            _torso.Scale = new Vector2(1f + heartbeat * 0.018f, 1f + idle * 0.025f + heartbeat * 0.018f);
        }

        if (_shirtPulse is not null)
        {
            float pulse = _isExhausted ? heartbeat * 0.28f : heartbeat * 0.16f;
            _shirtPulse.Modulate = Colors.White.Lerp(new Color(1f, 0.72f, 0.62f), pulse);
        }

        if (_head is not null)
        {
            _head.Position = new Vector2(0f, _headAnchorY + idle * 0.8f);
            _head.Rotation = idle * 0.025f;
        }

        AnimateNoseBleedDrip();

        if (_hair is not null)
        {
            _hair.Rotation = Mathf.Sin(_stateTime * 5.4f + 0.7f) * 0.055f;
            _hair.Position = new Vector2(-1f, -12f + idle * 0.3f);
        }

        if (_hairTail is not null)
        {
            _hairTail.Rotation = Mathf.Sin(_stateTime * 6.8f + 1.2f) * 0.12f;
        }

        if (_vestFlap is not null)
        {
            _vestFlap.Rotation = Mathf.Sin(_stateTime * 4.8f + 0.4f) * 0.08f;
        }

        if (_clothSway is not null)
        {
            _clothSway.Rotation = Mathf.Sin(_stateTime * 5.2f + 0.9f) * 0.1f;
            _clothSway.Position = new Vector2(8f, -14f + idle * 0.4f);
        }

        switch (_layeredState)
        {
            case "walk":
                AnimateWalk(walk, walkOpposite, limpFactor);
                if (LayeredPreset != LayeredPrototypePreset.Caua)
                {
                    AnimatePresetWalk(walk, walkOpposite);
                }

                break;
            case "attack":
                if (_equippedWeaponKind != ImprovisedWeaponKind.None)
                {
                    AnimateWeaponAttack();
                }
                else
                {
                    AnimateUnarmedAttack();
                }

                break;
            case "finisher":
                AnimateFinisherAttack();
                break;
            case "reload":
                AnimateReload();
                break;
            case "parry":
                AnimateParry();
                break;
            case "guard":
                AnimateGuard();
                break;
            case "posture_stagger":
                AnimatePostureStagger();
                break;
            case "parry_success":
                AnimateParrySuccessMatrix();
                break;
            case "dash":
                AnimateDash();
                break;
            case "hurt":
                AnimateHurt();
                break;
            case "telegraph":
                AnimateTelegraph();
                break;
            case "death":
                AnimateDeath();
                break;
            case "parry_stagger":
                AnimateParryStagger();
                break;
            case "shoot":
                AnimateShoot();
                break;
            default:
                AnimateIdle(idle);
                break;
        }
    }

    private void AnimateNoseBleedDrip()
    {
        if (_noseBleedDrip is null || _damageTier < EnemyDamageVisualTier.Hurt)
        {
            return;
        }

        _noseBleedDrip.Visible = true;
        float drip = (_stateTime * 2.6f) % 1f;
        _noseBleedDrip.Position = new Vector2(13f, 11f + drip * 14f);
        _noseBleedDrip.Modulate = new Color(1f, 1f, 1f, 1f - drip * 0.85f);
    }

    private void ResetLayeredPose()
    {
        SetPart(_backArm, new Vector2(-15f, -58f), -0.12f, Vector2.One);
        SetPart(_frontArm, new Vector2(15f, -58f), 0.08f, Vector2.One);
        SetPart(_backLeg, new Vector2(-8f, -12f), 0.04f, Vector2.One);
        SetPart(_frontLeg, new Vector2(8f, -12f), -0.04f, Vector2.One);
        SetPart(_backLegShin, new Vector2(0f, 24f), 0f, Vector2.One);
        SetPart(_frontLegShin, new Vector2(0f, 24f), 0f, Vector2.One);
        SetPart(_backArmForearm, new Vector2(0f, 30f), 0f, Vector2.One);
        SetPart(_frontArmForearm, new Vector2(0f, 30f), 0f, Vector2.One);
        if (_rig is not null)
        {
            _rig.Position = Vector2.Zero;
            _rig.Modulate = Colors.White;
        }

        if (_frontArm is not null)
        {
            _frontArm.Modulate = Colors.White;
        }

        if (_torso is not null)
        {
            _torso.Modulate = Colors.White;
            _torso.Scale = LayeredPreset switch
            {
                LayeredPrototypePreset.Brute => new Vector2(1.18f, 1.12f),
                LayeredPrototypePreset.Fast => new Vector2(0.92f, 0.96f),
                _ => Vector2.One,
            };
        }
    }

    private void AnimateIdle(float idle)
    {
        if (_torso is not null && _combatStyle != CombatStyleKind.Rua)
        {
            _torso.Rotation = _combatStyle switch
            {
                CombatStyleKind.Boxe => -0.06f + idle * 0.02f,
                CombatStyleKind.MuayThai => 0.04f + idle * 0.015f,
                CombatStyleKind.Capoeira => idle * 0.04f,
                _ => idle * 0.02f,
            };
        }

        if (_frontArm is not null && _combatStyle == CombatStyleKind.Boxe)
        {
            _frontArm.Rotation = -0.35f + idle * 0.04f;
            _frontArm.Position = new Vector2(12f, -56f);
        }

        if (_backArm is not null && _combatStyle == CombatStyleKind.Boxe)
        {
            _backArm.Rotation = -0.55f - idle * 0.03f;
            _backArm.Position = new Vector2(-14f, -58f);
        }

        if (_frontArm is not null && _combatStyle == CombatStyleKind.MuayThai)
        {
            _frontArm.Rotation = -0.25f + idle * 0.035f;
        }

        if (_combatStyle == CombatStyleKind.Capoeira && _backLeg is not null && _frontLeg is not null)
        {
            _backLeg.Rotation = idle * 0.08f;
            _frontLeg.Rotation = -idle * 0.06f;
        }

        if (_torso is not null && _isExhausted)
        {
            _torso.Position += new Vector2(0f, idle * 2.4f);
            _torso.Rotation = idle * 0.04f;
        }

        if (_head is not null && _isExhausted)
        {
            _head.Position += new Vector2(0f, idle * 1.4f);
            _head.Rotation += idle * 0.05f;
        }

        if (_frontArm is not null && _combatStyle == CombatStyleKind.Rua)
        {
            _frontArm.Rotation += idle * (_isExhausted ? 0.055f : 0.035f);
            if (_isExhausted)
            {
                _frontArm.Position += new Vector2(0f, idle * 0.8f);
            }
        }

        if (_backArm is not null && _combatStyle == CombatStyleKind.Rua)
        {
            _backArm.Rotation -= idle * (_isExhausted ? 0.04f : 0.025f);
        }

        AnimatePresetIdle(idle);
    }

    private void AnimatePresetIdle(float idle)
    {
        float phase = _stateTime + _idlePersonalityOffset;
        float sway = Mathf.Sin(phase * 3.4f);

        switch (LayeredPreset)
        {
            case LayeredPrototypePreset.Caua:
                AnimateCauaFighterIdle(idle, sway);
                break;
            case LayeredPrototypePreset.QuebraOsso:
                AnimateQuebraOssoIdle(idle, sway);
                break;
            case LayeredPrototypePreset.Fast:
                AnimateFastIdle(phase);
                break;
            case LayeredPrototypePreset.Brute:
                AnimateBruteIdle(idle, sway);
                break;
            case LayeredPrototypePreset.Infected:
                AnimateInfectedIdle(phase, idle);
                break;
            case LayeredPrototypePreset.MiniBoss:
                AnimateMiniBossIdle(idle, sway);
                break;
        }
    }

    private void AnimateCauaFighterIdle(float idle, float sway)
    {
        float breath = idle * 0.5f;
        ApplyFighterStance(1.05f);
        ApplyFighterGuardArm(0.52f + breath * 0.04f);

        if (_torso is not null)
        {
            _torso.Rotation = sway * 0.008f - 0.03f;
            _torso.Position = new Vector2(sway * 0.9f, -54f + breath * 0.8f);
        }

        if (_head is not null)
        {
            _head.Rotation = sway * 0.008f - 0.02f;
            _head.Position = new Vector2(sway * 0.4f, _headAnchorY + breath * 0.4f);
        }
    }

    private void AnimateQuebraOssoIdle(float idle, float sway)
    {
        if (_torso is not null)
        {
            _torso.Rotation = sway * 0.015f;
            _torso.Position = new Vector2(sway * 0.8f, -56f + idle * 0.5f);
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = -0.72f;
            _frontArm.Position = new Vector2(12f, -60f);
        }

        if (_backArm is not null)
        {
            _backArm.Rotation = -1.05f;
            _backArm.Position = new Vector2(-12f, -58f);
        }

        if (_head is not null)
        {
            _head.Rotation = sway * 0.012f;
            _head.Position = new Vector2(0f, _headAnchorY + idle * 0.25f);
        }
    }

    private void AnimateFastIdle(float phase)
    {
        float jitter = Mathf.Sin(phase * 16f) * 0.04f;

        if (_rig is not null)
        {
            _rig.Position = new Vector2(Mathf.Sin(phase * 11f) * 2.8f, Mathf.Abs(Mathf.Sin(phase * 13f)) * 1.2f);
        }

        if (_torso is not null)
        {
            _torso.Rotation = 0.1f + jitter;
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = -0.35f + jitter * 2f;
        }

        if (_frontLeg is not null)
        {
            _frontLeg.Rotation = -0.08f + jitter;
        }
    }

    private void AnimateBruteIdle(float idle, float sway)
    {
        if (_torso is not null)
        {
            _torso.Rotation = sway * 0.035f;
            _torso.Position = new Vector2(0f, -54f + idle * 1.6f);
            _torso.Scale = new Vector2(1.14f + idle * 0.02f, 1.08f + idle * 0.025f);
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = -0.18f;
            _frontArm.Position = new Vector2(18f, -54f);
        }

        if (_backArm is not null)
        {
            _backArm.Rotation = 0.12f;
        }

        if (_frontLeg is not null && _backLeg is not null)
        {
            _frontLeg.Position = new Vector2(12f, -12f);
            _backLeg.Position = new Vector2(-12f, -12f);
        }
    }

    private void AnimateInfectedIdle(float phase, float idle)
    {
        float spasm = Mathf.Sin(phase * 7.2f) * 0.05f;

        if (_torso is not null)
        {
            _torso.Rotation = 0.04f + spasm;
            _torso.Modulate = Colors.White.Lerp(new Color(0.78f, 1f, 0.72f), 0.12f + Mathf.Abs(spasm) * 2f);
        }

        if (_head is not null)
        {
            _head.Rotation = 0.08f + spasm * 1.4f;
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = -0.42f + spasm * 2f;
        }
    }

    private void AnimateMiniBossIdle(float idle, float sway)
    {
        if (_torso is not null)
        {
            _torso.Rotation = -0.04f + sway * 0.03f;
            _torso.Position = new Vector2(0f, -58f + idle * 1.2f);
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = -0.35f - sway * 0.08f;
            _frontArm.Position = new Vector2(20f, -56f);
        }

        if (_backArm is not null)
        {
            _backArm.Rotation = 0.22f + sway * 0.06f;
        }

        if (_head is not null)
        {
            _head.Rotation = -0.05f + sway * 0.025f;
        }
    }

    private void AnimateWalk(float walk, float walkOpposite, float limpFactor)
    {
        float stride = (_isRunning ? 1.05f : 0.74f) * limpFactor;
        float lift = _isRunning ? 14f : 10f;
        float bob = _isRunning ? 5.2f : 3.8f;
        float guard = _isRunning ? 0.28f : 0.46f;

        if (_rig is not null)
        {
            _rig.Position = new Vector2(walkOpposite * (_isRunning ? 7f : 4f), Mathf.Abs(walk) * bob * 0.42f);
        }

        if (_torso is not null)
        {
            float runBoost = _isRunning ? 1.25f : _isExhausted ? 0.72f : 1f;
            _torso.Position += new Vector2(walkOpposite * 3f, -2f + Mathf.Abs(walk) * bob * runBoost * 0.35f);
            _torso.Rotation = walk * (_isRunning ? 0.05f : 0.028f) - (_isRunning ? 0.04f : 0.05f);
            if (_isRunning)
            {
                _torso.Position += new Vector2(6f, -3f);
                _torso.Rotation += 0.08f;
            }
        }

        if (_frontLeg is not null)
        {
            _frontLeg.Rotation = walk * stride - 0.06f;
            _frontLeg.Position = new Vector2(12f + walk * 9f, -10f - Mathf.Max(0f, walk) * lift);
        }

        if (_frontLegShin is not null)
        {
            _frontLegShin.Rotation = Mathf.Clamp(-walk * 1.45f + 0.12f, -0.1f, 1.65f);
        }

        if (_backLeg is not null)
        {
            _backLeg.Rotation = -walk * stride * 0.92f + 0.05f;
            _backLeg.Position = new Vector2(-12f - walk * 8f, -11f - Mathf.Max(0f, -walk) * lift);
        }

        if (_backLegShin is not null)
        {
            _backLegShin.Rotation = Mathf.Clamp(walk * 1.35f - 0.1f, -0.1f, 1.5f);
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = -walk * 0.5f - guard * 0.38f;
            _frontArm.Position = new Vector2(14f - walk * 5f, -54f - guard * 2f + Mathf.Abs(walk) * 1.4f);
        }

        if (_frontArmForearm is not null)
        {
            _frontArmForearm.Rotation = -Mathf.Abs(walk) * 0.52f - guard * 0.22f;
        }

        if (_backArm is not null)
        {
            _backArm.Rotation = walk * 0.48f - 0.78f - guard * 0.15f;
            _backArm.Position = new Vector2(-13f + walk * 5f, -56f - guard * 2f);
        }

        if (_backArmForearm is not null)
        {
            _backArmForearm.Rotation = -Mathf.Abs(walkOpposite) * 0.46f - guard * 0.18f;
        }

        if (_head is not null)
        {
            _head.Rotation += walk * 0.012f - 0.02f;
        }

        SetCaseiraKnifeVisible(false);
    }

    private void SetCaseiraKnifeVisible(bool visible)
    {
        if (_caseiraKnife is not null)
        {
            _caseiraKnife.Visible = visible;
        }

        if (_knifeBlood is not null)
        {
            _knifeBlood.Visible = visible;
        }
    }

    private void AnimateWeaponAttack()
    {
        switch (_equippedWeaponKind)
        {
            case ImprovisedWeaponKind.Hammer:
                AnimateHammerSwing();
                break;
            case ImprovisedWeaponKind.Knife:
                AnimateKnifeSlash();
                break;
            default:
                AnimateRebarSwing();
                break;
        }
    }

    private void AnimateRebarSwing()
    {
        float duration = 0.16f;
        float t = Mathf.Clamp(_stateTime / duration, 0f, 1f);
        float swing = Mathf.Sin(t * Mathf.Pi);

        if (_torso is not null)
        {
            _torso.Rotation = swing * 0.18f;
            _torso.Position += new Vector2(swing * 8f, 0f);
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = -0.2f - swing * 1.15f;
            _frontArm.Position = new Vector2(14f + swing * 10f, -58f);
        }

        if (_improvisedWeapon is not null)
        {
            _improvisedWeapon.Rotation = -swing * 0.35f;
        }

        TrySpawnImpact(0, swing, _frontArm);
    }

    private void AnimateHammerSwing()
    {
        float duration = 0.2f;
        float t = Mathf.Clamp(_stateTime / duration, 0f, 1f);
        float raise = t < 0.35f ? t / 0.35f : 1f - ((t - 0.35f) / 0.65f);
        float smash = t >= 0.35f ? Mathf.Sin(((t - 0.35f) / 0.65f) * Mathf.Pi) : 0f;

        if (_torso is not null)
        {
            _torso.Rotation = -raise * 0.22f + smash * 0.28f;
            _torso.Position += new Vector2(smash * 6f, raise * -4f + smash * 3f);
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = -1.1f * raise + smash * 0.95f;
            _frontArm.Position = new Vector2(12f, -58f - raise * 8f + smash * 4f);
        }

        if (_improvisedWeapon is not null)
        {
            _improvisedWeapon.Rotation = -raise * 0.4f + smash * 0.5f;
        }

        TrySpawnImpact(1, smash, _frontArm);
    }

    private void AnimateKnifeSlash()
    {
        (float windup, float strike, float recover) = SampleMartialPhase(0.28f, 0.16f);
        SetCaseiraKnifeVisible(true);
        ApplySifuLunge(windup, strike, recover, 14f, 2f);

        if (_torso is not null)
        {
            _torso.Rotation = -windup * 0.1f + strike * 0.28f - recover * 0.08f;
            _torso.Position = new Vector2(-windup * 8f + strike * 16f, -54f - strike * 3f);
        }

        ApplySifuHipTorque(windup, strike, recover, 1f, 0.3f);

        if (_frontArm is not null)
        {
            _frontArm.Rotation = 0.2f - windup * 0.55f - strike * 1.45f + recover * 0.2f;
            _frontArm.Position = new Vector2(10f + strike * 20f, -54f - strike * 4f);
        }

        if (_frontArmForearm is not null)
        {
            _frontArmForearm.Rotation = -windup * 0.2f - strike * 1.1f + recover * 0.15f;
        }

        if (_backArm is not null)
        {
            _backArm.Rotation = -0.5f + windup * 0.25f - strike * 0.15f;
        }

        TrySpawnImpact(1, strike, _frontArmForearm ?? _frontArm);
    }

    private void AnimatePipeOverhead()
    {
        (float windup, float strike, float recover) = SampleMartialPhase(0.38f, 0.16f);
        ApplySifuLunge(windup, strike, recover, 10f, 6f);

        if (_torso is not null)
        {
            _torso.Rotation = -windup * 0.22f + strike * 0.18f - recover * 0.06f;
            _torso.Position = new Vector2(-windup * 6f, -54f - windup * 8f + strike * 4f);
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = -0.1f - windup * 1.25f + strike * 0.95f + recover * 0.15f;
            _frontArm.Position = new Vector2(12f, -54f - windup * 16f + strike * 8f);
        }

        if (_backLeg is not null)
        {
            _backLeg.Rotation = windup * 0.1f + strike * 0.2f;
        }

        TrySpawnImpact(0, strike, _frontArm);
    }

    private void AnimateCleaverChop()
    {
        (float windup, float strike, float recover) = SampleMartialPhase(0.36f, 0.17f);
        ApplySifuLunge(windup, strike, recover, 16f, 4f);

        if (_torso is not null)
        {
            _torso.Rotation = windup * 0.1f + strike * 0.3f - recover * 0.08f;
            _torso.Position = new Vector2(-windup * 8f + strike * 16f, -54f + windup * 2f);
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = 0.05f - windup * 0.85f - strike * 1.15f + recover * 0.12f;
            _frontArm.Position = new Vector2(14f + strike * 18f, -54f - strike * 4f);
        }

        TrySpawnImpact(0, strike, _frontArmForearm ?? _frontArm);
    }

    private void AnimateHeavyPunch()
    {
        (float windup, float strike, float recover) = SampleMartialPhase(0.40f, 0.18f);
        ApplySifuLunge(windup, strike, recover, 22f, 3f);

        if (_torso is not null)
        {
            _torso.Rotation = -windup * 0.1f + strike * 0.24f - recover * 0.08f;
            _torso.Position = new Vector2(-windup * 14f + strike * 28f, -54f + strike * 3f);
        }

        ApplySifuHipTorque(windup, strike, recover, 1f, 0.42f);

        if (_frontArm is not null)
        {
            _frontArm.Rotation = 0.05f - windup * 0.55f - strike * 1.35f + recover * 0.15f;
            _frontArm.Position = new Vector2(14f + strike * 30f, -54f - strike * 4f);
        }

        if (_frontArmForearm is not null)
        {
            _frontArmForearm.Rotation = -windup * 0.3f - strike * 0.5f;
        }

        TrySpawnImpact(0, strike, _frontArmForearm ?? _frontArm);
    }

    private void AnimateBatonStrike()
    {
        (float windup, float strike, float recover) = SampleMartialPhase(0.34f, 0.15f);
        ApplySifuLunge(windup, strike, recover, 14f, 2f);

        if (_torso is not null)
        {
            _torso.Rotation = -windup * 0.08f + strike * 0.16f;
            _torso.Position = new Vector2(strike * 12f, -56f);
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = -0.28f - windup * 0.65f - strike * 1.05f + recover * 0.2f;
            _frontArm.Position = new Vector2(12f + strike * 16f, -56f);
        }

        if (_sidearm is not null && strike > 0.35f)
        {
            _sidearm.Visible = true;
        }

        TrySpawnImpact(0, strike, _frontArm);
    }

    private void AnimateFinisherAttack()
    {
        float duration = 0.58f;
        float t = Mathf.Clamp(_stateTime / duration, 0f, 1f);
        float windup = t < 0.35f ? t / 0.35f : 0f;
        float strike = t >= 0.35f ? Mathf.Sin(((t - 0.35f) / 0.65f) * Mathf.Pi) : 0f;

        if (_torso is not null)
        {
            _torso.Rotation = _equippedWeaponKind == ImprovisedWeaponKind.Hammer
                ? -windup * 0.28f + strike * 0.34f
                : windup * -0.12f + strike * 0.22f;
            _torso.Position += new Vector2(strike * 14f, _equippedWeaponKind == ImprovisedWeaponKind.Rebar ? strike * 6f : -strike * 2f);
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = _equippedWeaponKind switch
            {
                ImprovisedWeaponKind.Hammer => -1.25f * windup + strike * 1.05f,
                ImprovisedWeaponKind.Knife => 0.55f - strike * 1.55f,
                _ => -0.35f - strike * 1.25f,
            };
            _frontArm.Position = new Vector2(14f + strike * 18f, -58f);
        }

        if (_frontLeg is not null && _equippedWeaponKind == ImprovisedWeaponKind.Rebar)
        {
            _frontLeg.Rotation = -strike * 0.85f;
            _frontLeg.Position = new Vector2(8f + strike * 16f, -12f - strike * 8f);
        }

        if (_improvisedWeapon is not null)
        {
            _improvisedWeapon.Rotation = strike * 0.45f;
        }

        TrySpawnImpact(2, strike, _equippedWeaponKind == ImprovisedWeaponKind.Rebar ? _frontLeg : _frontArm);
    }

    private void AnimateReload()
    {
        float t = 1f - Mathf.Clamp(_reloadAnimTime / 1.25f, 0f, 1f);
        float phase = t < 0.45f ? t / 0.45f : 1f - ((t - 0.45f) / 0.55f);

        if (_sidearm is not null)
        {
            _sidearm.Visible = true;
        }

        if (_torso is not null)
        {
            _torso.Rotation = -phase * 0.06f;
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = -0.45f * phase;
            _frontArm.Position = new Vector2(12f - phase * 4f, -58f);
        }

        if (_backArm is not null)
        {
            _backArm.Rotation = -0.75f * phase;
        }

        if (_sidearm is not null)
        {
            _sidearm.Rotation = 0.25f + phase * 0.35f;
        }
    }

    private void AnimatePostureStagger()
    {
        float wobble = Mathf.Sin(_stateTime * 11f) * 0.06f;

        if (_torso is not null)
        {
            _torso.Rotation = wobble;
            _torso.Position = new Vector2(0f, -50f + wobble * 8f);
            _torso.Modulate = new Color(0.82f, 0.72f, 0.95f);
        }

        if (_head is not null)
        {
            _head.Rotation = wobble * 1.4f;
            _head.Position = new Vector2(wobble * 6f, _headAnchorY + 4f);
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = 0.45f;
        }

        if (_backArm is not null)
        {
            _backArm.Rotation = -0.35f;
        }

        if (_frontLeg is not null)
        {
            _frontLeg.Rotation = -0.22f;
        }

        if (_backLeg is not null)
        {
            _backLeg.Rotation = 0.18f;
        }
    }

    private void AnimateGuard()
    {
        float brace = 0.5f + Mathf.Sin(_stateTime * 9f) * 0.08f;

        if (_torso is not null)
        {
            _torso.Rotation = -0.06f;
            _torso.Position = new Vector2(0f, -54f);
            _torso.Modulate = Colors.White.Lerp(new Color(0.72f, 0.82f, 1f), brace * 0.25f);
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = -0.95f;
            _frontArm.Position = new Vector2(10f, -54f);
        }

        if (_frontArmForearm is not null)
        {
            _frontArmForearm.Rotation = -0.35f;
        }

        if (_backArm is not null)
        {
            _backArm.Rotation = -1.05f;
            _backArm.Position = new Vector2(-12f, -56f);
        }

        if (_backArmForearm is not null)
        {
            _backArmForearm.Rotation = -0.42f;
        }

        if (_frontLeg is not null)
        {
            _frontLeg.Rotation = -0.12f;
        }

        if (_backLeg is not null)
        {
            _backLeg.Rotation = 0.18f;
        }

        if (_head is not null)
        {
            _head.Rotation = -0.04f;
            _head.Position = new Vector2(0f, _headAnchorY + 1f);
        }
    }

    private void AnimateParry()
    {
        float t = Mathf.Clamp(_stateTime / 0.36f, 0f, 1f);
        float guard = Mathf.Sin(t * Mathf.Pi);

        if (_torso is not null)
        {
            _torso.Rotation = -guard * 0.08f;
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = -0.55f - guard * 0.35f;
            _frontArm.Position = new Vector2(10f, -58f);
        }

        if (_backArm is not null)
        {
            _backArm.Rotation = -0.75f - guard * 0.2f;
        }
    }

    private void AnimateParrySuccessMatrix()
    {
        float t = Mathf.Clamp(_stateTime / 0.22f, 0f, 1f);
        float dodge = Mathf.Sin(t * Mathf.Pi);
        float leanSign = _facingSign;

        if (_torso is not null)
        {
            _torso.Rotation = -0.42f * dodge * leanSign;
            _torso.Position += new Vector2(-10f * dodge * leanSign, -5f * dodge);
            _torso.Modulate = Colors.White.Lerp(new Color(0.55f, 0.82f, 1f), dodge * 0.55f);
        }

        if (_head is not null)
        {
            _head.Rotation = -0.32f * dodge * leanSign;
            _head.Position += new Vector2(-8f * dodge * leanSign, -8f * dodge);
        }

        if (_frontLeg is not null)
        {
            _frontLeg.Rotation = 0.22f * dodge;
            _frontLeg.Position += new Vector2(-3f * dodge * leanSign, 2f * dodge);
        }

        if (_backLeg is not null)
        {
            _backLeg.Rotation = -0.18f * dodge;
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = -1.05f * dodge;
            _frontArm.Position = new Vector2(6f - 4f * dodge * leanSign, -54f - 6f * dodge);
        }

        if (_backArm is not null)
        {
            _backArm.Rotation = -0.95f * dodge;
        }
    }

    private void AnimateUnarmedAttack()
    {
        if (LayeredPreset != LayeredPrototypePreset.Caua)
        {
            switch (LayeredPreset)
            {
                case LayeredPrototypePreset.QuebraOsso:
                case LayeredPrototypePreset.Fast:
                    if (_activeMoveAnim is MoveAnimProfile.Teep or MoveAnimProfile.LowKick or MoveAnimProfile.SideKick)
                    {
                        AnimatePipeOverhead();
                    }
                    else
                    {
                        AnimatePipeOverhead();
                    }

                    return;
                case LayeredPrototypePreset.Brute:
                    AnimateHeavyPunch();
                    return;
                case LayeredPrototypePreset.Infected:
                    AnimateCleaverChop();
                    return;
                case LayeredPrototypePreset.MiniBoss:
                    if (_activeMoveAnim == MoveAnimProfile.Cross)
                    {
                        AnimateBatonStrike();
                    }
                    else
                    {
                        AnimateBatonStrike();
                    }

                    return;
            }
        }

        bool playerStyled = LayeredPreset == LayeredPrototypePreset.Caua;
        SetCaseiraKnifeVisible(_activeMoveAnim == MoveAnimProfile.KnifeSlash);

        switch (_activeMoveAnim)
        {
            case MoveAnimProfile.KnifeSlash:
                AnimateKnifeSlash();
                break;
            case MoveAnimProfile.HighKick:
                AnimateHighKick();
                break;
            case MoveAnimProfile.LowKick:
                AnimateLowKick();
                break;
            case MoveAnimProfile.BoxLead:
                AnimateBoxLead();
                break;
            case MoveAnimProfile.Cross:
                AnimateCross();
                break;
            case MoveAnimProfile.Hook:
                AnimateHook();
                break;
            case MoveAnimProfile.Uppercut:
                AnimateUppercut();
                break;
            case MoveAnimProfile.RunningHook:
                AnimateRunningHook();
                break;
            case MoveAnimProfile.Teep:
                AnimateTeep(_isRunning);
                break;
            case MoveAnimProfile.Knee:
            case MoveAnimProfile.FlyingKnee:
                AnimateKnee(_activeMoveAnim == MoveAnimProfile.FlyingKnee);
                break;
            case MoveAnimProfile.Elbow:
                AnimateElbow();
                break;
            case MoveAnimProfile.MeiaLua:
                AnimateMeiaLua();
                break;
            case MoveAnimProfile.GingaKick:
                AnimateGingaKick();
                break;
            case MoveAnimProfile.RunningPunch:
                AnimateRunningPunch();
                break;
            case MoveAnimProfile.RunningKick:
                AnimateRunningKick();
                break;
            case MoveAnimProfile.SideKick:
                if (LayeredPreset == LayeredPrototypePreset.Caua)
                {
                    AnimateSideKick();
                }
                else
                {
                    AnimateHeadbutt();
                }
                break;
            case MoveAnimProfile.FlyingKick:
                AnimateFlyingKick();
                break;
            default:
                AnimateJab();
                break;
        }

        if (playerStyled && _combatStyle != CombatStyleKind.Rua)
        {
            ApplyStyleAttackTint();
        }
    }

    private void ApplyStyleAttackTint()
    {
        if (_torso is null || _combatStyle == CombatStyleKind.Rua)
        {
            return;
        }

        Color tint = _combatStyle switch
        {
            CombatStyleKind.Boxe => new Color(1f, 0.72f, 0.55f),
            CombatStyleKind.MuayThai => new Color(1f, 0.92f, 0.62f),
            CombatStyleKind.Capoeira => new Color(0.72f, 1f, 0.72f),
            CombatStyleKind.Karate => new Color(0.88f, 0.88f, 1f),
            _ => Colors.White,
        };

        float pulse = 0.65f + Mathf.Sin(_stateTime * 24f) * 0.35f;
        _torso.Modulate = Colors.White.Lerp(tint, pulse * 0.45f);
        if (_frontArm is not null)
        {
            _frontArm.Modulate = Colors.White.Lerp(tint, pulse * 0.35f);
        }
    }

    private void AnimateBoxLead()
    {
        (float windup, float strike, float recover) = SampleMartialPhase(0.36f, 0.22f);

        if (_torso is not null)
        {
            _torso.Rotation = -0.08f + strike * 0.08f - recover * 0.04f;
            _torso.Position = new Vector2(-windup * 8f + strike * 18f, -56f - strike * 2f);
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = -0.42f - windup * 0.35f - strike * 1.45f + recover * 0.3f;
            _frontArm.Position = new Vector2(14f + strike * 32f, -56f - strike * 5f);
        }

        ApplyFighterGuardArm(0.45f + strike * 0.2f, includeFrontArm: false);

        if (_head is not null)
        {
            _head.Rotation = -strike * 0.06f;
        }

        TrySpawnImpact(0, strike, _frontArm);
    }

    private void AnimateGingaKick()
    {
        float duration = 0.26f;
        float t = Mathf.Clamp(_stateTime / duration, 0f, 1f);
        float swing = Mathf.Sin(t * Mathf.Pi);

        if (_torso is not null)
        {
            _torso.Rotation = -swing * 0.32f;
            _torso.Position += new Vector2(swing * 8f, -swing * 8f);
        }

        if (_frontLeg is not null)
        {
            _frontLeg.Rotation = -swing * 1.15f;
            _frontLeg.Position = new Vector2(6f + swing * 20f, -10f - swing * 6f);
        }

        if (_backLeg is not null)
        {
            _backLeg.Rotation = swing * 0.55f;
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = 0.65f + swing * 0.45f;
        }

        TrySpawnImpact(1, swing, _frontLeg);
    }

    private void AnimateCross()
    {
        (float windup, float strike, float recover) = SampleMartialPhase(0.34f, 0.15f);
        SetCaseiraKnifeVisible(false);
        ApplySifuLunge(windup, strike, recover, 26f, 4f);

        if (_torso is not null)
        {
            _torso.Rotation = windup * 0.18f - strike * 0.38f - recover * 0.1f;
            _torso.Position = new Vector2(-windup * 14f + strike * 30f, -54f - strike * 7f + recover * 3f);
        }

        ApplySifuHipTorque(windup, strike, recover, 1f, 0.38f);

        if (_backArm is not null)
        {
            _backArm.Rotation = -0.08f - windup * 0.65f - strike * 1.9f + recover * 0.45f;
            _backArm.Position = new Vector2(-13f + strike * 42f, -56f - strike * 10f);
        }

        if (_backArmForearm is not null)
        {
            _backArmForearm.Rotation = -windup * 0.35f - strike * 1.05f + recover * 0.12f;
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = 0.52f - strike * 0.55f + recover * 0.15f;
            _frontArm.Position = new Vector2(14f - strike * 4f, -54f);
        }

        if (_backLeg is not null)
        {
            _backLeg.Rotation = windup * 0.08f + strike * 0.42f;
            if (_backLegShin is not null)
            {
                _backLegShin.Rotation = strike * 0.25f;
            }
        }

        if (_frontLeg is not null)
        {
            _frontLeg.Rotation = -windup * 0.1f - strike * 0.22f;
        }

        if (_head is not null)
        {
            _head.Position = new Vector2(strike * 8f, _headAnchorY - strike * 4f);
            _head.Rotation = -strike * 0.12f;
        }

        TrySpawnImpact(0, strike, _backArmForearm ?? _backArm);
    }

    private void AnimateHook()
    {
        (float windup, float strike, float recover) = SampleMartialPhase(0.36f, 0.16f);
        SetCaseiraKnifeVisible(false);
        ApplySifuLunge(windup, strike, recover, 14f, 3f);

        if (_torso is not null)
        {
            _torso.Rotation = -windup * 0.2f + strike * 0.32f - recover * 0.08f;
            _torso.Position = new Vector2(-windup * 6f + strike * 12f, -54f - strike * 5f);
        }

        ApplySifuHipTorque(windup, strike, recover, -1f, 0.35f);

        if (_frontArm is not null)
        {
            _frontArm.Rotation = 0.65f + windup * 0.65f - strike * 1.25f + recover * 0.4f;
            _frontArm.Position = new Vector2(6f + strike * 20f, -52f - strike * 12f);
        }

        if (_frontArmForearm is not null)
        {
            _frontArmForearm.Rotation = windup * 0.35f - strike * 0.55f;
        }

        if (_head is not null)
        {
            _head.Rotation = windup * 0.06f + strike * 0.14f - recover * 0.05f;
            _head.Position = new Vector2(strike * 4f, _headAnchorY - strike * 2f);
        }

        ApplyFighterGuardArm(0.38f, includeFrontArm: false);

        TrySpawnImpact(2, strike, _frontArmForearm ?? _frontArm);
    }

    private void AnimateUppercut()
    {
        (float windup, float strike, float recover) = SampleMartialPhase(0.32f, 0.16f);
        SetCaseiraKnifeVisible(false);
        ApplySifuLunge(windup, strike, recover, 10f, 8f);

        if (_torso is not null)
        {
            _torso.Rotation = windup * 0.14f - strike * 0.28f + recover * 0.06f;
            _torso.Position = new Vector2(-windup * 8f + strike * 10f, -52f - strike * 14f + recover * 6f);
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = 0.28f + windup * 0.7f - strike * 1.85f + recover * 0.35f;
            _frontArm.Position = new Vector2(6f + strike * 12f, -54f - strike * 22f);
        }

        if (_frontArmForearm is not null)
        {
            _frontArmForearm.Rotation = -windup * 0.42f - strike * 0.65f;
        }

        if (_backLeg is not null)
        {
            _backLeg.Rotation = windup * 0.12f + strike * 0.25f;
            if (_backLegShin is not null)
            {
                _backLegShin.Rotation = -windup * 0.15f + strike * 0.2f;
            }
        }

        if (_frontLeg is not null)
        {
            _frontLeg.Rotation = -windup * 0.18f - strike * 0.08f;
        }

        if (_head is not null)
        {
            _head.Position = new Vector2(strike * 3f, _headAnchorY - strike * 8f);
            _head.Rotation = -strike * 0.08f;
        }

        TrySpawnImpact(3, strike, _frontArmForearm ?? _frontArm);
    }

    private void AnimateRunningHook()
    {
        float duration = 0.17f;
        float t = Mathf.Clamp(_stateTime / duration, 0f, 1f);
        float extend = Mathf.Sin(t * Mathf.Pi);

        if (_torso is not null)
        {
            _torso.Rotation = 0.08f + extend * 0.22f;
            _torso.Position += new Vector2(extend * 14f, -extend * 4f);
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = 0.65f - extend * 1.25f;
            _frontArm.Position = new Vector2(10f + extend * 18f, -52f - extend * 10f);
        }

        TrySpawnImpact(1, extend, _frontArm);
    }

    private void AnimateTeep(bool running)
    {
        float duration = running ? 0.15f : 0.26f;
        float t = Mathf.Clamp(_stateTime / duration, 0f, 1f);
        float windup = t < 0.28f ? t / 0.28f : 0f;
        float extend = t >= 0.28f ? Mathf.Sin(((t - 0.28f) / 0.72f) * Mathf.Pi) : 0f;

        if (_torso is not null)
        {
            _torso.Rotation = -windup * 0.08f - extend * 0.12f;
            _torso.Position += new Vector2(-windup * 8f + extend * (running ? 16f : 10f), -extend * 3f);
        }

        if (_backLeg is not null)
        {
            _backLeg.Rotation = windup * 0.18f + extend * 0.08f;
            _backLegShin!.Rotation = extend * 0.12f;
        }

        if (_frontLeg is not null)
        {
            _frontLeg.Rotation = windup * 0.35f - extend * 0.85f;
            _frontLeg.Position = new Vector2(8f - windup * 6f + extend * (running ? 26f : 22f), -12f - extend * 8f);
        }

        if (_frontLegShin is not null)
        {
            _frontLegShin.Rotation = windup * 0.55f - extend * 0.35f;
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = 0.35f + windup * 0.25f;
        }

        if (_backArm is not null)
        {
            _backArm.Rotation = -0.45f - windup * 0.2f;
        }

        TrySpawnImpact(1, extend, _frontLegShin ?? _frontLeg);
    }

    private void AnimateKnee(bool aerial)
    {
        (float windup, float chamber, float strike, float recover) = SampleSifuKickPhase(0.24f, 0.28f, 0.14f);
        ApplySifuLunge(windup, strike, recover, aerial ? 18f : 12f, aerial ? 12f : 8f);

        if (_torso is not null)
        {
            _torso.Rotation = -windup * 0.08f - chamber * 0.06f - strike * 0.18f;
            _torso.Position = new Vector2(-windup * 6f + strike * 8f, -54f - chamber * 6f - strike * (aerial ? 16f : 10f));
        }

        if (_frontLeg is not null)
        {
            _frontLeg.Rotation = windup * 0.15f + chamber * 0.95f - strike * 1.45f + recover * 0.15f;
            _frontLeg.Position = new Vector2(6f + chamber * 4f + strike * 10f, -10f - chamber * 16f - strike * 14f);
        }

        if (_frontLegShin is not null)
        {
            _frontLegShin.Rotation = -chamber * 0.5f - strike * 0.35f;
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = -0.38f - chamber * 0.25f - strike * 0.35f;
        }

        ApplyFighterGuardArm(0.5f, includeFrontArm: false);
        TrySpawnImpact(2, strike, _frontLeg);
    }

    private void AnimateElbow()
    {
        (float windup, float strike, float recover) = SampleMartialPhase(0.28f, 0.14f);
        ApplySifuLunge(windup, strike, recover, 10f, 3f);

        if (_torso is not null)
        {
            _torso.Rotation = windup * 0.12f + strike * 0.22f - recover * 0.06f;
            _torso.Position = new Vector2(strike * 8f, -52f + strike * 6f);
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = -1.05f + windup * 0.35f + strike * 0.65f - recover * 0.2f;
            _frontArm.Position = new Vector2(12f, -48f + strike * 10f);
        }

        TrySpawnImpact(2, strike, _frontArmForearm ?? _frontArm);
    }

    private void AnimateMeiaLua()
    {
        float duration = 0.22f;
        float t = Mathf.Clamp(_stateTime / duration, 0f, 1f);
        float arc = Mathf.Sin(t * Mathf.Pi);

        if (_torso is not null)
        {
            _torso.Rotation = -arc * 0.28f;
            _torso.Position += new Vector2(arc * 10f, -arc * 6f);
        }

        if (_backLeg is not null)
        {
            _backLeg.Rotation = arc * 1.45f;
            _backLeg.Position = new Vector2(-6f + arc * 18f, -14f - arc * 8f);
        }

        if (_frontLeg is not null)
        {
            _frontLeg.Rotation = -arc * 0.35f;
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = 0.45f + arc * 0.35f;
        }

        TrySpawnImpact(2, arc, _backLeg);
    }

    private void AnimateRunningPunch()
    {
        float duration = 0.11f;
        float t = Mathf.Clamp(_stateTime / duration, 0f, 1f);
        float extend = Mathf.Sin(t * Mathf.Pi);

        if (_torso is not null)
        {
            _torso.Rotation = 0.12f + extend * 0.1f;
            _torso.Position += new Vector2(extend * 10f, 0f);
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = -0.05f - extend * 1.05f;
            _frontArm.Position = new Vector2(18f + extend * 26f, -58f);
        }

        TrySpawnImpact(0, extend, _frontArm);
    }

    private void AnimateRunningKick()
    {
        float duration = 0.13f;
        float t = Mathf.Clamp(_stateTime / duration, 0f, 1f);
        float extend = Mathf.Sin(t * Mathf.Pi);

        if (_torso is not null)
        {
            _torso.Rotation = -extend * 0.08f;
            _torso.Position += new Vector2(extend * 8f, -extend * 2f);
        }

        if (_frontLeg is not null)
        {
            _frontLeg.Rotation = -extend * 1.1f;
            _frontLeg.Position = new Vector2(10f + extend * 22f, -12f);
        }

        TrySpawnImpact(1, extend, _frontLeg);
    }

    private void AnimateShoot()
    {
        float t = Mathf.Clamp(_stateTime / 0.22f, 0f, 1f);
        bool drawing = t < 0.35f;
        float fire = drawing ? 0f : Mathf.Sin((t - 0.35f) / 0.65f * Mathf.Pi);

        if (_sidearm is not null)
        {
            _sidearm.Visible = true;
        }

        if (_torso is not null)
        {
            _torso.Rotation = drawing ? -0.04f : fire * 0.06f;
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = drawing ? -0.35f : -0.15f + fire * 0.08f;
            _frontArm.Position = new Vector2(15f + (drawing ? -4f : 2f), -58f);
        }

        if (_backArm is not null)
        {
            _backArm.Rotation = drawing ? -0.55f : -0.25f;
        }

        if (_sidearm is not null)
        {
            _sidearm.Rotation = drawing ? 0.4f : fire * 0.05f;
        }

        if (!drawing && fire > 0.7f && !_impactSpawned)
        {
            _impactSpawned = true;
        }

        if (t >= 0.99f && _sidearm is not null)
        {
            _sidearm.Visible = false;
        }
    }

    private void AnimateJab()
    {
        (float windup, float strike, float recover) = SampleMartialPhase(0.30f, 0.14f);
        SetCaseiraKnifeVisible(false);
        ApplySifuLunge(windup, strike, recover, 20f, 2f);

        if (_torso is not null)
        {
            _torso.Rotation = -windup * 0.12f + strike * 0.14f - recover * 0.05f;
            _torso.Position = new Vector2(-windup * 12f + strike * 24f, -54f - strike * 4f + recover * 2f);
        }

        ApplySifuHipTorque(windup, strike, recover, -1f, 0.22f);

        if (_frontArm is not null)
        {
            _frontArm.Rotation = 0.15f - windup * 0.95f - strike * 1.72f + recover * 0.4f;
            _frontArm.Position = new Vector2(9f - windup * 14f + strike * 40f, -55f - strike * 6f);
        }

        if (_frontArmForearm is not null)
        {
            _frontArmForearm.Rotation = -windup * 0.45f - strike * 1.08f + recover * 0.15f;
        }

        ApplyFighterGuardArm(0.42f + strike * 0.2f, includeFrontArm: false);

        if (_backLeg is not null)
        {
            _backLeg.Rotation = windup * 0.06f + strike * 0.35f;
            if (_backLegShin is not null)
            {
                _backLegShin.Rotation = strike * 0.28f - recover * 0.1f;
            }
        }

        if (_frontLeg is not null)
        {
            _frontLeg.Rotation = -windup * 0.08f - strike * 0.18f;
            if (_frontLegShin is not null)
            {
                _frontLegShin.Rotation = windup * 0.1f;
            }
        }

        if (_head is not null)
        {
            _head.Position = new Vector2(strike * 6f - recover * 2f, _headAnchorY - strike * 3f);
            _head.Rotation = -windup * 0.05f - strike * 0.1f + recover * 0.04f;
        }

        TrySpawnImpact(0, strike, _frontArmForearm ?? _frontArm);
    }

    private void AnimateHeadbutt()
    {
        (float windup, float strike, float recover) = SampleMartialPhase(0.48f, 0.28f);

        if (_torso is not null)
        {
            _torso.Rotation = windup * 0.15f + strike * 0.32f - recover * 0.1f;
            _torso.Position = new Vector2(-windup * 10f + strike * 18f, -56f + strike * 4f);
        }

        if (_head is not null)
        {
            _head.Position = new Vector2(-windup * 4f + strike * 22f, _headAnchorY + strike * 6f);
            _head.Rotation = windup * 0.08f + strike * 0.18f;
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = 0.35f + windup * 0.15f;
        }

        TrySpawnImpact(0, strike, _head);
    }

    private void AnimateSideKick()
    {
        (float windup, float chamber, float strike, float recover) = SampleSifuKickPhase(0.24f, 0.26f, 0.14f);
        SetCaseiraKnifeVisible(false);
        ApplySifuLunge(windup + chamber * 0.4f, strike, recover, 28f, 2f);

        if (_torso is not null)
        {
            _torso.Rotation = windup * 0.08f - chamber * 0.05f - strike * 0.28f;
            _torso.Position = new Vector2(-windup * 10f + strike * 16f, -54f - chamber * 4f - strike * 5f);
        }

        if (_frontLeg is not null)
        {
            float legRot = windup * 0.2f + chamber * 0.85f - strike * 1.35f + recover * 0.2f;
            _frontLeg.Rotation = legRot;
            _frontLeg.Position = new Vector2(
                8f - windup * 8f + chamber * 5f + strike * 46f,
                -10f - chamber * 15f - strike * 12f + recover * 4f);
        }

        if (_frontLegShin is not null)
        {
            _frontLegShin.Rotation = -chamber * 0.65f - strike * 0.78f + recover * 0.15f;
        }

        if (_backLeg is not null)
        {
            _backLeg.Rotation = 0.02f + strike * 0.18f;
            if (_backLegShin is not null)
            {
                _backLegShin.Rotation = strike * 0.15f;
            }
        }

        ApplyFighterGuardArm(0.58f + strike * 0.12f, includeFrontArm: false);

        if (_frontArm is not null)
        {
            _frontArm.Rotation = 0.3f + chamber * 0.15f + strike * 0.22f;
        }

        TrySpawnImpact(1, strike, _frontLeg);
    }

    private void AnimateHighKick()
    {
        (float windup, float chamber, float strike, float recover) = SampleSifuKickPhase(0.26f, 0.28f, 0.13f);
        SetCaseiraKnifeVisible(false);
        ApplySifuLunge(windup, strike, recover, 12f, 10f);

        if (_torso is not null)
        {
            _torso.Rotation = -windup * 0.12f - chamber * 0.08f - strike * 0.22f;
            _torso.Position = new Vector2(-windup * 8f + strike * 8f, -54f - chamber * 8f - strike * 16f);
        }

        if (_frontLeg is not null)
        {
            _frontLeg.Rotation = windup * 0.15f + chamber * 1.1f - strike * 1.65f + recover * 0.18f;
            _frontLeg.Position = new Vector2(6f + chamber * 6f + strike * 20f, -10f - chamber * 20f - strike * 24f);
        }

        if (_frontLegShin is not null)
        {
            _frontLegShin.Rotation = -chamber * 0.4f - strike * 0.65f;
        }

        if (_backLeg is not null)
        {
            _backLeg.Rotation = 0.06f + strike * 0.25f;
        }

        ApplyFighterGuardArm(0.68f);
        TrySpawnImpact(2, strike, _frontLeg);
    }

    private void AnimateLowKick()
    {
        (float windup, float chamber, float strike, float recover) = SampleSifuKickPhase(0.22f, 0.20f, 0.14f);
        SetCaseiraKnifeVisible(false);
        ApplySifuLunge(windup, strike, recover, 16f, -2f);

        if (_torso is not null)
        {
            _torso.Rotation = windup * 0.16f - strike * 0.14f;
            _torso.Position = new Vector2(-windup * 10f + strike * 14f, -52f + chamber * 3f + strike * 5f);
        }

        if (_frontLeg is not null)
        {
            _frontLeg.Rotation = windup * 0.25f + chamber * 0.35f - strike * 0.95f + recover * 0.12f;
            _frontLeg.Position = new Vector2(10f + strike * 28f, -8f + chamber * 4f + strike * 8f);
        }

        if (_frontLegShin is not null)
        {
            _frontLegShin.Rotation = chamber * 0.2f + strike * 0.55f;
        }

        if (_backLeg is not null)
        {
            _backLeg.Rotation = -windup * 0.14f;
        }

        TrySpawnImpact(3, strike, _frontLeg);
    }

    private void AnimateFlyingKick()
    {
        float duration = 0.34f;
        float t = Mathf.Clamp(_stateTime / duration, 0f, 1f);
        float windup = t < 0.28f ? t / 0.28f : 0f;
        float extend = t >= 0.28f ? Mathf.Sin(((t - 0.28f) / 0.72f) * Mathf.Pi) : 0f;
        float lift = extend * 18f;

        if (_torso is not null)
        {
            _torso.Rotation = -windup * 0.1f + extend * 0.2f;
            _torso.Position += new Vector2(-windup * 3f + extend * 12f, -lift);
        }

        if (_head is not null)
        {
            _head.Position += new Vector2(extend * 4f, -lift * 0.35f);
        }

        if (_frontLeg is not null)
        {
            _frontLeg.Rotation = -extend * 1.05f;
            _frontLeg.Position = new Vector2(12f + extend * 24f, -12f - lift * 0.45f);
        }

        if (_backLeg is not null)
        {
            _backLeg.Rotation = extend * 0.35f;
            _backLeg.Position += new Vector2(-extend * 4f, -lift * 0.2f);
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = 0.55f;
        }

        if (_backArm is not null)
        {
            _backArm.Rotation = -0.62f;
        }

        if (_hair is not null)
        {
            _hair.Rotation -= extend * 0.14f;
        }

        TrySpawnImpact(2, extend, _frontLeg);
    }

    private void TrySpawnImpact(int comboIndex, float extend, Node2D? strikePart)
    {
        if (extend < 0.55f || _impactSpawned || strikePart is null)
        {
            return;
        }

        _impactSpawned = true;
        SpawnStrikeImpact(comboIndex, strikePart);
    }

    private void AnimateHurt()
    {
        float maxHurt = 0.48f + _hurtStrength * 0.07f;
        float t = 1f - Mathf.Clamp(_hurtTime / maxHurt, 0f, 1f);
        float snap = EaseOutExpo(Mathf.Clamp(t * 1.4f, 0f, 1f));
        float settle = t < 0.55f ? 0f : EaseOutCubic((t - 0.55f) / 0.45f);
        float recoil = snap * (1f - settle * 0.6f);
        float lean = _hurtDirection.X * recoil * 0.38f * _hurtStrength;
        float buckle = recoil * _hurtStrength;

        if (_rig is not null)
        {
            _rig.Position = new Vector2(_hurtDirection.X * recoil * 8f * _hurtStrength, buckle * 2f);
        }

        if (_torso is not null)
        {
            _torso.Rotation = lean;
            _torso.Position += new Vector2(_hurtDirection.X * recoil * 14f * _hurtStrength, buckle * 5f);
            _torso.Modulate = Colors.White.Lerp(new Color(1f, 0.55f, 0.5f), recoil * 0.5f);
        }

        if (_head is not null)
        {
            _head.Rotation = -lean * 1.25f - recoil * 0.08f;
            _head.Position += new Vector2(_hurtDirection.X * recoil * 12f, recoil * 4f);
        }

        if (_painGrimace is not null)
        {
            _painGrimace.Visible = true;
            _painGrimace.Scale = Vector2.One * (0.85f + recoil * 0.35f);
        }

        if (_hair is not null)
        {
            _hair.Rotation += _hurtDirection.X * recoil * 0.28f;
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = 0.55f * recoil;
            _frontArm.Position += new Vector2(_hurtDirection.X * recoil * 4f, 0f);
        }

        if (_backArm is not null)
        {
            _backArm.Rotation = -0.72f * recoil;
        }

        if (_frontLeg is not null)
        {
            _frontLeg.Rotation = -0.28f * buckle;
            _frontLeg.Position += new Vector2(0f, buckle * 4f);
        }

        if (_backLeg is not null)
        {
            _backLeg.Rotation = 0.18f * buckle;
        }
    }

    private void AnimateTelegraph()
    {
        switch (LayeredPreset)
        {
            case LayeredPrototypePreset.Fast:
                AnimateFastTelegraph();
                return;
            case LayeredPrototypePreset.Brute:
                AnimateBruteTelegraph();
                return;
            case LayeredPrototypePreset.Infected:
                AnimateInfectedTelegraph();
                return;
            case LayeredPrototypePreset.MiniBoss:
                AnimateMiniBossTelegraph();
                return;
            case LayeredPrototypePreset.QuebraOsso:
                AnimateQuebraOssoTelegraph();
                return;
        }

        AnimateDefaultTelegraph();
    }

    private void AnimateDefaultTelegraph()
    {
        float pulse = 0.5f + Mathf.Sin(_stateTime * 28f) * 0.5f;
        float warn = 0.35f + Mathf.Sin(_stateTime * 18f) * 0.35f;
        bool headbutt = LayeredPreset != LayeredPrototypePreset.Caua && _attackComboIndex == 1;

        if (_torso is not null)
        {
            _torso.Rotation = (-0.18f - (headbutt ? 0.1f : 0f)) * pulse;
            _torso.Position += new Vector2(-8f * pulse, headbutt ? 3f * pulse : 0f);
            _torso.Modulate = Colors.White.Lerp(new Color(1f, 0.22f, 0.12f), warn * 0.65f);
            _torso.Scale = new Vector2(1f + warn * 0.04f, 1f + warn * 0.06f);
        }

        if (_head is not null)
        {
            _head.Position += new Vector2((headbutt ? 5f : -4f) * pulse, headbutt ? -4f * pulse : -1f * warn);
            _head.Rotation = headbutt ? pulse * 0.22f : -warn * 0.08f;
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = headbutt ? 0.35f : -1.05f * pulse;
            _frontArm.Position = new Vector2(18f - 10f * pulse, -58f - 2f * warn);
        }

        if (_frontLeg is not null)
        {
            _frontLeg.Rotation = 0.14f * pulse;
        }
    }

    private void AnimateQuebraOssoTelegraph()
    {
        float pulse = 0.5f + Mathf.Sin(_stateTime * 24f) * 0.5f;
        float warn = 0.35f + Mathf.Sin(_stateTime * 16f) * 0.35f;
        bool headbutt = _attackComboIndex == 1;

        if (_torso is not null)
        {
            _torso.Rotation = (0.08f + (headbutt ? 0.22f : -0.12f)) * pulse;
            _torso.Modulate = Colors.White.Lerp(new Color(1f, 0.28f, 0.1f), warn * 0.7f);
        }

        if (_head is not null)
        {
            _head.Rotation = headbutt ? pulse * 0.35f : -0.12f * pulse;
            _head.Position = new Vector2(2f + (headbutt ? 8f : -4f) * pulse, _headAnchorY + (headbutt ? -6f : 2f) * pulse);
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = headbutt ? 0.55f : -0.95f * pulse;
        }
    }

    private void AnimateFastTelegraph()
    {
        float pulse = 0.5f + Mathf.Sin(_stateTime * 34f) * 0.5f;

        if (_rig is not null)
        {
            _rig.Position = new Vector2(-10f * pulse, -2f * pulse);
        }

        if (_torso is not null)
        {
            _torso.Rotation = -0.28f * pulse;
            _torso.Modulate = Colors.White.Lerp(new Color(0.72f, 1f, 0.35f), pulse * 0.45f);
        }

        if (_frontLeg is not null)
        {
            _frontLeg.Rotation = -0.55f * pulse;
            _frontLeg.Position = new Vector2(8f + 6f * pulse, -12f - 4f * pulse);
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = -1.15f * pulse;
        }
    }

    private void AnimateBruteTelegraph()
    {
        float charge = 0.35f + Mathf.Sin(_stateTime * 12f) * 0.35f;

        if (_torso is not null)
        {
            _torso.Rotation = -0.22f * charge;
            _torso.Position = new Vector2(-14f * charge, -52f + charge * 4f);
            _torso.Modulate = Colors.White.Lerp(new Color(1f, 0.45f, 0.12f), charge * 0.55f);
            _torso.Scale = new Vector2(1.14f + charge * 0.06f, 1.08f + charge * 0.08f);
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = -0.85f * charge;
        }

        if (_head is not null)
        {
            _head.Rotation = -0.08f * charge;
        }
    }

    private void AnimateInfectedTelegraph()
    {
        float spasm = 0.4f + Mathf.Sin(_stateTime * 22f) * 0.4f;

        if (_torso is not null)
        {
            _torso.Rotation = spasm * 0.18f;
            _torso.Modulate = Colors.White.Lerp(new Color(0.55f, 1f, 0.35f), spasm * 0.65f);
        }

        if (_head is not null)
        {
            _head.Rotation = spasm * 0.25f;
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = -0.75f * spasm;
            _frontArm.Position = new Vector2(14f + spasm * 8f, -56f);
        }
    }

    private void AnimateMiniBossTelegraph()
    {
        float pulse = 0.45f + Mathf.Sin(_stateTime * 14f) * 0.45f;

        if (_torso is not null)
        {
            _torso.Rotation = -0.15f * pulse;
            _torso.Position = new Vector2(-10f * pulse, -58f);
            _torso.Modulate = Colors.White.Lerp(new Color(1f, 0.35f, 0.08f), pulse * 0.6f);
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = -1.05f * pulse;
            _frontArm.Position = new Vector2(22f - 8f * pulse, -58f);
        }

        if (_backArm is not null)
        {
            _backArm.Rotation = 0.35f * pulse;
        }
    }

    private void AnimateParryStagger()
    {
        float t = 1f - Mathf.Clamp(_stateLockRemaining / 0.58f, 0f, 1f);
        float stagger = Mathf.Sin(t * Mathf.Pi);

        if (_torso is not null)
        {
            _torso.Rotation = -0.38f * stagger;
            _torso.Position = new Vector2(-12f * stagger, -52f + stagger * 6f);
            _torso.Modulate = Colors.White.Lerp(new Color(0.62f, 0.82f, 1f), stagger * 0.65f);
        }

        if (_head is not null)
        {
            _head.Rotation = -0.28f * stagger;
            _head.Position = new Vector2(-8f * stagger, _headAnchorY + 4f * stagger);
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = 0.65f * stagger;
        }

        if (_backArm is not null)
        {
            _backArm.Rotation = -0.55f * stagger;
        }

        if (_frontLeg is not null)
        {
            _frontLeg.Rotation = -0.32f * stagger;
        }
    }

    private void AnimateDeath()
    {
        float t = Mathf.Clamp(_stateTime / CombatPacing.DeathBodySeconds, 0f, 1f);
        float collapse = Mathf.SmoothStep(0f, 1f, Mathf.Min(t * 1.35f, 1f));
        float alpha = 1f - Mathf.Clamp((t - 0.72f) / 0.28f, 0f, 1f);

        if (_rig is not null)
        {
            _rig.Rotation = collapse * 1.35f;
            _rig.Position = new Vector2(collapse * 22f, collapse * 52f);
            _rig.Modulate = new Color(0.88f, 0.72f, 0.72f, alpha);
        }

        if (_torso is not null)
        {
            _torso.Rotation = collapse * 0.55f;
            _torso.Position = new Vector2(0f, -56f + collapse * 34f);
        }

        if (_head is not null)
        {
            _head.Rotation = collapse * 0.85f;
            _head.Position = new Vector2(collapse * 8f, _headAnchorY + collapse * 22f);
        }

        if (_frontLeg is not null)
        {
            _frontLeg.Rotation = collapse * 1.05f;
        }

        if (_backLeg is not null)
        {
            _backLeg.Rotation = -collapse * 0.72f;
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = collapse * 0.55f;
        }
    }

    private (float windup, float strike, float recover) SampleMartialPhase(float windupPortion = 0.32f, float strikePortion = 0.18f)
    {
        float dur = Mathf.Max(_activeMoveDuration, 0.2f);
        float t = Mathf.Clamp(_stateTime / dur, 0f, 1f);
        float windupEnd = windupPortion;
        float strikeEnd = windupPortion + strikePortion;
        float recoverSpan = Mathf.Max(1f - strikeEnd, 0.01f);

        float windupRaw = t < windupEnd ? t / Mathf.Max(windupEnd, 0.001f) : 1f;
        float windup = EaseInCubic(windupRaw);

        float strike = 0f;
        if (t >= windupEnd && t <= strikeEnd)
        {
            float st = (t - windupEnd) / Mathf.Max(strikePortion, 0.001f);
            strike = EaseOutExpo(st);
        }

        float recover = t > strikeEnd ? EaseOutCubic((t - strikeEnd) / recoverSpan) : 0f;
        return (windup, strike, recover);
    }

    private (float windup, float chamber, float strike, float recover) SampleSifuKickPhase(
        float windupPortion = 0.26f,
        float chamberPortion = 0.24f,
        float strikePortion = 0.14f)
    {
        float dur = Mathf.Max(_activeMoveDuration, 0.22f);
        float t = Mathf.Clamp(_stateTime / dur, 0f, 1f);
        float wEnd = windupPortion;
        float cEnd = windupPortion + chamberPortion;
        float sEnd = cEnd + strikePortion;
        float recoverSpan = Mathf.Max(1f - sEnd, 0.01f);

        float windup = t < wEnd ? EaseInCubic(t / Mathf.Max(wEnd, 0.001f)) : 1f;
        float chamber = t >= wEnd && t < cEnd
            ? EaseInOutQuad((t - wEnd) / Mathf.Max(chamberPortion, 0.001f))
            : t >= cEnd ? 1f : 0f;
        float strike = t >= cEnd && t <= sEnd
            ? EaseOutExpo((t - cEnd) / Mathf.Max(strikePortion, 0.001f))
            : 0f;
        float recover = t > sEnd ? EaseOutCubic((t - sEnd) / recoverSpan) : 0f;
        return (windup, chamber, strike, recover);
    }

    private static float EaseInCubic(float x) => x * x * x;

    private static float EaseOutCubic(float x)
    {
        float inv = 1f - x;
        return 1f - inv * inv * inv;
    }

    private static float EaseInOutQuad(float x) =>
        x < 0.5f ? 2f * x * x : 1f - Mathf.Pow(-2f * x + 2f, 2f) / 2f;

    private static float EaseOutExpo(float x) =>
        x >= 1f ? 1f : 1f - Mathf.Pow(2f, -10f * x);

    private void ApplySifuLunge(float windup, float strike, float recover, float forward = 16f, float dip = 3f)
    {
        if (_rig is null)
        {
            return;
        }

        float x = -windup * 5f + strike * forward - recover * 8f;
        float y = windup * 1.5f - strike * dip + recover * 2f;
        _rig.Position = new Vector2(x, y);
    }

    private void ApplySifuHipTorque(float windup, float strike, float recover, float sign, float power = 0.32f)
    {
        if (_torso is null)
        {
            return;
        }

        _torso.Rotation += sign * (-windup * power * 0.55f + strike * power - recover * power * 0.35f);
    }

    private void ApplyFighterStance(float depth = 1f)
    {
        if (_frontLeg is not null)
        {
            _frontLeg.Rotation = -0.16f * depth;
            _frontLeg.Position = new Vector2(13f, -10f + depth * 3f);
        }

        if (_frontLegShin is not null)
        {
            _frontLegShin.Rotation = 0.22f * depth;
        }

        if (_backLeg is not null)
        {
            _backLeg.Rotation = 0.12f * depth;
            _backLeg.Position = new Vector2(-13f, -11f + depth * 2f);
        }

        if (_backLegShin is not null)
        {
            _backLegShin.Rotation = -0.18f * depth;
        }
    }

    private void ApplyFighterGuardArm(float guardLevel, bool includeFrontArm = true)
    {
        if (_backArm is null)
        {
            return;
        }

        _backArm.Rotation = -0.76f - guardLevel * 0.28f;
        _backArm.Position = new Vector2(-13f - guardLevel * 6f, -57f - guardLevel * 3f);
        if (_backArmForearm is not null)
        {
            _backArmForearm.Rotation = -0.34f - guardLevel * 0.34f;
        }

        if (!includeFrontArm)
        {
            return;
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = -0.32f - guardLevel * 0.16f;
            _frontArm.Position = new Vector2(15f + guardLevel * 3f, -54f - guardLevel * 2f);
        }

        if (_frontArmForearm is not null)
        {
            _frontArmForearm.Rotation = -0.22f - guardLevel * 0.22f;
        }
    }

    private void AnimatePresetWalk(float walk, float walkOpposite)
    {
        switch (LayeredPreset)
        {
            case LayeredPrototypePreset.QuebraOsso:
                if (_torso is not null)
                {
                    _torso.Rotation = 0.1f + walk * 0.05f;
                }

                if (_head is not null)
                {
                    _head.Rotation = 0.06f + walkOpposite * 0.04f;
                }

                break;
            case LayeredPrototypePreset.Fast:
                if (_rig is not null)
                {
                    _rig.Position += new Vector2(walkOpposite * 4f, 0f);
                }

                if (_torso is not null)
                {
                    _torso.Rotation = 0.14f + walk * 0.08f;
                }

                if (_frontLeg is not null)
                {
                    _frontLeg.Rotation = walk * 0.85f;
                }

                if (_backLeg is not null)
                {
                    _backLeg.Rotation = -walk * 0.75f;
                }

                break;
            case LayeredPrototypePreset.Brute:
                if (_torso is not null)
                {
                    _torso.Rotation = walk * 0.045f;
                    _torso.Position += new Vector2(walkOpposite * 3f, Mathf.Abs(walk) * 2f);
                }

                if (_frontLeg is not null && _backLeg is not null)
                {
                    _frontLeg.Rotation = walk * 0.42f;
                    _backLeg.Rotation = -walk * 0.38f;
                }

                break;
            case LayeredPrototypePreset.Infected:
                if (_torso is not null)
                {
                    _torso.Rotation = 0.05f + Mathf.Sin(_stateTime * 8f) * 0.04f;
                }

                if (_frontArm is not null)
                {
                    _frontArm.Rotation = -0.35f + walk * 0.12f;
                }

                break;
            case LayeredPrototypePreset.MiniBoss:
                if (_torso is not null)
                {
                    _torso.Rotation = -0.03f + walk * 0.035f;
                }

                if (_frontArm is not null)
                {
                    _frontArm.Rotation = -0.28f - walk * 0.06f;
                }

                break;
        }
    }

    private void SpawnStrikeImpact(int comboIndex, Node2D strikePart)
    {
        Node? parent = GetParent()?.GetParent();
        if (parent is null)
        {
            return;
        }

        Color burstColor = comboIndex switch
        {
            2 => new Color(1f, 0.55f, 0.22f, 0.85f),
            1 => new Color(1f, 0.82f, 0.38f, 0.78f),
            _ => new Color(1f, 0.92f, 0.62f, 0.72f),
        };

        Color arcColor = comboIndex switch
        {
            2 => new Color(1f, 0.42f, 0.12f, 0.55f),
            1 => new Color(1f, 0.72f, 0.28f, 0.48f),
            _ => new Color(1f, 0.88f, 0.52f, 0.42f),
        };

        float size = comboIndex switch
        {
            2 => 1.35f,
            1 => 1.15f,
            _ => 1f,
        };

        Vector2 impactPos = strikePart.GlobalPosition + new Vector2(_facingSign * 14f, 0f);

        Polygon2D arc = new()
        {
            Color = arcColor,
            Polygon =
            [
                new Vector2(-4f, -28f),
                new Vector2(38f * _facingSign, -18f),
                new Vector2(42f * _facingSign, -8f),
                new Vector2(8f * _facingSign, -2f),
                new Vector2(-6f, -12f),
            ],
            ZIndex = 11,
        };
        parent.AddChild(arc);
        arc.GlobalPosition = impactPos;
        Tween arcTween = arc.CreateTween();
        arcTween.TweenProperty(arc, "modulate:a", 0f, 0.12f);
        arcTween.TweenCallback(Callable.From(arc.QueueFree));

        Polygon2D burst = new()
        {
            Color = burstColor,
            Polygon =
            [
                new Vector2(-8f, -8f),
                new Vector2(8f, -8f),
                new Vector2(10f, 0f),
                new Vector2(8f, 8f),
                new Vector2(-8f, 8f),
                new Vector2(-10f, 0f),
            ],
            Scale = Vector2.One * size,
            ZIndex = 12,
        };

        parent.AddChild(burst);
        burst.GlobalPosition = impactPos;

        Tween tween = burst.CreateTween();
        tween.TweenProperty(burst, "scale", burst.Scale * 1.8f, 0.05f);
        tween.Parallel().TweenProperty(burst, "modulate:a", 0f, 0.09f);
        tween.TweenCallback(Callable.From(burst.QueueFree));
    }

    private void AnimateDash()
    {
        if (_torso is not null)
        {
            _torso.Rotation = 0.18f;
            _torso.Position += new Vector2(7f, 2f);
        }

        if (_head is not null)
        {
            _head.Position += new Vector2(5f, 1f);
        }

        if (_hair is not null)
        {
            _hair.Rotation = -0.16f;
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = -0.38f;
        }

        if (_backArm is not null)
        {
            _backArm.Rotation = 0.38f;
        }

        if (_frontLeg is not null)
        {
            _frontLeg.Rotation = 0.32f;
        }

        if (_backLeg is not null)
        {
            _backLeg.Rotation = -0.32f;
        }
    }

    private static void SetPart(Node2D? node, Vector2 position, float rotation, Vector2 scale)
    {
        if (node is null)
        {
            return;
        }

        node.Position = position;
        node.Rotation = rotation;
        node.Scale = scale;
        node.Modulate = Colors.White;
    }
}
