using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HealthModule))]
public class Boss : Character
{
    [Header("Boss Settings")]
    public string bossName = "Unknown Entity";
    public int musicId=-1;
    public bool showBossBarOnStart = false; // 👈 por defecto NO se muestra hasta activarse

    [Tooltip("Daño base de los golpes del jefe")]
    public float baseAttackDamage = 20f;

    [Tooltip("Multiplicador de daño que inflige cuando está en Rage")]
    public float rageDamageMultiplier = 1.5f;

    [Tooltip("Multiplicador del daño que RECIBE (0.25 = 25%)")]
    [Range(0f, 1f)] public float receivedDamageMultiplier = 0.25f;

    [Header("Estado del jefe (runtime)")]
    [SerializeField] protected bool isActive = false;
    [SerializeField] protected bool isDead = false;



    public bool IsEnraged { get; private set; } = false;
    public bool IsActive => isActive;

    /// Daño actual que infligen los hitboxes del jefe
    public float CurrentAttackDamage => baseAttackDamage * (IsEnraged ? rageDamageMultiplier : 1f);

    protected Animator animator;

    protected override void Start()
    {
        base.Start();
        animator = GetComponentInChildren<Animator>();

        // Se suscriben eventos
        Health.OnHealthChanged += OnBossHealthChanged;
        Health.OnDie += OnBossDefeated;

        // 🔹 El jefe arranca “apagado”: no hace nada, no se mueve, ni ataca
        if (!isActive)
        {
            if (animator != null)
            {
                animator.SetBool("Walk", false);
                animator.speed = 0f; // pausa animaciones
            }

            enabled = false; // desactiva Update de scripts derivados (como LooseController)
        }

        if (showBossBarOnStart)
            ActivateBossBar();
    }

    protected virtual void Update()
    {
        if (!isActive || isDead) return;
    }

    // ───────────────────────────────────────────────
    // ACTIVACIÓN / DESACTIVACIÓN

    public virtual void ActivateBoss()
    {
        if (isActive || isDead) return;

        isActive = true;

        // Reactiva animaciones y lógica
        if (animator != null)
            animator.speed = 1f;

        enabled = true;

        if (BossLifeBar.Instance != null)
            BossLifeBar.Instance.ShowBar(bossName);

        Debug.Log($"🔥 Boss {bossName} activado.");
    }

    public virtual void DeactivateBoss()
    {
        isActive = false;
        if (animator != null)
            animator.speed = 0f;
        enabled = false;
    }

    // ───────────────────────────────────────────────
    protected virtual void OnBossHealthChanged(HealthType type, float current, float max)
    {
        if (type == HealthType.Physical && BossLifeBar.Instance != null)
            BossLifeBar.Instance.UpdateHealth(current / max);
    }

    public override void ApplyKnockback(Vector3 direction, float force, float duration = 0.2f) => 
        /* No knockback para bosses */ 
        Debug.Log($"{bossName} es inmune al knockback.");

    public override void TakePhysicalDamage(float dmg, Character damager = null)
    {
        dmg *= receivedDamageMultiplier;
        base.TakePhysicalDamage(dmg, damager);
    }

    protected virtual void OnBossDefeated()
    {
        isDead = true;
        isActive = false;

        if (BossLifeBar.Instance != null)
            BossLifeBar.Instance.HideBar();

        Debug.Log($"☠️ Boss {bossName} derrotado.");
    }

    public virtual void ActivateBossBar()
    {
        if (BossLifeBar.Instance != null)
            BossLifeBar.Instance.ShowBar(bossName);
    }

    public void SetEnraged(bool value) => IsEnraged = value;
}
