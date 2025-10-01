using UnityEngine;

public class ComboController
{
    public int maxComboCount = 0;
    private int currentComboCount = -1; // opción A
    private int pendingStep = 0;

    public bool canContinueCombo => (currentComboCount + pendingStep) < maxComboCount;

    public void SetNewMaxCombo(int newMaxCombo)
    {
        if (!canContinueCombo) ResetCombo();
        maxComboCount = newMaxCombo;
    }

    public int GetCombo() => currentComboCount;

    public void ResetCombo()
    {
        currentComboCount = -1; // opción A
        pendingStep = 0;
    }

    public bool TryConsumeNext()
    {
        if (!canContinueCombo) return false;
        pendingStep++;
        return true;
    }

    public void ConfirmStep()
    {
        if (pendingStep > 0)
        {
            currentComboCount += 1;   // -1 -> 0 en el primer impacto
            pendingStep -= 1;
        }
    }

    // 🔹 NUEVO: índice planeado (lo que se va a ejecutar ahora)
    public int GetPlannedComboIndex()
    {
        int next = currentComboCount + pendingStep; // tras TryConsumeNext será el siguiente válido
        // Clamp por seguridad (aunque TryConsumeNext lo garantiza)
        if (maxComboCount > 0)
            next = Mathf.Clamp(next, 0, maxComboCount - 1);
        return next;
    }
}
