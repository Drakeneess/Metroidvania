using System;
using System.Collections;
using UnityEngine;

public class CharacterDash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashSpeed = 15f; // Velocidad del Dash
    public float dashDuration = 0.3f; // Duración del Dash

    private Rigidbody rb;
    private Player player;
    private bool isDashing = false; // Para verificar si el personaje está en dash
    private CharacterMovement characterMovement; // Referencia al script de movimiento del personaje

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GetComponent<Player>();
        characterMovement = GetComponent<CharacterMovement>();
    }
    private void OnEnable() {
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnDash += HandleDashInput;
        }
    }

    private void OnDisable() 
    {
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnDash -= HandleDashInput;
        }
    }


    private void HandleDashInput()
    {
        if (!isDashing && player.GetCurrentHealth(HealthType.Mental) > 0f) // Evita que se active el dash si ya está en proceso
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        player.CanTakePhysicalDamage = false;
        isDashing = true;
        player.UseMentalPulse(5f);
        RumbleController.RumblePulse(0.05f, 0.2f, 0.1f);
        float dashEndTime = Time.time + dashDuration;

        // Calculamos la dirección hacia donde el jugador está mirando
        Vector3 dashDirection = new Vector3(characterMovement.Direction, 0f, 0f);

        // Aplicamos la velocidad del dash
        rb.velocity = dashDirection * dashSpeed;

        // Mientras dure el dash, mantenemos la velocidad
        while (Time.time < dashEndTime)
        {
            yield return null;
        }

        // Restablecemos la velocidad a su valor normal después del dash
        rb.velocity = Vector3.zero;
        isDashing = false;
        player.CanTakePhysicalDamage = true;
    }
}
