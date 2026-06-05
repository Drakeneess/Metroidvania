using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private LightAttack lightAttack;
    [SerializeField] private HeavyAttack heavyAttack;
    [SerializeField] private MirrorVisualController mirrorVisual;

    private PlayerAnimationController anim;

    public ComboController comboController { get; private set; }
    private MovementControl movementControl;
    public WeaponContext weaponContext;

    private Coroutine recoveryCoroutine;
    private bool inputReceivedInWindow = false;
    private bool canChangeWeapon = true;
    private bool isStringActive = false;
    private Weapon pendingWeapon;

    public CombatState CurrentState { get; private set; }
    public bool CanChangeWeapon => canChangeWeapon;

    public float ComboResetTime { get; private set; }
    public float RecoveryTime { get; private set; }
    public float WeaponMentalHealthUsage { get; private set; }

    public bool isHeavyAttackActive { get; set; }

    [SerializeField] private float recoveryStateDuration = 0.20f;
    private float attackCooldownEndTime = 0f;

    public bool IsAttackOnCooldown => Time.time < attackCooldownEndTime;

    private float stateStartTime;
    private float windowEndTime;
    private float lastRecoveryDuration;

    public bool IsWindowOpen => windowEndTime > Time.time;

    void Awake()
    {
        comboController = new ComboController();
        weaponContext = new WeaponContext();
    }

    void Start()
    {
        if (player == null) player = FindObjectOfType<Player>();
        movementControl = player.GetComponent<MovementControl>();
        anim = PlayerAnimationController.Instance;

        weaponContext.player = player;
        weaponContext.movementControl = movementControl;
        weaponContext.combatController = this;
        weaponContext.comboController = comboController;

        lightAttack.Init(weaponContext);
        heavyAttack.Init(weaponContext);
    }

    void OnEnable()
    {
        var input = InputActionController.Instance;
        if (input != null)
        {
            input.OnActionTriggered += LightAttackInput;
            input.OnFloatInput += HeavyAttackInput;
        }
    }

    void OnDisable()
    {
        var input = InputActionController.Instance;
        if (input != null)
        {
            input.OnActionTriggered -= LightAttackInput;
            input.OnFloatInput -= HeavyAttackInput;
        }
    }

    private void LightAttackInput(InputActionType actionName)
    {
        if (actionName != InputActionType.LightAttack || weaponContext.weapon==null) return;
        if (!CanAcceptAttackInput()) return;

        PlayerActionLogger.Instance.Log("Light Attacking", new List<string>
        {
            $"Current Weapon: {weaponContext.weapon.GetToolName()}",
            $"Current Combo: {comboController.GetPlannedComboIndex()}"
        }, true);

        lightAttack.Execute();

        if (recoveryCoroutine != null)
            inputReceivedInWindow = true;
    }

    private void HeavyAttackInput(InputActionType actionName, float value)
    {
        if (actionName != InputActionType.HeavyAttack || weaponContext.weapon==null) return;
        if (!CanAcceptAttackInput()) return;

        if (value > 0.5f)
        {
            heavyAttack.StartCharge();
            PlayerActionLogger.Instance.Log("Heavy Attack Charge",
                new List<string> { $"Current Weapon: {weaponContext.weapon.GetToolName()}" }, true);
        }
        else
        {
            heavyAttack.Release();
            PlayerActionLogger.Instance.Log("Heavy Attack Release",
                new List<string> { $"Current Weapon: {weaponContext.weapon.GetToolName()}" }, true);
        }

        if (recoveryCoroutine != null)
            inputReceivedInWindow = true;
    }

    private bool CanAcceptAttackInput()
    {
        if (CurrentState == CombatState.Recovery) return false;
        if (IsAttackOnCooldown) return false;

        if (IsWindowOpen && !comboController.canContinueCombo) return false;

        return true;
    }

    public void TriggerAttack(bool keepWeaponActive)
    {
        anim.Attack();

        int plannedIndex = comboController.GetPlannedComboIndex();
        anim.SetAttackComboState(plannedIndex);

        mirrorVisual.Mirror.SetAttackingState(true);

        if (!isStringActive && mirrorVisual.IsMirrorActive)
        {
            mirrorVisual.TransitionMirrorToWeapon();
            isStringActive = true;
        }
    }

    public void StartRecoveryWindow(float windowSeconds)
    {
        if (recoveryCoroutine != null)
            StopCoroutine(recoveryCoroutine);

        recoveryCoroutine = StartCoroutine(RecoveryWindowCoroutine(windowSeconds));
    }

    private IEnumerator RecoveryWindowCoroutine(float windowSeconds)
    {
        inputReceivedInWindow = false;
        float elapsed = 0f;

        while (elapsed < windowSeconds)
        {
            if (inputReceivedInWindow && comboController.canContinueCombo)
            {
                inputReceivedInWindow = false;
                elapsed = 0f;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        EndCombo();
    }

    public void EndCombo()
    {
        mirrorVisual.TransitionWeaponToMirror();
        isStringActive = false;

        if (pendingWeapon != null)
        {
            var w = pendingWeapon;
            pendingWeapon = null;
            SetActiveWeapon(w);
        }

        attackCooldownEndTime = Time.time + RecoveryTime;
        StartCoroutine(RecoverFromAttack());
    }

    public IEnumerator RecoverFromAttack()
    {
        float recStart = Time.time;

        if (recoveryStateDuration > 0f)
            yield return new WaitForSeconds(recoveryStateDuration);

        lastRecoveryDuration = Time.time - recStart;

        anim.Move(movementControl.isMoving);
        anim.SetAttackComboState(0);

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
        RecoveryTime = weapon.GetRecoveryTime();
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
