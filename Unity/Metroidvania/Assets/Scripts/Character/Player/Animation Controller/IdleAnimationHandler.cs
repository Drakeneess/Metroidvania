using System;
using System.Collections;
using UnityEngine;

public class IdleAnimationHandler
{
    private readonly Animator animator;
    private readonly MonoBehaviour coroutineRunner;

    private Coroutine idleRoutine;
    private readonly float idleThreshold;
    private System.Random rng = new System.Random();
    private int lastIdle = 0;
    private int idleStreak = 0;

    public IdleAnimationHandler(Animator animator, MonoBehaviour runner, float threshold = 5f)
    {
        this.animator = animator;
        this.coroutineRunner = runner;
        this.idleThreshold = threshold;
    }

    public void StartIdle()
    {
        animator.SetBool("isIdle", true);
        animator.SetInteger("IdleState", GetImmediateIdleState());

        if (idleRoutine == null)
            idleRoutine = coroutineRunner.StartCoroutine(IdleTimerCoroutine());
    }

    public void StopIdle()
    {
        animator.SetBool("isIdle", false);
        animator.SetInteger("IdleState", 0); // reset

        if (idleRoutine != null)
        {
            coroutineRunner.StopCoroutine(idleRoutine);
            idleRoutine = null;
        }
    }

    private IEnumerator IdleTimerCoroutine()
    {
        float wait = idleThreshold + UnityEngine.Random.Range(-1f, 1f);
        yield return new WaitForSeconds(wait);

        animator.SetInteger("IdleState", GetRandomIdleState());
        idleRoutine = null;
    }

    private int GetImmediateIdleState()
    {
        int next = rng.Next(2) + 1;
        if (next == lastIdle)
            next = (next % 2) + 1;

        lastIdle = next;
        idleStreak++;
        return next;
    }

    private int GetRandomIdleState()
    {
        if (idleStreak >= 3 && rng.Next(100) < 60)
        {
            idleStreak = 0;
            lastIdle = 3;
            return 3;
        }

        return GetImmediateIdleState();
    }
}
