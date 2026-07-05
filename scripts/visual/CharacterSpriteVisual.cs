namespace SangueNoAsfalto.Visual;

/// <summary>
/// Controle visual de personagem com AnimatedSprite2D — flip, pulo e estados de animação.
/// </summary>
public partial class CharacterSpriteVisual : Node2D
{
    [Export]
    public NodePath SpritePath { get; set; } = new("AnimatedSprite2D");

    [Export]
    public float DisplayScale { get; set; } = 0.11f;

    [Export]
    public bool SourceFacesRight { get; set; } = true;

    private AnimatedSprite2D? _sprite;
    private Vector2 _basePosition;
    private Vector2 _jumpOffset;
    private Vector2 _movementOffset;
    private string _currentAnim = string.Empty;

    public override void _Ready()
    {
        _basePosition = Position;
        _sprite = GetNodeOrNull<AnimatedSprite2D>(SpritePath);
        if (_sprite is null)
        {
            return;
        }

        _sprite.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
        _sprite.Scale = Vector2.One * DisplayScale;
    }

    public void SetFacing(int sign)
    {
        if (_sprite is null || sign == 0)
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
}
