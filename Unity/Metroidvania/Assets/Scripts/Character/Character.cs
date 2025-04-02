using System.Collections;
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
    public bool CanTakePhysicalDamage { get { return canTakePhysicalDamage; } set { canTakePhysicalDamage = value; } }

    // Configuraciones para la regeneración de salud
    public float mentalHealthRegenerationRate = 5f; // Cuánto se regenera por segundo
    public float mentalHealthRegenerationDelay = 3f;  // Tiempo de espera después de usar la salud mental antes de regenerar

    protected virtual void Start()
    {
        InitializeHealth();
        // Inicia la regeneración de salud mental tras un retraso
        StartMentalHealthRegenerationDelay();
    }
    protected virtual void Update(){

    }

    // Se elimina la regeneración en Update para que no se regenere constantemente

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
    public virtual void TakePhysicalDamage(float damage)
    {
        if (CanTakePhysicalDamage)
        {
            RumbleController.RumblePulse(0.1f,0.2f,0.06f);
            CurrentPhysicalHealth -= damage;
            if (CurrentPhysicalHealth < 0f)
            {
                Die();
                RumbleController.RumblePulse(0.5f,0.9f,0.1f);
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

    // Pulsos de daño (efectos temporales)
    public virtual void UseMentalPulse(float amount)
    {
        CurrentMentalHealth -= amount;
        if (CurrentMentalHealth < 0f)
        {
            CurrentMentalHealth = 0f;
        }

        // Reinicia la regeneración: se detiene la regeneración actual (si existe)
        if (mentalHealingCoroutine != null)
        {
            StopCoroutine(mentalHealingCoroutine);
            mentalHealingCoroutine = null;
        }
        // Reinicia el retraso de regeneración
        StartMentalHealthRegenerationDelay();
    }

    public virtual void UseEmotionPulse(float amount)
    {
        CurrentEmotionalHealth -= amount;
        if (CurrentEmotionalHealth < 0f)
        {
            CurrentEmotionalHealth = 0f;
        }
    }

    // Regeneración de salud mental tras un retraso
    private void StartMentalHealthRegenerationDelay()
    {
        if (mentalHealingCoroutine != null)
        {
            StopCoroutine(mentalHealingCoroutine);
        }
        mentalHealingCoroutine = StartCoroutine(MentalHealthRegenerationDelayCoroutine());
    }

    private IEnumerator MentalHealthRegenerationDelayCoroutine()
    {
        // Espera el retraso sin regenerar salud mental
        yield return new WaitForSeconds(mentalHealthRegenerationDelay);

        // Regenera la salud mental de forma continua hasta llegar al máximo
        while (CurrentMentalHealth < mentalHealth)
        {
            RestoreMentalHealth(mentalHealthRegenerationRate * Time.deltaTime);
            yield return null;
        }
        mentalHealingCoroutine = null;
    }

    // Restaurar salud
    protected virtual void RestorePhysicalHealth(float amount)
    {
        CurrentPhysicalHealth += amount;
        if (CurrentPhysicalHealth > physicalHealth)
        {
            CurrentPhysicalHealth = physicalHealth;
        }
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
        CurrentEmotionalHealth += amount;
        if (CurrentEmotionalHealth > emotionalHealth)
        {
            CurrentEmotionalHealth = emotionalHealth;
        }
    }

    protected virtual void Die()
    {
        // Implementa la lógica de muerte
        CurrentPhysicalHealth = 0f;
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
