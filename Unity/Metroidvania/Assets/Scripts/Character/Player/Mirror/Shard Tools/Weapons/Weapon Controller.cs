using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponController : MonoBehaviour
{
    public static WeaponController Instance { get; private set; }
    public Weapon[] weapons;
    public Image weaponImageUI;
    public WeaponChangeController weaponChange;

    private Weapon currentWeapon;
    public Weapon CurrentWeapon
    { 
        get => currentWeapon;
        private set => currentWeapon = value; // sin UI aquí
    }

    private int currentWeaponIndex = 0;

    void Awake()
    {
        Instance = this;

        if (weaponChange == null)
            weaponChange = FindObjectOfType<WeaponChangeController>();

        // Arma inicial: pedimos el cambio (si el espejo está activo, se aplicará ya)
        currentWeapon = weapons[currentWeaponIndex];
        weaponChange?.RequestWeaponChange(currentWeapon);
    }

    void Start()
    {
        int savedWeapon = SaveDataController.Instance.saveData.currentWeapon;
        if (SaveDataController.AreSavedData() && savedWeapon != -1)
        {
            foreach (Weapon weapon in weapons)
            {
                if (savedWeapon == weapon.GetToolID())
                {
                    ApplyWeaponChange(weapon);
                }
            }
        }
    }

    private void OnEnable()
    {
        if (InputActionController.Instance != null)
            InputActionController.Instance.OnVector2Input += ChangeWeapon;
    }

    private void OnDisable()
    {
        if (InputActionController.Instance != null)
            InputActionController.Instance.OnVector2Input -= ChangeWeapon;
    }

    private void ChangeWeapon(string actionName, Vector2 value)
    {
        if(actionName != "ChangeWeapon") return;

        int newWeaponIndex = -1;
        if      (value == Vector2.up)    newWeaponIndex = 0;
        else if (value == Vector2.down)  newWeaponIndex = 1;
        else if (value == Vector2.left)  newWeaponIndex = 2;
        else if (value == Vector2.right) newWeaponIndex = 3;

        if (newWeaponIndex < 0 || newWeaponIndex >= weapons.Length) return;
        if (!weapons[newWeaponIndex].GetUnlocked()) { Debug.Log("Weapon locked."); return; }

        // Evita re-aplicar la misma
        if (newWeaponIndex == currentWeaponIndex) return;

        currentWeaponIndex = newWeaponIndex;
        var desired = weapons[currentWeaponIndex];
        CurrentWeapon = desired;

        // 👉 Pide el cambio (bloquea o encola según estado del espejo)
        weaponChange?.RequestWeaponChange(desired);
    }

    // Llamado SOLO cuando el cambio realmente se aplicó
    public void ApplyWeaponChange(Weapon newWeapon)
    {
        CurrentWeapon = newWeapon;
        if (newWeapon != null)
        {
            UpdateWeaponUI(newWeapon.GetToolImage());
            SaveDataController.Instance.saveData.currentWeapon = newWeapon.GetToolID();
        }
    }

    private void UpdateWeaponUI(Sprite sprite)
    {
        if (weaponImageUI != null && sprite != null && weaponImageUI.sprite != sprite)
            weaponImageUI.sprite = sprite;
    }
}
