namespace SangueNoAsfalto.Ui;

public partial class BeatEmUpHud : CanvasLayer
{
    private Label? _healthLabel;
    private Label? _staminaLabel;
    private Label? _waveLabel;
    private Label? _statusLabel;
    private Label? _controlsLabel;
    private Label? _debugLabel;
    private Label? _tutorialLabel;
    private Label? _stageTitleLabel;
    private Label? _stageTaglineLabel;
    private Label? _playerNameLabel;
    private Label? _weaponSlotLabel;
    private Label? _comboCalloutLabel;
    private Label? _comboStatsLabel;
    private ProgressBar? _healthBar;
    private ProgressBar? _staminaBar;
    private ProgressBar? _furyBar;
    private PanelContainer? _centerOverlay;
    private Label? _overlayTitleLabel;
    private Label? _overlayBodyLabel;
    private SideScrollerPlayerController? _player;
    private SideScrollerDirector? _director;

    public override void _Ready()
    {
        _healthLabel = GetNodeOrNull<Label>("PlayerPanel/StatsColumn/HealthLabel");
        _staminaLabel = GetNodeOrNull<Label>("PlayerPanel/StatsColumn/StaminaLabel");
        _waveLabel = GetNodeOrNull<Label>("PlayerPanel/StatsColumn/WaveLabel");
        _statusLabel = GetNodeOrNull<Label>("PlayerPanel/StatsColumn/StatusLabel");
        _controlsLabel = GetNodeOrNull<Label>("BottomBar/ControlsLabel");
        _debugLabel = GetNodeOrNull<Label>("BottomBar/DebugLabel");
        _tutorialLabel = GetNodeOrNull<Label>("TutorialPanel/TutorialLabel");
        _stageTitleLabel = GetNodeOrNull<Label>("StageBanner/StageTitleLabel");
        _stageTaglineLabel = GetNodeOrNull<Label>("StageBanner/StageTaglineLabel");
        _playerNameLabel = GetNodeOrNull<Label>("PlayerPanel/StatsColumn/PlayerNameLabel");
        _weaponSlotLabel = GetNodeOrNull<Label>("WeaponStrip/WeaponSlotLabel");
        _comboCalloutLabel = GetNodeOrNull<Label>("ComboCallout");
        _comboStatsLabel = GetNodeOrNull<Label>("ComboStatsLabel");
        _healthBar = GetNodeOrNull<ProgressBar>("PlayerPanel/StatsColumn/HealthBar");
        _staminaBar = GetNodeOrNull<ProgressBar>("PlayerPanel/StatsColumn/StaminaBar");
        _furyBar = GetNodeOrNull<ProgressBar>("PlayerPanel/StatsColumn/FuryBar");
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
    }

    public override void _Process(double delta)
    {
        if (_player is not null)
        {
            if (_staminaLabel is not null)
            {
                _staminaLabel.Text = $"Stamina {(int)_player.CurrentStamina}/{(int)_player.MaxStamina}";
            }

            if (_staminaBar is not null)
            {
                _staminaBar.MaxValue = _player.MaxStamina;
                _staminaBar.Value = _player.CurrentStamina;
            }

            if (_furyBar is not null)
            {
                _furyBar.Value = _player.Fury;
            }

            if (_weaponSlotLabel is not null)
            {
                string durability = _player.WeaponDurability > 0
                    ? $"Durabilidade {_player.WeaponDurability}"
                    : "Sem durabilidade extra";
                _weaponSlotLabel.Text = $"Arma: {_player.WeaponName}  |  {durability}  |  Continue x{_player.Continues}";
            }

            if (_comboCalloutLabel is not null)
            {
                _comboCalloutLabel.Visible = _player.ShowComboCallout;
                _comboCalloutLabel.Text = _player.ShowComboCallout
                    ? $"{_player.ComboCalloutText}\n{_player.ComboHitCount} HITS"
                    : string.Empty;
            }

            if (_comboStatsLabel is not null)
            {
                _comboStatsLabel.Text = _player.ComboHitCount > 0
                    ? $"Combo {_player.ComboHitCount}  |  Melhor {_player.BestCombo}"
                    : $"Melhor combo {_player.BestCombo}";
            }
        }

        if (_director is not null)
        {
            if (_waveLabel is not null)
            {
                _waveLabel.Text = $"Etapa {_director.WaveNumber}/{_director.TotalWaves}  |  Inimigos {_director.EnemiesRemaining}";
            }

            if (_statusLabel is not null)
            {
                _statusLabel.Text = _director.StatusText;
            }

            if (_stageTitleLabel is not null)
            {
                _stageTitleLabel.Text = $"ETAPA {_director.WaveNumber}: {_director.StageTitle}";
            }

            if (_stageTaglineLabel is not null)
            {
                _stageTaglineLabel.Text = _director.StageTagline;
            }

            if (_debugLabel is not null)
            {
                string checkpoint = _director.HasCheckpoint ? "Checkpoint ativo" : "Sem checkpoint";
                string controls = _director.AlternateControls ? "Alt ON" : "Alt OFF";
                _debugLabel.Visible = _director.ShowDebugHud;
                _debugLabel.Text = $"Objetivo: {_director.ObjectiveText}  |  {checkpoint}  |  {controls}";
            }

            if (_controlsLabel is not null)
            {
                _controlsLabel.Text = GetControlsText();
            }

            UpdateTutorialText();
            UpdateCenterOverlay();
        }
    }

    private void OnHealthChanged(int current, int maximum)
    {
        if (_healthLabel is not null)
        {
            _healthLabel.Text = $"Vida {current}/{maximum}";
        }

        if (_healthBar is not null)
        {
            _healthBar.MaxValue = maximum;
            _healthBar.Value = current;
        }
    }

    private string GetControlsText()
    {
        if (_director?.IsCompleted == true)
        {
            return "Fim da demo. R: jogar de novo  M: menu";
        }

        if (_director?.IsGameOver == true)
        {
            return _director.HasCheckpoint ? "R: voltar ao checkpoint  M: menu" : "R: voltar ao inicio  M: menu";
        }

        if (_director?.AlternateControls == true)
        {
            return "Alt: H ataque  U tiro  Shift esquiva  Espaco pulo  F1 HUD  F2 controles  F4 limpar save";
        }

        return "W/S: lane  A/D: andar  J: combo  L: tiro  K: esquiva  Espaco: pulo  F1 HUD  F2 alt  F4 limpar save";
    }

    private void UpdateTutorialText()
    {
        if (_tutorialLabel is null || _director is null)
        {
            return;
        }

        _tutorialLabel.Visible = !_director.IsGameOver && !_director.IsCompleted;
        _tutorialLabel.Text = _director.WaveNumber switch
        {
            1 when _director.ObjectiveText.Contains("altar", StringComparison.OrdinalIgnoreCase) =>
                "Tutorial: checkpoint ativado no altar. Se cair depois daqui, R volta para este trecho.",
            1 => "Tutorial: A/D anda, W/S troca a profundidade da rua. J ataca e L dispara.",
            2 => "Tutorial: corredores fecham rapido. Use K para esquivar e Espaco para pular por cima do caos.",
            3 => "Tutorial: brutos aguentam mais dano. Use tiro, combo e arma improvisada se tiver pego.",
            4 => "Tutorial: chefes mostram o ataque antes de bater. Saia da lane ou use esquiva.",
            5 => "Tutorial: chuva e infectados punem erro de posicionamento. Mantenha distancia curta.",
            6 => "Tutorial: chefe alpha da demo. Guarde stamina para esquivar dos golpes pesados.",
            _ => "Tutorial: pickups de cura, arma e continue aparecem pela rua. Pegue passando por cima."
        };
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
}
