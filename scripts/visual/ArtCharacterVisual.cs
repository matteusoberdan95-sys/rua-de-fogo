namespace SangueNoAsfalto.Visual;

/// <summary>
/// Visual baseado em SpriteFrames de art/production/. Fallback automatico para CharacterSpriteVisual.
/// </summary>
public partial class ArtCharacterVisual : Node2D, IActorVisual
{
    [Export]
    public NodePath SpritePath { get; set; } = new("AnimatedSprite2D");

    [Export]
    public ProductionCharacterId CharacterId { get; set; } = ProductionCharacterId.Caua;

    [Export]
    public float DisplayScale { get; set; } = 1f;

    [Export]
    public bool SourceFacesRight { get; set; } = true;

    [Export]
    public Vector2 SpriteOffset { get; set; } = new(0f, -52f);

    private AnimatedSprite2D? _sprite;
    private CharacterSpriteVisual? _fallbackVisual;
    private bool _isActive;
    private int _facingSign = 1;
    private Vector2 _jumpOffset;
    private string _currentAnim = string.Empty;
    private float _attackLockRemaining;
    private float _stateLockRemaining;
    private bool _isDying;

    public bool IsProductionArtActive => _isActive;

    public override void _Ready()
    {
        _sprite = GetNodeOrNull<AnimatedSprite2D>(SpritePath);
        if (_sprite is not null)
        {
            _sprite.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
            _sprite.Position = SpriteOffset;
            _sprite.Visible = false;
        }

        Visible = false;
    }

    /// <summary>
    /// Carrega pacote real se existir. Desativa rig procedural quando ativo.
    /// </summary>
    public bool TryActivate(CharacterSpriteVisual? fallback, ProductionCharacterId? characterOverride = null)
    {
        _fallbackVisual = fallback;
        ProductionCharacterId id = characterOverride ?? CharacterId;
        CharacterId = id;

        SpriteFrames? frames = ProductionArtCatalog.LoadSpriteFrames(id);
        if (frames is null || _sprite is null || frames.GetAnimationNames().Length == 0)
        {
            _isActive = false;
            Visible = false;
            if (_sprite is not null)
            {
                _sprite.Visible = false;
            }

            return false;
        }

        if (_sprite is null)
        {
            return false;
        }

        _sprite.SpriteFrames = frames;
        DisplayScale = ProductionSpriteFrameBuilder.ReadDisplayScale(id);
        SpriteOffset = ProductionSpriteFrameBuilder.ReadSpriteOffset(id);
        _sprite.Position = SpriteOffset;
        _sprite.Scale = Vector2.One * DisplayScale;
        _sprite.Visible = true;
        Visible = true;
        _isActive = true;

        if (_fallbackVisual is not null)
        {
            _fallbackVisual.Visible = false;
            _fallbackVisual.SetProcess(false);
        }

        PlayLoop("idle");
        ApplyFacing();
        return true;
    }

    public void EnsureLayeredRig(LayeredPrototypePreset preset)
    {
        // Arte de producao nao usa rig procedural.
    }

    public void SetFacing(int sign)
    {
        if (sign == 0)
        {
            return;
        }

        _facingSign = sign > 0 ? 1 : -1;
        ApplyFacing();
    }

    public void SetJumpOffset(float heightOffset)
    {
        _jumpOffset = new Vector2(0f, -heightOffset);
        ApplyVisualOffset();
    }

    public void SetDamageVisualTier(EnemyDamageVisualTier tier)
    {
        if (!_isActive || _sprite is null)
        {
            return;
        }

        _sprite.Modulate = tier switch
        {
            EnemyDamageVisualTier.Hurt => new Color(1f, 0.82f, 0.82f),
            EnemyDamageVisualTier.Critical => new Color(1f, 0.68f, 0.68f),
            _ => Colors.White,
        };
    }

    public void PlayDeath()
    {
        _isDying = true;
        PlayOnce("death", "hurt");
    }

    public void PlayParryStagger()
    {
        PlayOnce("parry_stagger", "hurt");
        _stateLockRemaining = 0.35f;
    }

    public void PlayPostureStagger()
    {
        PlayOnce("posture_stagger", "hurt");
        _stateLockRemaining = 0.45f;
    }

    public void PlayGuard()
    {
        PlayLoop("guard", "idle");
    }

    public void PlayBlockImpact()
    {
        PlayOnce("block_impact", "hurt");
        _stateLockRemaining = 0.12f;
    }

    public void PlayShoot()
    {
        PlayOnce("shoot", "attack");
        _stateLockRemaining = 0.22f;
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
        if (!_isActive || _isDying)
        {
            return;
        }

        if (_attackLockRemaining > 0f || _stateLockRemaining > 0f)
        {
            return;
        }

        string next = isHurt ? "hurt"
            : isPostureStaggered ? "posture_stagger"
            : isGuarding ? "guard"
            : isParrying ? "parry"
            : isReloading ? "reload"
            : isTelegraphing ? "telegraph"
            : isFinisherAttack ? "finisher"
            : isAttacking ? "attack"
            : isDashing ? "dash"
            : isMoving ? isRunning ? "run" : "walk"
            : "idle";

        PlayLoop(next);
    }

    public void SetEquippedWeapon(ImprovisedWeaponKind kind)
    {
        // Futuro: trocar overlay ou animacao por arma.
    }

    public void PlayWeaponAttack(ImprovisedWeaponKind kind)
    {
        PlayOnce("weapon_attack", "attack");
        _attackLockRemaining = 0.32f;
    }

    public void PlayFinisherAttack(ImprovisedWeaponKind kind)
    {
        PlayOnce("finisher", "attack");
        _attackLockRemaining = 0.55f;
    }

    public void PlayReload()
    {
        PlayLoop("reload", "idle");
    }

    public void EndReload()
    {
        PlayLoop("idle");
    }

    public void PlayParryWindup()
    {
        PlayLoop("parry_windup", "parry");
        _stateLockRemaining = 0.18f;
    }

    public void PlayParrySuccessMatrix()
    {
        PlayOnce("parry_success", "parry");
        _stateLockRemaining = 0.28f;
    }

    public void PlayParryRiposte()
    {
        PlayOnce("parry_riposte", "attack");
        _attackLockRemaining = 0.36f;
    }

    public void PlayPostureKill()
    {
        PlayOnce("posture_kill", "attack");
        _attackLockRemaining = 0.48f;
    }

    public void BeginEnemyStrike(float duration, int patternIndex, MoveAnimProfile anim)
    {
        SetAttackMove(anim, patternIndex, CombatStyleKind.Rua, duration);
    }

    public void SetAttackCombo(int comboIndex)
    {
        // Compatibilidade com API antiga do rig procedural.
    }

    public void SetCombatStyle(CombatStyleKind style)
    {
        // Futuro: trocar sprite set por estilo.
    }

    public void SetAttackMove(MoveAnimProfile anim, int impactComboIndex, CombatStyleKind style, float duration = 0.32f)
    {
        if (!_isActive)
        {
            return;
        }

        string animName = MapMoveAnim(anim);
        PlayOnce(animName, "attack");
        _attackLockRemaining = Mathf.Max(duration, 0.12f);
    }

    public void PlayHitReaction(Vector2 direction, float severity = 1f)
    {
        PlayOnce("hurt", "idle");
        _stateLockRemaining = Mathf.Clamp(0.14f * severity, 0.1f, 0.35f);
        Modulate = new Color(1f, 0.55f, 0.52f);
        Tween tween = CreateTween();
        tween.TweenProperty(this, "modulate", Colors.White, 0.12f);
    }

    public override void _Process(double delta)
    {
        if (!_isActive)
        {
            return;
        }

        float dt = (float)delta;
        if (_attackLockRemaining > 0f)
        {
            _attackLockRemaining = Mathf.Max(_attackLockRemaining - dt, 0f);
        }

        if (_stateLockRemaining > 0f)
        {
            _stateLockRemaining = Mathf.Max(_stateLockRemaining - dt, 0f);
        }
    }

    private static string MapMoveAnim(MoveAnimProfile anim) => anim switch
    {
        MoveAnimProfile.Jab or MoveAnimProfile.BoxLead => "jab",
        MoveAnimProfile.Cross => "cross",
        MoveAnimProfile.Hook => "hook",
        MoveAnimProfile.Uppercut => "uppercut",
        MoveAnimProfile.SideKick or MoveAnimProfile.Teep => "kick",
        MoveAnimProfile.HighKick or MoveAnimProfile.LowKick => "kick",
        MoveAnimProfile.Knee or MoveAnimProfile.FlyingKnee => "knee",
        MoveAnimProfile.KnifeSlash => "knife_slash",
        MoveAnimProfile.RunningPunch => "running_punch",
        MoveAnimProfile.RunningKick => "running_kick",
        MoveAnimProfile.FlyingKick => "flying_kick",
        _ => "attack",
    };

    private void PlayLoop(string primary, string? fallback = null)
    {
        if (TryPlay(primary, true))
        {
            return;
        }

        if (fallback is not null)
        {
            TryPlay(fallback, true);
        }
    }

    private void PlayOnce(string primary, string? fallback = null)
    {
        if (TryPlay(primary, false))
        {
            return;
        }

        if (fallback is not null)
        {
            TryPlay(fallback, false);
        }
    }

    private bool TryPlay(string animationName, bool loop)
    {
        if (!_isActive || _sprite?.SpriteFrames is null)
        {
            return false;
        }

        if (!_sprite.SpriteFrames.HasAnimation(animationName))
        {
            return false;
        }

        if (_currentAnim == animationName && _sprite.IsPlaying())
        {
            return true;
        }

        _currentAnim = animationName;
        _sprite.Play(animationName);
        return true;
    }

    private void ApplyFacing()
    {
        if (_sprite is null)
        {
            return;
        }

        float mirror = SourceFacesRight ? _facingSign : -_facingSign;
        _sprite.Scale = new Vector2(Mathf.Abs(DisplayScale) * mirror, DisplayScale);
    }

    private void ApplyVisualOffset()
    {
        if (_sprite is null)
        {
            return;
        }

        _sprite.Position = SpriteOffset + _jumpOffset;
    }
}
