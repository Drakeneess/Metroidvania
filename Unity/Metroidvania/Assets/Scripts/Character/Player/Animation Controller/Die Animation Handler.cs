using UnityEngine;

public class DieAnimationHandler
{
    private readonly Animator animator;
    private System.Random rng = new System.Random();
    private int last = -1;

    private const string BOOL  = "isDying";
    private const string STATE = "Die State";

    public DieAnimationHandler(Animator animator)
    {
        this.animator = animator;
    }

    /// <summary>
    /// Activa estado de muerte con variante aleatoria (0..3) evitando repetir.
    /// </summary>
    public void Play()
    {
        int next = NextVariant();
        animator.SetInteger(STATE, next);
        animator.SetBool(BOOL, true);
    }

    /// <summary>
    /// Variante específica (0..3)
    /// </summary>
    public void PlayVariant(int variant)
    {
        int v = Mathf.Clamp(variant, 0, 3);
        last = v;
        animator.SetInteger(STATE, v);
        animator.SetBool(BOOL, true);
    }

    private int NextVariant()
    {
        int v = rng.Next(0, 4);
        if (v == last) v = (v + 1) % 4;
        last = v;
        return v;
    }
}
