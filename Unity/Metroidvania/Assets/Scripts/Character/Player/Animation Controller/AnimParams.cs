using UnityEngine;

public static class AnimParams
{
    public static readonly int CurrentHealthPercentage = Animator.StringToHash("CurrentHealthPercentage");
    public static readonly int IsOnAir = Animator.StringToHash("isOnAir");
    public static readonly int IsMoving = Animator.StringToHash("isMoving");
    public static readonly int IsAttacking = Animator.StringToHash("isAttacking");
    public static readonly int IsBlocking = Animator.StringToHash("isBlocking");
    public static readonly int IsResting = Animator.StringToHash("isResting");
    public static readonly int IsHeavyAttack = Animator.StringToHash("isHeavyAttack");

    public static readonly int TrigJumping = Animator.StringToHash("isJumping");
    public static readonly int TrigEvade = Animator.StringToHash("isEvading");
    public static readonly int TrigDrink = Animator.StringToHash("isDrinking");
    public static readonly int TrigHit = Animator.StringToHash("isTakingDamage"); // <- tu trigger de daño
    public static readonly int TrigDie = Animator.StringToHash("isDying");        // aquí usaremos bool, ver más abajo

    public static readonly int ComboState = Animator.StringToHash("Combo State");
    public static readonly int DamageState = Animator.StringToHash("Damage State");
    public static readonly int DieState = Animator.StringToHash("Die State");
}
