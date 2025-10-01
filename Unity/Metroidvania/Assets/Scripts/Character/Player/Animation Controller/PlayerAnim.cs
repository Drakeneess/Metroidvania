using UnityEngine;

public static class PlayerAnim
{
    private static IPlayerAnimator _impl;

    public static void Bind(IPlayerAnimator impl) => _impl = impl;

    private static IPlayerAnimator Impl {
        get {
            if (_impl == null) _impl = (IPlayerAnimator)Object.FindObjectOfType<PlayerAnimationController>();
            return _impl;
        }
    }

    // Reexpone toda tu API:
    public static void SetCurrentHealthPercentage(float v) => Impl.SetCurrentHealthPercentage(v);
    public static void IsOnAir(bool v) => Impl.SetOnAir(v);
    public static void SetWalkState(bool w, bool force=false) => Impl.SetWalkState(w, force);
    public static void SetBlocking() => Impl.SetBlocking();
    public static void SetMoving(bool v) => Impl.SetMoving(v);
    public static void StartJumping() => Impl.StartJumping();
    public static void SetResting() => Impl.SetResting();
    public static void SetAttackState(bool force=true) => Impl.SetAttackState(force);
    public static void SetAttackComboState(int combo) => Impl.SetAttackComboState(combo);
    public static void SetHeavyAttack(bool v) => Impl.SetHeavyAttack(v);
    public static void SetEvading() => Impl.SetEvading();
    public static void SetCuring() => Impl.SetCuring();
    public static void SetDying() => Impl.SetDying();
    public static void SetTakingDamage() => Impl.SetTakingDamage();
    public static void NotifyTransientFinished() => Impl.NotifyTransientFinished();
}
