namespace SangueNoAsfalto.Ui;

/// <summary>
/// Tema escuro com borda vermelha — alinhado ao HUD das referências.
/// </summary>
public static class GameUiTheme
{
    private static Theme? _cached;

    public static Theme BeatEmUp()
    {
        if (_cached is not null)
        {
            return _cached;
        }

        var theme = new Theme();

        var panel = new StyleBoxFlat
        {
            BgColor = new Color(0.035f, 0.038f, 0.042f, 0.92f),
            BorderColor = new Color(0.55f, 0.08f, 0.06f, 0.95f),
            BorderWidthLeft = 2,
            BorderWidthTop = 2,
            BorderWidthRight = 2,
            BorderWidthBottom = 2,
            CornerRadiusTopLeft = 4,
            CornerRadiusTopRight = 4,
            CornerRadiusBottomLeft = 4,
            CornerRadiusBottomRight = 4,
            ShadowColor = new Color(0f, 0f, 0f, 0.45f),
            ShadowSize = 6,
            ContentMarginLeft = 12,
            ContentMarginTop = 10,
            ContentMarginRight = 12,
            ContentMarginBottom = 10,
        };
        theme.SetStylebox("panel", "PanelContainer", panel);

        theme.SetColor("font_color", "Label", new Color(0.88f, 0.84f, 0.76f));
        theme.SetColor("font_shadow_color", "Label", new Color(0f, 0f, 0f, 0.7f));
        theme.SetConstant("shadow_offset_x", "Label", 1);
        theme.SetConstant("shadow_offset_y", "Label", 1);
        theme.SetFontSize("font_size", "Label", 15);

        theme.SetStylebox("background", "ProgressBar", BuildBarBackground(new Color(0.08f, 0.08f, 0.09f, 0.95f)));
        theme.SetStylebox("fill", "ProgressBar", BuildBarFill(new Color(0.62f, 0.09f, 0.07f)));

        _cached = theme;
        return theme;
    }

    public static void ApplyBarColors(ProgressBar bar, Color fillColor)
    {
        bar.AddThemeStyleboxOverride("background", BuildBarBackground(new Color(0.08f, 0.08f, 0.09f, 0.95f)));
        bar.AddThemeStyleboxOverride("fill", BuildBarFill(fillColor));
    }

    private static StyleBoxFlat BuildBarBackground(Color bg)
    {
        return new StyleBoxFlat
        {
            BgColor = bg,
            BorderColor = new Color(0.22f, 0.22f, 0.24f, 0.8f),
            BorderWidthLeft = 1,
            BorderWidthTop = 1,
            BorderWidthRight = 1,
            BorderWidthBottom = 1,
            CornerRadiusTopLeft = 2,
            CornerRadiusTopRight = 2,
            CornerRadiusBottomLeft = 2,
            CornerRadiusBottomRight = 2,
        };
    }

    private static StyleBoxFlat BuildBarFill(Color fill)
    {
        return new StyleBoxFlat
        {
            BgColor = fill,
            CornerRadiusTopLeft = 2,
            CornerRadiusTopRight = 2,
            CornerRadiusBottomLeft = 2,
            CornerRadiusBottomRight = 2,
        };
    }
}
