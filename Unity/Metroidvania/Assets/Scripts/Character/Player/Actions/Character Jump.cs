using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterJump : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpForce = 5.0f;
    public int maxJump = 2;

    private Rigidbody rb;
    private Player player;
    private bool isGrounded;
    private int jumpCount;
    private bool wasGrounded;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GetComponent<Player>();
        jumpCount = 0;
    }

    void Update()
    {
        // Se verifica el estado del suelo (para resetear saltos)
        CheckGround();
    }

    private void OnEnable() 
    {
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnJump += HandleJumpEvent;
        }
    }

    private void OnDisable() 
    {
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnJump -= HandleJumpEvent;
        }
    }

    private void CheckGround()
    {
        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, groundDistance, groundMask);

        // Si acaba de aterrizar, se reinicia el contador de saltos
        if (isGrounded && !wasGrounded)
        {
            jumpCount = 0;
        }
        wasGrounded = isGrounded;
    }

    /// <summary>
    /// Comprueba si el personaje puede saltar.
    /// </summary>
    private bool CanJump()
    {
        return (isGrounded || jumpCount < maxJump) && player.GetCurrentHealth(HealthType.Mental) > 0f;
    }

    /// <summary>
    /// Realiza el salto, aplicando fuerza y reseteando la velocidad vertical.
    /// </summary>
    private void HandleJumpEvent()
    {
        if (CanJump())
        {
            PerformJump();
        }
    }

    private void PerformJump()
    {
        // Resetea la velocidad vertical para evitar acumulación
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        jumpCount++;
        player.UseMentalPulse(2.5f);
        RumbleController.RumblePulse(0.05f, 0.2f, 0.1f);
    }
}
