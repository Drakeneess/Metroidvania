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
    
    // WeaponData.cs
    [Header("Timings")]
    public float executionTime = 0.35f;     // tiempo hasta el impacto
    public float comboResetTime = 0.5f;     // ya lo tienes (ventana total)
    public float recoveryTime = 0.2f;       // extra opcional tras cerrar combo
}

[System.Serializable]
public struct ComboPose
{
    [Header("Rotación base (absoluta, en grados)")]
    public Vector3 startRotation;
    public Vector3 endRotation;

    [Header("Tiempo total del ataque")]
    public float duration;

    [Header("Momento de impacto (0..1)")]
    [Range(0f, 1f)] public float impactTime;

    [Header("Curvas por eje (0..1)")]
    public AnimationCurve curveX;   // si está vacío, cae a lineal
    public AnimationCurve curveY;
    public AnimationCurve curveZ;

    [Header("Extras")]
    public float shakeIntensity;    // leve vibración procedural
    public float recoilDegrees;     // retroceso opcional al final
    public float recoilDuration;    // duración del retroceso

    public bool forceDeltaX;
    public bool forceDeltaY;
    public bool forceDeltaZ;
}
