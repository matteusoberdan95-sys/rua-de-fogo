namespace SangueNoAsfalto.Combat;

public enum EnemyDamageVisualTier
{
    Intact,
    Hurt,
    Critical
}

public static class EnemyDamageState
{
    public static EnemyDamageVisualTier FromHealth(int current, int maximum)
    {
        if (maximum <= 0 || current <= 0)
        {
            return EnemyDamageVisualTier.Critical;
        }

        float ratio = current / (float)maximum;
        if (ratio > 0.66f)
        {
            return EnemyDamageVisualTier.Intact;
        }

        if (ratio > 0.33f)
        {
            return EnemyDamageVisualTier.Hurt;
        }

        return EnemyDamageVisualTier.Critical;
    }
}
