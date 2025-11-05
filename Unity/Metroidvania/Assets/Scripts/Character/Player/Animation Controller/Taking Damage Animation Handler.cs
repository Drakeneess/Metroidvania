using UnityEngine;

public class TakingDamageAnimationHandler
{
    private readonly Animator animator;
    private System.Random rng = new System.Random();
    private int last = -1;

    private static readonly int TRIGGER = Animator.StringToHash("isTakingDamage");
    private static readonly int STATE   = Animator.StringToHash("Damage State");

    public TakingDamageAnimationHandler(Animator animator)
    {
        this.animator = animator;
    }

    public void Play()
    {
        int next = NextVariant();
        animator.SetInteger(STATE, next);
        animator.ResetTrigger(TRIGGER);
        animator.SetTrigger(TRIGGER);
    }

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
        int v = rng.Next(0, 4);
        if (v == last) v = (v + 1) % 4;
        last = v;
        return v;
    }
}
