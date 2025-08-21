using System.Collections;
using UnityEngine;

public class Weapon : ShardTool
{
    [Header("Weapon Config")]
    [SerializeField] private LayerMask characterMask;
    [SerializeField] protected ComboPose[] comboAnimations;

    protected bool isAttacking = false;

    protected Quaternion originalRotation;

    protected WeaponData WeaponData => shardToolData as WeaponData;

    protected override void Start()
    {
        base.Start();
        originalRotation = transform.localRotation;
    }

    public void SetAsCurrentWeapon()
    {
        // Lógica futura para convertirla en el arma activa
    }

    public virtual void LightAttack(int comboIndex)
    {
        ActivateDamageArea();
    }

    public virtual void HeavyAttack(float damage)
    {
        ActivateDamageArea();
    }

    public virtual void ResetWeaponPosition()
    {
        transform.localRotation = originalRotation;
    }

    protected IEnumerator RotateWeapon(Quaternion startRot, Quaternion endRot, Vector3 startPos, Vector3 endPos, float speed)
    {
        isAttacking = true;

        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            float t = elapsedTime / WeaponData.attackSpeed;
            t = Mathf.SmoothStep(0f, 1f, t); // O AnimationCurve si querés más control


            transform.localRotation = Quaternion.Slerp(startRot, endRot, t);
            transform.localPosition = Vector3.Lerp(startPos, endPos, t);

            elapsedTime += Time.deltaTime * speed;
            yield return null;
        }

        transform.localRotation = endRot;
        transform.localPosition = endPos;

        yield return new WaitForSeconds(GetRecoveryTime());

        StartCoroutine(SmoothReset(0.2f)); // Vuelve a la posición original con suavidad
        isAttacking = false;
    }

    protected IEnumerator SmoothReset(float duration)
    {
        Quaternion currentRot = transform.localRotation;
        Vector3 currentPos = transform.localPosition;

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

    private void ActivateDamageArea()
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, GetDamageRange(), characterMask);

        foreach (var enemy in enemiesInRange)
        {
            if (enemy.CompareTag("Enemy"))
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                float damage = CalculateDamage(distance);
                enemy.GetComponent<Enemy>().TakePhysicalDamage(damage);
            }
        }
    }
    public override void UnlockTool()
    {
        base.UnlockTool();
        WeaponController.Instance.CurrentWeapon=this;
    }

    private float CalculateDamage(float distance)
    {
        float damage = GetDamage() * (1 - (distance / GetDamageRange()));
        return Mathf.Max(damage, 0);
    }

    // Acceso a datos específicos de WeaponData
    public float GetDamage() => WeaponData.damage;
    public float GetRange() => WeaponData.range;
    public float GetDamageRange() => WeaponData.damageRange;

    public int GetMaxCombo() => WeaponData.maxCombo;
    public float GetMaxTimeChargedAttack() => WeaponData.maxTimeChargedAttack;
    public float GetRecoveryTime() => WeaponData.recoveryTime;
    public float GetComboResetTime() => WeaponData.comboResetTime;

    public override Sprite GetToolImage() => WeaponData.toolImageUI; // por si difiere
}
