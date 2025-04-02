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
    public MirrorAttack mirrorAttack;

    private Weapon currentWeapon;
    public Weapon CurrentWeapon { 
        get { 
            return currentWeapon; 
        } 
        private set { 
            currentWeapon = value;
            if(mirrorAttack!= null){
                mirrorAttack.SetActiveWeapon(value);
            }
            UpdateWeaponUI(value.GetToolImage());    
        } 
    }
    private int currentWeaponIndex = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if(mirrorAttack == null){
            mirrorAttack = FindObjectOfType<MirrorAttack>();
        }
        // Asignamos el arma inicial si existe
        currentWeapon = weapons[currentWeaponIndex];
        CurrentWeapon = currentWeapon;
        currentWeapon.SetAsCurrentWeapon();
    }

    private void OnEnable()
    {
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnVector2Input += ChangeWeapon;
        }
    }

    private void OnDisable()
    {
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnVector2Input -= ChangeWeapon;
        }
    }

    private void ChangeWeapon(string actionName, Vector2 value)
    {
        if(actionName == "ChangeWeapon"){
            int newWeaponIndex = -1;

            // Compara las componentes de Vector2 para determinar el arma
            if (value.x == 0f && value.y == 1f) // Vector2.up
            {
                newWeaponIndex = 0;
                ShowWeaponFeedback("Sword");
            }
            else if (value.x == 0f && value.y == -1f) // Vector2.down
            {
                newWeaponIndex = 1;
                ShowWeaponFeedback("Scythe");
            }
            else if (value.x == -1f && value.y == 0f) // Vector2.left
            {
                newWeaponIndex = 2;
                ShowWeaponFeedback("Spear");
            }
            else if (value.x == 1f && value.y == 0f) // Vector2.right
            {
                newWeaponIndex = 3;
                ShowWeaponFeedback("Claws");
            }

            // Verifica si el arma está desbloqueada antes de cambiar
            if (newWeaponIndex >= 0 && newWeaponIndex < weapons.Length && weapons[newWeaponIndex].GetUnlocked())
            {
                SetNewWeapon(newWeaponIndex);
            }
            else
            {
                Debug.Log("Weapon not unlocked, keeping current weapon.");
            }
        }
    }

    private void SetNewWeapon(int newIndex)
    {
        // Solo cambiamos si el índice es diferente
        if (newIndex != currentWeaponIndex)
        {
            currentWeaponIndex = newIndex;
            currentWeapon = weapons[currentWeaponIndex];
            CurrentWeapon = currentWeapon;
            currentWeapon.SetAsCurrentWeapon();
        }
    }

    private void ShowWeaponFeedback(string weaponName)
    {
        // Aquí puedes implementar una forma más visual de mostrar el cambio de arma, como texto o iconos
        Debug.Log("Weapon changed to: " + weaponName);
    }

    private void UpdateWeaponUI(Sprite weaponImageUI)
    {
        if(Instance.weaponImageUI.sprite != weaponImageUI && Instance.weaponImageUI != null && weaponImageUI != null){
            Instance.weaponImageUI.sprite = weaponImageUI;
        }
    }
}
