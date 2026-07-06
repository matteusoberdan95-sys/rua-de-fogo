namespace SangueNoAsfalto.Pickups;

public partial class Pickup : Area2D
{
    public enum PickupKind
    {
        Heal,
        ImprovisedWeapon,
        Continue,
        SidearmAmmo,
    }

    [Export]
    public PickupKind Kind { get; set; }

    [Export]
    public int Amount { get; set; } = 35;

    [Export]
    public ImprovisedWeaponKind WeaponKind { get; set; } = ImprovisedWeaponKind.Rebar;

    [Export]
    public int WeaponDurability { get; set; }

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;

        if (Kind == PickupKind.ImprovisedWeapon && WeaponDurability <= 0)
        {
            WeaponDurability = ImprovisedWeaponCatalog.GetDefaultDurability(WeaponKind);
        }
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
                player.EquipImprovisedWeapon(WeaponKind, WeaponDurability);
                break;
            case PickupKind.Continue:
                player.AddContinue();
                break;
            case PickupKind.SidearmAmmo:
                player.AddSidearmAmmo(Amount > 0 ? Amount : 3);
                break;
        }

        QueueFree();
    }
}
