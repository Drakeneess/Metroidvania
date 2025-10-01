using UnityEngine;

public class MirrorVisualController : MonoBehaviour
{
    [SerializeField] private GameObject mirrorComponents;
    [SerializeField] private ParticleSystem summonEffect;
    [SerializeField] private Mirror mirror;

    private bool isMirrorActive = true;
    private Weapon currentWeapon;

    public bool IsActive       => mirrorComponents && mirrorComponents.activeSelf;
    public bool IsMirrorActive => isMirrorActive;
    public Mirror Mirror       => mirror;

    public event System.Action OnMirrorShown; // Weapon -> Mirror (espejo visible)
    public event System.Action OnWeaponShown; // Mirror -> Weapon (arma visible)

    /// Transición Espejo -> Arma (partícula una sola vez).
    public void TransitionMirrorToWeapon()
    {
        if (!isMirrorActive) return; // ya estaba en arma
        isMirrorActive = false;

        mirrorComponents?.SetActive(false);
        mirror?.SetAttackingState(true);
        currentWeapon?.SetToolActive(true);

        summonEffect?.Play();
        OnWeaponShown?.Invoke();
    }

    /// Transición Arma -> Espejo (partícula una sola vez).
    public void TransitionWeaponToMirror()
    {
        if (isMirrorActive) return; // ya estaba en espejo
        isMirrorActive = true;

        mirrorComponents?.SetActive(true);
        mirror?.SetAttackingState(false);
        currentWeapon?.SetToolActive(false);

        summonEffect?.Play();
        OnMirrorShown?.Invoke();
    }

    public void SetNewWeapon(Weapon newWeapon)
    {
        if (currentWeapon != null && currentWeapon != newWeapon)
            currentWeapon.SetToolActive(false); // cinturón y tirantes

        currentWeapon = newWeapon;

        if (isMirrorActive)
            currentWeapon.SetToolActive(false);
    }
}
