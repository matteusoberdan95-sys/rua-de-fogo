namespace SangueNoAsfalto.Ui;

/// <summary>
/// Aviso visual acima do inimigo — indica janela de parry (Q).
/// </summary>
public partial class ParryTelegraphMarker : Node2D
{
    private Label? _label;
    private ColorRect? _glow;
    private bool _active;

    public override void _Ready()
    {
        BuildMarker();
        Visible = false;
    }

    public void SetActive(bool active)
    {
        _active = active;
        Visible = active;
    }

    public override void _Process(double delta)
    {
        if (!_active || _label is null || _glow is null)
        {
            return;
        }

        float pulse = 0.55f + Mathf.Sin(Time.GetTicksMsec() * 0.014f) * 0.45f;
        _label.Scale = Vector2.One * (0.92f + pulse * 0.18f);
        _label.Modulate = new Color(1f, 0.28f + pulse * 0.12f, 0.12f, 0.92f + pulse * 0.08f);
        _glow.Modulate = new Color(1f, 0.15f, 0.08f, 0.18f + pulse * 0.22f);
        _glow.Size = new Vector2(52f + pulse * 10f, 18f + pulse * 4f);
        _glow.Position = new Vector2(-26f - pulse * 5f, -10f - pulse * 2f);
    }

    private void BuildMarker()
    {
        _glow = new ColorRect
        {
            Size = new Vector2(52f, 18f),
            Position = new Vector2(-26f, -10f),
            Color = new Color(1f, 0.12f, 0.06f, 0.35f),
            ZIndex = 38,
        };
        AddChild(_glow);

        _label = new Label
        {
            Text = "!  PARRY  !",
            HorizontalAlignment = HorizontalAlignment.Center,
            Position = new Vector2(-48f, -14f),
            Size = new Vector2(96f, 22f),
            ZIndex = 39,
        };
        _label.AddThemeFontSizeOverride("font_size", 14);
        _label.AddThemeColorOverride("font_color", new Color(1f, 0.92f, 0.35f));
        AddChild(_label);
    }
}
