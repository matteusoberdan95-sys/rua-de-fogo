namespace SangueNoAsfalto.Visual;

/// <summary>Cena de teste isolada para Caua production art v0.</summary>
public partial class CauaProductionArtTest : Node2D
{
    private ArtCharacterVisual? _productionVisual;
    private CharacterSpriteVisual? _fallbackVisual;
    private Label? _statusLabel;

    public override void _Ready()
    {
        _productionVisual = GetNodeOrNull<ArtCharacterVisual>("CauaActor/ProductionArtVisual");
        _fallbackVisual = GetNodeOrNull<CharacterSpriteVisual>("CauaActor/SpriteVisual");
        _statusLabel = GetNodeOrNull<Label>("Hud/StatusLabel");

        bool active = _productionVisual?.TryActivate(_fallbackVisual, ProductionCharacterId.Caua) == true;
        UpdateStatus(active);
    }

    public override void _Process(double delta)
    {
        if (_productionVisual is null || !_productionVisual.IsProductionArtActive)
        {
            return;
        }

        bool moving = Input.IsActionPressed("move_right") || Input.IsActionPressed("move_left");
        bool attacking = Input.IsActionJustPressed("attack");
        _productionVisual.UpdateLocomotion(moving, attacking, false);

        if (Input.IsActionJustPressed("attack"))
        {
            _productionVisual.SetAttackMove(MoveAnimProfile.Jab, 0, CombatStyleKind.Rua, 0.32f);
        }

        if (Input.IsKeyPressed(Key.Key2))
        {
            _productionVisual.SetAttackMove(MoveAnimProfile.Cross, 1, CombatStyleKind.Rua, 0.34f);
        }

        int facing = Input.IsActionPressed("move_left") ? -1 : 1;
        _productionVisual.SetFacing(facing);
    }

    private void UpdateStatus(bool productionActive)
    {
        if (_statusLabel is null)
        {
            return;
        }

        _statusLabel.Text = productionActive
            ? "Caua production art v0 ATIVO (art/production/characters/caua/)\nA/D mover | J jab | 2 cross"
            : "Fallback procedural — pacote production art nao encontrado";
    }
}
