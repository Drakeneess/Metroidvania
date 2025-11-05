using UnityEngine;

public class WalkAnimationHandler
{
    private readonly Animator animator;
    private readonly UnityEngine.MonoBehaviour owner;

    private readonly int walkHash = Animator.StringToHash("isWalking");

    public WalkAnimationHandler(Animator animator, UnityEngine.MonoBehaviour owner)
    {
        this.animator = animator;
        this.owner = owner;
    }

    public void StartWalk()
    {
        animator.SetBool(walkHash, true);
    }

    public void StopWalk()
    {
        animator.SetBool(walkHash, false);
    }
}
