using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorVisualController : MonoBehaviour
{
    [SerializeField] private GameObject mirrorComponents;
    [SerializeField] private ParticleSystem summonEffect;
    [SerializeField] private Mirror mirror;

    private Weapon currentWeapon;
    private Coroutine reactivateCoroutine;

    public float ReactivateDelay { get; set; }
    public bool IsActive => mirrorComponents.activeSelf;
    public Mirror Mirror => mirror;

    /// <summary>
    /// Activa o desactiva el espejo visualmente y en su lógica.
    /// </summary>
    public void SetMirrorState(bool active)
    {
        mirrorComponents?.SetActive(active);
        mirror?.SetAttackingState(!active);

        if (currentWeapon != null)
            currentWeapon.SetToolActive(!active);

        if (!active && summonEffect != null)
            summonEffect.Play();
    }

    /// <summary>
    /// Reinicia el temporizador para volver a activar el espejo.
    /// </summary>
    public void ResetActivationTimer(bool waitWhileHeavyAttack, System.Func<bool> isHeavyAttackActive)
    {
        if (reactivateCoroutine != null)
            StopCoroutine(reactivateCoroutine);

        reactivateCoroutine = StartCoroutine(ActivateAfterDelay(waitWhileHeavyAttack, isHeavyAttackActive));
    }

    private IEnumerator ActivateAfterDelay(bool waitWhileHeavyAttack, System.Func<bool> isHeavyAttackActive)
    {
        if (waitWhileHeavyAttack)
        {
            // Esperar mientras el ataque pesado sigue activo
            while (isHeavyAttackActive != null && isHeavyAttackActive())
            {
                yield return null;
            }
        }

        yield return new WaitForSeconds(ReactivateDelay);
        SetMirrorState(true);
    }

    public void SetNewWeapon(Weapon newWeapon)
    {
        currentWeapon = newWeapon;
    }
}
