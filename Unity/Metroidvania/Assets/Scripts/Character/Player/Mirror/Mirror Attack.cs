using System.Collections;
using UnityEngine;

public class MirrorAttack : MonoBehaviour
{
    [SerializeField] private GameObject mirrorComponents;
    [SerializeField] private float reactivateMirrorTime = 2f;
    [SerializeField] private Mirror mirror;

    private CombatState currentState = CombatState.Idle;
    

    private int maxCombo;
    private float recoveryTime;
    private float comboResetTime;
    
    private Weapon currentWeapon;
    private Coroutine mirrorCoroutine;
    private bool isHeavyAttackActive = false;
    private Player player;
    private CharacterJump jump;
    private int comboCounter = 0;

    void Start()
    {
        if (mirror == null && !TryGetComponent(out mirror))
        {
            Debug.LogWarning("Mirror component not found on " + gameObject.name);
        }
        player = FindObjectOfType<Player>();
        if(player != null){
            jump = player.GetComponent<CharacterJump>();
        }

        LightAttack("LightAttack");
    }

    protected virtual void OnEnable()
    {
        var input = InputActionController.Instance;
        if (input != null)
        {
            input.OnActionTriggered += LightAttack;
            input.OnFloatInput += HeavyAttack;
        }
    }

    protected virtual void OnDisable()
    {
        var input = InputActionController.Instance;
        if (input != null)
        {
            input.OnActionTriggered -= LightAttack;
            input.OnFloatInput -= HeavyAttack;
        }
    }

    private void LightAttack(string actionName)
    {
        if(actionName == "LightAttack"){
            // Si se está cargando un heavy attack, ignoramos el light attack
            if (currentState == CombatState.HeavyAttacking) return;

            ChangeState(CombatState.LightAttacking);
            StartCoroutine(SetPlayerMovement(0.5f));
            TriggerAttack(false); // En light attack, el arma se desactiva momentáneamente
            if(currentWeapon.gameObject.activeSelf){
                StartCoroutine(WeaponAttack(0));
            }
            player.UseMentalPulse(currentWeapon.GetMentalHealthUsage());
            //comboCounter++;
            if(!jump.IsGrounded && comboCounter<=maxCombo){
                jump.StallAir(0.2f);
            }
        }
    }

    private IEnumerator WeaponAttack(int type){
        yield return new WaitForSeconds(0.4f);  // Esperar un pequeño tiempo antes de ejecutar el ataque.
        switch(type){
            case 0:
                currentWeapon.LightAttack(comboCounter);
                break;
            case 1:
                currentWeapon.HeavyAttack(comboCounter);
                break;
            default:
                Debug.LogError("Invalid attack type");
                break;
        }

    }

    /// <summary>
    /// Este método se llamará continuamente mientras se mantenga presionado el botón de heavy attack.
    /// Cuando el valor es mayor a 0.5 se activa la carga,
    /// cuando es menor se libera el ataque y se pasa a Recovery.
    /// </summary>
    /// <param name="value">Valor del input del heavy attack</param>
    private void HeavyAttack(string actionName, float value)
    {
        if(actionName == "HeavyAttack"){
            if (value > 0.5f)
            {
                // Inicia la carga si aún no se ha iniciado
                if (currentState != CombatState.HeavyAttacking)
                {
                    isHeavyAttackActive = true;
                    // Cancelar cualquier acción previa (por ejemplo, un light attack en curso)
                    StopAllCoroutines();
                    mirror?.SetAttackingState(true);

                    ChangeState(CombatState.HeavyAttacking);
                    StartCoroutine(SetPlayerMovement(0.7f)); // Duración del bloqueo de movimiento para heavy attack
                    TriggerAttack(true); // En heavy attack, mantenemos el arma activa (modo carga)
                }
                // Si ya estamos en HeavyAttack, se mantiene la carga (aquí se podría actualizar la UI o animación de carga)
            }
            else
            {
                // Si el valor es menor a 0.5 y estamos en heavy attack, se libera el ataque
                if (currentState == CombatState.HeavyAttacking)
                {
                    isHeavyAttackActive = false;
                    ReleaseHeavyAttack();
                }
            }
        }
    }


    /// <summary>
    /// Inicia el ataque y ajusta el estado del arma según el parámetro.
    /// </summary>
    /// <param name="keepWeaponActive">
    /// true: Mantiene el arma en modo carga (no desactiva mirrorComponents).  
    /// false: Se desactiva momentáneamente para el ataque light.
    /// </param>
    private void TriggerAttack(bool keepWeaponActive)
    {
        mirror?.SetAttackingState(true);
        ResetMirrorTimer();

        SetMirrorState(false);
        if (!keepWeaponActive)
        {
            // En light attack se inicia la recuperación automáticamente.
            StartCoroutine(RecoverFromAttack());
        }
    }


    /// <summary>
    /// Libera el heavy attack, restablece el estado del arma y activa la recuperación.
    /// </summary>
    private void ReleaseHeavyAttack()
    {
        StartCoroutine(ActivateMirror());
        StartCoroutine(SetPlayerMovement(1));
        // Inicia el proceso de recuperación
        StartCoroutine(RecoverFromAttack());
        player.UseMentalPulse(currentWeapon.GetMentalHealthUsage()*8);
        StartCoroutine(WeaponAttack(1));
        if(!jump.IsGrounded){
            jump.StallAir(0.4f);
        }
    }

    private void ChangeState(CombatState newState)
    {
        currentState = newState;
    }

    private IEnumerator RecoverFromAttack()
    {
        yield return new WaitForSeconds(recoveryTime);
        ChangeState(CombatState.Recovery);
        yield return new WaitForSeconds(0.5f);
        ChangeState(CombatState.Idle);
    }

    /// <summary>
    /// Controla la visibilidad de mirrorComponents y el estado del arma.
    /// Si isHeavyAttackActive es true, se mantiene el arma en modo carga.
    /// </summary>
    /// <param name="state">true para activar mirrorComponents, false para desactivarlos.</param>
    private void SetMirrorState(bool state)
    {
        mirrorComponents.SetActive(state);
        currentWeapon.SetToolActive(!state);
    }

    private void ResetMirrorTimer()
    {
        if (mirrorCoroutine != null)
        {
            StopCoroutine(mirrorCoroutine);
        }
        mirrorCoroutine = StartCoroutine(ActivateMirror());
    }

    private IEnumerator SetPlayerMovement(float duration)
    {
        mirror.Player.CanMove = false;
        yield return new WaitForSeconds(duration);
        mirror.Player.CanMove = true;
    }

    private IEnumerator ActivateMirror()
    {
        // Mientras se esté manteniendo el heavy attack, se espera.
        while (isHeavyAttackActive)
        {
            yield return null;
        }

        yield return new WaitForSeconds(reactivateMirrorTime);
        SetMirrorState(true);
        mirror?.SetAttackingState(false);
    }

    public void SetActiveWeapon(Weapon activeWeapon)
    {
        currentWeapon = activeWeapon;
        if (mirror != null)
        {
            mirror.attackRange = currentWeapon.GetRange();
            maxCombo = currentWeapon.GetMaxCombo();
            recoveryTime = currentWeapon.GetRecoveryTime();
            comboResetTime = currentWeapon.GetComboResetTime();
        }
    }
}

public enum CombatState
{
    Idle,
    LightAttacking,
    HeavyAttacking,
    Recovery
}
