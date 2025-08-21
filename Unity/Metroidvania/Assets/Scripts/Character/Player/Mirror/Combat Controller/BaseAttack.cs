using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAttack : MonoBehaviour
{
    protected WeaponContext ctx;

    public void Init(WeaponContext context) => ctx = context;

    protected void PrepareAttack(CombatState state, bool keepWeaponActive)
    {
        ctx.combatController.ChangeState(state);
        ctx.movementControl.LockMovement(ctx.combatController.ComboResetTime);
        ctx.combatController.TriggerAttack(keepWeaponActive);
    }

    protected IEnumerator DoWeaponAttack(float delay, System.Action<Weapon> attackAction)
    {
        if (ctx.weapon != null)
        {
            ctx.weapon.SetToolActive(true);
        }
        ctx.combatController.StartRecoveryWindow();

        yield return new WaitForSeconds(delay);
        attackAction?.Invoke(ctx.weapon);
    }
}

