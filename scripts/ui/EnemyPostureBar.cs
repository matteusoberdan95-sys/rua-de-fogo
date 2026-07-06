namespace SangueNoAsfalto.Ui;

using SangueNoAsfalto.Combat;

/// <summary>
/// Barra de postura flutuante acima do inimigo — leitura estilo Sekiro.
/// </summary>
public partial class EnemyPostureBar : Node2D
{
    [Export]
    public NodePath PosturePath { get; set; } = "../Posture";

    [Export]
    public Vector2 Offset { get; set; } = new(0f, -92f);

    private PostureComponent? _posture;
    private ColorRect? _back;
    private ColorRect? _fill;
    private ColorRect? _brokenFlash;

    public override void _Ready()
    {
        Position = Offset;
        _posture = GetNodeOrNull<PostureComponent>(PosturePath);
        BuildBar();
        if (_posture is not null)
        {
            _posture.PostureChanged += OnPostureChanged;
            OnPostureChanged(_posture.CurrentPosture, _posture.MaxPosture);
        }
    }

    public override void _Process(double delta)
    {
        if (_posture is null || _fill is null || _back is null)
        {
            Visible = false;
            return;
        }

        bool show = _posture.CurrentPosture > 4f || _posture.IsBroken;
        Visible = show;
        if (!show)
        {
            return;
        }

        float ratio = _posture.MaxPosture > 0f
            ? Mathf.Clamp(_posture.CurrentPosture / _posture.MaxPosture, 0f, 1f)
            : 0f;

        _fill.Size = new Vector2(44f * ratio, 5f);
        _fill.Color = _posture.IsBroken
            ? new Color(1f, 0.78f, 0.18f, 0.95f)
            : ratio > 0.72f
                ? new Color(0.95f, 0.42f, 0.12f, 0.92f)
                : new Color(0.82f, 0.72f, 0.22f, 0.88f);

        if (_brokenFlash is not null)
        {
            _brokenFlash.Visible = _posture.IsBroken;
            _brokenFlash.Modulate = new Color(1f, 1f, 1f, 0.45f + Mathf.Sin(Time.GetTicksMsec() * 0.018f) * 0.35f);
        }
    }

    private void BuildBar()
    {
        _back = new ColorRect
        {
            Size = new Vector2(44f, 5f),
            Position = new Vector2(-22f, 0f),
            Color = new Color(0.08f, 0.06f, 0.05f, 0.82f),
            ZIndex = 30,
        };
        AddChild(_back);

        _fill = new ColorRect
        {
            Size = new Vector2(44f, 5f),
            Position = new Vector2(-22f, 0f),
            Color = new Color(0.82f, 0.72f, 0.22f, 0.88f),
            ZIndex = 31,
        };
        AddChild(_fill);

        _brokenFlash = new ColorRect
        {
            Size = new Vector2(48f, 9f),
            Position = new Vector2(-24f, -2f),
            Color = new Color(1f, 0.35f, 0.08f, 0.35f),
            Visible = false,
            ZIndex = 29,
        };
        AddChild(_brokenFlash);
    }

    private void OnPostureChanged(float current, float maximum)
    {
        if (_fill is null)
        {
            return;
        }

        float ratio = maximum > 0f ? current / maximum : 0f;
        _fill.Size = new Vector2(44f * ratio, 5f);
    }
}
