using Godot;
using SangueNoAsfalto.Core;
using SangueNoAsfalto.Player;

namespace SangueNoAsfalto.Ui;

public partial class BeatEmUpHud : CanvasLayer
{
    private Label? _healthLabel;
    private Label? _staminaLabel;
    private Label? _waveLabel;
    private Label? _statusLabel;
    private Label? _debugLabel;
    private SideScrollerPlayerController? _player;
    private SideScrollerDirector? _director;

    public override void _Ready()
    {
        _healthLabel = GetNodeOrNull<Label>("Panel/VBoxContainer/HealthLabel");
        _staminaLabel = GetNodeOrNull<Label>("Panel/VBoxContainer/StaminaLabel");
        _waveLabel = GetNodeOrNull<Label>("Panel/VBoxContainer/WaveLabel");
        _statusLabel = GetNodeOrNull<Label>("Panel/VBoxContainer/StatusLabel");
        _debugLabel = GetNodeOrNull<Label>("Panel/VBoxContainer/DebugLabel");
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
                _staminaLabel.Text = $"Stamina: {(int)_player.CurrentStamina}/{(int)_player.MaxStamina}";
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
                _waveLabel.Text = $"Onda: {_director.WaveNumber}/{_director.TotalWaves}  Inimigos: {_director.EnemiesRemaining}";
            }

            if (_statusLabel is not null)
            {
                _statusLabel.Text = _director.StatusText;
            }
        }
    }

    private void OnHealthChanged(int current, int maximum)
    {
        if (_healthLabel is not null)
        {
            _healthLabel.Text = $"Vida: {current}/{maximum}";
        }
    }
}
