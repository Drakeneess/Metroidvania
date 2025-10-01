using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CureController : MonoBehaviour
{
    public static CureController Instance { get; private set; }

    [SerializeField] private Player player;
    [SerializeField] private int   maxCureQuantity = 3;
    [SerializeField] private float curingTime      = 0.5f; // duración del estado Healing (UI)
    [SerializeField] private Cure cure;
    private int currentCures;
    private bool isHealing;
    private Coroutine healingRoutine;

    // Eventos existentes (tu UI ya los usa)
    public event Action<int, int> OnCureUsed;      // (cures restantes, max) -> se dispara al CONSUMIR la cura
    public event Action<int>      OnCureUpgraded;  // (nuevo max)
    public event Action<int>      OnCureRestored;  // (cures actuales tras restaurar)

    // Eventos opcionales para otros sistemas (si quieres reaccionar a inicio/fin de curación)
    public event Action<float> OnHealingStarted;   // pasa la duración (curingTime)
    public event Action        OnHealingFinished;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        RestoreCures();
    }

    /// <summary>
    /// Intenta iniciar una curación. Devuelve false si no hay cargas, ya se está curando, o el player está a full HP (opcional).
    /// </summary>
    public bool TryUseCure(int healAmount = 40, bool blockWhenFullHP = true)
    {
        if (isHealing) return false;
        if (currentCures <= 0) return false;

        if (blockWhenFullHP && player != null)
        {
            float cur = player.Health.Get(HealthType.Physical);
            float max = player.Health.GetMax(HealthType.Physical);
            if (cur >= max - 0.001f) return false; // no tiene sentido curar
        }

        // Lanza la corutina de curación
        healingRoutine = StartCoroutine(HealRoutine(healAmount));
        return true;
    }

    private IEnumerator HealRoutine(int healAmount)
    {
        isHealing = true;

        // 1) Consumimos la carga YA, para que el UI lo refleje y marque Healing
        currentCures = Mathf.Max(0, currentCures - 1);
        OnCureUsed?.Invoke(currentCures, maxCureQuantity);

        // 2) Avisamos inicio de curación (otros sistemas pueden bloquear inputs, etc.)
        OnHealingStarted?.Invoke(curingTime);
        cure.ActivateCure();
        PlayerAnimationController.SetCuring();


        // 3) Espera durante la animación/feedback de cura
        yield return new WaitForSeconds(curingTime);

        // 4) Aplicamos la curación real al player
        if (player != null) player.Health.Restore(HealthType.Physical, healAmount);
        cure.DeactivateCure();
        // 5) Fin de curación
        PlayerActionLogger.Instance.Log("Cure", new List<string> { $"Remaining cures: {currentCures}" });
        OnHealingFinished?.Invoke();
        isHealing = false;
        healingRoutine = null;
    }

    /// <summary> Restaura todas las curas al máximo. </summary>
    public void RestoreCures()
    {
        currentCures = maxCureQuantity;
        OnCureRestored?.Invoke(currentCures);
    }

    /// <summary> Aumenta el máximo y opcionalmente llena. </summary>
    public void UpgradeMaxCures(int amount = 1, bool fillAfter = true)
    {
        maxCureQuantity += amount;
        if (fillAfter) currentCures = maxCureQuantity;
        OnCureUpgraded?.Invoke(maxCureQuantity);
        OnCureRestored?.Invoke(currentCures);
    }

    /// <summary> Añade curas sin exceder el máximo. </summary>
    public void AddCure(int amount = 1)
    {
        currentCures = Mathf.Min(currentCures + amount, maxCureQuantity);
        OnCureRestored?.Invoke(currentCures);
    }

    public (int current, int max) GetCureInfo() => (currentCures, maxCureQuantity);
    public float GetCuringTime() => curingTime;

    // Helpers opcionales
    public bool  IsHealing() => isHealing;
    public bool  HasCharges() => currentCures > 0;
    public int   GetCurrentCures() => currentCures;
    public int   GetMaxCures() => maxCureQuantity;
}

public enum CureStates
{
    Ready,
    FullHP,
    NoCharges,
    Healing,
    Cooldown
}
