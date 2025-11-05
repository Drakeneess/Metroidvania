using UnityEngine;

/// <summary>
/// Fachada estática opcional para invocar animaciones del jugador sin tener 
/// que guardar referencias al PlayerAnimationController.
/// </summary>
public static class PlayerAnim
{
    private static IPlayerAnimator _impl;

    /// <summary>
    /// Llamado por PlayerAnimationController al inicializarse.
    /// </summary>
    public static void Bind(IPlayerAnimator impl)
    {
        _impl = impl;
    }

    private static IPlayerAnimator Impl
    {
        get
        {
            if (_impl == null)
                _impl = Object.FindObjectOfType<PlayerAnimationController>() as IPlayerAnimator;

            return _impl;
        }
    }

    // ===========================
    // Parámetros generales
    // ===========================

    public static void SetCurrentHealthPercentage(float value) => Impl?.SetCurrentHealthPercentage(value);
    public static void SetOnAir(bool value) => Impl?.OnAir(value);
    public static void SetMove(bool isMoving) => Impl?.Move(isMoving);

    // ===========================
    // Estados persistentes (ON/OFF)
    // ===========================

    public static void SetBlock(bool enable) => Impl?.Block(enable);
    public static void SetRest(bool enable) => Impl?.Rest(enable);
    public static void SetDie(bool enable) => Impl?.Die(enable);

    // ===========================
    // Estados transitorios (acción única)
    // ===========================

    public static void SetAttack() => Impl?.Attack();
    public static void SetHeavyAttack(bool enable) => Impl?.HeavyAttack(enable);
    public static void SetJump() => Impl?.Jump();
    public static void SetEvade() => Impl?.Evade();
    public static void SetCure() => Impl?.Cure();
    public static void SetClimb() => Impl?.Climb();
    public static void SetTakeDamage() => Impl?.TakeDamage();

    // ===========================
    // Parámetros auxiliares
    // ===========================

    public static void SetAttackComboState(int combo) => Impl?.SetAttackComboState(combo);

    // ===========================
    // Utilidades
    // ===========================

    public static void SetClearAllPersistents() => Impl?.ClearAllPersistents();

    /// <summary>
    /// Llamar desde Animation Event al terminar un estado transitorio basado en Evento.
    /// </summary>
    public static void AnimEvent_EndTransient()
    {
        Impl?.NotifyTransientFinished();
    }
}
