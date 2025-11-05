using UnityEngine;

// Idle: usa tus handlers de idle/walk para mantener el ciclo base.
public sealed class IdleState : IAnimState
{
    private readonly IdleAnimationHandler idle;
    private readonly WalkAnimationHandler walk;
    public PlayerAnimationState Id => PlayerAnimationState.Idle;

    public IdleState(IdleAnimationHandler idleHandler, WalkAnimationHandler walkHandler)
    {
        idle = idleHandler;
        walk = walkHandler;
    }

    public void Enter()
    {
        walk.StopWalk();
        idle.StartIdle();
    }

    public void Exit()
    {
        idle.StopIdle();
    }
}

// Walk: activa caminar y desactiva idle.
public sealed class WalkState : IAnimState
{
    private readonly IdleAnimationHandler idle;
    private readonly WalkAnimationHandler walk;
    public PlayerAnimationState Id => PlayerAnimationState.Walk;

    public WalkState(IdleAnimationHandler idleHandler, WalkAnimationHandler walkHandler)
    {
        idle = idleHandler;
        walk = walkHandler;
    }

    public void Enter()
    {
        idle.StopIdle();
        walk.StartWalk();
    }

    public void Exit()
    {
        walk.StopWalk();
    }
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

// Transitorios
public sealed class AttackState : AnimatorBoolState
{
    public AttackState(Animator a) : base(a, AnimParams.IsAttacking) { }
    public override PlayerAnimationState Id => PlayerAnimationState.Attacking;
}

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

public sealed class ClimbState : AnimatorTriggerState
{
    public ClimbState(Animator a) : base(a, AnimParams.TrigClimb) { }
    public override PlayerAnimationState Id => PlayerAnimationState.Climb;
}

// Daño con handler propio
public sealed class TakingDamageState : IAnimState
{
    private readonly TakingDamageAnimationHandler handler;
    public PlayerAnimationState Id => PlayerAnimationState.TakingDamage;

    public TakingDamageState(Animator a)
    {
        handler = new TakingDamageAnimationHandler(a);
    }

    public void Enter()
    {
        handler.Play();
    }

    public void Exit() { }
}

// Muerte con handler propio
public sealed class DieState : IAnimState
{
    private readonly DieAnimationHandler handler;
    public PlayerAnimationState Id => PlayerAnimationState.Die;

    public DieState(Animator a)
    {
        handler = new DieAnimationHandler(a);
    }

    public void Enter()
    {
        handler.Play(); // activa isDying y variante
    }

    public void Exit()
    {
        // Limpia para respawn (sin esto se queda pose o bloquea)
        handler.Stop();
    }
}

