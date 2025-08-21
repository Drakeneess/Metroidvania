using System;
using System.Collections;
using UnityEngine;

public class PlayerAnimationController : CharacterAnimationController
{
    public static PlayerAnimationController Instance { get; private set; }

    private IdleAnimationHandler idleHandler;
    private WalkAnimationHandler walkHandler;
    private CombatAnimationHandler combatHandler;
    private PlayerAnimationState currentState;
    private PlayerAnimationState previousState = PlayerAnimationState.Idle;


    private Coroutine revertCoroutine;

    void Awake()
    {

    }
    protected override void Start()
    {
        base.Start();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destruye este objeto si ya existe una instancia
            return;
        }
        // Se establece la instancia como la única existente
        Instance = this;

        idleHandler = new IdleAnimationHandler(animator, this, 5f);
        walkHandler = new WalkAnimationHandler(animator, this);
        Instance = this;
    }

    void OnEnable()
    {

    }

    void OnDisable()
    {

    }
    public void SetAnimationState(PlayerAnimationState newState, bool force = false)
    {
        bool isSameState = newState == currentState;

        var newMeta = AnimationStateRegistry.Get(newState);
        var currentMeta = AnimationStateRegistry.Get(currentState);

        if (!force && isSameState)
            return;

        if (!force && newMeta.Priority < currentMeta.Priority)
            return;

        if (newMeta.Type == AnimationStateType.Transient)
            previousState = currentState;

        if (!isSameState)
        {
            ExitState(currentState);
            currentState = newState;
            EnterState(newState);
        }
        else if (force)
        {
            EnterState(newState);
        }

        if (newMeta.Type == AnimationStateType.Transient)
            StartRevertCoroutine(newMeta.Duration);
    }

    private void StartRevertCoroutine(float delay)
    {
        if (revertCoroutine != null)
            StopCoroutine(revertCoroutine);

        revertCoroutine = StartCoroutine(RevertAfterDelay(delay));
    }

    private IEnumerator RevertAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (currentState == PlayerAnimationState.Jumping ||
            currentState == PlayerAnimationState.Attacking ||
            currentState == PlayerAnimationState.TakingDamage ||
            currentState == PlayerAnimationState.Evading)
        {
            SetAnimationState(previousState, force: true);
        }

        revertCoroutine = null;
    }

    private void EnterState(PlayerAnimationState state)
    {
        switch (state)
        {
            case PlayerAnimationState.Idle:
                idleHandler.StartIdle();
                walkHandler.StopWalk();
                break;

            case PlayerAnimationState.Walk:
                walkHandler.StartWalk();
                idleHandler.StopIdle();
                break;

            case PlayerAnimationState.Jumping:
                animator.SetTrigger("isJumping");
                break;

            case PlayerAnimationState.Attacking:
                animator.SetBool("isAttacking", true);
                break;

            case PlayerAnimationState.Blocking:
                animator.SetBool("isBlocking", true);
                break;

            case PlayerAnimationState.Evading:
                animator.SetTrigger("isEvading");
                break;

            case PlayerAnimationState.TakingDamage:
                animator.SetTrigger("Hit");
                break;

            case PlayerAnimationState.Die:
                animator.SetTrigger("Die");
                break;

            case PlayerAnimationState.Rest:
                animator.SetBool("isResting", true);
                break;
        }
    }

    private void ExitState(PlayerAnimationState state)
    {
        switch (state)
        {
            case PlayerAnimationState.Idle:
                idleHandler.StopIdle();
                break;

            case PlayerAnimationState.Walk:
                walkHandler.StopWalk();
                break;

            case PlayerAnimationState.Blocking:
                animator.SetBool("isBlocking", false);
                break;

            case PlayerAnimationState.Rest:
                animator.SetBool("isResting", false);
                break;
            case PlayerAnimationState.Attacking:
                animator.SetBool("isAttacking", false);
                break;
        }
    }

    #region Animation Controllers
    public static void SetCurrentHealthPercentage(float currentPlayerHealth)
    {
        Instance.animator.SetFloat("CurrentHealthPercentage", currentPlayerHealth);
    }
    public static void IsOnAir(bool onAir)
    {
        Instance.animator.SetBool("isOnAir", onAir);
    }

    public static void SetWalkState(bool isWalking, bool force = false)
    {
        if (isWalking)
        {
            Instance.SetAnimationState(PlayerAnimationState.Walk, force);
        }
        else
        {
            Instance.SetAnimationState(PlayerAnimationState.Idle, force);
        }
    }

    public static void SetBlocking()
    {
        Instance.SetAnimationState(PlayerAnimationState.Blocking);
    }

    public static void SetMoving(bool state)
    {
        Instance.animator.SetBool("isMoving", state);
    }
    public static void StartJumping()
    {
        Instance.SetAnimationState(PlayerAnimationState.Jumping);
    }

    public static void SetResting()
    {
        Instance.SetAnimationState(PlayerAnimationState.Rest);
    }

    public static void SetAttackState()
    {
        Instance.SetAnimationState(PlayerAnimationState.Attacking, true);
    }
    public static void SetAttackComboState(int combo)
    {
        Instance.animator.SetInteger("Combo State", combo);
    }
    public static void SetHeavyAttack(bool isHeavy)
    {
        Instance.animator.SetBool("isHeavyAttack", isHeavy);
    }
    public static void SetEvading()
    {
        Instance.SetAnimationState(PlayerAnimationState.Evading);
    }

    #endregion
}
