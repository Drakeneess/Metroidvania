using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MirrorAttack : MonoBehaviour
{
    [SerializeField]
    private GameObject mirrorComponents;
    [SerializeField]
    private float reactivateMirrorTime = 2f;

    private Weapon currentWeapon;
    private int comboIndex;
    private Coroutine mirrorCoroutine; // Para rastrear la corrutina actual
    private bool isHeavyAttackActive = false; // Controla si el Heavy Attack sigue activo
    
    protected virtual void OnEnable()
    {
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnLightAttack += LightAttack;
            InputActionController.Instance.OnHeavyAttack += HeavyAttack;
        }
    }

    protected virtual void OnDisable()
    {
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnLightAttack -= LightAttack;
            InputActionController.Instance.OnHeavyAttack -= HeavyAttack;
        }
    }

    private void LightAttack()
    {
        ResetMirrorTimer();
        SetMirrorState(false);
        currentWeapon.LightAttack(comboIndex);
    }

    private void HeavyAttack(float value)
    {
        if (value > 0.5f)
        {
            isHeavyAttackActive = true;
            ResetMirrorTimer(); // Reinicia el contador mientras el ataque pesado esté activo
        }
        else
        {
            isHeavyAttackActive = false;
        }

        SetMirrorState(false);
        currentWeapon.HeavyAttack(comboIndex);
    }

    private void SetMirrorState(bool state)
    {
        mirrorComponents.SetActive(state);
        currentWeapon.SetWeaponActive(!state);
    }

    public void SetActiveWeapon(Weapon activeWeapon)
    {
        currentWeapon = activeWeapon;
        print(currentWeapon.name);
    }

    private void ResetMirrorTimer()
    {
        // Si ya hay una corrutina en ejecución, detenerla
        if (mirrorCoroutine != null)
        {
            StopCoroutine(mirrorCoroutine);
        }

        // Iniciar una nueva corrutina para reactivar el espejo solo si el Heavy Attack no está activo
        mirrorCoroutine = StartCoroutine(ActivateMirror());
    }

    private IEnumerator ActivateMirror()
    {
        while (isHeavyAttackActive) // Mientras el Heavy Attack esté activo, el tiempo se reinicia
        {
            yield return null; // Espera un frame antes de volver a verificar
        }

        yield return new WaitForSeconds(reactivateMirrorTime);
        SetMirrorState(true);
    }
}
