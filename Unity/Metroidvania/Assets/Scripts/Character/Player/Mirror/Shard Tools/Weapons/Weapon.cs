using System.Collections;
using UnityEngine;

public class Weapon : ShardTool
{
    [Header("Weapon Config")]
    [SerializeField] private LayerMask characterMask;
    [SerializeField] protected ComboPose[] comboAnimations;

    protected bool isAttacking = false;
    protected WeaponData WeaponData => shardToolData as WeaponData;
    protected CombatController combatController;

    protected override void Start()
    {
        base.Start();
        originalRotation = Quaternion.identity;
    }

    public virtual void LightAttack(int comboIndex) { }
    public virtual void HeavyAttack(float chargeFactor) { }

    // Animación de preparación para Heavy Attack
    public virtual void PlayChargePose()
    {
        if (isAttacking) return;
        StartCoroutine(ChargeRoutine());
    }

    private IEnumerator ChargeRoutine()
    {
        isAttacking = true;

        Quaternion startRot = transform.localRotation;
        Quaternion chargeRot = startRot * Quaternion.Euler(0f, 0f, -30f);

        float chargeTime = 0.3f;
        float t = 0f;
        while (t < 1f)
        {
            transform.localRotation = Quaternion.Slerp(startRot, chargeRot, t);
            t += Time.deltaTime / chargeTime;
            yield return null;
        }
        transform.localRotation = chargeRot;

        isAttacking = false;
    }

    public virtual void ResetWeaponPosition()
    {
        transform.localRotation = originalRotation;
    }

    protected IEnumerator SmoothReset(float duration)
    {
        Quaternion currentRot = transform.localRotation;
        Vector3 currentPos    = transform.localPosition;

        float t = 0f;
        while (t < 1f)
        {
            transform.localRotation = Quaternion.Slerp(currentRot, originalRotation, t);
            transform.localPosition = Vector3.Lerp(currentPos, Vector3.zero, t);
            t += Time.deltaTime / duration;
            yield return null;
        }

        transform.localRotation = originalRotation;
        transform.localPosition = Vector3.zero;
    }

    // 🔹 Ataque con daño + knockback
    protected void ActivateDamageArea(bool isHeavy = false, float chargeFactor = 1f)
    {
        int facing = GetFacingDirection();               // +1 derecha, -1 izquierda
        Vector3 forward2D = Vector3.right * facing;      // “frente” en 2.5D (eje X)

        Collider[] hits = Physics.OverlapSphere(transform.position, GetDamageRange(), characterMask);

        foreach (var col in hits)
        {
            if (!col.CompareTag("Enemy")) continue;

            // Vector hacia el enemigo, a piso (evita empuje vertical)
            Vector3 toEnemy = col.transform.position - transform.position;
            toEnemy.y = 0f;

            // ✅ Solo enemigos delante (dot > 0 => delante)
            if (toEnemy.sqrMagnitude < 0.0001f) continue;
            float frontDot = Vector3.Dot(toEnemy.normalized, forward2D);
            if (frontDot <= 0f) continue;

            // Daño con distancia + heavy/charge
            float distance = toEnemy.magnitude;
            float damage   = CalculateDamage(distance, isHeavy, chargeFactor);

            var e = col.GetComponent<Enemy>();
            if (e == null) continue;

            e.TakePhysicalDamage(damage,null);

            // ✅ Knockback siempre horizontal hacia el frente del ataque
            float knockForce = Mathf.Max(0f, WeaponData.knockback * (1f - (distance / GetDamageRange())));
            Vector3 pushDir = forward2D;   // pura X, sin Y/Z
            e.ApplyKnockback(pushDir, knockForce, 0.15f);
        }
    }

    private float CalculateDamage(float distance, bool isHeavy = false, float chargeFactor = 1f)
    {
        float factorDist = Mathf.Max(0.5f, 1f - (distance / GetDamageRange()));
        float typeMult   = isHeavy ? 1.75f : 1f;
        float chargeMult = Mathf.Lerp(0.8f, 2f, chargeFactor);
        return GetDamage() * factorDist * typeMult * chargeMult;
    }

    public void SetCombatController(CombatController controller)
    {
        combatController = controller;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        transform.localRotation = Quaternion.identity;
        transform.localPosition = Vector3.zero;
    }

    // Accessors
    public float GetDamage()               => WeaponData.damage;
    public float GetRange()                => WeaponData.range;
    public float GetDamageRange()          => WeaponData.damageRange;
    public int   GetMaxCombo()             => WeaponData.maxCombo;
    public float GetMaxTimeChargedAttack() => WeaponData.maxTimeChargedAttack;
    public float GetExecutionTime()        => WeaponData.executionTime;
    public float GetRecoveryTime()         => WeaponData.recoveryTime;
    public float GetComboResetTime()       => WeaponData.comboResetTime;
    public override Sprite GetToolImage()  => WeaponData.toolImageUI;
}
