namespace SangueNoAsfalto.Systems;

public sealed class GameSave
{
    public bool CheckpointUnlocked { get; set; }

    public bool HasImprovisedWeapon { get; set; }

    public int WeaponKind { get; set; }

    public int WeaponDurability { get; set; }

    public int Continues { get; set; }

    public bool ShowDebugHud { get; set; } = true;

    public bool AlternateControls { get; set; }
}
