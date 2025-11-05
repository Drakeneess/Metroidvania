public interface IAnimState
{
    PlayerAnimationState Id { get; }
    void Enter();
    void Exit();
}

public abstract class AnimatorBoolState : IAnimState
{
    protected readonly UnityEngine.Animator A;
    private readonly int param;
    public abstract PlayerAnimationState Id { get; }

    protected AnimatorBoolState(UnityEngine.Animator animator, int boolHash)
    {
        A = animator;
        param = boolHash;
    }

    public virtual void Enter() => A.SetBool(param, true);
    public virtual void Exit()  => A.SetBool(param, false);
}

public abstract class AnimatorTriggerState : IAnimState
{
    protected readonly UnityEngine.Animator A;
    private readonly int param;
    public abstract PlayerAnimationState Id { get; }

    protected AnimatorTriggerState(UnityEngine.Animator animator, int trigHash)
    {
        A = animator;
        param = trigHash;
    }

    public virtual void Enter()
    {
        A.ResetTrigger(param);
        A.SetTrigger(param);
    }

    public virtual void Exit() { }
}
