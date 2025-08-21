using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkAnimationHandler
{
    private readonly Animator animator;
    private readonly MonoBehaviour owner;

    private readonly string walkBool = "isWalking";

    public WalkAnimationHandler(Animator animator, MonoBehaviour owner)
    {
        this.animator = animator;
        this.owner = owner;
    }

    public void StartWalk()
    {
        animator.SetBool(walkBool, true);
    }

    public void StopWalk()
    {
        animator.SetBool(walkBool, false);
    }
}

