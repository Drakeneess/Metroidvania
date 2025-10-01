using UnityEngine;
public interface IAnimState
{
    PlayerAnimationState Id { get; }
    void Enter();
    void Exit();
}

public abstract class AnimatorBoolState : IAnimState
{
    protected readonly Animator A;
    private readonly int _param;
    public abstract PlayerAnimationState Id { get; }

    protected AnimatorBoolState(Animator a, int boolHash) { A = a; _param = boolHash; }
    public virtual void Enter() => A.SetBool(_param, true);
    public virtual void Exit()  => A.SetBool(_param, false);
}

public abstract class AnimatorTriggerState : IAnimState
{
    protected readonly Animator A;
    private readonly int _param;
    public abstract PlayerAnimationState Id { get; }

    protected AnimatorTriggerState(Animator a, int trigHash) { A = a; _param = trigHash; }
    public virtual void Enter() { A.ResetTrigger(_param); A.SetTrigger(_param); }
    public virtual void Exit() { }
}
