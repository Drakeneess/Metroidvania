using System.Collections;
using UnityEngine;

public class HeavyAttack : BaseAttack
{
    private bool isCharging;
    private float chargeStartTime;

    public void StartCharge()
    {
        if (ctx.player.Health.GetPercent(HealthType.Mental) < 0.1f) return;
        isCharging = true;
        chargeStartTime = Time.time;

        ctx.combatController.isHeavyAttackActive = true;
        PlayerAnimationController.SetHeavyAttack(true);
        PrepareAttack(CombatState.HeavyAttacking, true);

        ctx.weapon.PlayChargePose();
    }

    public void Release()
    {
        if (!isCharging) return;
        isCharging = false;

        PlayerAnimationController.SetHeavyAttack(false);
        ctx.combatController.isHeavyAttackActive = false;

        float chargeDuration = Time.time - chargeStartTime;
        float chargeFactor = Mathf.Clamp01(chargeDuration / ctx.weapon.GetMaxTimeChargedAttack());

        // Consumo de mental en base a carga
        ctx.player.UseMentalPulse(ctx.weapon.GetMentalHealthUsage() * Mathf.Lerp(4, 8, chargeFactor));

        // Pasamos el factor a HeavyAttack
        StartCoroutine(DoWeaponAttack(w => w.HeavyAttack(chargeFactor)));


        if (ctx.movementControl.IsJumping)
            ctx.movementControl.StallAir(0.4f);
    }
}
