using UnityEngine;

[RequireComponent(typeof(HealthModule))]
public class DisableOnDeath : MonoBehaviour
{
    private HealthModule health;

    void Awake() => health = GetComponent<HealthModule>();
    void OnEnable()  => health.OnDeath += HandleDeath;
    void OnDisable() => health.OnDeath -= HandleDeath;

    private void HandleDeath()
    {
        // 🔥 Solo desactivamos — no destruimos
        //gameObject.SetActive(false);
    }
}
