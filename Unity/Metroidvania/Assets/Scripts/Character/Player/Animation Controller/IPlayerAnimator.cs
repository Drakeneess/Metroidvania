using UnityEngine;

/// <summary>
/// Interfaz que define todas las acciones de animación que el jugador puede ejecutar.
/// </summary>
public interface IPlayerAnimator
{
    // Parámetros generales
    void SetCurrentHealthPercentage(float value);
    void OnAir(bool value);
    void Move(bool isMoving);

    // Estados persistentes (ON/OFF)
    void Block(bool enable);
    void Rest(bool enable);
    void Die(bool enable);

    // Estados transitorios
    void Attack();
    void HeavyAttack(bool enable);
    void Jump();
    void Evade();
    void Cure();
    void Climb();
    void TakeDamage();

    // Auxiliares
    void SetAttackComboState(int combo);
    void NotifyTransientFinished();

    // Utilidades
    void ClearAllPersistents();
}
