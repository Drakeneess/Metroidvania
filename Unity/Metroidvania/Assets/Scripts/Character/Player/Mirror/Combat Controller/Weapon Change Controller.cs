using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponChangeController : MonoBehaviour
{
    [SerializeField] private MirrorVisualController mirrorVisual;
    [SerializeField] private CombatController combatController;

    private Weapon pendingWeapon;

    void Awake()
    {
        if (combatController == null) combatController = GetComponent<CombatController>();
        if (mirrorVisual == null)     mirrorVisual     = GetComponent<MirrorVisualController>();
    }

    void OnEnable()
    {
        if (mirrorVisual != null)
            mirrorVisual.OnMirrorShown += ApplyPendingIfAny; // aplicar cola cuando vuelve el espejo
    }

    void OnDisable()
    {
        if (mirrorVisual != null)
            mirrorVisual.OnMirrorShown -= ApplyPendingIfAny;
    }

    /// Llamado por WeaponController. No toca CombatController más que para SetActiveWeapon (existente).
    public void RequestWeaponChange(Weapon desired)
    {
        if (desired == null || combatController == null || mirrorVisual == null) return;

        // Si el espejo está visible, aplicamos inmediato
        if (mirrorVisual.IsMirrorActive)
        {
            ApplyNow(desired);
            return;
        }

        // Si el arma está afuera, encolamos
        pendingWeapon = desired;
    }

    private void ApplyNow(Weapon w)
    {
        // No tocamos la lógica interna del CombatController más que su API pública existente
        combatController.SetActiveWeapon(w);

        // Actualizamos el UI SOLO cuando la aplicación fue real
        WeaponController.Instance?.ApplyWeaponChange(w);
    }

    private void ApplyPendingIfAny()
    {
        if (pendingWeapon == null) return;

        var w = pendingWeapon;
        pendingWeapon = null;
        ApplyNow(w);
    }
}
