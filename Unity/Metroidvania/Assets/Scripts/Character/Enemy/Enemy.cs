using System.Collections;
using UnityEngine;

public class Enemy : Character
{
    [Header("Atributos Generales")]
    [Tooltip("Daño físico básico infligido al jugador.")]
    public float damage = 10f;

    [Tooltip("Velocidad base de movimiento.")]
    public float baseSpeed = 2f;

    [Tooltip("Multiplicador de velocidad actual (por dash, slow, etc.).")]
    [Range(0.1f, 3f)] public float speedMultiplier = 1f;

    [Header("Ataque Base")]
    [Tooltip("Tiempo entre ataques consecutivos.")]
    public float attackCooldown = 2f;
    protected bool isAttacking = false;
    public float nextAttackTime = 0f;

    protected Animator animator;
    protected Rigidbody rb;
    protected Transform player;

    private Vector3 originalPosition;
    public bool isDead { get; private set; } = false;

    /// <summary> Velocidad actual efectiva (base × multiplicador). </summary>
    public float CurrentSpeed => baseSpeed * speedMultiplier;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
    }

    protected override void Start()
    {
        base.Start();
        animator = GetComponentInChildren<Animator>();
        originalPosition = transform.position;

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj) player = playerObj.transform;

        EnemyManager.Instance?.Register(this);
    }

    private void OnDisable()
    {
        // Evita que se duplique o se quede registro colgado
        if (EnemyManager.Instance != null)
            EnemyManager.Instance.Unregister(this);
    }

    // ────────────────────────────────────────────────
    // ⚔️ COMBATE
    // ────────────────────────────────────────────────
    public bool CanDealDamage()
    {
        if (isDead) return false;
        return true;
    }

    public virtual void PerformAttack()
    {
        if (isAttacking || Time.time < nextAttackTime || isDead) return;
        StartCoroutine(DoAttack());
    }

    protected virtual IEnumerator DoAttack()
    {
        // Sobrescribir en subclases
        yield return null;
    }

    // ────────────────────────────────────────────────
    // 🩸 DAÑO Y MUERTE
    // ────────────────────────────────────────────────
    private void OnCollisionEnter(Collision collision)
    {
        if (!CanDealDamage()) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
                player.TakePhysicalDamage(damage, this);
        }
    }

    protected override void Die()
    {
        if (isDead) return;
        isDead = true;

        // 🔥 Bloquear daño y colisiones
        foreach (var c in GetComponentsInChildren<Collider>())
            c.enabled = false;

        rb.velocity = Vector3.zero;
        rb.isKinematic = true;

        animator?.SetBool("Die", true);
        OnEnemyDisabled?.Invoke(this);

        StartCoroutine(DisableEnemyCoroutine());
    }

    private IEnumerator DisableEnemyCoroutine()
    {
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }

    public override void Respawn()
    {
        isDead = false;
        gameObject.SetActive(true);

        // ✅ Restaurar colisiones y física
        foreach (var c in GetComponentsInChildren<Collider>())
            c.enabled = true;

        rb.isKinematic = false;

        transform.position = originalPosition;

        if (animator != null)
        {
            animator.SetBool("Die", false);
            animator.Play("Idle");
        }

        health.Initialize();
    }

    public static event System.Action<Enemy> OnEnemyDisabled;

    public override void TakePhysicalDamage(float dmg, Character damager)
    {
        base.TakePhysicalDamage(dmg, damager);
        FeedbackManager.Instance.TriggerHitStop(0.1f);
        CameraShaker.Instance.Shake(0.05f, 0.1f);
    }

    // ────────────────────────────────────────────────
    // 💨 VELOCIDAD
    // ────────────────────────────────────────────────
    public void SetSpeedMultiplier(float value)
    {
        speedMultiplier = Mathf.Clamp(value, 0.1f, 3f);
    }

    public void ResetSpeed() => speedMultiplier = 1f;

    // Hooks virtuales
    public virtual void EnterAlert() { }
    public virtual void ExitAlert() { }
}
