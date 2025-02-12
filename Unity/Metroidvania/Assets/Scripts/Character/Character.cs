using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float physicalHealth = 100f;
    public float mentalHealth = 60f;
    public float emotionalHealth = 20f;
    public float damage = 10f;

    protected float CurrentPhysicalHealth { get; private set; }
    protected float CurrentMentalHealth { get; private set; }
    protected float CurrentEmotionalHealth { get; private set; }

    private Coroutine mentalHealingCoroutine = null;
    private bool canTakePhysicalDamage = true;
    private bool isMentalHealthUsed = false; // Para verificar si se ha usado la salud mental
    public bool CanTakePhysicalDamage { get { return canTakePhysicalDamage; } set { canTakePhysicalDamage = value; } }

    // Configuraciones para la regeneración de salud
    public float mentalHealthRegenerationRate = 5f; // Cuánto se regenera por segundo
    public float mentalHealthRegenerationDelay = 3f; // Tiempo de espera después de usar la salud mental antes de que comience a regenerarse

    // Start is called before the first frame update
    protected virtual void Start()
    {
        InitializeHealth();
        StartMentalHealthRegeneration();
    }

    protected virtual void Update()
    {
        // Aquí podrías gestionar efectos continuos como el veneno o la regeneración.
        if (!isMentalHealthUsed)
        {
            // Solo permite la regeneración si no se ha usado la salud mental recientemente
            if (CurrentMentalHealth < mentalHealth)
            {
                RestoreMentalHealth(mentalHealthRegenerationRate * Time.deltaTime);
            }
        }
    }

    protected virtual void InitializeHealth()
    {
        InitializePhysicalHealth();
        InitializeMentalHealth();
        InitializeEmotionalHealth();
    }

    protected virtual void InitializePhysicalHealth()
    {
        CurrentPhysicalHealth = physicalHealth;
    }

    protected virtual void InitializeMentalHealth()
    {
        CurrentMentalHealth = mentalHealth;
    }

    protected virtual void InitializeEmotionalHealth()
    {
        CurrentEmotionalHealth = emotionalHealth;
    }

    // Métodos de daño
    protected virtual void TakePhysicalDamage(float damage)
    {
        if (CanTakePhysicalDamage)
        {
            CurrentPhysicalHealth -= damage;
            if (CurrentPhysicalHealth < 0f)
            {
                Die();
            }
        }
    }

    protected virtual void TakeMentalDamage(float damage)
    {
        CurrentMentalHealth -= damage;
        if (CurrentMentalHealth < 0f)
        {
            CurrentMentalHealth = 0f;
        }
    }

    protected virtual void TakeEmotionalDamage(float damage)
    {
        CurrentEmotionalHealth -= damage;
        if (CurrentEmotionalHealth < 0f)
        {
            CurrentEmotionalHealth = 0f;
        }
    }

    // Pulsos de daño (Efectos temporales)
    public virtual void UseMentalPulse(float amount)
    {
        CurrentMentalHealth -= amount;
        isMentalHealthUsed = true; // Se usó la salud mental
        // Reinicia la regeneración después de un tiempo
        StartCoroutine(MentalHealthRegenerationDelay());
    }

    public virtual void UseEmotionPulse(float amount)
    {
        CurrentEmotionalHealth -= amount;
    }

    // Regeneración de salud mental
    private void StartMentalHealthRegeneration()
    {
        if (mentalHealingCoroutine == null)
        {
            mentalHealingCoroutine = StartCoroutine(MentalHealthRegeneration());
        }
    }

    private IEnumerator MentalHealthRegeneration()
    {
        while (CurrentMentalHealth < mentalHealth)
        {
            // Regeneración de salud mental
            RestoreMentalHealth(mentalHealthRegenerationRate);
            yield return new WaitForSeconds(1f);
        }
    }

    // Temporizador para reiniciar la regeneración después de usar salud mental
    private IEnumerator MentalHealthRegenerationDelay()
    {
        yield return new WaitForSeconds(mentalHealthRegenerationDelay);
        isMentalHealthUsed = false; // La salud mental ya no se está usando
    }

    // Restaurar salud
    protected virtual void RestorePhysicalHealth(float amount)
    {
        RestoreValue(amount, CurrentPhysicalHealth, physicalHealth);
    }

    protected virtual void RestoreMentalHealth(float amount)
    {
        CurrentMentalHealth += amount;
        if (CurrentMentalHealth > mentalHealth)
        {
            CurrentMentalHealth = mentalHealth;
        }
    }

    protected virtual void RestoreEmotionalHealth(float amount)
    {
        RestoreValue(amount, CurrentEmotionalHealth, emotionalHealth);
    }

    private void RestoreValue(float amount, float value, float max)
    {
        value += amount;
        if (value > max)
        {
            value = max;
        }
        if (value < 0)
        {
            value = 0;
        }
    }

    protected virtual void Die()
    {
        // Implementar lógica de muerte
    }

    public float GetCurrentHealth(HealthType type)
    {
        switch (type)
        {
            case HealthType.Physical:
                return CurrentPhysicalHealth;
            case HealthType.Mental:
                return CurrentMentalHealth;
            case HealthType.Emotional:
                return CurrentEmotionalHealth;
            default:
                return 0;
        }
    }

    public float GetMaxHealth(HealthType type)
    {
        switch (type)
        {
            case HealthType.Physical:
                return physicalHealth;
            case HealthType.Mental:
                return mentalHealth;
            case HealthType.Emotional:
                return emotionalHealth;
            default:
                return 0;
        }
    }
}

public enum HealthType
{
    Physical,
    Mental,
    Emotional
}
