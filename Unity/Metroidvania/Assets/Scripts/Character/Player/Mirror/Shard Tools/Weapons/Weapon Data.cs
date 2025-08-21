using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Tool/Weapons/New Weapon")]
public class WeaponData : ShardToolData
{
    [Header("Weapon Info")]
    public float damage = 10f;
    public float range = 10f;
    public float knockback = 5f;
    public float attackSpeed = 1.0f;


    [Header("Dome Info")]
    public float damageRange = 2f;


    [Header("Combo Info")]
    public int maxCombo = 3;
    public float maxTimeChargedAttack = 2f;
    public float recoveryTime = 0.3f;
    public float comboResetTime = 0.5f;
}

[System.Serializable]
public struct ComboPose
{
    public Vector3 startRotation;
    public Vector3 endRotation;
    public float duration;
    public AnimationCurve rotationCurve;
    public float shakeIntensity;
}
