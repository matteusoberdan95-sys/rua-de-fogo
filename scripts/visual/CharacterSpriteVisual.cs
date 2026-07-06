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
public partial class CharacterSpriteVisual : Node2D
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

        if (_flashTime > 0f)
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
                _rig.Scale = new Vector2(_facingSign, 1f);
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
    }

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

            _movementOffset = isMoving && !isAttacking && !isDashing && !isHurt && !isTelegraphing && _shootAnimTime <= 0f
                ? new Vector2(0f, Mathf.Sin(Time.GetTicksMsec() * bobSpeed) * bobAmount)
                : Vector2.Zero;
            ApplyVisualOffset();

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
                return;
            }

            string nextState = isHurt || _hurtTime > 0f ? "hurt"
                : isPostureStaggered || _layeredState == "posture_stagger" ? "posture_stagger"
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

    public void SetAttackMove(MoveAnimProfile anim, int impactComboIndex, CombatStyleKind style)
    {
        _combatStyle = style;
        _activeMoveAnim = anim;
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
        _rig = new Node2D { Name = $"Layered{LayeredPreset}Rig", YSortEnabled = true };
        AddChild(_rig);
        _rig.Scale = palette.RigScale;

        Color skin = palette.Skin;
        Color backSkin = skin.Darkened(0.08f);
        Color pants = palette.Pants;
        Color backPants = pants.Darkened(0.12f);
        Color shirt = palette.Shirt;
        Color vest = palette.Vest;
        Color shoeAccent = palette.ShoeAccent;

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
        AddSoftPolygon(_shirtPulse, "TorsoCloth", shirt, MakeTorsoSilhouette(), 0);
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
            _clothSway = AddJoint(_torso, "ScarfSway", new Vector2(8f, -14f), 5);
            AddPolygon(_clothSway, "Scarf", new Color(0.58f, 0.02f, 0.03f), new[]
            {
                new Vector2(-4f, -2f), new Vector2(16f, -4f), new Vector2(22f, 8f), new Vector2(6f, 14f), new Vector2(-2f, 6f)
            }, 0);
            AddPolygon(_torso, "ShoulderPatch", new Color(0.11f, 0.09f, 0.075f), new[]
            {
                new Vector2(-19f, -16f), new Vector2(-8f, -20f), new Vector2(-4f, -8f), new Vector2(-16f, -4f)
            }, 5);
        }

        _frontLeg = AddLegRig(_rig, "FrontLeg", new Vector2(8f, -12f), pants, shoeAccent, z: 6, out _frontLegShin);
        _headAnchorY = palette.HeadYOffset;
        _head = AddJoint(_rig, "Head", new Vector2(0f, _headAnchorY), 8);
        AddPolygon(_head, "Neck", skin.Darkened(0.12f), new[]
        {
            new Vector2(-7f, 10f), new Vector2(8f, 10f), new Vector2(7f, 23f), new Vector2(-8f, 22f)
        }, 0);
        AddPolygon(_head, "Face", skin, MakeEllipse(0f, 0f, palette.FaceRx, palette.FaceRy, 12), 2);
        AddPolygon(_head, "JawShadow", skin.Darkened(0.18f), new[]
        {
            new Vector2(-10f, 6f), new Vector2(11f, 5f), new Vector2(9f, 14f), new Vector2(-8f, 15f)
        }, 3);
        AddPolygon(_head, "Brow", new Color(0.075f, 0.045f, 0.035f), new[]
        {
            new Vector2(-9f, -4f), new Vector2(10f, -7f), new Vector2(14f, -3f), new Vector2(-8f, 1f)
        }, 4);
        AddPolygon(_head, "Nose", new Color(0.72f, 0.45f, 0.24f), new[]
        {
            new Vector2(8f, -1f), new Vector2(17f, 3f), new Vector2(7f, 7f)
        }, 5);
        AddPolygon(_head, "EarL", skin.Darkened(0.15f), MakeEllipse(-14f, 2f, 4f, 6f, 6), 1);
        AddPolygon(_head, "EarR", skin.Darkened(0.15f), MakeEllipse(14f, 1f, 4f, 6f, 6), 1);
        AddPolygon(_head, "Mouth", new Color(0.28f, 0.08f, 0.07f), new[]
        {
            new Vector2(-6f, 10f), new Vector2(6f, 10f), new Vector2(4f, 14f), new Vector2(-4f, 14f)
        }, 6);
        AddEnemyEyes(_head, palette);
        _blackEye = AddPolygon(_head, "BlackEye", new Color(0.32f, 0.1f, 0.48f, 0.92f), new[]
        {
            new Vector2(-10f, -3f), new Vector2(-2f, -5f), new Vector2(-1f, 2f), new Vector2(-11f, 4f)
        }, 9);
        _blackEye.Visible = false;
        _noseBleed = AddPolygon(_head, "NoseBleed", new Color(0.48f, 0.01f, 0.02f, 0.88f), new[]
        {
            new Vector2(10f, 4f), new Vector2(14f, 3f), new Vector2(13f, 9f), new Vector2(9f, 10f)
        }, 9);
        _noseBleed.Visible = false;
        _noseBleedDrip = AddPolygon(_head, "NoseBleedDrip", new Color(0.55f, 0.01f, 0.02f, 0.75f), new[]
        {
            new Vector2(11f, 10f), new Vector2(13f, 10f), new Vector2(12f, 16f), new Vector2(10f, 16f)
        }, 10);
        _noseBleedDrip.Visible = false;
        _bloodSmearFace = AddPolygon(_head, "BloodSmearFace", new Color(0.42f, 0.01f, 0.02f, 0.72f), new[]
        {
            new Vector2(4f, 6f), new Vector2(16f, 4f), new Vector2(18f, 12f), new Vector2(6f, 14f)
        }, 9);
        _bloodSmearFace.Visible = false;
        _shirtTear = AddPolygon(_torso, "ShirtTear", new Color(0.04f, 0.04f, 0.045f, 0.85f), new[]
        {
            new Vector2(-6f, 2f), new Vector2(8f, -2f), new Vector2(12f, 14f), new Vector2(-2f, 18f), new Vector2(-10f, 10f)
        }, 7);
        _shirtTear.Visible = false;
        _faceBruise = AddPolygon(_head, "FaceBruise", new Color(0.38f, 0.08f, 0.12f, 0.88f), new[]
        {
            new Vector2(-12f, -2f), new Vector2(-2f, -4f), new Vector2(0f, 4f), new Vector2(-10f, 6f)
        }, 7);
        _faceBruise.Visible = false;
        _painGrimace = AddPolygon(_head, "PainGrimace", new Color(0.42f, 0.06f, 0.08f, 0.92f), new[]
        {
            new Vector2(-7f, 8f), new Vector2(7f, 8f), new Vector2(5f, 12f), new Vector2(0f, 10f), new Vector2(-5f, 12f)
        }, 8);
        _painGrimace.Visible = false;
        _hair = AddJoint(_head, "Hair", new Vector2(-1f, -12f), 6);
        AddPolygon(_hair, "HairSpikes", palette.HairColor, palette.HairShape, 0);
        _hairTail = AddJoint(_hair, "HairTail", new Vector2(6f, 6f), 1);
        AddPolygon(_hairTail, "TailStrand", palette.HairTailColor, new[]
        {
            new Vector2(-3f, 0f), new Vector2(8f, -2f), new Vector2(12f, 10f), new Vector2(0f, 12f)
        }, 0);

        if (LayeredPreset == LayeredPrototypePreset.Caua)
        {
            AddPolygon(_hair, "HairStrandA", new Color(0.045f, 0.032f, 0.026f), new[]
            {
                new Vector2(-12f, -2f), new Vector2(-6f, -16f), new Vector2(-2f, -4f)
            }, 1);
            AddPolygon(_hair, "HairStrandB", new Color(0.04f, 0.028f, 0.022f), new[]
            {
                new Vector2(6f, -1f), new Vector2(12f, -15f), new Vector2(8f, -3f)
            }, 1);
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
                HairShape =
                [
                    new Vector2(-15f, 4f), new Vector2(-10f, -14f), new Vector2(-4f, -5f), new Vector2(1f, -19f),
                    new Vector2(5f, -5f), new Vector2(14f, -13f), new Vector2(10f, 4f)
                ],
                RigScale = Vector2.One,
                HeadYOffset = -96f,
                FaceRx = 13f,
                FaceRy = 17f,
                ShowKnuckles = true,
                CorruptedEyes = false,
            },
            LayeredPrototypePreset.Fast => new PresetPalette
            {
                Skin = new Color(0.42f, 0.24f, 0.16f),
                Pants = new Color(0.04f, 0.038f, 0.042f),
                Shirt = new Color(0.22f, 0.018f, 0.03f),
                Vest = new Color(0.08f, 0.02f, 0.025f),
                ShoeAccent = new Color(0.14f, 0.12f, 0.1f),
                ChestMark = new Color(0.55f, 0.04f, 0.05f, 0.55f),
                HemColor = new Color(0.18f, 0.01f, 0.02f),
                HairColor = new Color(0.025f, 0.018f, 0.015f),
                HairTailColor = new Color(0.03f, 0.022f, 0.018f),
                EyeGlow = new Color(0.75f, 1f, 0.18f),
                HairShape = MakeEllipse(0f, -4f, 11f, 7f, 10),
                RigScale = new Vector2(0.84f, 0.84f),
                HeadYOffset = -88f,
                FaceRx = 11f,
                FaceRy = 14f,
                ShowKnuckles = true,
                CorruptedEyes = true,
            },
            LayeredPrototypePreset.Brute => new PresetPalette
            {
                Skin = new Color(0.48f, 0.27f, 0.17f),
                Pants = new Color(0.12f, 0.055f, 0.08f),
                Shirt = new Color(0.31f, 0.025f, 0.035f),
                Vest = new Color(0.08f, 0.04f, 0.045f),
                ShoeAccent = new Color(0.2f, 0.16f, 0.13f),
                ChestMark = new Color(0.62f, 0.05f, 0.04f, 0.7f),
                HemColor = new Color(0.22f, 0.012f, 0.015f),
                HairColor = new Color(0.02f, 0.015f, 0.012f),
                HairTailColor = new Color(0.025f, 0.018f, 0.015f),
                EyeGlow = new Color(1f, 0.58f, 0.08f),
                HairShape = MakeEllipse(0f, -2f, 14f, 9f, 10),
                RigScale = new Vector2(1.16f, 1.16f),
                HeadYOffset = -100f,
                FaceRx = 15f,
                FaceRy = 18f,
                ShowKnuckles = true,
                CorruptedEyes = true,
            },
            LayeredPrototypePreset.Infected => new PresetPalette
            {
                Skin = new Color(0.38f, 0.46f, 0.28f),
                Pants = new Color(0.08f, 0.12f, 0.06f),
                Shirt = new Color(0.06f, 0.14f, 0.05f),
                Vest = new Color(0.05f, 0.1f, 0.04f),
                ShoeAccent = new Color(0.12f, 0.18f, 0.08f),
                ChestMark = new Color(0.42f, 0.72f, 0.18f, 0.55f),
                HemColor = new Color(0.1f, 0.22f, 0.06f),
                HairColor = new Color(0.04f, 0.06f, 0.03f),
                HairTailColor = new Color(0.05f, 0.08f, 0.04f),
                EyeGlow = new Color(0.55f, 1f, 0.35f),
                HairShape = MakeEllipse(0f, -3f, 12f, 8f, 10),
                RigScale = new Vector2(1.04f, 1.04f),
                HeadYOffset = -94f,
                FaceRx = 12f,
                FaceRy = 16f,
                ShowKnuckles = true,
                CorruptedEyes = true,
            },
            LayeredPrototypePreset.MiniBoss => new PresetPalette
            {
                Skin = new Color(0.44f, 0.26f, 0.17f),
                Pants = new Color(0.055f, 0.045f, 0.05f),
                Shirt = new Color(0.28f, 0.015f, 0.026f),
                Vest = new Color(0.1f, 0.02f, 0.03f),
                ShoeAccent = new Color(0.22f, 0.14f, 0.1f),
                ChestMark = new Color(0.78f, 0.09f, 0.035f, 0.86f),
                HemColor = new Color(0.2f, 0.01f, 0.015f),
                HairColor = new Color(0.02f, 0.012f, 0.01f),
                HairTailColor = new Color(0.025f, 0.015f, 0.012f),
                EyeGlow = new Color(1f, 0.72f, 0.08f),
                HairShape = MakeEllipse(0f, -2f, 13f, 9f, 10),
                RigScale = new Vector2(1.22f, 1.22f),
                HeadYOffset = -102f,
                FaceRx = 14f,
                FaceRy = 18f,
                ShowKnuckles = true,
                CorruptedEyes = true,
            },
            _ => new PresetPalette
            {
                Skin = new Color(0.50f, 0.29f, 0.17f),
                Pants = new Color(0.17f, 0.085f, 0.28f),
                Shirt = new Color(0.025f, 0.025f, 0.027f),
                Vest = new Color(0.055f, 0.06f, 0.055f),
                ShoeAccent = new Color(0.18f, 0.16f, 0.13f),
                ChestMark = new Color(0.82f, 0.78f, 0.66f, 0.7f),
                HemColor = new Color(0.14f, 0.012f, 0.018f),
                HairColor = new Color(0.03f, 0.02f, 0.018f),
                HairTailColor = new Color(0.04f, 0.03f, 0.025f),
                EyeGlow = new Color(1f, 0.82f, 0.12f),
                HairShape = MakeEllipse(0f, -3f, 12f, 8f, 10),
                RigScale = Vector2.One,
                HeadYOffset = -96f,
                FaceRx = 12f,
                FaceRy = 16f,
                ShowKnuckles = true,
                CorruptedEyes = true,
            },
        };
    }

    private void AddEnemyEyes(Node2D head, PresetPalette palette)
    {
        if (!palette.CorruptedEyes)
        {
            AddPolygon(head, "EyeL", new Color(0.92f, 0.9f, 0.82f), MakeEllipse(-6f, 1f, 3f, 4f, 6), 6);
            AddPolygon(head, "EyeR", new Color(0.92f, 0.9f, 0.82f), MakeEllipse(6f, 0f, 3f, 4f, 6), 6);
            AddPolygon(head, "PupilL", new Color(0.08f, 0.05f, 0.04f), MakeEllipse(-6f, 2f, 1.4f, 2f, 6), 7);
            AddPolygon(head, "PupilR", new Color(0.08f, 0.05f, 0.04f), MakeEllipse(6f, 1f, 1.4f, 2f, 6), 7);
            return;
        }

        AddPolygon(head, "EyeGlowL", palette.EyeGlow, MakeEllipse(-7f, 0f, 4f, 5f, 6), 6);
        AddPolygon(head, "EyeGlowR", palette.EyeGlow, MakeEllipse(7f, -1f, 4f, 5f, 6), 7);
        AddPolygon(head, "EyeCoreL", palette.EyeGlow.Lightened(0.35f), MakeEllipse(-7f, 0f, 1.8f, 2.2f, 6), 8);
        AddPolygon(head, "EyeCoreR", palette.EyeGlow.Lightened(0.35f), MakeEllipse(7f, -1f, 1.8f, 2.2f, 6), 8);
    }

    private void AddPresetAccents(PresetPalette palette)
    {
        _variantAccentRoot?.QueueFree();
        _variantAccentRoot = AddJoint(_rig!, "VariantAccents", Vector2.Zero, 12);

        switch (LayeredPreset)
        {
            case LayeredPrototypePreset.QuebraOsso:
                AddPolygon(_torso!, "CorruptionVein", new Color(0.36f, 0.04f, 0.08f, 0.72f), new[]
                {
                    new Vector2(-14f, -6f), new Vector2(-2f, -10f), new Vector2(8f, 0f), new Vector2(-4f, 8f), new Vector2(-16f, 4f)
                }, 7);
                AddPolygon(_frontArm!, "RustKnife", new Color(0.62f, 0.58f, 0.48f), new[]
                {
                    new Vector2(6f, 28f), new Vector2(24f, 24f), new Vector2(28f, 30f), new Vector2(8f, 34f)
                }, 11);
                break;
            case LayeredPrototypePreset.Fast:
                AddPolygon(_frontArm!, "ClawA", new Color(0.82f, 0.76f, 0.52f), new[]
                {
                    new Vector2(8f, 30f), new Vector2(22f, 26f), new Vector2(30f, 32f), new Vector2(12f, 36f)
                }, 11);
                AddPolygon(_frontArm!, "ClawB", new Color(0.82f, 0.76f, 0.52f), new[]
                {
                    new Vector2(10f, 34f), new Vector2(26f, 30f), new Vector2(32f, 36f), new Vector2(14f, 40f)
                }, 11);
                if (_torso is not null)
                {
                    _torso.Rotation = 0.12f;
                }
                break;
            case LayeredPrototypePreset.Brute:
                AddPolygon(_variantAccentRoot, "ConcreteArm", new Color(0.36f, 0.32f, 0.27f), new[]
                {
                    new Vector2(34f, -42f), new Vector2(78f, -48f), new Vector2(92f, -18f), new Vector2(48f, -8f)
                }, 0);
                AddPolygon(_variantAccentRoot, "ConcreteChunk", new Color(0.46f, 0.43f, 0.36f), new[]
                {
                    new Vector2(72f, -52f), new Vector2(118f, -58f), new Vector2(128f, -34f), new Vector2(82f, -26f)
                }, 1);
                break;
            case LayeredPrototypePreset.Infected:
                AddPolygon(_torso!, "SickVeinA", new Color(0.35f, 0.82f, 0.18f, 0.45f), new[]
                {
                    new Vector2(-10f, -8f), new Vector2(6f, -12f), new Vector2(12f, 2f), new Vector2(-2f, 6f)
                }, 7);
                AddPolygon(_torso!, "SickVeinB", new Color(0.35f, 0.82f, 0.18f, 0.45f), new[]
                {
                    new Vector2(4f, 6f), new Vector2(16f, 2f), new Vector2(18f, 18f), new Vector2(2f, 20f)
                }, 7);
                AddPolygon(_head!, "BiteMark", new Color(0.22f, 0.42f, 0.08f, 0.75f), new[]
                {
                    new Vector2(-8f, 10f), new Vector2(0f, 6f), new Vector2(8f, 11f), new Vector2(0f, 14f)
                }, 9);
                break;
            case LayeredPrototypePreset.MiniBoss:
                AddPolygon(_variantAccentRoot, "SpikeL", new Color(0.13f, 0.012f, 0.02f), new[]
                {
                    new Vector2(-38f, -72f), new Vector2(-62f, -64f), new Vector2(-42f, -52f)
                }, 0);
                AddPolygon(_variantAccentRoot, "SpikeR", new Color(0.13f, 0.012f, 0.02f), new[]
                {
                    new Vector2(28f, -76f), new Vector2(58f, -66f), new Vector2(34f, -54f)
                }, 0);
                AddPolygon(_variantAccentRoot, "Club", new Color(0.46f, 0.43f, 0.36f), new[]
                {
                    new Vector2(52f, -28f), new Vector2(96f, -34f), new Vector2(106f, -8f), new Vector2(58f, 0f)
                }, 1);
                AddPolygon(_torso!, "ChestCrack", palette.ChestMark, new[]
                {
                    new Vector2(-12f, -17f), new Vector2(14f, -22f), new Vector2(18f, -13f), new Vector2(-8f, -9f),
                    new Vector2(-18f, -2f), new Vector2(17f, -4f), new Vector2(20f, 4f), new Vector2(-12f, 10f)
                }, 8);
                break;
        }
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
            new Vector2(-15f, -18f), new Vector2(-6f, -20f), new Vector2(8f, -19f), new Vector2(16f, -14f),
            new Vector2(19f, 2f), new Vector2(15f, 20f), new Vector2(6f, 33f), new Vector2(-4f, 35f),
            new Vector2(-14f, 30f), new Vector2(-19f, 12f), new Vector2(-18f, -2f)
        ];
    }

    private static Vector2[] MakeThighSilhouette()
    {
        return
        [
            new Vector2(-7f, -34f), new Vector2(4f, -36f), new Vector2(9f, -24f), new Vector2(8f, -8f),
            new Vector2(5f, -2f), new Vector2(-4f, -4f), new Vector2(-9f, -16f), new Vector2(-10f, -28f)
        ];
    }

    private static Vector2[] MakeShinSilhouette()
    {
        return
        [
            new Vector2(-6f, -10f), new Vector2(6f, -11f), new Vector2(7f, 2f), new Vector2(5f, 14f),
            new Vector2(0f, 16f), new Vector2(-5f, 13f), new Vector2(-7f, 0f)
        ];
    }

    private static Vector2[] MakeUpperArmSilhouette()
    {
        return
        [
            new Vector2(-5f, -9f), new Vector2(6f, -10f), new Vector2(8f, 0f), new Vector2(7f, 12f),
            new Vector2(2f, 15f), new Vector2(-4f, 12f), new Vector2(-7f, 2f)
        ];
    }

    private static Vector2[] MakeForearmSilhouette()
    {
        return
        [
            new Vector2(-6f, -5f), new Vector2(7f, -6f), new Vector2(8f, 8f), new Vector2(6f, 17f),
            new Vector2(0f, 19f), new Vector2(-5f, 16f), new Vector2(-7f, 4f)
        ];
    }

    private static Polygon2D AddSoftPolygon(Node parent, string name, Color color, Vector2[] points, int z)
    {
        Color outline = color.Darkened(0.35f);
        AddPolygon(parent, $"{name}Outline", outline, points, z - 1);
        return AddPolygon(parent, name, color, points, z);
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
        shinJoint = AddJoint(hip, "ShinJoint", new Vector2(0f, 22f), 1);
        AddSoftPolygon(shinJoint, "Shin", cloth.Darkened(0.15f), MakeShinSilhouette(), 0);
        AddSoftPolygon(shinJoint, "Boot", new Color(0.09f, 0.06f, 0.045f), new[]
        {
            new Vector2(-11f, 10f), new Vector2(4f, 9f), new Vector2(16f, 12f), new Vector2(18f, 18f),
            new Vector2(6f, 20f), new Vector2(-10f, 19f), new Vector2(-13f, 14f)
        }, 1);
        AddPolygon(shinJoint, "ShoeAccent", accent, new[]
        {
            new Vector2(-5f, 7f), new Vector2(7f, 8f), new Vector2(7f, 12f), new Vector2(-6f, 12f)
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
        forearmJoint = AddJoint(shoulder, "ForearmJoint", new Vector2(0f, 24f), 1);
        AddSoftPolygon(forearmJoint, "Forearm", skin.Lightened(0.04f), MakeForearmSilhouette(), 0);
        AddPolygon(forearmJoint, "Wrap", new Color(0.85f, 0.78f, 0.62f), new[]
        {
            new Vector2(-9f, 2f), new Vector2(9f, 1f), new Vector2(9f, 7f), new Vector2(-8f, 8f)
        }, 1);
        float fistRx = knuckles ? 9f : 8f;
        AddPolygon(forearmJoint, "Fist", skin.Darkened(0.08f), MakeEllipse(1f, 18f, fistRx, 7f, 8), 2);
        if (knuckles)
        {
            AddPolygon(forearmJoint, "Knuckles", new Color(0.78f, 0.72f, 0.58f), new[]
            {
                new Vector2(-4f, 14f), new Vector2(6f, 13f), new Vector2(5f, 18f), new Vector2(-3f, 19f)
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
        _noseBleedDrip.Position = new Vector2(11f, 10f + drip * 14f);
        _noseBleedDrip.Modulate = new Color(1f, 1f, 1f, 1f - drip * 0.85f);
    }

    private void ResetLayeredPose()
    {
        SetPart(_backArm, new Vector2(-15f, -58f), -0.12f, Vector2.One);
        SetPart(_frontArm, new Vector2(15f, -58f), 0.08f, Vector2.One);
        SetPart(_backLeg, new Vector2(-8f, -12f), 0.04f, Vector2.One);
        SetPart(_frontLeg, new Vector2(8f, -12f), -0.04f, Vector2.One);
        SetPart(_backLegShin, new Vector2(0f, 22f), 0f, Vector2.One);
        SetPart(_frontLegShin, new Vector2(0f, 22f), 0f, Vector2.One);
        SetPart(_backArmForearm, new Vector2(0f, 24f), 0f, Vector2.One);
        SetPart(_frontArmForearm, new Vector2(0f, 24f), 0f, Vector2.One);
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
    }

    private void AnimateWalk(float walk, float walkOpposite, float limpFactor)
    {
        float stride = (_isRunning ? 0.78f : 0.52f) * limpFactor;
        float lift = _isRunning ? 9f : 6f;
        float bob = _isRunning ? 3.8f : 2.6f;

        if (_rig is not null)
        {
            _rig.Position = new Vector2(walkOpposite * (_isRunning ? 6f : 3.5f), Mathf.Abs(walk) * bob * 0.35f);
        }

        if (_torso is not null)
        {
            float runBoost = _isRunning ? 1.25f : _isExhausted ? 0.72f : 1f;
            _torso.Position += new Vector2(walkOpposite * 2.5f, Mathf.Abs(walk) * bob * runBoost);
            _torso.Rotation = walk * (_isRunning ? 0.06f : _isExhausted ? 0.022f : 0.035f);
            if (_isRunning)
            {
                _torso.Position += new Vector2(5f, -2f);
                _torso.Rotation += 0.1f;
            }
        }

        if (_frontLeg is not null)
        {
            _frontLeg.Rotation = walk * stride;
            _frontLeg.Position = new Vector2(8f + walk * 5f, -12f - Mathf.Max(0f, walk) * lift);
        }

        if (_frontLegShin is not null)
        {
            _frontLegShin.Rotation = Mathf.Clamp(-walk * 1.05f, -0.08f, 1.25f);
        }

        if (_backLeg is not null)
        {
            _backLeg.Rotation = -walk * stride * 0.95f;
            _backLeg.Position = new Vector2(-8f - walk * 4f, -12f - Mathf.Max(0f, -walk) * lift);
        }

        if (_backLegShin is not null)
        {
            _backLegShin.Rotation = Mathf.Clamp(walk * 0.95f, -0.08f, 1.15f);
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = -walk * 0.38f + 0.08f;
            _frontArm.Position = new Vector2(15f - walk * 2f, -58f + Mathf.Abs(walk) * 1.2f);
        }

        if (_frontArmForearm is not null)
        {
            _frontArmForearm.Rotation = -Mathf.Abs(walk) * 0.42f;
        }

        if (_backArm is not null)
        {
            _backArm.Rotation = walk * 0.36f - 0.12f;
            _backArm.Position = new Vector2(-15f + walk * 2f, -58f);
        }

        if (_backArmForearm is not null)
        {
            _backArmForearm.Rotation = -Mathf.Abs(walkOpposite) * 0.35f;
        }

        if (_head is not null)
        {
            _head.Rotation += walk * 0.018f;
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
        float duration = 0.14f;
        float t = Mathf.Clamp(_stateTime / duration, 0f, 1f);
        float slash = Mathf.Sin(t * Mathf.Pi);

        if (_torso is not null)
        {
            _torso.Rotation = slash * 0.1f;
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = 0.35f - slash * 1.35f;
            _frontArm.Position = new Vector2(16f + slash * 14f, -56f - slash * 4f);
        }

        if (_improvisedWeapon is not null)
        {
            _improvisedWeapon.Rotation = slash * 0.6f;
        }

        TrySpawnImpact(1, slash, _frontArm);
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
        bool playerStyled = LayeredPreset == LayeredPrototypePreset.Caua;

        switch (_activeMoveAnim)
        {
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
        float duration = 0.18f;
        float t = Mathf.Clamp(_stateTime / duration, 0f, 1f);
        float snap = t >= 0.28f ? Mathf.Sin(((t - 0.28f) / 0.72f) * Mathf.Pi) : 0f;

        if (_torso is not null)
        {
            _torso.Rotation = -0.08f + snap * 0.06f;
            _torso.Position += new Vector2(snap * 14f, -snap * 2f);
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = -0.42f - snap * 1.35f;
            _frontArm.Position = new Vector2(14f + snap * 30f, -56f - snap * 4f);
        }

        if (_backArm is not null)
        {
            _backArm.Rotation = -0.72f;
            _backArm.Position = new Vector2(-16f, -58f);
        }

        if (_head is not null)
        {
            _head.Rotation = -snap * 0.06f;
        }

        TrySpawnImpact(0, snap, _frontArm);
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
        float duration = 0.2f;
        float t = Mathf.Clamp(_stateTime / duration, 0f, 1f);
        float windup = t < 0.3f ? t / 0.3f : 0f;
        float extend = t >= 0.3f ? Mathf.Sin(((t - 0.3f) / 0.7f) * Mathf.Pi) : 0f;

        if (_torso is not null)
        {
            _torso.Rotation = windup * 0.14f - extend * 0.22f;
            _torso.Position += new Vector2(-windup * 5f + extend * 16f, -extend * 4f);
        }

        if (_backArm is not null)
        {
            _backArm.Rotation = -0.15f - extend * 1.35f;
            _backArm.Position = new Vector2(-14f + extend * 22f, -56f - extend * 6f);
        }

        if (_backArmForearm is not null)
        {
            _backArmForearm.Rotation = -extend * 0.48f;
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = 0.45f - extend * 0.35f;
        }

        if (_backLeg is not null)
        {
            _backLeg.Rotation = extend * 0.2f;
        }

        TrySpawnImpact(0, extend, _backArmForearm ?? _backArm);
    }

    private void AnimateHook()
    {
        float duration = 0.24f;
        float t = Mathf.Clamp(_stateTime / duration, 0f, 1f);
        float windup = t < 0.35f ? t / 0.35f : 0f;
        float extend = t >= 0.35f ? Mathf.Sin(((t - 0.35f) / 0.65f) * Mathf.Pi) : 0f;

        if (_torso is not null)
        {
            _torso.Rotation = -windup * 0.12f + extend * 0.18f;
            _torso.Position += new Vector2(extend * 6f, -extend * 3f);
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = 0.55f + windup * 0.45f - extend * 0.95f;
            _frontArm.Position = new Vector2(8f + extend * 14f, -54f - extend * 8f);
        }

        if (_head is not null)
        {
            _head.Rotation = extend * 0.08f;
        }

        TrySpawnImpact(2, extend, _frontArm);
    }

    private void AnimateUppercut()
    {
        float duration = 0.26f;
        float t = Mathf.Clamp(_stateTime / duration, 0f, 1f);
        float windup = t < 0.34f ? t / 0.34f : 0f;
        float extend = t >= 0.34f ? Mathf.Sin(((t - 0.34f) / 0.66f) * Mathf.Pi) : 0f;

        if (_torso is not null)
        {
            _torso.Rotation = windup * 0.1f - extend * 0.22f;
            _torso.Position += new Vector2(-windup * 6f + extend * 8f, -extend * 10f);
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = 0.35f + windup * 0.55f - extend * 1.65f;
            _frontArm.Position = new Vector2(8f + extend * 10f, -58f - extend * 18f);
        }

        if (_frontArmForearm is not null)
        {
            _frontArmForearm.Rotation = -windup * 0.35f - extend * 0.55f;
        }

        if (_backLeg is not null)
        {
            _backLeg.Rotation = extend * 0.25f;
        }

        if (_head is not null)
        {
            _head.Rotation = -extend * 0.1f;
            _head.Position += new Vector2(0f, -extend * 6f);
        }

        TrySpawnImpact(2, extend, _frontArmForearm ?? _frontArm);
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
        float duration = aerial ? 0.32f : 0.26f;
        float t = Mathf.Clamp(_stateTime / duration, 0f, 1f);
        float lift = Mathf.Sin(t * Mathf.Pi);

        if (_torso is not null)
        {
            _torso.Rotation = -lift * 0.14f;
            _torso.Position += new Vector2(lift * 6f, -lift * (aerial ? 14f : 8f));
        }

        if (_frontLeg is not null)
        {
            _frontLeg.Rotation = -lift * 1.35f;
            _frontLeg.Position = new Vector2(4f + lift * 8f, -18f - lift * 12f);
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = -0.35f - lift * 0.4f;
        }

        TrySpawnImpact(2, lift, _frontLeg);
    }

    private void AnimateElbow()
    {
        float duration = 0.2f;
        float t = Mathf.Clamp(_stateTime / duration, 0f, 1f);
        float drop = Mathf.Sin(t * Mathf.Pi);

        if (_torso is not null)
        {
            _torso.Rotation = drop * 0.16f;
            _torso.Position += new Vector2(drop * 5f, drop * 4f);
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = -0.95f + drop * 0.55f;
            _frontArm.Position = new Vector2(14f, -48f + drop * 8f);
        }

        TrySpawnImpact(2, drop, _frontArm);
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
        float duration = 0.22f;
        float t = Mathf.Clamp(_stateTime / duration, 0f, 1f);
        float windup = t < 0.32f ? t / 0.32f : 0f;
        float extend = t >= 0.32f ? Mathf.Sin(((t - 0.32f) / 0.68f) * Mathf.Pi) : 0f;

        if (_torso is not null)
        {
            _torso.Rotation = -windup * 0.12f + extend * 0.14f;
            _torso.Position += new Vector2(-windup * 6f + extend * 12f, -extend * 2f);
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = 0.15f - windup * 0.65f - extend * 0.95f;
            _frontArm.Position = new Vector2(12f - windup * 8f + extend * 10f, -58f - extend * 4f);
        }

        if (_frontArmForearm is not null)
        {
            _frontArmForearm.Rotation = -windup * 0.25f - extend * 0.55f;
        }

        if (_backArm is not null)
        {
            _backArm.Rotation = -0.12f - extend * 0.45f;
            _backArm.Position = new Vector2(-16f - extend * 4f, -58f);
        }

        if (_backLeg is not null)
        {
            _backLeg.Rotation = extend * 0.22f;
            _backLegShin!.Rotation = extend * 0.18f;
        }

        if (_frontLeg is not null)
        {
            _frontLeg.Rotation = -extend * 0.08f;
        }

        if (_head is not null)
        {
            _head.Position += new Vector2(extend * 4f, -extend * 1f);
            _head.Rotation = -extend * 0.05f;
        }

        TrySpawnImpact(0, extend, _frontArmForearm ?? _frontArm);
    }

    private void AnimateHeadbutt()
    {
        float duration = 0.15f;
        float t = Mathf.Clamp(_stateTime / duration, 0f, 1f);
        float lunge = Mathf.Sin(t * Mathf.Pi);

        if (_torso is not null)
        {
            _torso.Rotation = lunge * 0.22f;
            _torso.Position += new Vector2(lunge * 12f, lunge * 2f);
        }

        if (_head is not null)
        {
            _head.Position += new Vector2(lunge * 16f, lunge * 4f);
            _head.Rotation = lunge * 0.12f;
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = 0.35f;
        }

        TrySpawnImpact(0, lunge, _head);
    }

    private void AnimateSideKick()
    {
        float duration = 0.28f;
        float t = Mathf.Clamp(_stateTime / duration, 0f, 1f);
        float windup = t < 0.32f ? t / 0.32f : 0f;
        float extend = t >= 0.32f ? Mathf.Sin(((t - 0.32f) / 0.68f) * Mathf.Pi) : 0f;

        if (_torso is not null)
        {
            _torso.Rotation = windup * 0.08f - extend * 0.16f;
            _torso.Position += new Vector2(-windup * 5f + extend * 6f, -extend * 2f);
        }

        if (_frontLeg is not null)
        {
            _frontLeg.Rotation = windup * 0.25f - extend * 1.05f;
            _frontLeg.Position = new Vector2(6f - windup * 4f + extend * 22f, -12f - extend * 5f);
        }

        if (_backLeg is not null)
        {
            _backLeg.Rotation = 0.04f + extend * 0.08f;
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = 0.35f + extend * 0.15f;
        }

        if (_backArm is not null)
        {
            _backArm.Rotation = -0.45f;
        }

        TrySpawnImpact(1, extend, _frontLeg);
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
        if (extend < 0.72f || _impactSpawned || strikePart is null)
        {
            return;
        }

        _impactSpawned = true;
        SpawnStrikeImpact(comboIndex, strikePart);
    }

    private void AnimateHurt()
    {
        float maxHurt = 0.55f + _hurtStrength * 0.08f;
        float t = 1f - Mathf.Clamp(_hurtTime / maxHurt, 0f, 1f);
        float recoil = Mathf.Sin(t * Mathf.Pi);
        float lean = _hurtDirection.X * recoil * 0.32f * _hurtStrength;
        float buckle = recoil * _hurtStrength;

        if (_torso is not null)
        {
            _torso.Rotation = lean;
            _torso.Position += new Vector2(_hurtDirection.X * recoil * 11f * _hurtStrength, buckle * 3f);
            _torso.Modulate = Colors.White.Lerp(new Color(1f, 0.55f, 0.5f), recoil * 0.45f);
        }

        if (_head is not null)
        {
            _head.Rotation = -lean * 1.1f;
            _head.Position += new Vector2(_hurtDirection.X * recoil * 9f, recoil * 3f);
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

        float size = comboIndex switch
        {
            2 => 1.35f,
            1 => 1.15f,
            _ => 1f,
        };

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
        burst.GlobalPosition = strikePart.GlobalPosition + new Vector2(_facingSign * 14f, 0f);

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
