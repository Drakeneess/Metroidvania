public interface IPlayerAnimator
{
    void SetCurrentHealthPercentage(float v);
    void SetOnAir(bool v);
    void SetWalkState(bool isWalking, bool force = false);
    void SetBlocking();
    void SetMoving(bool v);
    void StartJumping();
    void SetResting();
    void SetAttackState(bool force = true);
    void SetAttackComboState(int combo);
    void SetHeavyAttack(bool v);
    void SetEvading();
    void SetCuring();
    void SetDying();
    void SetTakingDamage();

    // Para clips transitorios con duración = 0
    void NotifyTransientFinished();
}
