namespace SangueNoAsfalto.Combat;

/// <summary>
/// Escala stun/knockback conforme o dano — pancada pesada = inimigo sofre mais tempo.
/// </summary>
public static class CombatImpactFeel
{
    public static float ScaleHitStun(int damage, float baseStun)
    {
        return baseStun * Mathf.Clamp(1f + damage / 32f, 1f, 2.4f);
    }

    public static float ScaleKnockback(int damage, float baseForce)
    {
        return baseForce * Mathf.Clamp(1f + damage / 55f, 1f, 1.45f);
    }

    public static float StaminaCostForCombo(int comboIndex, bool isRunning)
    {
        if (isRunning)
        {
            return comboIndex switch
            {
                1 => 16f,
                _ => 12f,
            };
        }

        return comboIndex switch
        {
            2 => 24f,
            1 => 15f,
            _ => 10f,
        };
    }
}
