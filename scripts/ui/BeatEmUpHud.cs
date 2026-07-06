namespace SangueNoAsfalto.Ui;

using SangueNoAsfalto.Combat;
using SangueNoAsfalto.Enemies;

public partial class BeatEmUpHud : CanvasLayer
{
    private Label? _levelLabel;
    private Label? _healthLabel;
    private Label? _staminaLabel;
    private Label? _xpLabel;
    private Label? _weaponLabel;
    private Label? _debugLabel;
    private Label? _styleToast;
    private Label? _parryHint;
    private Label? _techniquesLabel;
    private ProgressBar? _healthBar;
    private ProgressBar? _staminaBar;
    private ProgressBar? _xpBar;
    private PanelContainer? _centerOverlay;
    private Label? _overlayTitleLabel;
    private Label? _overlayBodyLabel;
    private ProgressBar? _postureBar;
    private SideScrollerPlayerController? _player;
    private SideScrollerDirector? _director;
    private float _styleToastRemaining;

    public override void _Ready()
    {
        Theme theme = GameUiTheme.BeatEmUp();
        ApplyThemeRecursive(this, theme);

        _levelLabel = GetNodeOrNull<Label>("PlayerPanel/StatsColumn/LevelLabel");
        _healthLabel = GetNodeOrNull<Label>("PlayerPanel/StatsColumn/HealthLabel");
        _staminaLabel = GetNodeOrNull<Label>("PlayerPanel/StatsColumn/StaminaLabel");
        _xpLabel = GetNodeOrNull<Label>("PlayerPanel/StatsColumn/XpLabel");
        _weaponLabel = GetNodeOrNull<Label>("PlayerPanel/StatsColumn/WeaponLabel");
        _techniquesLabel = GetNodeOrNull<Label>("PlayerPanel/StatsColumn/TechniquesLabel");
        _debugLabel = GetNodeOrNull<Label>("DebugLabel");
        _healthBar = GetNodeOrNull<ProgressBar>("PlayerPanel/StatsColumn/HealthBar");
        _staminaBar = GetNodeOrNull<ProgressBar>("PlayerPanel/StatsColumn/StaminaBar");
        _xpBar = GetNodeOrNull<ProgressBar>("PlayerPanel/StatsColumn/XpBar");
        _postureBar = GetNodeOrNull<ProgressBar>("PlayerPanel/StatsColumn/PostureBar");
        _centerOverlay = GetNodeOrNull<PanelContainer>("CenterOverlay");
        _overlayTitleLabel = GetNodeOrNull<Label>("CenterOverlay/VBoxContainer/OverlayTitle");
        _overlayBodyLabel = GetNodeOrNull<Label>("CenterOverlay/VBoxContainer/OverlayBody");
        _player = GetTree().GetFirstNodeInGroup("side_player") as SideScrollerPlayerController;
        _director = GetTree().GetFirstNodeInGroup("side_director") as SideScrollerDirector;

        _styleToast = new Label
        {
            Name = "StyleUnlockToast",
            HorizontalAlignment = HorizontalAlignment.Center,
            Visible = false,
            ZIndex = 40,
        };
        _styleToast.AddThemeFontSizeOverride("font_size", 22);
        _styleToast.AddThemeColorOverride("font_color", new Color(0.98f, 0.88f, 0.52f));
        _styleToast.SetAnchorsPreset(Control.LayoutPreset.CenterTop);
        _styleToast.OffsetTop = 72f;
        _styleToast.OffsetBottom = 108f;
        _styleToast.OffsetLeft = -220f;
        _styleToast.OffsetRight = 220f;
        AddChild(_styleToast);

        _parryHint = new Label
        {
            Name = "ParryHint",
            Text = "SEGURE Q = defender  |  TOQUE Q no !PARRY! = parry",
            HorizontalAlignment = HorizontalAlignment.Center,
            Visible = false,
            ZIndex = 41,
        };
        _parryHint.AddThemeFontSizeOverride("font_size", 24);
        _parryHint.AddThemeColorOverride("font_color", new Color(1f, 0.82f, 0.28f));
        _parryHint.SetAnchorsPreset(Control.LayoutPreset.CenterTop);
        _parryHint.OffsetTop = 108f;
        _parryHint.OffsetBottom = 144f;
        _parryHint.OffsetLeft = -180f;
        _parryHint.OffsetRight = 180f;
        AddChild(_parryHint);

        if (_techniquesLabel is null)
        {
            _techniquesLabel = new Label
            {
                Name = "TechniquesLabel",
            };
            _techniquesLabel.AddThemeFontSizeOverride("font_size", 13);
            _techniquesLabel.AddThemeColorOverride("font_color", new Color(0.82f, 0.78f, 0.62f));
            GetNodeOrNull<VBoxContainer>("PlayerPanel/StatsColumn")?.AddChild(_techniquesLabel);
        }

        if (_player is not null)
        {
            _player.StyleUnlocked += OnStyleUnlocked;
        }

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

        if (_postureBar is not null)
        {
            GameUiTheme.ApplyBarColors(_postureBar, new Color(0.78f, 0.58f, 0.14f));
        }

        if (_levelLabel is not null)
        {
            _levelLabel.AddThemeFontSizeOverride("font_size", 18);
            _levelLabel.AddThemeColorOverride("font_color", new Color(0.95f, 0.82f, 0.62f));
        }
    }

    public override void _Process(double delta)
    {
        float dt = (float)delta;
        if (_styleToastRemaining > 0f)
        {
            _styleToastRemaining = Mathf.Max(_styleToastRemaining - dt, 0f);
            if (_styleToast is not null)
            {
                _styleToast.Visible = _styleToastRemaining > 0f;
                _styleToast.Modulate = new Color(1f, 1f, 1f, Mathf.Clamp(_styleToastRemaining / 0.35f, 0f, 1f));
            }
        }

        if (_player is not null)
        {
            if (_levelLabel is not null)
            {
                Color styleColor = _player.ActiveCombatStyle switch
                {
                    CombatStyleKind.Boxe => new Color(1f, 0.55f, 0.22f),
                    CombatStyleKind.MuayThai => new Color(1f, 0.88f, 0.28f),
                    CombatStyleKind.Capoeira => new Color(0.5f, 0.98f, 0.45f),
                    _ => new Color(0.95f, 0.82f, 0.62f),
                };
                _levelLabel.Text = $"CAUA  Nv {_player.Level}  [{_player.CombatStyleName}]";
                _levelLabel.AddThemeColorOverride("font_color", styleColor);
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

            if (_postureBar is not null && _player.Posture is not null)
            {
                _postureBar.MaxValue = _player.Posture.MaxPosture;
                _postureBar.Value = _player.Posture.CurrentPosture;
            }

            if (_weaponLabel is not null)
            {
                string durability = _player.WeaponDurability > 0
                    ? $" ({_player.WeaponDurability})"
                    : string.Empty;
                string reload = _player.IsReloading ? "  [REC]" : string.Empty;
                string nextStyle = _player.NextStyleUnlock is StyleUnlockInfo next
                    ? $"  |  Prox: {next.DisplayName} (Nv {next.Level})"
                    : string.Empty;
                _weaponLabel.Text = $"Estilo: {_player.CombatStyleName}{nextStyle}  |  {_player.WeaponName}{durability}  |  Pistola {_player.SidearmAmmo}/{_player.SidearmMaxAmmo}{reload}";
            }

            if (_techniquesLabel is not null)
            {
                string current = MoveCatalog.GetComboMoveName(
                    _player.ActiveCombatStyle,
                    _player.ComboChainSlot,
                    _player.IsRunning);
                string next = MoveCatalog.GetNextComboMoveName(
                    _player.ActiveCombatStyle,
                    _player.ComboChainSlot,
                    _player.IsRunning);
                string lastMove = string.IsNullOrEmpty(_player.LastMoveDisplayName)
                    ? string.Empty
                    : $"  |  ultimo: {_player.LastMoveDisplayName}";
                _techniquesLabel.Text = $"Golpe: {current}  ->  prox: {next}{lastMove}";
            }

            UpdateParryHint();
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

    private void UpdateParryHint()
    {
        if (_parryHint is null || _player is null)
        {
            return;
        }

        bool show = false;
        foreach (Node node in GetTree().GetNodesInGroup("side_enemy"))
        {
            if (node is not SideScrollerEnemyController enemy || !enemy.IsTelegraphing || enemy.TelegraphUrgency < 0.45f)
            {
                continue;
            }

            Vector2 delta = enemy.GlobalPosition - _player.GlobalPosition;
            if (Mathf.Abs(delta.X) <= 92f && Mathf.Abs(delta.Y) <= 58f)
            {
                show = true;
                break;
            }
        }

        _parryHint.Visible = show && !ScreenshotMode.IsActive;
        if (show)
        {
            float pulse = 0.75f + Mathf.Sin(Time.GetTicksMsec() * 0.016f) * 0.25f;
            _parryHint.Modulate = new Color(1f, 1f, 1f, pulse);
        }
    }

    private void OnStyleUnlocked(StyleUnlockInfo info)
    {
        if (_styleToast is null)
        {
            return;
        }

        _styleToast.Text = $"Estilo desbloqueado: {info.DisplayName} — {info.Tagline}";
        _styleToast.Visible = true;
        _styleToast.Modulate = Colors.White;
        _styleToastRemaining = 3.2f;
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
