using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : CharacterAnimationController, IPlayerAnimator
{
    public static PlayerAnimationController Instance { get; private set; }

    [Header("Config")]
    [SerializeField] private AnimationStateRegistryAsset stateRegistry;

    // Handlers existentes
    private IdleAnimationHandler idleHandler;
    private WalkAnimationHandler walkHandler;

    // FSM
    private AnimStateMachine fsm;
    private readonly Dictionary<PlayerAnimationState, IAnimState> states = new();

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

        // FSM
        fsm = new AnimStateMachine(this, stateRegistry, s => states.TryGetValue(s, out var st) ? st : null);
        fsm.Request(PlayerAnimationState.Idle, force: true);

        // Enlaza fachada
        PlayerAnim.Bind(this);
    }

    // ========= Implementación IPlayerAnimator =========
    void IPlayerAnimator.SetCurrentHealthPercentage(float v) => animator.SetFloat(AnimParams.CurrentHealthPercentage, v);
    void IPlayerAnimator.SetOnAir(bool v) => animator.SetBool(AnimParams.IsOnAir, v);
    void IPlayerAnimator.SetWalkState(bool isWalking, bool force) => fsm.Request(isWalking ? PlayerAnimationState.Walk : PlayerAnimationState.Idle, force);
    void IPlayerAnimator.SetBlocking() => fsm.Request(PlayerAnimationState.Blocking);
    void IPlayerAnimator.SetMoving(bool v) => animator.SetBool(AnimParams.IsMoving, v);
    void IPlayerAnimator.StartJumping() => fsm.Request(PlayerAnimationState.Jumping);
    void IPlayerAnimator.SetResting() => fsm.Request(PlayerAnimationState.Rest);
    void IPlayerAnimator.SetAttackState(bool force) => fsm.Request(PlayerAnimationState.Attacking, force);
    void IPlayerAnimator.SetAttackComboState(int combo) => animator.SetInteger(AnimParams.ComboState, combo);
    void IPlayerAnimator.SetHeavyAttack(bool v) => animator.SetBool(AnimParams.IsHeavyAttack, v);
    void IPlayerAnimator.SetEvading() => fsm.Request(PlayerAnimationState.Evading);
    void IPlayerAnimator.SetCuring() => fsm.Request(PlayerAnimationState.Curing);
    void IPlayerAnimator.SetDying() => fsm.Request(PlayerAnimationState.Die);
    void IPlayerAnimator.SetTakingDamage() => fsm.Request(PlayerAnimationState.TakingDamage);

    void IPlayerAnimator.NotifyTransientFinished() => fsm.NotifyTransientFinished();

    // ========= Compat: API estática original =========
    public static void SetCurrentHealthPercentage(float v) { if (Instance) PlayerAnim.SetCurrentHealthPercentage(v); }
    public static void IsOnAir(bool v) { if (Instance) PlayerAnim.IsOnAir(v); }
    public static void SetWalkState(bool w, bool force=false) { if (Instance) PlayerAnim.SetWalkState(w, force); }
    public static void SetBlocking() { if (Instance) PlayerAnim.SetBlocking(); }
    public static void SetMoving(bool v) { if (Instance) PlayerAnim.SetMoving(v); }
    public static void StartJumping() { if (Instance) PlayerAnim.StartJumping(); }
    public static void SetResting() { if (Instance) PlayerAnim.SetResting(); }
    public static void SetAttackState() { if (Instance) PlayerAnim.SetAttackState(true); }
    public static void SetAttackComboState(int combo) { if (Instance) PlayerAnim.SetAttackComboState(combo); }
    public static void SetHeavyAttack(bool v) { if (Instance) PlayerAnim.SetHeavyAttack(v); }
    public static void SetEvading() { if (Instance) PlayerAnim.SetEvading(); }
    public static void SetCuring() { if (Instance) PlayerAnim.SetCuring(); }
    public static void SetDying() { if (Instance) PlayerAnim.SetDying(); }
    public static void SetTakingDamage() { if (Instance) PlayerAnim.SetTakingDamage(); }

    // Llama esto desde Animation Events al final de clips transitorios con duración=0
    public static void AnimEvent_EndTransient()
    {
        if (Instance) PlayerAnim.NotifyTransientFinished();
    }
}
