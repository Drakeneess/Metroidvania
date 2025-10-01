using System.Collections;
using UnityEngine;

public abstract class BaseAttack : MonoBehaviour
{
    protected WeaponContext ctx;

    public void Init(WeaponContext context) => ctx = context;

    protected void PrepareAttack(CombatState state, bool keepWeaponActive)
    {
        ctx.combatController.TriggerAttack(keepWeaponActive);
    }

    /// Maneja ejecución + impacto + ventana de recuperación
    protected IEnumerator DoWeaponAttack(System.Action<Weapon> attackAction)
    {
        if (ctx.weapon != null)
            ctx.weapon.SetToolActive(true);

        float exec = ctx.weapon != null ? ctx.weapon.GetExecutionTime() : 0.35f;
        yield return new WaitForSeconds(exec);

        attackAction?.Invoke(ctx.weapon);

        float total  = ctx.combatController.ComboResetTime;
        float window = Mathf.Max(0f, total - exec);
        if (window > 0f)
            ctx.combatController.StartRecoveryWindow(window);
        else
            ctx.combatController.EndCombo();
    }
}
