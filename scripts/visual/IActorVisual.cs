namespace SangueNoAsfalto.Visual;

/// <summary>Contrato compartilhado entre rig procedural e arte de producao.</summary>
public interface IActorVisual
{
    bool IsProductionArtActive { get; }

    void EnsureLayeredRig(LayeredPrototypePreset preset);

    void SetFacing(int sign);

    void SetJumpOffset(float heightOffset);

    void SetDamageVisualTier(EnemyDamageVisualTier tier);

    void PlayDeath();

    void PlayParryStagger();

    void PlayPostureStagger();

    void PlayGuard();

    void PlayBlockImpact();

    void PlayShoot();

    void UpdateLocomotion(
        bool isMoving,
        bool isAttacking,
        bool isDashing,
        bool isHurt = false,
        bool isTelegraphing = false,
        bool isRunning = false,
        bool isFinisherAttack = false,
        bool isReloading = false,
        bool isGuarding = false,
        bool isParrying = false,
        bool isPostureStaggered = false,
        bool isExhausted = false);

    void SetEquippedWeapon(ImprovisedWeaponKind kind);

    void PlayWeaponAttack(ImprovisedWeaponKind kind);

    void PlayFinisherAttack(ImprovisedWeaponKind kind);

    void PlayReload();

    void EndReload();

    void PlayParryWindup();

    void PlayParrySuccessMatrix();

    void PlayParryRiposte();

    void PlayPostureKill();

    void BeginEnemyStrike(float duration, int patternIndex, MoveAnimProfile anim);

    void SetAttackCombo(int comboIndex);

    void SetCombatStyle(CombatStyleKind style);

    void SetAttackMove(MoveAnimProfile anim, int impactComboIndex, CombatStyleKind style, float duration = 0.32f);

    void PlayHitReaction(Vector2 direction, float severity = 1f);
}
