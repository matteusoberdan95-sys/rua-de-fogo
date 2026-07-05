namespace SangueNoAsfalto.Ui;

public partial class BeatEmUpHud : CanvasLayer
{
    private Label? _levelLabel;
    private Label? _healthLabel;
    private Label? _staminaLabel;
    private Label? _xpLabel;
    private Label? _weaponLabel;
    private Label? _debugLabel;
    private ProgressBar? _healthBar;
    private ProgressBar? _staminaBar;
    private ProgressBar? _xpBar;
    private PanelContainer? _centerOverlay;
    private Label? _overlayTitleLabel;
    private Label? _overlayBodyLabel;
    private SideScrollerPlayerController? _player;
    private SideScrollerDirector? _director;

    public override void _Ready()
    {
        Theme theme = GameUiTheme.BeatEmUp();
        ApplyThemeRecursive(this, theme);

        _levelLabel = GetNodeOrNull<Label>("PlayerPanel/StatsColumn/LevelLabel");
        _healthLabel = GetNodeOrNull<Label>("PlayerPanel/StatsColumn/HealthLabel");
        _staminaLabel = GetNodeOrNull<Label>("PlayerPanel/StatsColumn/StaminaLabel");
        _xpLabel = GetNodeOrNull<Label>("PlayerPanel/StatsColumn/XpLabel");
        _weaponLabel = GetNodeOrNull<Label>("PlayerPanel/StatsColumn/WeaponLabel");
        _debugLabel = GetNodeOrNull<Label>("DebugLabel");
        _healthBar = GetNodeOrNull<ProgressBar>("PlayerPanel/StatsColumn/HealthBar");
        _staminaBar = GetNodeOrNull<ProgressBar>("PlayerPanel/StatsColumn/StaminaBar");
        _xpBar = GetNodeOrNull<ProgressBar>("PlayerPanel/StatsColumn/XpBar");
        _centerOverlay = GetNodeOrNull<PanelContainer>("CenterOverlay");
        _overlayTitleLabel = GetNodeOrNull<Label>("CenterOverlay/VBoxContainer/OverlayTitle");
        _overlayBodyLabel = GetNodeOrNull<Label>("CenterOverlay/VBoxContainer/OverlayBody");
        _player = GetTree().GetFirstNodeInGroup("side_player") as SideScrollerPlayerController;
        _director = GetTree().GetFirstNodeInGroup("side_director") as SideScrollerDirector;

        if (_player?.GetNodeOrNull<Health>("Health") is Health health)
        {
            health.Changed += OnHealthChanged;
            OnHealthChanged(health.CurrentHealth, health.MaxHealth);
        }

        if (_healthBar is not null)
        {
            GameUiTheme.ApplyBarColors(_healthBar, new Color(0.62f, 0.09f, 0.07f));
        }

        if (_staminaBar is not null)
        {
            GameUiTheme.ApplyBarColors(_staminaBar, new Color(0.72f, 0.58f, 0.18f));
        }

        if (_xpBar is not null)
        {
            GameUiTheme.ApplyBarColors(_xpBar, new Color(0.18f, 0.42f, 0.58f));
        }

        if (_levelLabel is not null)
        {
            _levelLabel.AddThemeFontSizeOverride("font_size", 18);
            _levelLabel.AddThemeColorOverride("font_color", new Color(0.95f, 0.82f, 0.62f));
        }
    }

    public override void _Process(double delta)
    {
        if (_player is not null)
        {
            if (_levelLabel is not null)
            {
                _levelLabel.Text = $"CAUA  Nv {_player.Level}";
            }

            if (_staminaLabel is not null)
            {
                _staminaLabel.Text = $"Stamina {(int)_player.CurrentStamina}/{(int)_player.MaxStamina}";
            }

            if (_staminaBar is not null)
            {
                _staminaBar.MaxValue = _player.MaxStamina;
                _staminaBar.Value = _player.CurrentStamina;
            }

            if (_xpLabel is not null)
            {
                _xpLabel.Text = $"XP {(int)_player.Experience}/{(int)_player.ExperienceToNext}";
            }

            if (_xpBar is not null)
            {
                _xpBar.MaxValue = _player.ExperienceToNext;
                _xpBar.Value = _player.Experience;
            }

            if (_weaponLabel is not null)
            {
                string durability = _player.WeaponDurability > 0
                    ? $" ({_player.WeaponDurability})"
                    : string.Empty;
                _weaponLabel.Text = $"Arma: {_player.WeaponName}{durability}";
            }
        }

        if (_director is not null)
        {
            if (_debugLabel is not null)
            {
                _debugLabel.Visible = _director.ShowDebugHud && !ScreenshotMode.IsActive;
                _debugLabel.Text = $"Etapa {_director.WaveNumber}/{_director.TotalWaves}  |  Inimigos {_director.EnemiesRemaining}  |  {_director.ObjectiveText}";
            }

            UpdateCenterOverlay();
        }
    }

    private void OnHealthChanged(int current, int maximum)
    {
        if (_healthLabel is not null)
        {
            _healthLabel.Text = $"HP {current}/{maximum}";
        }

        if (_healthBar is not null)
        {
            _healthBar.MaxValue = maximum;
            _healthBar.Value = current;
        }
    }

    private void UpdateCenterOverlay()
    {
        if (_centerOverlay is null || _overlayTitleLabel is null || _overlayBodyLabel is null || _director is null)
        {
            return;
        }

        _centerOverlay.Visible = _director.IsGameOver || _director.IsCompleted;
        if (!_centerOverlay.Visible)
        {
            return;
        }

        if (_director.IsCompleted)
        {
            _overlayTitleLabel.Text = "Fim da demo";
            _overlayBodyLabel.Text = "Voce limpou este trecho da Vila Esperanca.\nR: jogar de novo\nM: voltar ao menu";
            return;
        }

        _overlayTitleLabel.Text = "Voce caiu no asfalto";
        _overlayBodyLabel.Text = _director.HasCheckpoint
            ? "R: voltar ao checkpoint\nM: voltar ao menu"
            : "R: tentar desde o inicio\nM: voltar ao menu";
    }

    private static void ApplyThemeRecursive(Node node, Theme theme)
    {
        if (node is Control control)
        {
            control.Theme = theme;
        }

        foreach (Node child in node.GetChildren())
        {
            ApplyThemeRecursive(child, theme);
        }
    }
}
