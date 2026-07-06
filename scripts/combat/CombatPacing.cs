namespace SangueNoAsfalto.Combat;

/// <summary>
/// Constantes globais de ritmo — um único lugar para desacelerar combate e telegraphs.
/// </summary>
public static class CombatPacing
{
    public const float PlayerMoveDurationScale = 1.6f;

    public const float MinEnemyTelegraph = 1.2f;

    public const float MinEnemyAttackCooldown = 2.0f;

    public const float MinEnemyAttackDuration = 0.42f;

    public const float PlayerHitWindowStart = 0.34f;

    public const float PlayerHitWindowEnd = 0.56f;

    public const float EnemyHitWindowStart = 0.38f;

    public const float EnemyHitWindowEnd = 0.72f;

    public const float DeathBodySeconds = 2.1f;

    public static float ScalePlayerMoveDuration(float baseDuration)
    {
        return Mathf.Max(baseDuration * PlayerMoveDurationScale, 0.42f);
    }

    public static bool IsInHitWindow(float progress, float start, float end)
    {
        return progress >= start && progress <= end;
    }
}
