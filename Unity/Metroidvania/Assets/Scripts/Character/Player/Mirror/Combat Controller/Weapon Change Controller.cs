using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponChangeController : MonoBehaviour
{
    private CombatController combatController;
    private Weapon currentWeapon;

    void OnEnable()
    {
        if (combatController == null) combatController = GetComponent<CombatController>();
    }
    public void SetActiveWeapon(Weapon activeWeapon)
    {
        if (!combatController.CanChangeWeapon) return;

        currentWeapon = activeWeapon;
        if (combatController != null)
        {
            combatController.SetActiveWeapon(currentWeapon);
        }
        else
        {
            print("CombatController is null");
        }
    }
}
