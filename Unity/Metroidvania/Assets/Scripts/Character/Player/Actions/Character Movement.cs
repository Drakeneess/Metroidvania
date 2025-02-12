using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5.0f;

    private float direction = 1;
    public float Direction { get { return direction; } }
    private float currentSpeed;

    // Variables para almacenar los inputs actuales
    private float horizontalInput = 0f;

    void Start()
    {
        currentSpeed = speed;
    }

    private void OnEnable() 
    {
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnMovement += HandleMovementInput;
            InputActionController.Instance.OnRunning += HandleRunningInput;
        }
    }

    private void OnDisable() 
    {
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnMovement -= HandleMovementInput;
            InputActionController.Instance.OnRunning -= HandleRunningInput;
        }
    }

    void Update()
    {
        // Se actualiza la rotación y se mueve el personaje en función del input recibido
        HandleRotation();
    }

    #region Eventos de Input

    // Actualiza el valor del movimiento horizontal
    private void HandleMovementInput(float value)
    {
        horizontalInput = value;
    }

    // Actualiza la velocidad según el input de correr
    private void HandleRunningInput(float value)
    {
        if (value > 0.5f)
        {
            currentSpeed = speed * 1.5f;
        }
        else
        {
            currentSpeed = speed;
        }
    }

    #endregion

    private void HandleRotation()
    {
        if (Mathf.Abs(horizontalInput) > 0.01f)
        {
            // Determina la dirección según el input
            direction = Mathf.Sign(horizontalInput);
            float targetRotation = direction == 1 ? 0 : 180;
            transform.rotation = Quaternion.Euler(0, targetRotation, 0);
            HandleMovement(horizontalInput);
        }
    }

    // Mueve al personaje en el eje X (el eje Z se fuerza a cero)
    private void HandleMovement(float directionMove)
    {
        Vector3 move = transform.right * currentSpeed * directionMove * Time.deltaTime;
        move.z = 0f;
        transform.Translate(move, Space.Self);
    }
}
