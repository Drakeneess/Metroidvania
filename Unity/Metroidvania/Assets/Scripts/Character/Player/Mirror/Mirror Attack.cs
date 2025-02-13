using System;
using System.Collections;
using UnityEngine;

public class MirrorAttack : MonoBehaviour
{
    [SerializeField] private GameObject mirrorComponents;
    [SerializeField] private float reactivateMirrorTime = 2f;
    [SerializeField] private Mirror mirror;

    private Weapon currentWeapon;
    private Coroutine mirrorCoroutine;
    private bool isHeavyAttackActive = false;

    void Start()
    {
        if (mirror == null && !TryGetComponent(out mirror))
        {
            Debug.LogWarning("Mirror component not found on " + gameObject.name);
        }
    }

    protected virtual void OnEnable()
    {
        var input = InputActionController.Instance;
        if (input != null)
        {
            input.OnLightAttack += LightAttack;
            input.OnHeavyAttack += HeavyAttack;
        }
    }

    protected virtual void OnDisable()
    {
        var input = InputActionController.Instance;
        if (input != null)
        {
            input.OnLightAttack -= LightAttack;
            input.OnHeavyAttack -= HeavyAttack;
        }
    }

    private void LightAttack()
    {
        TriggerAttack();
    }

    private void HeavyAttack(float value)
    {
        isHeavyAttackActive = value > 0.5f;
        TriggerAttack();
    }

    private void TriggerAttack()
    {
        mirror?.SetAttackingState(true);
        ResetMirrorTimer();
        SetMirrorState(false);
    }

    private void SetMirrorState(bool state)
    {
        mirrorComponents.SetActive(state);
        currentWeapon.SetWeaponActive(!state);
    }

    private void ResetMirrorTimer()
    {
        if (mirrorCoroutine != null)
        {
            StopCoroutine(mirrorCoroutine);
        }
        mirrorCoroutine = StartCoroutine(ActivateMirror());
    }

    private IEnumerator ActivateMirror()
    {
        while (isHeavyAttackActive)
        {
            yield return null;
        }

        yield return new WaitForSeconds(reactivateMirrorTime);
        
        SetMirrorState(true);
        mirror?.SetAttackingState(false);
    }

    public void SetActiveWeapon(Weapon activeWeapon)
    {
        currentWeapon = activeWeapon;
        if (mirror != null)
        {
            mirror.attackRange = currentWeapon.range;
        }
    }
}
