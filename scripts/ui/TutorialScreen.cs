namespace SangueNoAsfalto.Ui;

public partial class TutorialScreen : Control
{
    private const string LevelScenePath = "res://scenes/levels/SideScrollerPrototype.tscn";
    private const string MenuScenePath = "res://scenes/ui/MainMenu.tscn";

    public override void _Ready()
    {
        BindButton("Root/StartButton", StartPhase);
        BindButton("Root/BackButton", ReturnToMenu);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is not InputEventKey { Pressed: true, Echo: false } keyEvent)
        {
            return;
        }

        switch (keyEvent.Keycode)
        {
            case Key.Enter:
            case Key.KpEnter:
                StartPhase();
                break;
            case Key.Escape:
                ReturnToMenu();
                break;
        }
    }

    private void StartPhase()
    {
        GetTree().ChangeSceneToFile(LevelScenePath);
    }

    private void ReturnToMenu()
    {
        GetTree().ChangeSceneToFile(MenuScenePath);
    }

    private void BindButton(string path, Action callback)
    {
        if (GetNodeOrNull<Button>(path) is Button button)
        {
            button.Pressed += callback;
        }
    }
}
