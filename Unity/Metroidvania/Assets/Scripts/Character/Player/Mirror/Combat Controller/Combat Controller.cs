using System;
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
    private WeaponContext weaponContext;
    private Coroutine recoveryCoroutine;
    private bool inputReceivedInWindow = false;

    private bool canChangeWeapon = true;
    public CombatState CurrentState { get; private set; }
    public bool CanChangeWeapon => canChangeWeapon;
    public float ComboResetTime { get; set; }
    public float RecoveryTime { get; set; }
    public float WeaponMentalHealthUsage { get; set; }
    public bool isHeavyAttackActive { get;  set; }

    void Awake()
    {
        comboController = new ComboController();
        weaponContext = new WeaponContext();
    }

    private void Start()
    {
        if (player == null) player = FindObjectOfType<Player>();
        movementControl = player.GetComponent<MovementControl>();

        weaponContext.player = player;
        weaponContext.movementControl = movementControl;
        weaponContext.combatController = this;
        weaponContext.comboController = comboController;

        lightAttack.Init(weaponContext);
        heavyAttack.Init(weaponContext);
    }

    private void OnEnable()
    {
        var input = InputActionController.Instance;
        if (input != null)
        {
            input.OnActionTriggered += LightAttackInput;
            input.OnFloatInput += HeavyAttackInput;
        }
    }

    private void OnDisable()
    {
        var input = InputActionController.Instance;
        if (input != null)
        {
            input.OnActionTriggered -= LightAttackInput;
            input.OnFloatInput -= HeavyAttackInput;
        }
    }

    private void LightAttackInput(string actionName)
    {
        if (actionName == "LightAttack")
        {
            lightAttack.Execute();
            inputReceivedInWindow = true;
        }
    }

    private void HeavyAttackInput(string actionName, float value)
    {
        if (actionName != "HeavyAttack") return;
        if (value > 0.5f) heavyAttack.StartCharge();
        else heavyAttack.Release();
    }

    public void ChangeState(CombatState newState) => CurrentState = newState;

    public void TriggerAttack(bool keepWeaponActive)
    {
        mirrorVisual.Mirror.SetAttackingState(false);
        PlayerAnimationController.SetAttackState();
        PlayerAnimationController.SetAttackComboState(comboController.GetCombo());

        mirrorVisual.SetMirrorState(false); // Solo desactiva si es LightAttack

        mirrorVisual.ResetActivationTimer(true, () => isHeavyAttackActive);

        if (!keepWeaponActive)
            StartCoroutine(RecoverFromAttack());
    }

    public IEnumerator RecoverFromAttack()
    {
        print("En recuperacion");
        yield return new WaitForSeconds(ComboResetTime);
        
        ChangeState(CombatState.Recovery);
        yield return new WaitForSeconds(RecoveryTime);
        PlayerAnimationController.SetWalkState(movementControl.isMoving, true);
        PlayerAnimationController.SetAttackComboState(0);
        ChangeState(CombatState.Idle);
        comboController.ResetCombo();
        canChangeWeapon = true;
    }

    public void StartRecoveryWindow()
    {
        // Cancelar ventana anterior
        if (recoveryCoroutine != null)
            StopCoroutine(recoveryCoroutine);

        // Iniciar nueva ventana
        recoveryCoroutine = StartCoroutine(RecoveryWindowCoroutine());
    }

    private IEnumerator RecoveryWindowCoroutine()
    {
        inputReceivedInWindow = false;

        float elapsed = 0f;
        while (elapsed < ComboResetTime)
        {
            // Si ya no puede continuar combo, se cierra inmediatamente
            if (!comboController.canContinueCombo)
            {
                EndCombo();
                yield break;
            }

            // Si llegó input y todavía hay combo disponible, reinicia el timer
            if (inputReceivedInWindow && comboController.canContinueCombo)
            {
                inputReceivedInWindow = false;
                elapsed = 0f;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Se acabó la ventana sin más inputs
        EndCombo();
    }


    public void EndCombo()
    {
        StartCoroutine(RecoverFromAttack());
    }

    public void SetActiveWeapon(Weapon weapon)
    {
        if (!canChangeWeapon) return;

        weaponContext.weapon = weapon;
        mirrorVisual.SetNewWeapon(weapon);
        mirrorVisual.Mirror.attackRange = weapon.GetRange();
        mirrorVisual.ReactivateDelay = weapon.GetComboResetTime();
        comboController.SetNewMaxCombo(weapon.GetMaxCombo());
        WeaponMentalHealthUsage = weapon.GetMentalHealthUsage();
        RecoveryTime = weapon.GetRecoveryTime();
        ComboResetTime = weapon.GetComboResetTime();
    }
}


public enum CombatState
{
    Idle,
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

