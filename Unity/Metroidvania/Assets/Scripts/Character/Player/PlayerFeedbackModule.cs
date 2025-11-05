using UnityEngine;
using UnityEngine.InputSystem.DualShock;

[RequireComponent(typeof(HealthModule))]
public class PlayerFeedbackModule : MonoBehaviour
{
    private HealthModule health;
    private PlayerAnimationController anim;

    void Awake()
    {
        health = GetComponent<HealthModule>();
        anim   = GetComponent<PlayerAnimationController>();
    }

    void OnEnable()
    {
        health.OnDamaged += OnDamaged;
        health.OnHealed  += OnHealed;
        health.OnDeath   += OnDeath;

        // Inicializa luz según salud actual
        UpdateLightBar();

        // Actualiza UI/Animación inicial de salud
        anim?.SetCurrentHealthPercentage(health.GetPercent(HealthType.Physical));
    }

    void OnDisable()
    {
        health.OnDamaged -= OnDamaged;
        health.OnHealed  -= OnHealed;
        health.OnDeath   -= OnDeath;
    }

    private void OnDamaged(HealthType type, float amount)
    {
        if (type == HealthType.Physical)
        {
            RumbleController.RumblePulse(0.1f, 0.2f, 0.06f);

            anim?.TakeDamage();
            anim?.SetCurrentHealthPercentage(health.GetPercent(HealthType.Physical));

            UpdateLightBar();
        }
    }

    private void OnHealed(HealthType type, float amount)
    {
        if (type == HealthType.Physical)
        {
            anim?.SetCurrentHealthPercentage(health.GetPercent(HealthType.Physical));
            UpdateLightBar();
        }
    }

    private void OnDeath()
    {
        RumbleController.RumblePulse(0.5f, 0.9f, 0.06f);
        anim?.Die(true);
    }

    private void UpdateLightBar()
    {
        var ds = DualShockGamepad.current;
        if (ds == null) return;

        float cur = health.Get(HealthType.Physical);
        float max = health.GetMax(HealthType.Physical);

        if (cur >= max * 0.6f)       ds.SetLightBarColor(Color.blue);
        else if (cur >= max * 0.3f) ds.SetLightBarColor(Color.yellow);
        else                        ds.SetLightBarColor(Color.red);
    }
}
