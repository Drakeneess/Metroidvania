using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CureController : MonoBehaviour
{
    public static CureController Instance { get; private set; }

    [SerializeField] private Player player;
    [SerializeField] private int maxCureQuantity = 3;
    [SerializeField] private float curingTime = 0.5f;
    [SerializeField] private Cure cure;

    private int currentCures;
    private bool isHealing;
    private Coroutine healingRoutine;
    private PlayerAnimationController anim;

    public event Action<int, int> OnCureUsed;
    public event Action<int> OnCureUpgraded;
    public event Action<int> OnCureRestored;
    public event Action<float> OnHealingStarted;
    public event Action OnHealingFinished;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        anim = PlayerAnimationController.Instance;
        RestoreCures();
    }

    public bool TryUseCure(int healAmount = 40, bool blockWhenFullHP = true)
    {
        if (isHealing) return false;
        if (currentCures <= 0) return false;

        if (blockWhenFullHP && player != null)
        {
            float cur = player.Health.Get(HealthType.Physical);
            float max = player.Health.GetMax(HealthType.Physical);
            if (cur >= max - 0.001f) return false;
        }

        healingRoutine = StartCoroutine(HealRoutine(healAmount));
        return true;
    }

    private IEnumerator HealRoutine(int healAmount)
    {
        isHealing = true;
        currentCures = Mathf.Max(0, currentCures - 1);
        OnCureUsed?.Invoke(currentCures, maxCureQuantity);

        OnHealingStarted?.Invoke(curingTime);
        cure.ActivateCure();
        anim.Cure(); // ✅ reemplazo del SetCuring()

        yield return new WaitForSeconds(curingTime);

        if (player != null) player.Health.Restore(HealthType.Physical, healAmount);
        cure.DeactivateCure();

        PlayerActionLogger.Instance.Log("Cure", new List<string> { $"Remaining cures: {currentCures}" });
        OnHealingFinished?.Invoke();
        isHealing = false;
        healingRoutine = null;
    }

    public void RestoreCures()
    {
        currentCures = maxCureQuantity;
        OnCureRestored?.Invoke(currentCures);
    }

    public void UpgradeMaxCures(int amount = 1, bool fillAfter = true)
    {
        maxCureQuantity += amount;
        if (fillAfter) currentCures = maxCureQuantity;
        OnCureUpgraded?.Invoke(maxCureQuantity);
        OnCureRestored?.Invoke(currentCures);
    }

    public void AddCure(int amount = 1)
    {
        currentCures = Mathf.Min(currentCures + amount, maxCureQuantity);
        OnCureRestored?.Invoke(currentCures);
    }

    public (int current, int max) GetCureInfo() => (currentCures, maxCureQuantity);
    public float GetCuringTime() => curingTime;
    public bool IsHealing() => isHealing;
    public bool HasCharges() => currentCures > 0;
}


public enum CureStates
{
    Ready,
    FullHP,
    NoCharges,
    Healing,
    Cooldown
}
