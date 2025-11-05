using System.Collections;
using UnityEngine;

public class HeavyAttack : BaseAttack
{
    private bool isCharging;
    private float chargeStartTime;
    private PlayerAnimationController anim;

    private void Start()
    {
        anim = PlayerAnimationController.Instance;
    }

    public void StartCharge()
    {
        if (ctx.player.Health.GetPercent(HealthType.Mental) < 0.1f || ctx.weapon!=null) return;
        isCharging = true;
        chargeStartTime = Time.time;

        ctx.combatController.isHeavyAttackActive = true;
        anim.HeavyAttack(true);
        PrepareAttack(CombatState.HeavyAttacking, true);

        ctx.weapon.PlayChargePose();
    }

    public void Release()
    {
        if (!isCharging) return;
        isCharging = false;

        anim.HeavyAttack(false);
        ctx.combatController.isHeavyAttackActive = false;

        float chargeDuration = Time.time - chargeStartTime;
        float chargeFactor = Mathf.Clamp01(chargeDuration / ctx.weapon.GetMaxTimeChargedAttack());

        ctx.player.UseMentalPulse(ctx.weapon.GetMentalHealthUsage() * Mathf.Lerp(4, 8, chargeFactor));
        StartCoroutine(DoWeaponAttack(w => w.HeavyAttack(chargeFactor)));

        if (ctx.movementControl.IsJumping)
            ctx.movementControl.StallAir(0.4f);
    }
}
