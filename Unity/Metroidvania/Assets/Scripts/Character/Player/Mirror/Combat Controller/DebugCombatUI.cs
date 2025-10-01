using UnityEngine;
using TMPro;

public class DebugCombatUI : MonoBehaviour
{
    [SerializeField] private CombatController combatController;
    [SerializeField] private TextMeshProUGUI debugText;

    void Update()
    {
        if (combatController == null || debugText == null) return;

        var w = combatController?.comboController != null && combatController?.GetType() != null
            ? combatController
            : null;

        // Accesos cómodos
        var state          = combatController.CurrentState;
        var stateElapsed   = combatController.StateElapsed;
        var execRemain     = combatController.ExecRemaining;
        var windowRemain   = combatController.WindowRemaining;

        var lastExec       = combatController.LastExecDuration;
        var lastWindow     = combatController.LastWindowDuration;
        var lastRecovery   = combatController.LastRecoveryDuration;

        // Arma actual (si quieres ver exec/comboReset por arma)
        float weaponExec  = combatController?.weaponContext?.weapon != null
            ? combatController.weaponContext.weapon.GetExecutionTime()
            : 0f;

        float weaponReset = combatController?.weaponContext?.weapon != null
            ? combatController.weaponContext.weapon.GetComboResetTime()
            : combatController.ComboResetTime;

        debugText.text =
            $"<b>Estado:</b> {state}  |  <b>t(estado):</b> {stateElapsed:0.00}s\n" +
            $"<b>Exec (arma):</b> {weaponExec:0.00}s  |  <b>ComboReset (arma):</b> {weaponReset:0.00}s\n" +
            $"<b>Exec restante:</b> {execRemain:0.00}s  |  <b>Ventana restante:</b> {windowRemain:0.00}s\n" +
            $"<b>Recovery cfg:</b> {combatController.RecoveryTime:0.00}s\n" +
            $"<b>Últimos:</b> exec={lastExec:0.00}s  |  window={lastWindow:0.00}s  |  recovery={lastRecovery:0.00}s\n" +
            $"<b>Combo:</b> {combatController.comboController.GetCombo()} / {combatController.comboController.maxComboCount}";
    }
}
