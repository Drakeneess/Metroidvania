using System;
using System.Collections;
using UnityEngine;

public class PlayerAnimationController : CharacterAnimationController
{
    [SerializeField]
    private Player player;

    private IdleAnimationHandler idleHandler;
    private PlayerAnimationState currentState;
    private PlayerAnimationState previousState;

    private float currentPlayerHealth;
    private float currentMovementInput = 0f;
    private bool movementInputReceived = false;

    private Coroutine movementCheckRoutine;
    private Coroutine revertCoroutine;

    protected override void Start()
    {
        base.Start();
        if (player == null)
            player = GetComponent<Player>();

        idleHandler = new IdleAnimationHandler(animator, this, 5f);
    }

    void OnEnable()
    {
        if (player?.CharacterMovement != null)
        {
            InputActionController.Instance.OnFloatInput += OnMovementInputChanged;
            InputActionController.Instance.OnActionTriggered += OnActionInputChanged;

            movementCheckRoutine = StartCoroutine(MovementCheckLoop());
        }
        else
        {
            Debug.LogError("Player or CharacterMovement not assigned.");
        }
    }

    void OnDisable()
    {
        if (player?.CharacterMovement != null)
        {
            InputActionController.Instance.OnFloatInput -= OnMovementInputChanged;
            InputActionController.Instance.OnActionTriggered -= OnActionInputChanged;
        }

        if (movementCheckRoutine != null)
        {
            StopCoroutine(movementCheckRoutine);
            movementCheckRoutine = null;
        }

        if (revertCoroutine != null)
        {
            StopCoroutine(revertCoroutine);
            revertCoroutine = null;
        }
    }

    private void OnMovementInputChanged(string actionName, float value)
    {
        if (actionName == "Movement")
        {
            movementInputReceived = value!=0;
            currentMovementInput = value;

            currentPlayerHealth = player.GetPercentageHealth(HealthType.Physical);
            animator.SetFloat("CurrentHealthPercentage", currentPlayerHealth);
        }
    }

    private void OnActionInputChanged(string actionName)
    {
        switch (actionName)
        {
            case "Jump":
                SetAnimationState(PlayerAnimationState.Jumping);
                break;
            case "Attack":
                SetAnimationState(PlayerAnimationState.Attacking);
                break;
            case "Block":
                SetAnimationState(PlayerAnimationState.Blocking);
                break;
        }
    }

    private IEnumerator MovementCheckLoop()
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);

        while (true)
        {
            float inputValue = movementInputReceived ? currentMovementInput : 0f;
            movementInputReceived = false;
            currentMovementInput = 0f;

            bool canMove = player != null && player.CharacterMovement != null && player.CharacterMovement.CanMove;
            bool isMoving = Mathf.Abs(inputValue) > 0.01f && canMove;

            if (isMoving)
            {
                if (currentState != PlayerAnimationState.Walk)
                    SetAnimationState(PlayerAnimationState.Walk);
            }
            else
            {
                if (currentState != PlayerAnimationState.Idle)
                    SetAnimationState(PlayerAnimationState.Idle);
            }

            yield return wait;
        }
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
            // Estado igual pero se forzó la reentrada
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
            currentState == PlayerAnimationState.TakingDamage)
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
                animator.SetBool("isIddle", true);
                idleHandler.StartIdle();
                break;

            case PlayerAnimationState.Walk:
                animator.SetBool("isWalking", true);
                idleHandler.StopIdle();
                break;

            case PlayerAnimationState.Jumping:
                animator.SetTrigger("IsJumping");
                break;

            case PlayerAnimationState.Attacking:
                animator.SetTrigger("Attack");
                break;

            case PlayerAnimationState.Blocking:
                animator.SetBool("IsBlocking", true);
                break;

            case PlayerAnimationState.TakingDamage:
                animator.SetTrigger("Hit");
                break;

            case PlayerAnimationState.Die:
                animator.SetTrigger("Die");
                break;

            case PlayerAnimationState.Rest:
                animator.SetBool("IsResting", true);
                break;
        }
    }

    private void ExitState(PlayerAnimationState state)
    {
        switch (state)
        {
            case PlayerAnimationState.Idle:
                idleHandler.StopIdle();
                animator.SetBool("isIddle", false);
                break;

            case PlayerAnimationState.Walk:
                animator.SetBool("isWalking", false);
                break;

            case PlayerAnimationState.Blocking:
                animator.SetBool("IsBlocking", false);
                break;

            case PlayerAnimationState.Rest:
                animator.SetBool("IsResting", false);
                break;
        }
    }
}
