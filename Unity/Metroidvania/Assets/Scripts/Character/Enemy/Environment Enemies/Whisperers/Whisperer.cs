using System.Collections;
using UnityEngine;

public class Whisperer : Enemy
{
    [Header("Ataque Dash")]
    [Tooltip("Multiplicador de velocidad durante el dash.")]
    public float dashSpeedMultiplier = 1.3f;
    [Tooltip("Duración del dash (en segundos).")]
    public float dashDuration = 0.4f;
    [Tooltip("Tiempo de recuperación tras el dash.")]
    public float postDashDelay = 0.25f;

    private bool isDashing = false;

    protected override IEnumerator DoAttack()
    {
        animator.SetBool("Alert", false);
        if (isDashing) yield break; // evita solaparse
        if (player == null) yield break;

        isAttacking = true;
        isDashing = true;
        nextAttackTime = Time.time + attackCooldown;

        // 🔹 Direccion horizontal pura (2.5D)
        float direction = (player.position.x > transform.position.x) ? 1f : -1f;
        Vector3 moveDir = new Vector3(direction, 0f, 0f);

        // 🔹 Movimiento por posición (no Rigidbody)
        float dashSpeed = CurrentSpeed * dashSpeedMultiplier;
        float timer = 0f;

        while (timer < dashDuration)
        {
            transform.position += moveDir * dashSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(postDashDelay);

        isAttacking = false;
        isDashing = false;
    }
}
