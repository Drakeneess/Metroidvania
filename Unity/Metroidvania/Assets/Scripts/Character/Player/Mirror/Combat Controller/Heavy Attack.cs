using System.Collections;
using UnityEngine;

public class HeavyAttack : BaseAttack
{
    private bool isCharging;

    public void StartCharge()
    {
        if (ctx.player.GetPercentageHealth(HealthType.Mental) < 0.1f) return;
        isCharging = true;
        ctx.combatController.isHeavyAttackActive = true;
        PlayerAnimationController.SetHeavyAttack(true);
        PrepareAttack(CombatState.HeavyAttacking, true);
    }

    public void Release()
    {
        if (!isCharging) return;
        isCharging = false;
        PlayerAnimationController.SetHeavyAttack(false);

        ctx.combatController.isHeavyAttackActive = false;
        ctx.player.UseMentalPulse(ctx.weapon.GetMentalHealthUsage() * 8);
        StartCoroutine(DoWeaponAttack(0.4f, w => w.HeavyAttack(ctx.comboController.GetCombo())));
        StartCoroutine(ctx.combatController.RecoverFromAttack());

        if (ctx.movementControl.IsJumping)
            ctx.movementControl.StallAir(0.4f);
    }
}

