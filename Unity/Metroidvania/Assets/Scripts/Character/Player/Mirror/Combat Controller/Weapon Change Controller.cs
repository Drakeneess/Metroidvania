using UnityEngine;

public class WeaponChangeController : MonoBehaviour
{
    [SerializeField] private MirrorVisualController mirrorVisual;
    [SerializeField] private CombatController combatController;

    private Weapon pendingWeapon;

    void Awake()
    {
        if (combatController == null) combatController = GetComponent<CombatController>();
        if (mirrorVisual == null) mirrorVisual = GetComponent<MirrorVisualController>();
    }

    public void RequestWeaponChange(Weapon desired)
    {
        if (desired == null)
        {
            WeaponController.Instance?.EquipWeapon(null);
            return;
        }

        // Si el espejo está activo, se equipa instantáneo
        if (mirrorVisual != null && mirrorVisual.IsMirrorActive)
        {
            WeaponController.Instance?.EquipWeapon(desired);
            return;
        }

        // Si no, lo dejamos pendiente hasta que el espejo active
        pendingWeapon = desired;
    }

    private void ApplyPendingIfAny()
    {
        if (pendingWeapon == null) return;

        var w = pendingWeapon;
        pendingWeapon = null;

        WeaponController.Instance?.EquipWeapon(w);
    }
}
