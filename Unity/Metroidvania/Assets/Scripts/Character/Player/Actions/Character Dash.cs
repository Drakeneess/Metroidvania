using System;
using System.Collections;
using UnityEngine;

public class CharacterDash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.3f;

    private Rigidbody rb;
    private Player player;
    private CharacterMovement characterMovement;
    private PlayerAnimationController anim;

    private bool isDashing = false;

    private int characterLayer;
    private int playerLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GetComponent<Player>();
        characterMovement = GetComponent<CharacterMovement>();

        anim = PlayerAnimationController.Instance;

        playerLayer = gameObject.layer;
        characterLayer = LayerMask.NameToLayer("Character");
    }

    private void OnEnable()
    {
        if (InputActionController.Instance != null)
            InputActionController.Instance.OnActionTriggered += HandleDashInput;
    }

    private void OnDisable()
    {
        if (InputActionController.Instance != null)
            InputActionController.Instance.OnActionTriggered -= HandleDashInput;
    }

    private void HandleDashInput(string actionName)
    {
        if (actionName != "Dash") return;

        if (isDashing) return;
        if (characterMovement.IsOnAir) return;
        if (player.Health.Get(HealthType.Mental) <= 0f) return;

        StartCoroutine(Dash());
        PlayerActionLogger.Instance.Log("Dash");
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        player.Health.canTakePhysicalDamage = false;

        Physics.IgnoreLayerCollision(playerLayer, characterLayer, true);

        player.UseMentalPulse(5f);
        anim.Evade(); // ✅ Nuevo llamado
        RumbleController.RumblePulse(0.05f, 0.2f, 0.1f);

        float dashEndTime = Time.time + dashDuration;
        Vector3 dashDirection = new(characterMovement.Direction, 0f, 0f);

        rb.velocity = dashDirection * dashSpeed;

        while (Time.time < dashEndTime)
            yield return null;

        rb.velocity = Vector3.zero;
        isDashing = false;

        Physics.IgnoreLayerCollision(playerLayer, characterLayer, false);

        player.Health.canTakePhysicalDamage = true;
    }
}
