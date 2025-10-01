using UnityEngine;

public class TakingDamageAnimationHandler
{
    private readonly Animator animator;
    private System.Random rng = new System.Random();
    private int last = -1;

    // Nombres de parámetros (si usas hashes, cámbialos aquí)
    private const string TRIGGER = "isTakingDamage";
    private const string STATE   = "Damage State";

    public TakingDamageAnimationHandler(Animator animator)
    {
        this.animator = animator;
    }

    /// <summary>
    /// Reproduce una animación de daño aleatoria (0..3) evitando repetir la última.
    /// </summary>
    public void Play()
    {
        int next = NextVariant();
        animator.SetInteger(STATE, next);

        // Reset por si hay triggers pendientes
        animator.ResetTrigger(TRIGGER);
        animator.SetTrigger(TRIGGER);
    }

    /// <summary>
    /// Si necesitas forzar una variante específica (0..3)
    /// </summary>
    public void PlayVariant(int variant)
    {
        int v = Mathf.Clamp(variant, 0, 3);
        last = v;
        animator.SetInteger(STATE, v);
        animator.ResetTrigger(TRIGGER);
        animator.SetTrigger(TRIGGER);
    }

    private int NextVariant()
    {
        int v = rng.Next(0, 4); // 0..3
        if (v == last) v = (v + 1) % 4;
        last = v;
        return v;
    }
}
