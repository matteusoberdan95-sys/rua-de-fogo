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
    private PanelContainer? _centerOverlay;
    private Label? _overlayTitleLabel;
    private Label? _overlayBodyLabel;
    private SideScrollerPlayerController? _player;
    private SideScrollerDirector? _director;

    public override void _Ready()
    {
        _healthLabel = GetNodeOrNull<Label>("Panel/VBoxContainer/HealthLabel");
        _staminaLabel = GetNodeOrNull<Label>("Panel/VBoxContainer/StaminaLabel");
        _waveLabel = GetNodeOrNull<Label>("Panel/VBoxContainer/WaveLabel");
        _statusLabel = GetNodeOrNull<Label>("Panel/VBoxContainer/StatusLabel");
        _controlsLabel = GetNodeOrNull<Label>("Panel/VBoxContainer/ControlsLabel");
        _debugLabel = GetNodeOrNull<Label>("Panel/VBoxContainer/DebugLabel");
        _tutorialLabel = GetNodeOrNull<Label>("TutorialPanel/TutorialLabel");
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
                string weapon = _player.WeaponDurability > 0
                    ? $"{_player.WeaponName} ({_player.WeaponDurability})"
                    : _player.WeaponName;
                _staminaLabel.Text = $"Stamina: {(int)_player.CurrentStamina}/{(int)_player.MaxStamina}  Arma: {weapon}  Continue: {_player.Continues}";
            }

            if (_debugLabel is not null)
            {
                Vector2 input = _player.LastMovementInput;
                Vector2 position = _player.GlobalPosition;
                _debugLabel.Text = $"Lane: {(int)position.Y}  Input: {input.X:0.0}, {input.Y:0.0}";
            }
        }

        if (_director is not null)
        {
            if (_waveLabel is not null)
            {
                _waveLabel.Text = $"Etapa: {_director.WaveNumber}/{_director.TotalWaves}  Inimigos: {_director.EnemiesRemaining}";
            }

            if (_statusLabel is not null)
            {
                _statusLabel.Text = _director.StatusText;
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
            _healthLabel.Text = $"Vida: {current}/{maximum}";
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
