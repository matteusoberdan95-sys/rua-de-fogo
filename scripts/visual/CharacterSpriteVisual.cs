namespace SangueNoAsfalto.Visual;

using SangueNoAsfalto.Combat;

public enum LayeredPrototypePreset
{
    Caua,
    QuebraOsso
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
    private Node2D? _shirtPulse;
    private Node2D? _vestFlap;
    private Node2D? _clothSway;
    private Node2D? _hairTail;
    private Polygon2D? _faceBruise;
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
    private bool _impactSpawned;
    private bool _isRunning;
    private float _shootAnimTime;
    private Node2D? _sidearm;
    private Node2D? _improvisedWeapon;
    private ImprovisedWeaponKind _equippedWeaponKind;
    private bool _finisherAttack;
    private float _reloadAnimTime;
    private EnemyDamageVisualTier _damageTier = EnemyDamageVisualTier.Intact;
    private int _facingSign = 1;

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
    }

    public override void _Process(double delta)
    {
        if (!UseLayeredPrototype || _rig is null)
        {
            return;
        }

        float dt = (float)delta;
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

        if (_torsoBlood is not null)
        {
            _torsoBlood.Visible = tier >= EnemyDamageVisualTier.Hurt;
        }

        if (_criticalLimp is not null)
        {
            _criticalLimp.Visible = tier >= EnemyDamageVisualTier.Critical;
        }
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
        bool isParrying = false)
    {
        _isRunning = isRunning;
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
            float bobSpeed = isRunning ? 0.028f : 0.018f;
            float bobAmount = isRunning ? 2.4f : 1.8f;
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

            string nextState = isHurt || _hurtTime > 0f ? "hurt"
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
        SetLayeredState("parry");
    }

    public void PlayParry()
    {
        _flashTime = 0.14f;
        SetLayeredState("parry");
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
        _rig = new Node2D { Name = $"Layered{LayeredPreset}Rig", YSortEnabled = true };
        AddChild(_rig);

        Color skin = LayeredPreset == LayeredPrototypePreset.Caua
            ? new Color(0.58f, 0.34f, 0.19f)
            : new Color(0.50f, 0.29f, 0.17f);
        Color backSkin = skin.Darkened(0.08f);
        Color pants = LayeredPreset == LayeredPrototypePreset.Caua
            ? new Color(0.065f, 0.075f, 0.06f)
            : new Color(0.17f, 0.085f, 0.28f);
        Color backPants = pants.Darkened(0.12f);
        Color shirt = LayeredPreset == LayeredPrototypePreset.Caua
            ? new Color(0.66f, 0.035f, 0.03f)
            : new Color(0.025f, 0.025f, 0.027f);
        Color vest = LayeredPreset == LayeredPrototypePreset.Caua
            ? new Color(0.13f, 0.09f, 0.075f)
            : new Color(0.055f, 0.06f, 0.055f);
        Color shoeAccent = LayeredPreset == LayeredPrototypePreset.Caua
            ? new Color(0.62f, 0.04f, 0.03f)
            : new Color(0.18f, 0.16f, 0.13f);

        Node2D backLayer = AddJoint(_rig, "BackLayer", Vector2.Zero, 0);
        _backLeg = AddLimb(backLayer, "BackLeg", new Vector2(-8f, -12f), backPants, shoeAccent, z: 0);
        _backArm = AddArm(backLayer, "BackArm", new Vector2(-15f, -58f), backSkin, z: 1, knuckles: true);

        _torso = AddJoint(_rig, "Torso", new Vector2(0f, -56f), 4);
        AddPolygon(_torso, "BackVest", vest, new[]
        {
            new Vector2(-20f, -19f), new Vector2(16f, -22f), new Vector2(22f, 8f), new Vector2(12f, 31f), new Vector2(-17f, 28f), new Vector2(-23f, 8f)
        }, 0);
        _shirtPulse = AddJoint(_torso, "ShirtPulse", Vector2.Zero, 2);
        AddPolygon(_shirtPulse, "TorsoCloth", shirt, new[]
        {
            new Vector2(-17f, -18f), new Vector2(17f, -18f), new Vector2(21f, 10f), new Vector2(12f, 34f), new Vector2(-13f, 32f), new Vector2(-21f, 9f)
        }, 0);
        AddPolygon(_shirtPulse, "ChestMark", LayeredPreset == LayeredPrototypePreset.Caua
            ? new Color(0.88f, 0.77f, 0.58f, 0.85f)
            : new Color(0.82f, 0.78f, 0.66f, 0.7f), new[]
        {
            new Vector2(-11f, -2f), new Vector2(9f, -3f), new Vector2(11f, 4f), new Vector2(-11f, 5f)
        }, 1);
        AddPolygon(_torso, "RaggedHem", new Color(0.34f, 0.015f, 0.012f), new[]
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
        }

        _frontLeg = AddLimb(_rig, "FrontLeg", new Vector2(8f, -12f), pants, shoeAccent, z: 6);
        _head = AddJoint(_rig, "Head", new Vector2(0f, -96f), 8);
        AddPolygon(_head, "Neck", skin.Darkened(0.12f), new[]
        {
            new Vector2(-7f, 10f), new Vector2(8f, 10f), new Vector2(7f, 23f), new Vector2(-8f, 22f)
        }, 0);
        AddPolygon(_head, "Face", skin, MakeEllipse(0f, 0f, 13f, 17f, 12), 2);
        AddPolygon(_head, "Brow", new Color(0.075f, 0.045f, 0.035f), new[]
        {
            new Vector2(-9f, -4f), new Vector2(10f, -7f), new Vector2(14f, -3f), new Vector2(-8f, 1f)
        }, 4);
        AddPolygon(_head, "Nose", new Color(0.72f, 0.45f, 0.24f), new[]
        {
            new Vector2(8f, -1f), new Vector2(17f, 3f), new Vector2(7f, 7f)
        }, 5);
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
        AddPolygon(_hair, "HairSpikes", LayeredPreset == LayeredPrototypePreset.Caua
            ? new Color(0.035f, 0.025f, 0.02f)
            : new Color(0.03f, 0.02f, 0.018f), LayeredPreset == LayeredPrototypePreset.Caua
            ? new[]
        {
            new Vector2(-15f, 4f), new Vector2(-10f, -14f), new Vector2(-4f, -5f), new Vector2(1f, -19f), new Vector2(5f, -5f), new Vector2(14f, -13f), new Vector2(10f, 4f)
        }
            : MakeEllipse(0f, -3f, 12f, 8f, 10), 0);
        _hairTail = AddJoint(_hair, "HairTail", new Vector2(6f, 6f), 1);
        AddPolygon(_hairTail, "TailStrand", LayeredPreset == LayeredPrototypePreset.Caua
            ? new Color(0.05f, 0.035f, 0.028f)
            : new Color(0.04f, 0.03f, 0.025f), new[]
        {
            new Vector2(-3f, 0f), new Vector2(8f, -2f), new Vector2(12f, 10f), new Vector2(0f, 12f)
        }, 0);

        _frontArm = AddArm(_rig, "FrontArm", new Vector2(15f, -58f), skin.Lightened(0.04f), z: 10, knuckles: true);
        _sidearm = AddJoint(_frontArm, "Sidearm", new Vector2(14f, 24f), 9);
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
    }

    private static Node2D AddJoint(Node parent, string name, Vector2 position, int z)
    {
        Node2D node = new() { Name = name, Position = position, ZIndex = z };
        parent.AddChild(node);
        return node;
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
        float idle = Mathf.Sin(_stateTime * 4.2f);
        float heartbeat = Mathf.Max(0f, Mathf.Sin(_stateTime * 8.5f));
        float walkSpeed = _damageTier == EnemyDamageVisualTier.Critical && !_isRunning ? 8.2f : _isRunning ? 14.5f : 10.5f;
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
            _shirtPulse.Modulate = Colors.White.Lerp(new Color(1f, 0.72f, 0.62f), heartbeat * 0.16f);
        }

        if (_head is not null)
        {
            _head.Position = new Vector2(0f, -96f + idle * 0.8f);
            _head.Rotation = idle * 0.025f;
        }

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

    private void ResetLayeredPose()
    {
        SetPart(_backArm, new Vector2(-15f, -58f), -0.12f, Vector2.One);
        SetPart(_frontArm, new Vector2(15f, -58f), 0.08f, Vector2.One);
        SetPart(_backLeg, new Vector2(-8f, -12f), 0.04f, Vector2.One);
        SetPart(_frontLeg, new Vector2(8f, -12f), -0.04f, Vector2.One);
        if (_torso is not null)
        {
            _torso.Modulate = Colors.White;
        }

        if (_frontArm is not null)
        {
            _frontArm.Modulate = Colors.White;
        }
    }

    private void AnimateIdle(float idle)
    {
        if (_frontArm is not null)
        {
            _frontArm.Rotation += idle * 0.035f;
        }

        if (_backArm is not null)
        {
            _backArm.Rotation -= idle * 0.025f;
        }
    }

    private void AnimateWalk(float walk, float walkOpposite, float limpFactor)
    {
        if (_torso is not null)
        {
            _torso.Position += new Vector2(0f, Mathf.Abs(walk) * 2.2f * limpFactor * (_isRunning ? 1.25f : 1f));
            _torso.Rotation = walk * (_isRunning ? 0.04f : 0.025f);
            if (_isRunning)
            {
                _torso.Position += new Vector2(4f, 0f);
                _torso.Rotation += 0.08f;
            }
        }

        if (_frontLeg is not null)
        {
            _frontLeg.Rotation = walk * 0.32f * limpFactor;
            _frontLeg.Position += new Vector2(walk * 2.4f, Mathf.Abs(walk) * 1.2f);
        }

        if (_backLeg is not null)
        {
            _backLeg.Rotation = -walk * 0.3f;
            _backLeg.Position += new Vector2(-walk * 2f, Mathf.Abs(walkOpposite) * 1.2f);
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = -walk * 0.22f + 0.08f;
        }

        if (_backArm is not null)
        {
            _backArm.Rotation = walk * 0.22f - 0.12f;
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

    private void AnimateParry()
    {
        float t = Mathf.Clamp(_stateTime / 0.26f, 0f, 1f);
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

    private void AnimateUnarmedAttack()
    {
        if (_isRunning && _attackComboIndex == 0)
        {
            AnimateRunningPunch();
            return;
        }

        if (_isRunning && _attackComboIndex == 1)
        {
            AnimateRunningKick();
            return;
        }

        switch (_attackComboIndex)
        {
            case 1:
                if (LayeredPreset == LayeredPrototypePreset.QuebraOsso)
                {
                    AnimateHeadbutt();
                }
                else
                {
                    AnimateSideKick();
                }
                break;
            case 2:
                AnimateFlyingKick();
                break;
            default:
                AnimateJab();
                break;
        }
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
        float windup = t < 0.38f ? t / 0.38f : 0f;
        float extend = t >= 0.38f ? Mathf.Sin(((t - 0.38f) / 0.62f) * Mathf.Pi) : 0f;

        if (_torso is not null)
        {
            _torso.Rotation = -windup * 0.06f + extend * 0.1f;
            _torso.Position += new Vector2(-windup * 4f + extend * 8f, 0f);
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = 0.2f - windup * 0.55f - extend * 1.05f;
            _frontArm.Position = new Vector2(12f - windup * 6f + extend * 26f, -58f - extend * 3f);
        }

        if (_backArm is not null)
        {
            _backArm.Rotation = -0.12f - extend * 0.3f;
        }

        if (_head is not null)
        {
            _head.Position += new Vector2(extend * 3f, 0f);
        }

        TrySpawnImpact(0, extend, _frontArm);
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
        bool headbutt = LayeredPreset == LayeredPrototypePreset.QuebraOsso && _attackComboIndex == 1;

        if (_torso is not null)
        {
            _torso.Rotation = (-0.12f - (headbutt ? 0.08f : 0f)) * pulse;
            _torso.Position += new Vector2(-4f * pulse, headbutt ? 2f * pulse : 0f);
            _torso.Modulate = Colors.White.Lerp(new Color(1f, 0.42f, 0.28f), pulse * 0.35f);
        }

        if (_head is not null)
        {
            _head.Position += new Vector2((headbutt ? 3f : -2f) * pulse, headbutt ? -3f * pulse : 0f);
            _head.Rotation = headbutt ? pulse * 0.18f : 0f;
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = headbutt ? 0.25f : -0.85f * pulse;
            _frontArm.Position = new Vector2(15f - 6f * pulse, -58f);
        }

        if (_frontLeg is not null)
        {
            _frontLeg.Rotation = 0.08f * pulse;
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
