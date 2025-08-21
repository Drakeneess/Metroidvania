using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterJump : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpForce = 5.0f;
    public int maxJump = 2;
    public float jumpUseCost = 2.5f;
    [Header("Air Jump Cooldown")]
    public float airJumpCooldown = 0.6f;
    private float airJumpTimer = 0f;

    private Rigidbody rb;
    private Player player;
    private CharacterMovement characterMovement;
    
    private bool isGrounded;
    public bool IsGrounded{ get { return isGrounded; } set { isGrounded = value; } }
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
        characterMovement = GetComponent<CharacterMovement>();
        jumpCount = 0;
    }

    void LateUpdate()
    {
        CheckGround();

        // Disminuye el cooldown si está activo
        if (airJumpTimer > 0f)
        {
            airJumpTimer -= Time.deltaTime;
        }
    }

    private void OnEnable() 
    {
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnActionTriggered += HandleJumpEvent;
        }
    }

    private void OnDisable() 
    {
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnActionTriggered -= HandleJumpEvent;
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
        characterMovement.IsOnAir = !isGrounded;
        PlayerAnimationController.IsOnAir(!isGrounded);
    }

    /// <summary>
    /// Comprueba si el personaje puede saltar.
    /// </summary>
    private bool CanJump()
    {
        bool enoughHealth = player.GetCurrentHealth(HealthType.Mental) > jumpUseCost;

        if (isGrounded)
        {
            return enoughHealth;
        }
        else
        {
            return jumpCount < maxJump && enoughHealth && airJumpTimer <= 0f;
        }
    }

    /// <summary>
    /// Realiza el salto, aplicando fuerza y reseteando la velocidad vertical.
    /// </summary>
    private void HandleJumpEvent(string actionName)
    {
        if(actionName == "Jump"){
            if (CanJump())
            {
                PerformJump();
            }
        }
    }

    private void PerformJump()
    {
        PlayerAnimationController.StartJumping();

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        jumpCount++;
        player.UseMentalPulse(jumpUseCost);
        RumbleController.RumblePulse(0.05f, 0.2f, 0.1f);

        // Solo activa el cooldown si NO estás en el suelo
        if (!isGrounded)
        {
            airJumpTimer = airJumpCooldown;
        }
        player.TakePhysicalDamage(10);
    }


    public void StallAir(float duration)
    {
        StartCoroutine(StallAirCoroutine(duration));
    }

    private IEnumerator StallAirCoroutine(float duration)
    {
        // Desactivamos la gravedad y “reseteamos” la velocidad vertical
        rb.useGravity = false;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // Esperamos un momento
        yield return new WaitForSeconds(duration);

        // Restauramos la gravedad a su valor original
        rb.useGravity = true;
    }
}
