namespace SangueNoAsfalto.Combat;

using SangueNoAsfalto.Visual;

/// <summary>
/// Padrões de ataque por arquétipo — cada capanga luta diferente (ref. personagens_ref).
/// </summary>
public readonly struct EnemyAttackPattern
{
    public MoveAnimProfile Anim { get; init; }
    public int VisualComboIndex { get; init; }
    public float DurationMultiplier { get; init; }
    public float TelegraphMultiplier { get; init; }
    public float RangeMultiplier { get; init; }
    public string Callout { get; init; }
}

public static class EnemyCombatProfile
{
    public static EnemyAttackPattern Resolve(LayeredPrototypePreset preset, int patternIndex)
    {
        int slot = patternIndex % 3;
        return preset switch
        {
            LayeredPrototypePreset.Fast => slot switch
            {
                0 => Pattern(MoveAnimProfile.Jab, 0, 0.82f, 0.78f, 0.92f, "Rapido"),
                1 => Pattern(MoveAnimProfile.Teep, 1, 0.78f, 0.72f, 1.05f, "Chute"),
                _ => Pattern(MoveAnimProfile.Jab, 0, 0.75f, 0.68f, 0.95f, "Combo"),
            },
            LayeredPrototypePreset.Brute => slot switch
            {
                0 => Pattern(MoveAnimProfile.Hook, 0, 1.18f, 1.22f, 1.12f, "Soco pesado"),
                1 => Pattern(MoveAnimProfile.Uppercut, 1, 1.12f, 1.15f, 1.08f, "Uppercut"),
                _ => Pattern(MoveAnimProfile.Hook, 0, 1.25f, 1.28f, 1.18f, "Esmaga"),
            },
            LayeredPrototypePreset.Infected => slot switch
            {
                0 => Pattern(MoveAnimProfile.Hook, 0, 1.02f, 1.0f, 1.08f, "Facada"),
                1 => Pattern(MoveAnimProfile.SideKick, 1, 0.95f, 0.92f, 1.12f, "Corte"),
                _ => Pattern(MoveAnimProfile.Uppercut, 2, 1.08f, 1.05f, 1.15f, "Facao"),
            },
            LayeredPrototypePreset.MiniBoss => slot switch
            {
                0 => Pattern(MoveAnimProfile.Jab, 0, 0.95f, 0.88f, 1.05f, "Bastonada"),
                1 => Pattern(MoveAnimProfile.Cross, 1, 0.88f, 0.82f, 1.1f, "Tiro"),
                _ => Pattern(MoveAnimProfile.Hook, 0, 1.05f, 0.95f, 1.12f, "Ordem"),
            },
            _ => slot switch
            {
                0 => Pattern(MoveAnimProfile.Jab, 0, 1.0f, 1.0f, 1.0f, "Soco"),
                1 => Pattern(MoveAnimProfile.Hook, 1, 1.05f, 1.02f, 1.05f, "Tubo"),
                _ => Pattern(MoveAnimProfile.Uppercut, 1, 1.1f, 1.08f, 1.08f, "Overhead"),
            },
        };
    }

    public static float GetMoveSpeedMultiplier(LayeredPrototypePreset preset) =>
        preset switch
        {
            LayeredPrototypePreset.Fast => 1.22f,
            LayeredPrototypePreset.Brute => 0.78f,
            LayeredPrototypePreset.Infected => 0.92f,
            LayeredPrototypePreset.MiniBoss => 0.85f,
            _ => 1f,
        };

    public static float GetAggroBias(LayeredPrototypePreset preset) =>
        preset switch
        {
            LayeredPrototypePreset.Fast => 1.15f,
            LayeredPrototypePreset.Brute => 0.88f,
            _ => 1f,
        };

    private static EnemyAttackPattern Pattern(
        MoveAnimProfile anim,
        int combo,
        float durationMul,
        float telegraphMul,
        float rangeMul,
        string callout) =>
        new()
        {
            Anim = anim,
            VisualComboIndex = combo,
            DurationMultiplier = durationMul,
            TelegraphMultiplier = telegraphMul,
            RangeMultiplier = rangeMul,
            Callout = callout,
        };
}
