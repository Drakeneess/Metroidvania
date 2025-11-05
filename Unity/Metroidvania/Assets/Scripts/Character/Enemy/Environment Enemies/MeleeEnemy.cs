using System.Collections;
using UnityEngine;

public abstract class MeleeEnemy : Enemy
{
    [Header("Melee Attack Settings")]
    public float meleeDamage = 10f;
    public float windupTime = 0.3f;
    public float activeTime = 0.2f;   // tiempo en que el trigger está activo
    public float recoveryTime = 0.4f;

    protected bool isAttackingMelee = false;
    protected MeleeTriggerZone triggerZone;

    protected override void Start()
    {
        base.Start();
        triggerZone = GetComponentInChildren<MeleeTriggerZone>();
    }

    protected override IEnumerator DoAttack()
    {
        if (isAttackingMelee || isDead) yield break;

        isAttackingMelee = true;
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;

        animator?.SetBool("Alert", false);
        animator?.SetTrigger("Attack");

        // Espera previa al golpe (windup)
        yield return new WaitForSeconds(windupTime);

        if (triggerZone != null)
            triggerZone.Activate();

        // Tiempo activo del golpe
        yield return new WaitForSeconds(activeTime);

        if (triggerZone != null)
            triggerZone.Deactivate();

        // Recuperación
        yield return new WaitForSeconds(recoveryTime);

        isAttackingMelee = false;
        isAttacking = false;
    }

    // Evento opcional si el golpe impacta
    public virtual void OnSuccessfulHit(Player player)
    {
        // Por ejemplo, podrías agregar retroceso, efectos, etc.
    }
}
