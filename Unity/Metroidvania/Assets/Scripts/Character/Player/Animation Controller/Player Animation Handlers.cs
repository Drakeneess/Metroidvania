using UnityEngine;
public sealed class IdleState : IAnimState
{
    private readonly IdleAnimationHandler idle;
    private readonly WalkAnimationHandler walk;
    public PlayerAnimationState Id => PlayerAnimationState.Idle;
    public IdleState(IdleAnimationHandler i, WalkAnimationHandler w) { idle = i; walk = w; }
    public void Enter() { walk.StopWalk(); idle.StartIdle(); }
    public void Exit()  { idle.StopIdle(); }
}

public sealed class WalkState : IAnimState
{
    private readonly IdleAnimationHandler idle;
    private readonly WalkAnimationHandler walk;
    public PlayerAnimationState Id => PlayerAnimationState.Walk;
    public WalkState(IdleAnimationHandler i, WalkAnimationHandler w) { idle = i; walk = w; }
    public void Enter() { idle.StopIdle(); walk.StartWalk(); }
    public void Exit()  { walk.StopWalk(); }
}

// Persistentes por bool
public sealed class BlockingState : AnimatorBoolState
{
    public BlockingState(Animator a) : base(a, AnimParams.IsBlocking) { }
    public override PlayerAnimationState Id => PlayerAnimationState.Blocking;
}

public sealed class RestState : AnimatorBoolState
{
    public RestState(Animator a) : base(a, AnimParams.IsResting) { }
    public override PlayerAnimationState Id => PlayerAnimationState.Rest;
}

public sealed class AttackState : AnimatorBoolState
{
    public AttackState(Animator a) : base(a, AnimParams.IsAttacking) { }
    public override PlayerAnimationState Id => PlayerAnimationState.Attacking;
}

// Transitorios por trigger
public sealed class JumpState : AnimatorTriggerState
{
    public JumpState(Animator a) : base(a, AnimParams.TrigJumping) { }
    public override PlayerAnimationState Id => PlayerAnimationState.Jumping;
}

public sealed class EvadeState : AnimatorTriggerState
{
    public EvadeState(Animator a) : base(a, AnimParams.TrigEvade) { }
    public override PlayerAnimationState Id => PlayerAnimationState.Evading;
}

public sealed class CureState : AnimatorTriggerState
{
    public CureState(Animator a) : base(a, AnimParams.TrigDrink) { }
    public override PlayerAnimationState Id => PlayerAnimationState.Curing;
}

// Daño y Muerte con random
public sealed class TakingDamageState : IAnimState
{
    private readonly Animator A;
    private readonly TakingDamageAnimationHandler handler;
    public PlayerAnimationState Id => PlayerAnimationState.TakingDamage;
    public TakingDamageState(Animator a) { A = a; handler = new TakingDamageAnimationHandler(a); }
    public void Enter() => handler.Play(); // set int 0..3 y trigger
    public void Exit() { }
}

public sealed class DieState : IAnimState
{
    private readonly Animator A;
    private readonly DieAnimationHandler handler;
    public PlayerAnimationState Id => PlayerAnimationState.Die;
    public DieState(Animator a) { A = a; handler = new DieAnimationHandler(a); }
    public void Enter() => handler.Play(); // set int 0..3 y bool isDying = true
    public void Exit() { /* muerte terminal */ }
}
