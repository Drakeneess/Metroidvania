using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HealthModule))]
public class MentalRegenModule : MonoBehaviour
{
    public float regenRate  = 5f;  // por segundo
    public float regenDelay = 3f;  // espera tras usar/recibir daño mental

    private HealthModule health;
    private Coroutine regenCo;

    void Awake()
    {
        health = GetComponent<HealthModule>();
    }

    void OnEnable()
    {
        health.OnDamaged += OnDamaged;
    }

    void OnDisable()
    {
        health.OnDamaged -= OnDamaged;
        StopRegen();
    }

    private void OnDamaged(HealthType type, float amount)
    {
        if (type != HealthType.Mental) return;
        RestartDelay();
    }

    // Llama esto también cuando hagas “UsePulse(Mental, x)”
    public void NotifyMentalUsed() => RestartDelay();

    private void RestartDelay()
    {
        StopRegen();
        regenCo = StartCoroutine(RegenRoutine());
    }

    private IEnumerator RegenRoutine()
    {
        yield return new WaitForSeconds(regenDelay);
        while (health.Get(HealthType.Mental) < health.GetMax(HealthType.Mental))
        {
            health.Restore(HealthType.Mental, regenRate * Time.deltaTime);
            yield return null;
        }
        regenCo = null;
    }

    private void StopRegen()
    {
        if (regenCo != null) StopCoroutine(regenCo);
        regenCo = null;
    }
}
