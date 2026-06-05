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
    private PlayerAnimationController anim;

    private bool isGrounded;
    public bool IsGrounded { get { return isGrounded; } set { isGrounded = value; } }
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
        StartCoroutine(GetAnimRef());
    }

    private IEnumerator GetAnimRef()
    {
        while (anim == null)
        {
            anim = PlayerAnimationController.Instance;
            yield return null; // espera 1 frame
        }
    }

    void LateUpdate()
    {
        CheckGround();

        if (airJumpTimer > 0f)
            airJumpTimer -= Time.deltaTime;
    }

    private void OnEnable()
    {
        if (InputActionController.Instance != null)
            InputActionController.Instance.OnActionTriggered += HandleJumpEvent;
    }

    private void OnDisable()
    {
        if (InputActionController.Instance != null)
            InputActionController.Instance.OnActionTriggered -= HandleJumpEvent;
    }

    private void CheckGround()
    {
        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, groundDistance, groundMask);

        if (isGrounded && !wasGrounded)
            jumpCount = 0;

        wasGrounded = isGrounded;

        characterMovement.IsOnAir = !isGrounded;
        anim.OnAir(!isGrounded); // ✅ reemplazo del viejo SetOnAir
    }

    private bool CanJump()
    {
        bool enoughHealth = player.Health.Get(HealthType.Mental) > jumpUseCost;

        if (isGrounded)
            return enoughHealth;
        else
            return jumpCount < maxJump && enoughHealth && airJumpTimer <= 0f;
    }

    private void HandleJumpEvent(InputActionType actionName)
    {
        if (actionName != InputActionType.Jump) return;

        if (CanJump())
        {
            PerformJump();
            PlayerActionLogger.Instance.Log("Jump",
                new List<string> { $"JumpCount:{jumpCount}" }, true);
        }
    }

    private void PerformJump()
    {
        anim.Jump(); // ✅ reemplazo del viejo StartJumping()

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        jumpCount++;
        player.UseMentalPulse(jumpUseCost);

        RumbleController.RumblePulse(0.05f, 0.2f, 0.1f);

        if (!isGrounded)
            airJumpTimer = airJumpCooldown;
    }

    public void StallAir(float duration)
    {
        StartCoroutine(StallAirCoroutine(duration));
    }

    private IEnumerator StallAirCoroutine(float duration)
    {
        rb.useGravity = false;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        yield return new WaitForSeconds(duration);

        rb.useGravity = true;
    }
}
