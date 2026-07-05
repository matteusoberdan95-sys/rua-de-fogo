namespace SangueNoAsfalto.Pickups;

public partial class Pickup : Area2D
{
    public enum PickupKind
    {
        Heal,
        ImprovisedWeapon,
        Continue
    }

    [Export]
    public PickupKind Kind { get; set; }

    [Export]
    public int Amount { get; set; } = 35;

    [Export]
    public int WeaponDurability { get; set; } = 8;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is not SideScrollerPlayerController player)
        {
            return;
        }

        switch (Kind)
        {
            case PickupKind.Heal:
                player.Heal(Amount);
                break;
            case PickupKind.ImprovisedWeapon:
                player.EquipImprovisedWeapon(WeaponDurability);
                break;
            case PickupKind.Continue:
                player.AddContinue();
                break;
        }

        QueueFree();
    }
}
