using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HealthModule))]
public abstract class Character : MonoBehaviour
{
    protected HealthModule health;
    public HealthModule Health => health;

    protected virtual void Awake()
    {
        health = GetComponent<HealthModule>();
        health.Initialize();
    }

    protected virtual void Start()
    {
        // Suscribirse al evento de muerte
        health.OnDie += Die;
    }

    protected virtual void OnDestroy()
    {
        if (health != null)
            health.OnDie -= Die;
    }

    public virtual void ApplyKnockback(Vector3 direction, float force, float duration = 0.2f)
    {
        StopCoroutine(nameof(DoKnockback)); // evita que se acumulen corrutinas
        StartCoroutine(DoKnockback(direction, force, duration));
    }

    private IEnumerator DoKnockback(Vector3 direction, float force, float duration)
    {
        float elapsed = 0f;
        Vector3 start = transform.position;
        Vector3 target = start + direction.normalized * force;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(start, target, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }


    // API común
    public virtual void TakePhysicalDamage(float dmg, Character damager = null) => health.TakeDamage(HealthType.Physical, dmg);
    public virtual void TakeMentalDamage(float dmg) => health.TakeDamage(HealthType.Mental, dmg);
    public virtual void TakeEmotionalDamage(float dmg) => health.TakeDamage(HealthType.Emotional, dmg);

    public virtual void HealPhysical(float amount) => health.Restore(HealthType.Physical, amount);
    public virtual void HealMental(float amount) => health.Restore(HealthType.Mental, amount);
    public virtual void HealEmotional(float amount) => health.Restore(HealthType.Emotional, amount);

    public float GetHealthPercent(HealthType type) => health.GetPercent(type);

    public virtual void Respawn() => health.Initialize();

    protected virtual void Die() {}
}
