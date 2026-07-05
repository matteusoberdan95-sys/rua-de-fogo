namespace SangueNoAsfalto.Ui;

public partial class MainMenu : Control
{
    private const string TutorialScenePath = "res://scenes/ui/TutorialScreen.tscn";

    private Label? _settingsLabel;
    private Label? _statusLabel;
    private Button? _toggleControlsButton;
    private Label? _versionLabel;

    public override void _Ready()
    {
        SaveManager.Load();
        InputBootstrap.ApplyAlternateControls(SaveManager.Current.AlternateControls);
        _settingsLabel = GetNodeOrNull<Label>("Root/SettingsPanel/SettingsText");
        _statusLabel = GetNodeOrNull<Label>("Root/StatusLabel");
        _toggleControlsButton = GetNodeOrNull<Button>("Root/MenuButtons/ToggleControlsButton");
        _versionLabel = GetNodeOrNull<Label>("Root/VersionLabel");
        BindButton("Root/MenuButtons/StartButton", StartDemo);
        BindButton("Root/MenuButtons/ClearSaveButton", ClearSave);
        BindButton("Root/MenuButtons/ToggleControlsButton", ToggleAlternateControls);
        BindButton("Root/MenuButtons/QuitButton", QuitIfStandalone);
        UpdateSettingsText();
        UpdateVersionLabel();
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
                StartDemo();
                break;
            case Key.C:
                ClearSave();
                break;
            case Key.F2:
                ToggleAlternateControls();
                break;
            case Key.Escape:
                QuitIfStandalone();
                break;
        }
    }

    private void StartDemo()
    {
        GetTree().ChangeSceneToFile(TutorialScenePath);
    }

    private void ClearSave()
    {
        SaveManager.Reset();
        InputBootstrap.ApplyAlternateControls(false);
        SetStatus("Save limpo. A demo vai iniciar do comeco.");
        UpdateSettingsText();
    }

    private void ToggleAlternateControls()
    {
        SaveManager.Current.AlternateControls = !SaveManager.Current.AlternateControls;
        InputBootstrap.ApplyAlternateControls(SaveManager.Current.AlternateControls);
        SaveManager.Save();
        SetStatus(SaveManager.Current.AlternateControls
            ? "Controles alternativos ativados."
            : "Controles alternativos desativados.");
        UpdateSettingsText();
    }

    private void QuitIfStandalone()
    {
        if (Engine.IsEditorHint())
        {
            SetStatus("No editor, use o botao Stop do Godot para sair.");
            return;
        }

        GetTree().Quit();
    }

    private void UpdateSettingsText()
    {
        if (_settingsLabel is null)
        {
            return;
        }

        if (_toggleControlsButton is not null)
        {
            _toggleControlsButton.Text = SaveManager.Current.AlternateControls
                ? "Desativar controles alternativos  (F2)"
                : "Ativar controles alternativos  (F2)";
        }

        string controlMode = SaveManager.Current.AlternateControls
            ? "Alternativo: H ataque, U tiro, Shift esquiva"
            : "Padrao: J ataque, L tiro, K esquiva";

        string checkpoint = SaveManager.Current.CheckpointUnlocked
            ? "Checkpoint salvo: ativo"
            : "Checkpoint salvo: nenhum";

        _settingsLabel.Text = $"{controlMode}\n{checkpoint}\nF1 no jogo: HUD debug | F2: alternar controles | F4: limpar save";
    }

    private void SetStatus(string text)
    {
        if (_statusLabel is not null)
        {
            _statusLabel.Text = text;
        }
    }

    private void UpdateVersionLabel()
    {
        if (_versionLabel is not null)
        {
            _versionLabel.Text = DemoInfo.VersionLabel;
        }
    }

    private void BindButton(string path, Action callback)
    {
        if (GetNodeOrNull<Button>(path) is Button button)
        {
            button.Pressed += callback;
        }
    }
}
