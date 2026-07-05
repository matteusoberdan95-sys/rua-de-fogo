namespace SangueNoAsfalto.Visual;

public enum LayeredPrototypePreset
{
    Caua,
    QuebraOsso
}

/// <summary>
/// Controle visual de personagem. Aceita SpriteFrames tradicionais e um rig 2D em camadas
/// para prototipar respiracao, cabelo, arma e impacto sem depender de recortes estaticos.
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
    private Node2D? _weapon;
    private Node2D? _shirtPulse;
    private Vector2 _basePosition;
    private Vector2 _jumpOffset;
    private Vector2 _movementOffset;
    private string _currentAnim = string.Empty;
    private string _layeredState = "idle";
    private float _stateTime;
    private float _flashTime;
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

    public void UpdateLocomotion(bool isMoving, bool isAttacking, bool isDashing)
    {
        if (UseLayeredPrototype)
        {
            _movementOffset = isMoving && !isAttacking && !isDashing
                ? new Vector2(0f, Mathf.Sin(Time.GetTicksMsec() * 0.018f) * 1.8f)
                : Vector2.Zero;
            ApplyVisualOffset();

            string nextState = isAttacking ? "attack" : isDashing ? "dash" : isMoving ? "walk" : "idle";
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

        _layeredState = nextState;
        _stateTime = 0f;
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
        _backArm = AddArm(backLayer, "BackArm", new Vector2(-15f, -58f), backSkin, z: 1);

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
        _hair = AddJoint(_head, "Hair", new Vector2(-1f, -12f), 6);
        AddPolygon(_hair, "HairSpikes", LayeredPreset == LayeredPrototypePreset.Caua
            ? new Color(0.035f, 0.025f, 0.02f)
            : new Color(0.03f, 0.02f, 0.018f), LayeredPreset == LayeredPrototypePreset.Caua
            ? new[]
        {
            new Vector2(-15f, 4f), new Vector2(-10f, -14f), new Vector2(-4f, -5f), new Vector2(1f, -19f), new Vector2(5f, -5f), new Vector2(14f, -13f), new Vector2(10f, 4f)
        }
            : MakeEllipse(0f, -3f, 12f, 8f, 10), 0);

        _frontArm = AddArm(_rig, "FrontArm", new Vector2(15f, -58f), skin.Lightened(0.04f), z: 10);
        _weapon = AddJoint(_frontArm, LayeredPreset == LayeredPrototypePreset.Caua ? "Machete" : "Pipe", new Vector2(19f, 28f), 8);
        AddPolygon(_weapon, "Handle", new Color(0.13f, 0.07f, 0.035f), new[]
        {
            new Vector2(-3f, -3f), new Vector2(16f, -2f), new Vector2(16f, 3f), new Vector2(-3f, 3f)
        }, 0);
        AddPolygon(_weapon, "Blade", LayeredPreset == LayeredPrototypePreset.Caua
            ? new Color(0.73f, 0.71f, 0.62f)
            : new Color(0.45f, 0.46f, 0.42f), new[]
        {
            new Vector2(12f, -4f), new Vector2(54f, -8f), new Vector2(66f, -1f), new Vector2(50f, 6f), new Vector2(12f, 4f)
        }, 1);
        AddPolygon(_weapon, "BladeBlood", LayeredPreset == LayeredPrototypePreset.Caua
            ? new Color(0.56f, 0.015f, 0.015f, 0.86f)
            : new Color(0.22f, 0.02f, 0.02f, 0.45f), new[]
        {
            new Vector2(37f, -5f), new Vector2(56f, -4f), new Vector2(50f, 1f), new Vector2(34f, 1f)
        }, 2);
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

    private static Node2D AddArm(Node parent, string name, Vector2 position, Color skin, int z)
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
        AddPolygon(arm, "Bandage", new Color(0.85f, 0.78f, 0.62f), new[]
        {
            new Vector2(-9f, 18f), new Vector2(9f, 17f), new Vector2(9f, 23f), new Vector2(-8f, 24f)
        }, 2);
        AddPolygon(arm, "Fist", skin.Darkened(0.08f), MakeEllipse(1f, 36f, 8f, 7f, 8), 3);
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
        float walk = Mathf.Sin(_stateTime * 10.5f);
        float walkOpposite = Mathf.Cos(_stateTime * 10.5f);

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

        switch (_layeredState)
        {
            case "walk":
                AnimateWalk(walk, walkOpposite);
                break;
            case "attack":
                AnimateAttack();
                break;
            case "dash":
                AnimateDash();
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
        SetPart(_weapon, new Vector2(19f, 28f), 0.05f, Vector2.One);
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

        if (_weapon is not null)
        {
            _weapon.Rotation += idle * 0.025f;
        }
    }

    private void AnimateWalk(float walk, float walkOpposite)
    {
        if (_torso is not null)
        {
            _torso.Position += new Vector2(0f, Mathf.Abs(walk) * 2.2f);
            _torso.Rotation = walk * 0.025f;
        }

        if (_frontLeg is not null)
        {
            _frontLeg.Rotation = walk * 0.32f;
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

        if (_weapon is not null)
        {
            _weapon.Rotation = -walk * 0.08f + 0.05f;
        }
    }

    private void AnimateAttack()
    {
        float t = Mathf.Clamp(_stateTime / 0.18f, 0f, 1f);
        float windup = 1f - Mathf.Abs(t * 2f - 1f);
        float swing = Mathf.Sin(t * Mathf.Pi);

        if (_torso is not null)
        {
            _torso.Rotation = -0.08f + swing * 0.2f;
            _torso.Position += new Vector2(swing * 5f, -windup * 1.5f);
        }

        if (_head is not null)
        {
            _head.Position += new Vector2(swing * 2f, -windup * 1f);
        }

        if (_frontArm is not null)
        {
            _frontArm.Rotation = -0.72f + swing * 1.55f;
            _frontArm.Position = new Vector2(18f + swing * 9f, -62f + windup * 3f);
        }

        if (_backArm is not null)
        {
            _backArm.Rotation = -0.36f - swing * 0.35f;
        }

        if (_weapon is not null)
        {
            _weapon.Rotation = -0.35f + swing * 1.15f;
            _weapon.Scale = new Vector2(1f + swing * 0.12f, 1f);
        }

        if (_frontLeg is not null)
        {
            _frontLeg.Rotation = 0.14f + swing * 0.1f;
        }

        if (_backLeg is not null)
        {
            _backLeg.Rotation = -0.16f;
        }
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
