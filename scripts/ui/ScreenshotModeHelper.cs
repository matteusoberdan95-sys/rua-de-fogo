namespace SangueNoAsfalto.Ui;

public static class ScreenshotMode
{
    public static bool IsActive { get; private set; }

    public static void Toggle()
    {
        IsActive = !IsActive;
    }
}

public partial class ScreenshotModeHelper : Node
{
    private Label? _hintLabel;
    private bool _f9WasDown;

    public override void _Ready()
    {
        AddToGroup("screenshot_helper");
        _hintLabel = GetParent()?.GetNodeOrNull<Label>("ScreenshotHintLabel");
        UpdateHint();
    }

    public override void _Process(double delta)
    {
        if (ConsumeKeyPress(Key.F9, ref _f9WasDown))
        {
            ScreenshotMode.Toggle();
            UpdateHint();
        }
    }

    private void UpdateHint()
    {
        if (_hintLabel is null)
        {
            return;
        }

        _hintLabel.Visible = ScreenshotMode.IsActive;
        _hintLabel.Text = ScreenshotMode.IsActive
            ? "Modo screenshot ON  |  F9 desliga"
            : string.Empty;
    }

    private static bool ConsumeKeyPress(Key key, ref bool wasDown)
    {
        bool isDown = Input.IsKeyPressed(key) || Input.IsPhysicalKeyPressed(key);
        bool justPressed = isDown && !wasDown;
        wasDown = isDown;
        return justPressed;
    }
}
