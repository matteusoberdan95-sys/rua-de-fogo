namespace SangueNoAsfalto.Combat;

public partial class Hurtbox : Area2D
{
    [Export]
    public NodePath HealthPath { get; set; } = "../Health";

    private Health? _health;

    public override void _Ready()
    {
        _health = GetNodeOrNull<Health>(HealthPath);
        AreaEntered += OnAreaEntered;
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area is not Hitbox hitbox || hitbox.Source == Owner)
        {
            return;
        }

        if (Owner is SideScrollerPlayerController playerDefender && hitbox.Source is not SideScrollerPlayerController)
        {
            if (playerDefender.TryParry(hitbox))
            {
                return;
            }
        }

        bool damaged = _health?.Damage(hitbox.Damage) == true;
        if (!damaged)
        {
            return;
        }

        if (hitbox.Source is SideScrollerPlayerController player)
        {
            player.RegisterCombatHit(hitbox.Damage);
            if (_health?.CurrentHealth <= 0)
            {
                player.RegisterEnemyDefeat();
            }
        }

        if (Owner is SideScrollerPlayerController hurtPlayer && hitbox.Source is not SideScrollerPlayerController)
        {
            hurtPlayer.Posture?.AddPosture(hitbox.PostureDamage);
        }

        if (Owner is SideScrollerEnemyController enemy && hitbox.Source is SideScrollerPlayerController)
        {
            enemy.Posture?.AddPosture(hitbox.PostureDamage * 0.55f);
        }

        ApplyKnockback(hitbox);
        ApplyBleed(hitbox);
        PlayFeedback(hitbox);
    }

    private void ApplyBleed(Hitbox hitbox)
    {
        if (Owner is null)
        {
            return;
        }

        float duration;
        float dps;

        if (hitbox is Projectile projectile && projectile.ApplyBleedOnHit)
        {
            duration = projectile.BleedDuration;
            dps = projectile.BleedDamagePerSecond;
        }
        else if (hitbox.ApplyBleedOnHit)
        {
            duration = hitbox.BleedDuration;
            dps = hitbox.BleedDamagePerSecond;
        }
        else
        {
            return;
        }

        BleedEffect? bleed = Owner.GetNodeOrNull<BleedEffect>("BleedEffect");
        bleed?.Apply(duration, dps);
    }

    private void ApplyKnockback(Hitbox hitbox)
    {
        if (Owner is not CharacterBody2D body || hitbox.Source is not Node2D source)
        {
            return;
        }

        Vector2 direction = body.GlobalPosition.DirectionTo(source.GlobalPosition) * -1f;
        float knockback = CombatImpactFeel.ScaleKnockback(hitbox.Damage, hitbox.KnockbackForce);
        float stun = CombatImpactFeel.ScaleHitStun(hitbox.Damage, hitbox.HitStunDuration);
        Vector2 impulse = direction * knockback;
        if (Owner is ICombatKnockbackReceiver receiver)
        {
            receiver.ReceiveKnockback(impulse, stun);
            return;
        }

        body.Velocity += impulse;
    }

    private void PlayFeedback(Hitbox hitbox)
    {
        if (Owner is not Node2D target || hitbox.Source is not Node2D source)
        {
            return;
        }

        CombatFeedback.PlayHit(target, source, hitbox.Damage);

        if (hitbox.IsFinisherHit && hitbox.Source is SideScrollerPlayerController finisherPlayer)
        {
            if (hitbox.IsParryRiposte)
            {
                CombatFeedback.PlayParryCounterKill(target, source);
                return;
            }

            if (hitbox.IsPostureKill)
            {
                CombatFeedback.PlayPostureKill(target, source);
                return;
            }

            ImprovisedWeaponKind weapon = finisherPlayer.EquippedWeaponKind;
            CombatFeedback.PlayFinisher(target, source, weapon);
        }
    }
}
