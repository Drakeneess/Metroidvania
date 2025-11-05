using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Punto central del sistema de animación del Player (sin métodos estáticos).
/// Conserva Instance para acceso global mientras migras el código.
/// </summary>
public class PlayerAnimationController : CharacterAnimationController, IPlayerAnimator
{
    public static PlayerAnimationController Instance { get; private set; }

    [Header("Config")]
    [SerializeField] private AnimationStateRegistryAsset stateRegistry;

    private AnimStateMachine fsm;
    private readonly Dictionary<PlayerAnimationState, IAnimState> states = new();

    // Handlers existentes (tuyos)
    private IdleAnimationHandler idleHandler;
    private WalkAnimationHandler walkHandler;

    protected override void Start()
    {
        base.Start();

        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (!animator)
        {
            Debug.LogError("[PlayerAnimationController] Animator no encontrado.");
            enabled = false; return;
        }
        if (!stateRegistry)
        {
            Debug.LogError("[PlayerAnimationController] Falta StateRegistry (ScriptableObject).");
            enabled = false; return;
        }

        // Handlers
        idleHandler = new IdleAnimationHandler(animator, this, 5f);
        walkHandler = new WalkAnimationHandler(animator, this);

        // Estados
        states[PlayerAnimationState.Idle]         = new IdleState(idleHandler, walkHandler);
        states[PlayerAnimationState.Walk]         = new WalkState(idleHandler, walkHandler);
        states[PlayerAnimationState.Blocking]     = new BlockingState(animator);
        states[PlayerAnimationState.Rest]         = new RestState(animator);
        states[PlayerAnimationState.Attacking]    = new AttackState(animator);
        states[PlayerAnimationState.Jumping]      = new JumpState(animator);
        states[PlayerAnimationState.Evading]      = new EvadeState(animator);
        states[PlayerAnimationState.Curing]       = new CureState(animator);
        states[PlayerAnimationState.TakingDamage] = new TakingDamageState(animator);
        states[PlayerAnimationState.Die]          = new DieState(animator);
        states[PlayerAnimationState.Climb]        = new ClimbState(animator);

        // Base: Idle si no hay movimiento; Walk si hay movimiento
        PlayerAnimationState ResolveBase() =>
            animator.GetBool(AnimParams.IsMoving) ? PlayerAnimationState.Walk : PlayerAnimationState.Idle;

        // FSM
        fsm = new AnimStateMachine(this, stateRegistry, s => states.TryGetValue(s, out var st) ? st : null, ResolveBase);
        fsm.Request(PlayerAnimationState.Idle, force: true);

        // Bind facade
        PlayerAnim.Bind(this);
    }

    // ===========================
    // IPlayerAnimator (instancia)
    // ===========================

    public void SetCurrentHealthPercentage(float value) => animator.SetFloat(AnimParams.CurrentHealthPercentage, value);
    public void OnAir(bool value) => animator.SetBool(AnimParams.IsOnAir, value);
    public void Move(bool isMoving) => animator.SetBool(AnimParams.IsMoving, isMoving);

    public void Block(bool enable) => fsm.TogglePersistent(PlayerAnimationState.Blocking, enable);
    public void Rest(bool enable)  => fsm.TogglePersistent(PlayerAnimationState.Rest, enable);

    /// <summary>
    /// Die ON limpia persistentes antes de activarse.
    /// </summary>
    public void Die(bool enable)
    {
        if (enable)
        {
            fsm.ClearAllPersistents();             // limpia mierda previa
            fsm.Request(PlayerAnimationState.Die, force: true); // IGNORA Idle/Walk
        }
        else
        {
            fsm.TogglePersistent(PlayerAnimationState.Die, false);
        }
    }

    public void Attack()                 => fsm.Request(PlayerAnimationState.Attacking, force: true);
    public void HeavyAttack(bool enable) => animator.SetBool(AnimParams.IsHeavyAttack, enable);
    public void Jump()                   => fsm.Request(PlayerAnimationState.Jumping);
    public void Evade()                  => fsm.Request(PlayerAnimationState.Evading);
    public void Cure()                   => fsm.Request(PlayerAnimationState.Curing);
    public void Climb()                  => fsm.Request(PlayerAnimationState.Climb);
    public void TakeDamage()
    {
        // No aceptar daño si estamos muertos
        if (fsm.Current == PlayerAnimationState.Die) return;

        fsm.Request(PlayerAnimationState.TakingDamage, force: true);
    }

    public void SetAttackComboState(int combo) => animator.SetInteger(AnimParams.ComboState, combo);

    public void NotifyTransientFinished() => fsm.NotifyTransientFinished();

    public void ClearAllPersistents() => fsm.ClearAllPersistents();

    // ===========================
    // Animation Event (instancia)
    // ===========================
    // Llama desde el Animation Event que marca final de un transitorio
    public void AnimEvent_EndTransient()
    {
        NotifyTransientFinished();
        ForceBaseIdleOrWalk();
    }

    public void ForceBaseIdleOrWalk()
    {
        // Si hay un persistente activo (Blocking, Rest, Die, etc.), no cambiar base
        if (fsm.HasAnyPersistentActive)
            return;

        bool isMoving = animator.GetBool(AnimParams.IsMoving);

        if (isMoving)
            fsm.Request(PlayerAnimationState.Walk, force: true);
        else
            fsm.Request(PlayerAnimationState.Idle, force: true);
    }
}
