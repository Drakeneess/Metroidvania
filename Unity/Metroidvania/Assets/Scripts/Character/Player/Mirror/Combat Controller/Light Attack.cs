using System.Collections;
using UnityEngine;

public class LightAttack : BaseAttack
{
    public void Execute()
    {
        if (ctx.weapon == null || ctx.combatController.CurrentState == CombatState.HeavyAttacking) return;
        if (ctx.player.GetCurrentHealth(HealthType.Mental) < ctx.combatController.WeaponMentalHealthUsage) return;

        PrepareAttack(CombatState.LightAttacking, false);

        StartCoroutine(DoWeaponAttack(0.4f, w => w.LightAttack(ctx.comboController.GetCombo())));
        ctx.player.UseMentalPulse(ctx.combatController.WeaponMentalHealthUsage);
        ctx.comboController.ComboFlow();

        // Solo abre ventana si todavía puede seguir combo
        if (ctx.comboController.canContinueCombo)
            ctx.combatController.StartRecoveryWindow();
        else
            ctx.combatController.EndCombo();

        if (ctx.movementControl.IsJumping && ctx.comboController.canContinueCombo)
            ctx.movementControl.StallAir(0.2f);
    }
}
