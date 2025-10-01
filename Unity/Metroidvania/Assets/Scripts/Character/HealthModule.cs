using System;
using UnityEngine;

[DisallowMultipleComponent]
public class HealthModule : MonoBehaviour
{
    [Header("Max Health")]
    public float physicalMax  = 100f;
    public float mentalMax    = 60f;
    public float emotionalMax = 20f;

    [Header("Runtime (read-only)")]
    [SerializeField] private float physical;
    [SerializeField] private float mental;
    [SerializeField] private float emotional;

    [Header("Flags")]
    public bool canTakePhysicalDamage = true;

    // Eventos
    public event Action<HealthType, float, float> OnHealthChanged; // (tipo, actual, max)
    public event Action<HealthType, float> OnDamaged;              // (tipo, cantidad)
    public event Action<HealthType, float> OnHealed;               // (tipo, cantidad)
    public event Action OnDie;     // ✅ Canonical
    public event Action OnDeath;   // ⚠️ Legacy alias (se invoca junto a OnDie)

    // ------- Lecturas -------
    public float Get(HealthType t) => t switch
    {
        HealthType.Physical  => physical,
        HealthType.Mental    => mental,
        HealthType.Emotional => emotional,
        _ => 0f
    };

    public float GetMax(HealthType t) => t switch
    {
        HealthType.Physical  => physicalMax,
        HealthType.Mental    => mentalMax,
        HealthType.Emotional => emotionalMax,
        _ => 0f
    };

    public float GetPercent(HealthType t)
    {
        float cur = Get(t), max = GetMax(t);
        return max > 0f ? cur / max : 0f;
    }

    // ------- Inicialización -------
    public void InitializeFrom(float phys, float ment, float emot)
    {
        physicalMax = Mathf.Max(0f, phys);
        mentalMax = Mathf.Max(0f, ment);
        emotionalMax = Mathf.Max(0f, emot);
        Initialize(); // setea current = max y dispara OnHealthChanged
    }

    public void Initialize()
    {
        physical = physicalMax;
        mental   = mentalMax;
        emotional= emotionalMax;
        RaiseAllChanged();
    }

    // ------- Max/Upgrades -------
    public void SetMax(HealthType t, float newMax, bool clampToMax = true)
    {
        newMax = Mathf.Max(0f, newMax);
        switch (t)
        {
            case HealthType.Physical:
                physicalMax = newMax;
                if (clampToMax) physical = Mathf.Min(physical, newMax);
                break;
            case HealthType.Mental:
                mentalMax = newMax;
                if (clampToMax) mental = Mathf.Min(mental, newMax);
                break;
            case HealthType.Emotional:
                emotionalMax = newMax;
                if (clampToMax) emotional = Mathf.Min(emotional, newMax);
                break;
        }
        OnHealthChanged?.Invoke(t, Get(t), GetMax(t));
    }

    // Alias por compatibilidad
    public void SetMaxHealth(HealthType t, float newMax, bool clampToMax = true) => SetMax(t, newMax, clampToMax);

    public void AddMax(HealthType t, float amount, bool alsoHealToMax = true)
    {
        if (amount == 0f) return;
        SetMax(t, GetMax(t) + amount, clampToMax: false);
        if (alsoHealToMax)
        {
            // Llevar current al nuevo máximo
            switch (t)
            {
                case HealthType.Physical:  physical  = physicalMax;  break;
                case HealthType.Mental:    mental    = mentalMax;    break;
                case HealthType.Emotional: emotional = emotionalMax; break;
            }
            OnHealthChanged?.Invoke(t, Get(t), GetMax(t));
        }
    }

    // ------- Daño / Curación -------
    public void TakeDamage(HealthType t, float amount)
    {
        amount = Mathf.Max(0f, amount);
        if (amount <= 0f) return;

        if (t == HealthType.Physical && !canTakePhysicalDamage) return;

        float before = Get(t);
        float after  = Mathf.Max(0f, before - amount);
        SetCurrent(t, after);
        OnDamaged?.Invoke(t, amount);

        if (t == HealthType.Physical && after <= 0f)
        {
            OnDie?.Invoke();   // ✅ nuevo
            OnDeath?.Invoke(); // legacy
        }
    }

    public void Restore(HealthType t, float amount)
    {
        amount = Mathf.Max(0f, amount);
        if (amount <= 0f) return;

        float before = Get(t);
        float after  = Mathf.Min(GetMax(t), before + amount);
        SetCurrent(t, after);
        OnHealed?.Invoke(t, amount);
    }

    public void UsePulse(HealthType t, float amount)
    {
        // Consumir recurso (mental/emocional) usando la misma ruta de daño
        TakeDamage(t, amount);
    }

    public void FullRestoreAll()
    {
        physical  = physicalMax;
        mental    = mentalMax;
        emotional = emotionalMax;
        RaiseAllChanged();
    }

    // Alias por compatibilidad
    public void RestoreFull() => FullRestoreAll();

    // ------- Internos -------
    private void SetCurrent(HealthType t, float v)
    {
        switch (t)
        {
            case HealthType.Physical:  physical  = v; break;
            case HealthType.Mental:    mental    = v; break;
            case HealthType.Emotional: emotional = v; break;
        }
        OnHealthChanged?.Invoke(t, v, GetMax(t));
    }

    private void RaiseAllChanged()
    {
        OnHealthChanged?.Invoke(HealthType.Physical,  physical,  physicalMax);
        OnHealthChanged?.Invoke(HealthType.Mental,    mental,    mentalMax);
        OnHealthChanged?.Invoke(HealthType.Emotional, emotional, emotionalMax);
    }
}

public enum HealthType { Physical, Mental, Emotional }
