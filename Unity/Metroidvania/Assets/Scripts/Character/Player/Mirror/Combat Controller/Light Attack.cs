using UnityEngine;

public class LightAttack : BaseAttack
{
    public void Execute()
    {
        if (ctx.weapon == null) return;
        if (ctx.combatController.CurrentState == CombatState.HeavyAttacking) return;
        if (ctx.player.Health.Get(HealthType.Mental) < ctx.combatController.WeaponMentalHealthUsage) return;
        if (!ctx.comboController.canContinueCombo) return;

        if (!ctx.comboController.TryConsumeNext()) return;

        // 🔹 Usar el índice que se va a ejecutar (no el confirmado aún)
        int plannedIndex = ctx.comboController.GetPlannedComboIndex();

        PrepareAttack(CombatState.LightAttacking, false);

        // Golpe con el índice planeado
        StartCoroutine(DoWeaponAttack(w => w.LightAttack(plannedIndex)));


        ctx.player.UseMentalPulse(ctx.combatController.WeaponMentalHealthUsage);

        if (ctx.movementControl.IsJumping && ctx.comboController.canContinueCombo)
            ctx.movementControl.StallAir(0.2f);
    }
}
