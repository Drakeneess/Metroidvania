using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5.0f;

    private float direction = 1;
    public float Direction => direction;

    private float currentSpeed;
    private bool canMove = true;
    public bool CanMove { get => canMove; set => canMove = value; }

    private bool isOnAir = false;
    public bool IsOnAir { get => isOnAir; set => isOnAir = value; }

    private float horizontalInput = 0f;
    public float HorizontalInput => horizontalInput;

    public event Action<float> OnMovementInputChanged;

    private PlayerAnimationController anim;

    private void Awake()
    {
        currentSpeed = speed;
        anim = PlayerAnimationController.Instance;

        // 🔥 Si todavía no existe en Awake, lo intentamos luego
        if (anim == null)
            StartCoroutine(WaitForAnimator());
    }

    private IEnumerator WaitForAnimator()
    {
        // Espera a que el PlayerAnimationController haga su Start y registre Instance
        while (PlayerAnimationController.Instance == null)
            yield return null;

        anim = PlayerAnimationController.Instance;
    }

    private void OnEnable()
    {
        if (InputActionController.Instance != null)
            InputActionController.Instance.OnFloatInput += HandleActionInput;
    }

    private void OnDisable()
    {
        if (InputActionController.Instance != null)
            InputActionController.Instance.OnFloatInput -= HandleActionInput;
    }

    private void HandleActionInput(string actionName, float value)
    {
        switch (actionName)
        {
            case "Movement":
                HandleMovementInput(value);
                break;
            case "Run":
                HandleRunningInput(value);
                break;
        }
    }

    private void Update()
    {
        HandleRotation();
    }

    private void HandleMovementInput(float value)
    {
        horizontalInput = value;
        OnMovementInputChanged?.Invoke(horizontalInput);

        string actionType = value == 0 ? "Stopped" : "Movement";
        string directionName = value == 0 ? "None" : (value > 0 ? "Right" : "Left");
        PlayerActionLogger.Instance.Log(actionType, new List<string> { $"Direction: {directionName}" });

        anim.Move(value != 0);

        // 🔥 Fuerza reevaluación inmediata de Idle/Walk en la FSM
        PlayerAnimationController.Instance?.ForceBaseIdleOrWalk();
    }

    private void HandleRunningInput(float value)
    {
        currentSpeed = value > 0.5f ? speed * 1.5f : speed;
    }

    private void HandleRotation()
    {
        if (Mathf.Abs(horizontalInput) <= 0.01f) return;

        direction = Mathf.Sign(horizontalInput);
        float targetRotation = direction == 1 ? 0 : 180;
        transform.rotation = Quaternion.Euler(0, targetRotation, 0);

        HandleMovement(horizontalInput);
    }

    private void HandleMovement(float directionMove)
    {
        if (!canMove) return;

        Vector3 move = transform.right * currentSpeed * directionMove * Time.deltaTime;
        move.z = 0f;
        transform.Translate(move, Space.Self);
    }
}
