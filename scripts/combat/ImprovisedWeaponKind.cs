namespace SangueNoAsfalto.Combat;

public enum ImprovisedWeaponKind
{
    None = 0,
    Rebar = 1,
    Hammer = 2,
    Knife = 3,
}

public static class ImprovisedWeaponCatalog
{
    public static string GetDisplayName(ImprovisedWeaponKind kind)
    {
        return kind switch
        {
            ImprovisedWeaponKind.Rebar => "Vergalhao",
            ImprovisedWeaponKind.Hammer => "Martelo",
            ImprovisedWeaponKind.Knife => "Faca",
            _ => "Punhos",
        };
    }

    public static int GetDefaultDurability(ImprovisedWeaponKind kind)
    {
        return kind switch
        {
            ImprovisedWeaponKind.Rebar => 6,
            ImprovisedWeaponKind.Hammer => 4,
            ImprovisedWeaponKind.Knife => 5,
            _ => 0,
        };
    }

    public static int GetBasicDamageBonus(ImprovisedWeaponKind kind)
    {
        return kind switch
        {
            ImprovisedWeaponKind.Rebar => 12,
            ImprovisedWeaponKind.Hammer => 18,
            ImprovisedWeaponKind.Knife => 10,
            _ => 0,
        };
    }

    public static float GetKnockbackBonus(ImprovisedWeaponKind kind)
    {
        return kind switch
        {
            ImprovisedWeaponKind.Rebar => 90f,
            ImprovisedWeaponKind.Hammer => 160f,
            ImprovisedWeaponKind.Knife => 40f,
            _ => 0f,
        };
    }

    public static int GetFinisherDamage(ImprovisedWeaponKind kind)
    {
        return kind switch
        {
            ImprovisedWeaponKind.Rebar => 120,
            ImprovisedWeaponKind.Hammer => 140,
            ImprovisedWeaponKind.Knife => 110,
            _ => 0,
        };
    }

    public static bool AppliesBleed(ImprovisedWeaponKind kind)
    {
        return kind == ImprovisedWeaponKind.Knife;
    }

    public static float BleedDuration => 2.8f;

    public static float BleedDamagePerSecond => 4.5f;
}
