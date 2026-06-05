using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class WeaponController : MonoBehaviour
{
    public static WeaponController Instance { get; private set; }

    [Header("Player Weapons")]
    public Weapon[] weapons;
    public Image weaponImageUI;
    public WeaponChangeController weaponChange;

    private Weapon currentWeapon;
    private int currentWeaponIndex = -1;

    public Weapon CurrentWeapon
    { 
        get => currentWeapon;
        private set => currentWeapon = value;
    }

    void Awake()
    {
        Instance = this;
        if (weaponChange == null)
            weaponChange = FindObjectOfType<WeaponChangeController>(true);
    }

    private void OnEnable()
    {
        // 🔥 Nos aseguramos de que primero esté cargado el save
        StartCoroutine(LoadWeaponOnEnable());

        if (InputActionController.Instance != null)
            InputActionController.Instance.OnVector2Input += ChangeWeapon;
    }

    private void OnDisable()
    {
        if (InputActionController.Instance != null)
            InputActionController.Instance.OnVector2Input -= ChangeWeapon;
    }

    private IEnumerator LoadWeaponOnEnable()
    {
        // ✅ Espera 1 frame para asegurar que SaveDataController ya inicializó
        yield return null;

        int savedWeaponID = SaveDataController.Instance.saveData.currentWeapon;

        if (SaveDataController.AreSavedData() && savedWeaponID != -1)
        {
            foreach (Weapon w in weapons)
            {
                if (savedWeaponID == w.GetToolID())
                {
                    EquipWeapon(w);
                    yield break;
                }
            }
        }

        // ✅ Si no hay armas desbloqueadas → sin arma
        if (!HasUnlockedWeapon())
        {
            EquipWeapon(null);
            yield break;
        }

        // ✅ Cargar primera arma desbloqueada
        foreach (var w in weapons)
        {
            if (w.GetUnlocked())
            {
                EquipWeapon(w);
                yield break;
            }
        }
    }

    private bool HasUnlockedWeapon()
    {
        foreach (var w in weapons)
            if (w.GetUnlocked()) return true;
        return false;
    }

    public void EquipWeapon(Weapon newWeapon)
    {
        if (newWeapon == null)
        {
            CurrentWeapon = null;
            currentWeaponIndex = -1;
            UpdateWeaponUI(null);
            SaveDataController.Instance.saveData.currentWeapon = -1;
            return;
        }

        CurrentWeapon = newWeapon;
        currentWeaponIndex = Array.IndexOf(weapons, newWeapon);

        UpdateWeaponUI(newWeapon.GetToolImage());

        SaveDataController.Instance.saveData.currentWeapon = newWeapon.GetToolID();

        CombatController combat = FindObjectOfType<CombatController>();
        combat?.SetActiveWeapon(newWeapon);
    }

    private void UpdateWeaponUI(Sprite sprite)
    {
        if (weaponImageUI == null) return;

        weaponImageUI.sprite = sprite;
        weaponImageUI.enabled = (sprite != null);
    }

    private void ChangeWeapon(InputActionType actionName, Vector2 value)
    {
        if (actionName != InputActionType.ChangeWeapon) return;

        int newWeaponIndex = -1;
        if      (value == Vector2.up)    newWeaponIndex = 0;
        else if (value == Vector2.down)  newWeaponIndex = 1;
        else if (value == Vector2.left)  newWeaponIndex = 2;
        else if (value == Vector2.right) newWeaponIndex = 3;

        if (newWeaponIndex < 0 || newWeaponIndex >= weapons.Length) return;

        var desired = weapons[newWeaponIndex];

        if (!desired.GetUnlocked()) return;
        if (desired == CurrentWeapon) return;

        weaponChange?.RequestWeaponChange(desired);
    }
}
