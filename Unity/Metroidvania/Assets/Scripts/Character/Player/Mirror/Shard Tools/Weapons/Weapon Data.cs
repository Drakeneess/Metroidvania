using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Tool/Weapons/New Weapon")]
public class WeaponData : ShardToolData
{
    [Header("Weapon Info")]
    public float damage = 10f;
    public float range = 10f;

    [Header("Dome Info")]
    public float damageRange = 2f;

    [Header("Combo Info")]
    public int maxCombo = 3;
    public float maxTimeChargedAttack = 2f;
    public float recoveryTime = 0.3f;
    public float comboResetTime = 0.5f;
}
