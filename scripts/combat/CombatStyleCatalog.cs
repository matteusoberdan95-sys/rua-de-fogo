namespace SangueNoAsfalto.Combat;

public enum CombatStyleKind
{
    Rua = 0,
    Boxe = 1,
    MuayThai = 2,
    Karate = 3,
    Capoeira = 4,
    JiuJitsu = 5,
}

public readonly record struct StyleUnlockInfo(CombatStyleKind Kind, int Level, string DisplayName, string Tagline);

public static class CombatStyleCatalog
{
    private static readonly StyleUnlockInfo[] UnlockOrder =
    [
        new(CombatStyleKind.Rua, 1, "Rua", "Sobrevivencia"),
        new(CombatStyleKind.Boxe, 3, "Boxe", "Pressao e ritmo"),
        new(CombatStyleKind.MuayThai, 5, "Muay Thai", "Clinch e pernas"),
        new(CombatStyleKind.Karate, 7, "Karate", "Linha e impacto"),
        new(CombatStyleKind.Capoeira, 9, "Capoeira", "Mobilidade"),
        new(CombatStyleKind.JiuJitsu, 11, "Jiu-Jitsu", "Controle"),
    ];

    public static CombatStyleKind GetActiveStyle(int playerLevel)
    {
        CombatStyleKind active = CombatStyleKind.Rua;
        foreach (StyleUnlockInfo info in UnlockOrder)
        {
            if (playerLevel >= info.Level)
            {
                active = info.Kind;
            }
        }

        return active;
    }

    public static string GetDisplayName(CombatStyleKind kind)
    {
        foreach (StyleUnlockInfo info in UnlockOrder)
        {
            if (info.Kind == kind)
            {
                return info.DisplayName;
            }
        }

        return "Rua";
    }

    public static StyleUnlockInfo? GetNextUnlock(int playerLevel)
    {
        foreach (StyleUnlockInfo info in UnlockOrder)
        {
            if (playerLevel < info.Level)
            {
                return info;
            }
        }

        return null;
    }

    public static StyleUnlockInfo? GetUnlockAtLevel(int level)
    {
        foreach (StyleUnlockInfo info in UnlockOrder)
        {
            if (info.Level == level)
            {
                return info;
            }
        }

        return null;
    }

    public static float GetDamageMultiplier(CombatStyleKind kind, int comboIndex)
    {
        return kind switch
        {
            CombatStyleKind.Boxe => comboIndex switch
            {
                0 => 1.12f,
                1 => 1.08f,
                _ => 1.05f,
            },
            CombatStyleKind.MuayThai => comboIndex switch
            {
                1 => 1.18f,
                0 => 1.06f,
                _ => 1.1f,
            },
            CombatStyleKind.Karate => comboIndex == 2 ? 1.2f : 1.07f,
            CombatStyleKind.Capoeira => comboIndex == 2 ? 1.15f : 1.04f,
            CombatStyleKind.JiuJitsu => comboIndex == 0 ? 1.1f : 1.06f,
            _ => 1f,
        };
    }

    public static float GetStaminaCostMultiplier(CombatStyleKind kind)
    {
        return kind switch
        {
            CombatStyleKind.Boxe => 0.92f,
            CombatStyleKind.MuayThai => 1.05f,
            CombatStyleKind.Karate => 1f,
            CombatStyleKind.Capoeira => 0.95f,
            CombatStyleKind.JiuJitsu => 0.98f,
            _ => 1f,
        };
    }
}
