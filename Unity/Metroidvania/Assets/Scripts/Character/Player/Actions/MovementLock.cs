using System.Collections;
using UnityEngine;
using System;

public class MovementControl : MonoBehaviour
{
    private CharacterMovement characterMovement;
    private CharacterJump characterJump;

    private Coroutine lockCoroutine;
    private bool isLocked;

    public bool IsLocked => isLocked;

    // Evento opcional para saber cuándo termina un lock
    public event Action OnUnlock;

    void Awake()
    {
        characterMovement = GetComponent<CharacterMovement>();
        characterJump = GetComponent<CharacterJump>();

        if (characterMovement == null)
            Debug.LogWarning("[MovementLock] No se encontró CharacterMovement");

        if (characterJump == null)
            Debug.LogWarning("[MovementLock] No se encontró CharacterJump");
    }

    /// <summary>
    /// Bloquea el movimiento y salto por un tiempo determinado.
    /// Si ya hay un bloqueo activo, lo reinicia.
    /// </summary>
    public void LockMovement(float duration)
    {
        if (lockCoroutine != null)
            StopCoroutine(lockCoroutine);

        lockCoroutine = StartCoroutine(LockRoutine(duration));
    }

    private IEnumerator LockRoutine(float duration)
    {
        isLocked = true;
        SetMovementState(false);

        yield return new WaitForSeconds(duration);

        SetMovementState(true);
        isLocked = false;

        OnUnlock?.Invoke();
    }

    private void SetMovementState(bool canMove)
    {
        if (characterMovement != null)
            characterMovement.CanMove = canMove;
    }

    public bool IsJumping => !characterJump.IsGrounded;
    public void StallAir(float time)
    {
        characterJump.StallAir(time);
    }
    public bool isMoving => characterMovement.HorizontalInput != 0;
}
