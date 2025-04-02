using UnityEngine;

public class Weapon : ShardTool
{
    [Header("Weapon Config")]
    [SerializeField] private LayerMask characterMask;
    [SerializeField] protected Vector3[] positions;

    protected Quaternion originalRotation;

    private WeaponData WeaponData => shardToolData as WeaponData;

    protected override void Start()
    {
        base.Start();
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

    public virtual void ResetWeaponPosition(int comboIndex)
    {
        // Animaciones o colocación
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
