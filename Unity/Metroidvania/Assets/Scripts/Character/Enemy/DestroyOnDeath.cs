using UnityEngine;

[RequireComponent(typeof(HealthModule))]
public class DestroyOnDeath : MonoBehaviour
{
    private HealthModule health;
    void Awake() => health = GetComponent<HealthModule>();
    void OnEnable()  => health.OnDeath += HandleDeath;
    void OnDisable() => health.OnDeath -= HandleDeath;
    private void HandleDeath() => Destroy(gameObject);
}

public class ContactDamageDealer : MonoBehaviour
{
    public float damage = 10f;
    private void OnCollisionEnter(Collision col)
    {
        var targetHealth = col.gameObject.GetComponent<HealthModule>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(HealthType.Physical, damage);
        }
    }
}
