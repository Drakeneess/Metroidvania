using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private LightAttack lightAttack;
    [SerializeField] private HeavyAttack heavyAttack;
    [SerializeField] private MirrorVisualController mirrorVisual;

    public ComboController comboController { get; private set; }
    private MovementControl movementControl;
    public  WeaponContext   weaponContext;

    private Coroutine recoveryCoroutine;
    private bool inputReceivedInWindow = false;
    private bool canChangeWeapon = true;
    private bool isStringActive = false;
    private Weapon pendingWeapon;

    public CombatState CurrentState { get; private set; }
    public bool CanChangeWeapon => canChangeWeapon;

    // Timings por arma activa
    public float ComboResetTime { get; private set; } // ventana total (exec + window)
    public float RecoveryTime   { get; private set; } // cooldown de ataque (gating de input)
    public float WeaponMentalHealthUsage { get; private set; }

    public bool isHeavyAttackActive { get; set; }

    // === Recovery (estado) fijo + Cooldown de ataque por arma ===
    [SerializeField] private float recoveryStateDuration = 0.20f; // duración del estado Recovery (visual)
    private float attackCooldownEndTime = 0f;                     // gating de input

    public bool  IsAttackOnCooldown      => Time.time < attackCooldownEndTime;
    public float AttackCooldownRemaining => Mathf.Max(0f, attackCooldownEndTime - Time.time);

    // === Marcadores de tiempo (debug) ===
    private float stateStartTime;
    private float execEndTime;
    private float windowEndTime;
    private float lastExecDuration;
    private float lastWindowDuration;
    private float lastRecoveryDuration;

    public float StateElapsed      => Time.time - stateStartTime;
    public float ExecRemaining     => execEndTime  > Time.time ? execEndTime  - Time.time : 0f;
    public float WindowRemaining   => windowEndTime> Time.time ? windowEndTime- Time.time : 0f;
    public float LastExecDuration  => lastExecDuration;
    public float LastWindowDuration=> lastWindowDuration;
    public float LastRecoveryDuration => lastRecoveryDuration;
    public bool  IsWindowOpen      => windowEndTime > Time.time;

    void Awake()
    {
        comboController = new ComboController();
        weaponContext   = new WeaponContext();
    }

    void Start()
    {
        if (player == null) player = FindObjectOfType<Player>();
        movementControl = player.GetComponent<MovementControl>();

        weaponContext.player           = player;
        weaponContext.movementControl  = movementControl;
        weaponContext.combatController = this;
        weaponContext.comboController  = comboController;

        lightAttack.Init(weaponContext);
        heavyAttack.Init(weaponContext);
    }

    void OnEnable()
    {
        var input = InputActionController.Instance;
        if (input != null)
        {
            input.OnActionTriggered += LightAttackInput;
            input.OnFloatInput      += HeavyAttackInput;
        }
    }

    void OnDisable()
    {
        var input = InputActionController.Instance;
        if (input != null)
        {
            input.OnActionTriggered -= LightAttackInput;
            input.OnFloatInput      -= HeavyAttackInput;
        }
    }

    private void LightAttackInput(string actionName)
    {
        if (actionName != "LightAttack") return;
        if (!CanAcceptAttackInput())     return;

        PlayerActionLogger.Instance.Log("Light Attacking",new List<string> { $"Current Weapon: {weaponContext.weapon.GetToolName()}", $"Current Combo: {comboController.GetCombo()}" },true);

        lightAttack.Execute();

        if (recoveryCoroutine != null) inputReceivedInWindow = true;
    }

    private void HeavyAttackInput(string actionName, float value)
    {
        if (actionName != "HeavyAttack") return;
        if (!CanAcceptAttackInput() )     return;

        if (value > 0.5f)
        {
            heavyAttack.StartCharge();
            PlayerActionLogger.Instance.Log("Heavy Attack Charge", new List<string>{ $"Current Weapon: {weaponContext.weapon.GetToolName()}" }, true);
        }
        else
        {
            heavyAttack.Release();
            PlayerActionLogger.Instance.Log("Heavy Attack Release", new List<string> { $"Current Weapon: {weaponContext.weapon.GetToolName()}" }, true);
        }

        if (recoveryCoroutine != null) inputReceivedInWindow = true;
    }

    private bool CanAcceptAttackInput()
    {
        if (CurrentState == CombatState.Recovery) return false;
        if (IsAttackOnCooldown) return false;

        // 🚫 Si hay ventana pero NO queda combo, no aceptar input (evita combo infinito)
        if (IsWindowOpen && !comboController.canContinueCombo) return false;

        return true;
    }

    public void TriggerAttack(bool keepWeaponActive)
    {
        PlayerAnimationController.SetAttackState();

        // 🔹 Usamos el índice planeado en vez de GetCombo()
        int plannedIndex = comboController.GetPlannedComboIndex();

        PlayerAnimationController.SetAttackComboState(plannedIndex);
        mirrorVisual.Mirror.SetAttackingState(true);

        if (!isStringActive && mirrorVisual.IsMirrorActive)
        {
            mirrorVisual.TransitionMirrorToWeapon();
            isStringActive = true;
        }
    }

    public void StartRecoveryWindow(float windowSeconds)
    {
        if (recoveryCoroutine != null) StopCoroutine(recoveryCoroutine);
        recoveryCoroutine = StartCoroutine(RecoveryWindowCoroutine(windowSeconds));
    }

    private IEnumerator RecoveryWindowCoroutine(float windowSeconds)
    {
        inputReceivedInWindow = false;
        float elapsed = 0f;

        while (elapsed < windowSeconds)
        {
            // Si llega input y AÚN queda combo, reinicia ventana
            if (inputReceivedInWindow && comboController.canContinueCombo)
            {
                inputReceivedInWindow = false;
                elapsed = 0f;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        EndCombo(); // ventana terminó: recién aquí volvemos al espejo
    }

    public void EndCombo()
    {
        mirrorVisual.TransitionWeaponToMirror();
        isStringActive = false;

        // Aplica cambio en cola (si lo hay)
        if (pendingWeapon != null)
        {
            var w = pendingWeapon;
            pendingWeapon = null;
            SetActiveWeapon(w);

            // (UI) -> si manejas UI diferido, notifícalo aquí
            // WeaponController.Instance?.ApplyWeaponChange(w);
        }

        // Cooldown por arma
        attackCooldownEndTime = Time.time + RecoveryTime;

        StartCoroutine(RecoverFromAttack());
    }

    public IEnumerator RecoverFromAttack()
    {
        float recStart = Time.time;
        if (recoveryStateDuration > 0f)
            yield return new WaitForSeconds(recoveryStateDuration);
        lastRecoveryDuration = Time.time - recStart;

        PlayerAnimationController.SetWalkState(movementControl.isMoving, true);
        PlayerAnimationController.SetAttackComboState(0);

        comboController.ResetCombo();
        canChangeWeapon = true;

        if (recoveryCoroutine != null)
        {
            StopCoroutine(recoveryCoroutine);
            recoveryCoroutine = null;
        }
    }

    public void SetActiveWeapon(Weapon weapon)
    {
        if (!canChangeWeapon) return;

        weaponContext.weapon = weapon;
        mirrorVisual.SetNewWeapon(weapon);
        mirrorVisual.Mirror.attackRange = weapon.GetRange();
        weapon.SetCombatController(this);

        comboController.SetNewMaxCombo(weapon.GetMaxCombo());
        WeaponMentalHealthUsage = weapon.GetMentalHealthUsage();

        ComboResetTime = weapon.GetComboResetTime();
        RecoveryTime   = weapon.GetRecoveryTime();
    }
}

public enum CombatState
{
    Idle,
    Preparing,
    LightAttacking,
    HeavyAttacking,
    Recovery
}

public class WeaponContext
{
    public Player player;
    public MovementControl movementControl;
    public CombatController combatController;
    public ComboController comboController;
    public Weapon weapon;
}
