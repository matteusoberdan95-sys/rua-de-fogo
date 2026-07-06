namespace SangueNoAsfalto.Enemies;

using SangueNoAsfalto.Ui;
using SangueNoAsfalto.Visual;

public partial class SideScrollerEnemyController : CharacterBody2D, ICombatKnockbackReceiver
{
    [Export]
    public float MoveSpeed { get; set; } = 120f;

    [Export]
    public float LaneSpeed { get; set; } = 95f;

    [Export]
    public float AggroRange { get; set; } = 900f;

    [Export]
    public float AttackRangeX { get; set; } = 58f;

    [Export]
    public float MinStandOffX { get; set; } = 46f;

    [Export]
    public float AttackRangeY { get; set; } = 34f;

    public float AttackCooldown { get; set; } = 2.25f;

    [Export]
    public float TelegraphDuration { get; set; } = 1.35f;

    [Export]
    public float AttackDuration { get; set; } = 0.45f;

    public bool IsDying => _dying;

    [Export]
    public float MinLaneY { get; set; } = 260f;

    [Export]
    public float MaxLaneY { get; set; } = 470f;

    private Node2D? _target;
    private Hitbox? _attackArea;
    private CollisionShape2D? _attackCollision;
    private float _cooldownRemaining;
    private float _telegraphRemaining;
    private float _attackTimeRemaining;
    private float _hitStunRemaining;
    private int _facingSign = -1;
    private int _attackPatternIndex;
    private CharacterSpriteVisual? _spriteVisual;
    private bool _dying;
    private Health? _health;
    private PostureComponent? _posture;
    private ParryTelegraphMarker? _parryMarker;
    private LayeredPrototypePreset _enemyPreset = LayeredPrototypePreset.QuebraOsso;
    private EnemyAttackPattern _pendingAttack;

    public PostureComponent? Posture => _posture;

    public bool IsPostureBroken => _posture?.IsBroken == true;

    public bool IsTelegraphing => _telegraphRemaining > 0f;

    public float TelegraphUrgency =>
        _telegraphRemaining > 0f && TelegraphDuration > 0f
            ? 1f - (_telegraphRemaining / TelegraphDuration)
            : 0f;

    public override void _Ready()
    {
        AddToGroup("enemy");
        AddToGroup("side_enemy");

        _spriteVisual = GetNodeOrNull<CharacterSpriteVisual>("SpriteVisual");
        _enemyPreset = EnemyLayeredVisual.ResolvePreset(this);
        if (_spriteVisual is null)
        {
            _spriteVisual = EnemyLayeredVisual.AttachLayeredRig(this, _enemyPreset);
        }
        else
        {
            _spriteVisual.EnsureLayeredRig(_enemyPreset);
            EnemyLayeredVisual.HideBlockMeshes(this);
        }

        _attackArea = GetNodeOrNull<Hitbox>("AttackArea");
        _attackCollision = GetNodeOrNull<CollisionShape2D>("AttackArea/CollisionShape2D");

        if (_attackArea is not null)
        {
            _attackArea.Source = this;
            _attackArea.Monitoring = false;
        }

        SetAttackCollision(false);

        _health = GetNodeOrNull<Health>("Health");
        _posture = GetNodeOrNull<PostureComponent>("Posture");
        if (_health is not null)
        {
            _health.Died += OnDied;
            _health.Changed += OnHealthChanged;
            OnHealthChanged(_health.CurrentHealth, _health.MaxHealth);
        }

        if (_posture is not null)
        {
            _posture.PostureBroken += OnPostureBroken;
            if (GetNodeOrNull<EnemyPostureBar>("PostureBar") is null)
            {
                EnemyPostureBar bar = new() { Name = "PostureBar" };
                AddChild(bar);
            }
        }

        _parryMarker = GetNodeOrNull<ParryTelegraphMarker>("ParryMarker");
        if (_parryMarker is null)
        {
            _parryMarker = new ParryTelegraphMarker
            {
                Name = "ParryMarker",
                Position = new Vector2(0f, -108f),
            };
            AddChild(_parryMarker);
        }

        TelegraphDuration = Mathf.Max(TelegraphDuration, CombatPacing.MinEnemyTelegraph);
        AttackCooldown = Mathf.Max(AttackCooldown, CombatPacing.MinEnemyAttackCooldown);
        AttackDuration = Mathf.Max(AttackDuration, CombatPacing.MinEnemyAttackDuration);
        MoveSpeed = Mathf.Min(MoveSpeed * EnemyCombatProfile.GetMoveSpeedMultiplier(_enemyPreset), 125f);
        AttackRangeX *= EnemyCombatProfile.GetAggroBias(_enemyPreset);
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;

        if (_dying)
        {
            Velocity = Velocity.MoveToward(Vector2.Zero, 720f * dt);
            MoveAndSlide();
            GlobalPosition = new Vector2(GlobalPosition.X, Mathf.Clamp(GlobalPosition.Y, MinLaneY, MaxLaneY));
            UpdateLocomotionVisual();
            return;
        }

        _cooldownRemaining = Mathf.Max(_cooldownRemaining - dt, 0f);
        TickTelegraph(dt);
        TickAttack(dt);

        if (TickHitStun(dt))
        {
            MoveAndSlide();
            GlobalPosition = new Vector2(GlobalPosition.X, Mathf.Clamp(GlobalPosition.Y, MinLaneY, MaxLaneY));
            return;
        }

        if (_posture?.IsBroken == true)
        {
            Velocity = Velocity.MoveToward(Vector2.Zero, 900f * dt);
            MoveAndSlide();
            UpdateLocomotionVisual();
            return;
        }

        _target ??= GetTree().GetFirstNodeInGroup("side_player") as Node2D;
        _target ??= GetTree().GetFirstNodeInGroup("player") as Node2D;
        if (_target is null)
        {
            return;
        }

        Vector2 deltaToTarget = _target.GlobalPosition - GlobalPosition;
        if (deltaToTarget.Length() > AggroRange)
        {
            Velocity = Vector2.Zero;
            MoveAndSlide();
            return;
        }

        if (Mathf.Abs(deltaToTarget.X) > 0.01f)
        {
            _facingSign = deltaToTarget.X > 0f ? 1 : -1;
        }

        UpdateAttackArc();
        UpdateFacingVisual();

        float absX = Mathf.Abs(deltaToTarget.X);
        float absY = Mathf.Abs(deltaToTarget.Y);
        bool tooClose = absX < MinStandOffX;
        bool inAttackBand = absX >= MinStandOffX && absX <= AttackRangeX && absY <= AttackRangeY;

        if (tooClose && _attackTimeRemaining <= 0f)
        {
            float retreatX = -Mathf.Sign(deltaToTarget.X) * MoveSpeed * 1.2f;
            float laneY = absY > AttackRangeY * 0.55f ? Mathf.Sign(deltaToTarget.Y) * LaneSpeed * 0.45f : 0f;
            Velocity = _telegraphRemaining > 0f
                ? new Vector2(retreatX * 0.55f, laneY)
                : new Vector2(retreatX, laneY);
        }
        else if (inAttackBand)
        {
            Velocity = Vector2.Zero;
            TryAttack();
        }
        else if (_telegraphRemaining <= 0f && _attackTimeRemaining <= 0f)
        {
            float x = absX > MinStandOffX ? Mathf.Sign(deltaToTarget.X) : 0f;
            float y = absY > AttackRangeY * 0.65f ? Mathf.Sign(deltaToTarget.Y) : 0f;
            Velocity = new Vector2(x * MoveSpeed, y * LaneSpeed);
        }

        MoveAndSlide();
        GlobalPosition = new Vector2(GlobalPosition.X, Mathf.Clamp(GlobalPosition.Y, MinLaneY, MaxLaneY));
        UpdateLocomotionVisual();
    }

    private void TryAttack()
    {
        if (_cooldownRemaining > 0f || _telegraphRemaining > 0f || _attackTimeRemaining > 0f)
        {
            return;
        }

        _cooldownRemaining = AttackCooldown;
        _pendingAttack = EnemyCombatProfile.Resolve(_enemyPreset, _attackPatternIndex);
        _telegraphRemaining = TelegraphDuration * _pendingAttack.TelegraphMultiplier;
        _spriteVisual?.SetAttackCombo(_pendingAttack.VisualComboIndex);
        _parryMarker?.SetActive(true);
    }

    private void TickTelegraph(float dt)
    {
        if (_telegraphRemaining <= 0f)
        {
            return;
        }

        _telegraphRemaining -= dt;

        if (_telegraphRemaining <= 0f)
        {
            _parryMarker?.SetActive(false);
            _attackTimeRemaining = AttackDuration * _pendingAttack.DurationMultiplier;
            _attackPatternIndex = (_attackPatternIndex + 1) % 3;
            _spriteVisual?.SetAttackCombo(_pendingAttack.VisualComboIndex);
            _spriteVisual?.BeginEnemyStrike(_attackTimeRemaining, _pendingAttack.VisualComboIndex, _pendingAttack.Anim);
            SetAttackCollision(false);
        }
    }

    private void TickAttack(float dt)
    {
        if (_attackTimeRemaining <= 0f)
        {
            return;
        }

        _attackTimeRemaining -= dt;

        if (AttackDuration > 0f)
        {
            float progress = 1f - (_attackTimeRemaining / AttackDuration);
            bool inWindow = CombatPacing.IsInHitWindow(
                progress,
                CombatPacing.EnemyHitWindowStart,
                CombatPacing.EnemyHitWindowEnd);
            SetAttackCollision(inWindow);
        }

        if (_attackTimeRemaining <= 0f)
        {
            SetAttackCollision(false);
        }
    }

    private bool TickHitStun(float dt)
    {
        if (_hitStunRemaining <= 0f)
        {
            return false;
        }

        _hitStunRemaining -= dt;
        Velocity = Velocity.MoveToward(Vector2.Zero, 1050f * dt);
        if (_hitStunRemaining <= 0f)
        {
            Velocity = Vector2.Zero;
            Modulate = Colors.White;
        }

        return true;
    }

    private void UpdateAttackArc()
    {
        if (_attackArea is null)
        {
            return;
        }

        _attackArea.Position = new Vector2(_facingSign * (38f * _pendingAttack.RangeMultiplier), -4f);
        _attackArea.Rotation = _facingSign > 0 ? 0f : Mathf.Pi;
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

    public void ReceiveKnockback(Vector2 impulse, float duration)
    {
        if (_dying)
        {
            return;
        }

        _telegraphRemaining = 0f;
        _attackTimeRemaining = 0f;
        _parryMarker?.SetActive(false);
        _hitStunRemaining = Mathf.Max(_hitStunRemaining, duration);
        Velocity = impulse;
        Modulate = Colors.White.Lerp(new Color(1f, 0.62f, 0.58f), 0.35f);
        SetAttackCollision(false);
        _spriteVisual?.PlayHitReaction(impulse.Normalized(), Mathf.Clamp(duration / 0.14f, 0.8f, 2.2f));
    }

    private void UpdateFacingVisual()
    {
        _spriteVisual?.SetFacing(_facingSign);
    }

    private void UpdateLocomotionVisual()
    {
        if (_dying)
        {
            return;
        }

        bool moving = Velocity.LengthSquared() > 225f
            && _telegraphRemaining <= 0f
            && _attackTimeRemaining <= 0f
            && _hitStunRemaining <= 0f;
        _spriteVisual?.UpdateLocomotion(
            moving,
            _attackTimeRemaining > 0f,
            false,
            _hitStunRemaining > 0f,
            _telegraphRemaining > 0f);
    }

    private void OnHealthChanged(int current, int maximum)
    {
        _spriteVisual?.SetDamageVisualTier(EnemyDamageState.FromHealth(current, maximum));
    }

    public void OnParried(SideScrollerPlayerController player)
    {
        _telegraphRemaining = 0f;
        _attackTimeRemaining = 0f;
        _parryMarker?.SetActive(false);
        _hitStunRemaining = Mathf.Max(_hitStunRemaining, 0.55f);
        SetAttackCollision(false);
        Velocity = (GlobalPosition - player.GlobalPosition).Normalized() * 240f;
        Modulate = new Color(0.75f, 0.88f, 1f, 1f);
        _spriteVisual?.PlayParryStagger();
        _spriteVisual?.PlayHitReaction(Velocity.Normalized(), 1.6f);
    }

    private void OnPostureBroken()
    {
        _telegraphRemaining = 0f;
        _attackTimeRemaining = 0f;
        _parryMarker?.SetActive(false);
        _hitStunRemaining = 0f;
        SetAttackCollision(false);
        Velocity = Vector2.Zero;
        Modulate = new Color(1f, 0.72f, 0.38f);
    }

    private void OnDied()
    {
        if (_dying)
        {
            return;
        }

        _dying = true;
        _telegraphRemaining = 0f;
        _attackTimeRemaining = 0f;
        _parryMarker?.SetActive(false);
        SetAttackCollision(false);
        Velocity = Vector2.Zero;
        CollisionLayer = 0;
        CollisionMask = 0;

        if (GetNodeOrNull<Area2D>("Hurtbox") is Area2D hurtbox)
        {
            hurtbox.Monitoring = false;
            hurtbox.Monitorable = false;
        }

        if (GetNodeOrNull<CollisionShape2D>("CollisionShape2D") is CollisionShape2D bodyShape)
        {
            bodyShape.Disabled = true;
        }

        _spriteVisual?.PlayDeath();
        CombatFeedback.PlayHit(this, this, 8);
        GetTree().CreateTimer(CombatPacing.DeathBodySeconds).Timeout += () =>
        {
            if (IsInstanceValid(this))
            {
                QueueFree();
            }
        };
    }
}
