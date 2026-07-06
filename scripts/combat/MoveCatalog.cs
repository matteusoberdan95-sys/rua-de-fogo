namespace SangueNoAsfalto.Combat;

public enum MoveAnimProfile
{
    Jab,
    BoxLead,
    Cross,
    Hook,
    Uppercut,
    SideKick,
    FlyingKick,
    RunningPunch,
    RunningKick,
    RunningHook,
    Teep,
    Knee,
    Elbow,
    FlyingKnee,
    MeiaLua,
    GingaKick,
}

public readonly record struct MartialMoveDefinition(
    string Id,
    string DisplayName,
    MoveAnimProfile Anim,
    int BaseDamage,
    float Duration,
    float Knockback,
    float PostureDamage,
    float StaminaCost,
    int ImpactComboIndex);

public static class MoveCatalog
{
    private static readonly MartialMoveDefinition StreetJab = Def("street_jab", "Soco", MoveAnimProfile.Jab, 24, 0.18f, 350f, 14f, 9f, 0);
    private static readonly MartialMoveDefinition StreetCross = Def("street_cross", "Cruzado", MoveAnimProfile.Cross, 28, 0.2f, 380f, 16f, 11f, 1);
    private static readonly MartialMoveDefinition StreetKick = Def("street_kick", "Chute", MoveAnimProfile.SideKick, 32, 0.22f, 400f, 18f, 13f, 2);
    private static readonly MartialMoveDefinition StreetKnee = Def("street_knee", "Joelhada", MoveAnimProfile.Knee, 38, 0.24f, 460f, 22f, 15f, 3);
    private static readonly MartialMoveDefinition StreetFlying = Def("street_flying", "Voadora", MoveAnimProfile.FlyingKick, 50, 0.28f, 520f, 22f, 22f, 3);
    private static readonly MartialMoveDefinition StreetRunPunch = Def("street_run_punch", "Soco corrida", MoveAnimProfile.RunningPunch, 26, 0.16f, 380f, 12f, 12f, 0);
    private static readonly MartialMoveDefinition StreetRunKick = Def("street_run_kick", "Chute corrida", MoveAnimProfile.RunningKick, 32, 0.18f, 400f, 16f, 16f, 1);

    private static readonly MartialMoveDefinition BoxJab = Def("box_jab", "Jab", MoveAnimProfile.BoxLead, 26, 0.18f, 340f, 15f, 9f, 0);
    private static readonly MartialMoveDefinition BoxCross = Def("box_cross", "Cross", MoveAnimProfile.Cross, 30, 0.2f, 380f, 17f, 12f, 1);
    private static readonly MartialMoveDefinition BoxHook = Def("box_hook", "Hook", MoveAnimProfile.Hook, 34, 0.22f, 420f, 19f, 13f, 2);
    private static readonly MartialMoveDefinition BoxUpper = Def("box_upper", "Uppercut", MoveAnimProfile.Uppercut, 40, 0.24f, 480f, 22f, 16f, 3);
    private static readonly MartialMoveDefinition BoxRunHook = Def("box_run_hook", "Hook corrida", MoveAnimProfile.RunningHook, 36, 0.17f, 440f, 16f, 13f, 1);

    private static readonly MartialMoveDefinition MuayJab = Def("muay_teep_open", "Teep", MoveAnimProfile.Teep, 28, 0.22f, 360f, 16f, 11f, 0);
    private static readonly MartialMoveDefinition MuayTeep = Def("muay_knee", "Joelhada", MoveAnimProfile.Knee, 38, 0.26f, 460f, 22f, 16f, 1);
    private static readonly MartialMoveDefinition MuayKnee = Def("muay_elbow", "Cotovelada", MoveAnimProfile.Elbow, 42, 0.2f, 480f, 24f, 17f, 2);
    private static readonly MartialMoveDefinition MuayElbow = Def("muay_lowkick", "Chute baixo", MoveAnimProfile.SideKick, 36, 0.22f, 440f, 20f, 14f, 3);
    private static readonly MartialMoveDefinition MuayRunTeep = Def("muay_run_teep", "Teep corrida", MoveAnimProfile.Teep, 34, 0.15f, 410f, 18f, 13f, 1);

    private static readonly MartialMoveDefinition CapoeiraMeiaLua = Def("capoeira_meia_lua", "Meia-lua", MoveAnimProfile.MeiaLua, 40, 0.22f, 480f, 20f, 15f, 1);
    private static readonly MartialMoveDefinition CapoeiraBencao = Def("capoeira_bencao", "Bencao", MoveAnimProfile.GingaKick, 34, 0.26f, 410f, 18f, 14f, 1);
    private static readonly MartialMoveDefinition CapoeiraGiro = Def("capoeira_giro", "Meia-lua", MoveAnimProfile.MeiaLua, 46, 0.26f, 500f, 22f, 17f, 2);
    private static readonly MartialMoveDefinition CapoeiraAu = Def("capoeira_au", "Au batido", MoveAnimProfile.FlyingKick, 44, 0.28f, 490f, 20f, 18f, 3);

    public static MartialMoveDefinition ResolveMove(
        CombatStyleKind style,
        int comboIndex,
        bool isRunning,
        bool airKick)
    {
        if (airKick)
        {
            return style switch
            {
                CombatStyleKind.MuayThai => MuayKnee with { DisplayName = "Joelhada aerea", Duration = 0.32f, Anim = MoveAnimProfile.FlyingKnee },
                _ => StreetFlying,
            };
        }

        if (isRunning)
        {
            return GetRunningMove(style);
        }

        MartialMoveDefinition[] chain = GetComboChain(style);
        int slot = Mathf.Clamp(comboIndex, 0, chain.Length - 1);
        MartialMoveDefinition move = chain[slot];
        float styleMult = CombatStyleCatalog.GetDamageMultiplier(style, slot);
        return move with { BaseDamage = Mathf.RoundToInt(move.BaseDamage * styleMult) };
    }

    public static MartialMoveDefinition GetRunningMove(CombatStyleKind style)
    {
        return style switch
        {
            CombatStyleKind.Boxe => BoxRunHook,
            CombatStyleKind.MuayThai => MuayRunTeep,
            CombatStyleKind.Capoeira => CapoeiraMeiaLua,
            _ => StreetFlying,
        };
    }

    public static string GetTechniqueDeckText(CombatStyleKind style)
    {
        if (style == CombatStyleKind.Boxe)
        {
            return "jab · cross · hook · uppercut  |  corrida: hook";
        }

        if (style == CombatStyleKind.MuayThai)
        {
            return "teep · joelhada · cotovelada · chute baixo  |  corrida: teep";
        }

        if (style == CombatStyleKind.Capoeira)
        {
            return "soco · bencao · meia-lua · au batido  |  corrida: meia-lua";
        }

        return "soco · cruzado · chute · joelhada  |  corrida: voadora";
    }

    public static string GetComboMoveName(CombatStyleKind style, int slot, bool isRunning)
    {
        if (isRunning)
        {
            return GetRunningMove(style).DisplayName;
        }

        MartialMoveDefinition[] chain = GetComboChain(style);
        return chain[Mathf.Clamp(slot, 0, chain.Length - 1)].DisplayName;
    }

    public static string GetNextComboMoveName(CombatStyleKind style, int currentSlot, bool isRunning)
    {
        if (isRunning)
        {
            return GetRunningMove(style).DisplayName;
        }

        MartialMoveDefinition[] chain = GetComboChain(style);
        int next = (Mathf.Clamp(currentSlot, 0, chain.Length - 1) + 1) % chain.Length;
        return chain[next].DisplayName;
    }

    public static IReadOnlyList<string> GetTechniqueNames(CombatStyleKind style)
    {
        MartialMoveDefinition[] chain = GetComboChain(style);
        List<string> names = [];
        foreach (MartialMoveDefinition move in chain)
        {
            names.Add(move.DisplayName.ToLowerInvariant());
        }

        names.Add($"corrida: {GetRunningMove(style).DisplayName.ToLowerInvariant()}");
        return names;
    }

    public static float GetStaminaCost(MartialMoveDefinition move, CombatStyleKind style, bool isRunning)
    {
        float cost = move.StaminaCost;
        if (isRunning && move.Anim is MoveAnimProfile.RunningPunch or MoveAnimProfile.RunningKick
            or MoveAnimProfile.RunningHook or MoveAnimProfile.Teep or MoveAnimProfile.MeiaLua)
        {
            cost = Mathf.Max(cost, 12f);
        }

        return cost * CombatStyleCatalog.GetStaminaCostMultiplier(style);
    }

    public static int GetComboLength(CombatStyleKind style)
    {
        return GetComboChain(style).Length;
    }

    private static MartialMoveDefinition[] GetComboChain(CombatStyleKind style)
    {
        return style switch
        {
            CombatStyleKind.Boxe => [BoxJab, BoxCross, BoxHook, BoxUpper],
            CombatStyleKind.MuayThai => [MuayJab, MuayTeep, MuayKnee, MuayElbow],
            CombatStyleKind.Capoeira => [StreetJab, CapoeiraBencao, CapoeiraGiro, CapoeiraAu],
            _ => [StreetJab, StreetCross, StreetKick, StreetKnee],
        };
    }

    private static MartialMoveDefinition Def(
        string id,
        string displayName,
        MoveAnimProfile anim,
        int baseDamage,
        float duration,
        float knockback,
        float postureDamage,
        float staminaCost,
        int impactComboIndex)
    {
        return new MartialMoveDefinition(
            id,
            displayName,
            anim,
            baseDamage,
            duration,
            knockback,
            postureDamage,
            staminaCost,
            impactComboIndex);
    }
}
