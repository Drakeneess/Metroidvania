using System.Collections;
using UnityEngine;

public class LooseController : Boss
{
    [Header("Movimiento / Patrón")]
    [Tooltip("Velocidad base de movimiento del jefe (cuando camina en su rutina).")]
    public float walkSpeed = 2.2f;

    [Tooltip("Distancia máxima que recorre antes de girar.")]
    public float patrolDistance = 5f;

    [Tooltip("Tiempo que se queda quieto antes de cambiar de dirección.")]
    public float idleTime = 1.5f;

    [Header("Ataques")]
    public float attackCooldown = 2.0f;
    public float comboPause = 0.6f; 
    public float comboChance = 0.5f; 

    [Header("Rage")]
    [Tooltip("Se activa cuando su vida física <= 60% (pierde 40%)")]
    [Range(0.1f, 0.9f)] public float rageThresholdPercent = 0.60f;

    // Animator params
    private static readonly int WalkHash        = Animator.StringToHash("Walk");
    private static readonly int AttackStateHash = Animator.StringToHash("AttackState");
    private static readonly int AttackHash      = Animator.StringToHash("Attack");
    private static readonly int RageHash        = Animator.StringToHash("Rage");
    private static readonly int DieHash         = Animator.StringToHash("Die");

    private bool facingRight = false; // Empieza mirando a la izquierda
    private bool isAttacking = false;
    private bool isMoving = false;
    private bool isIdle = false;

    private Vector3 startPos;
    private float walkedDistance = 0f;
    private float nextAttackTime = 0f;

    private BossMeleeHitbox[] hitboxes;
    private Coroutine behaviorRoutine;

    // ───────────────────────────────────────────────
    protected override void Start()
    {
        base.Start();

        animator = GetComponentInChildren<Animator>();
        startPos = transform.position;
        hitboxes = GetComponentsInChildren<BossMeleeHitbox>(true);

        Health.OnHealthChanged += OnHealthChanged_CheckRage;
        Health.OnDie += OnBossDie;

        // ⚠️ IMPORTANTE: No iniciar rutina todavía
        // El boss comienza "apagado" hasta que sea activado por BossFightTrigger
        if (IsActive)
            StartRoutine();
    }

    // ───────────────────────────────────────────────
    public override void ActivateBoss()
    {
        base.ActivateBoss();
        StartRoutine();
    }

    private void StartRoutine()
    {
        if (behaviorRoutine != null) StopCoroutine(behaviorRoutine);
        behaviorRoutine = StartCoroutine(BehaviorRoutine());
    }

    protected override void OnDestroy()
    {
        if (Health != null)
        {
            Health.OnHealthChanged -= OnHealthChanged_CheckRage;
            Health.OnDie -= OnBossDie;
        }
    }

    // ───────────────────────────────────────────────
    private IEnumerator BehaviorRoutine()
    {
        while (!isDead && Health.Get(HealthType.Physical) > 0f)
        {
            // 1️⃣ Caminar
            yield return StartCoroutine(WalkRoutine());

            // 2️⃣ Atacar si está disponible
            if (Time.time >= nextAttackTime)
            {
                yield return StartCoroutine(AttackRoutine());
                nextAttackTime = Time.time + attackCooldown;
            }

            // 3️⃣ Descanso breve
            yield return new WaitForSeconds(idleTime);
        }
    }

    private IEnumerator WalkRoutine()
    {
        isMoving = true;
        animator.SetBool(WalkHash, true);
        walkedDistance = 0f;

        float dir = facingRight ? 1f : -1f;

        while (walkedDistance < patrolDistance)
        {
            float step = walkSpeed * Time.deltaTime;
            transform.position += new Vector3(step * dir, 0f, 0f);
            walkedDistance += step;
            yield return null;
        }

        animator.SetBool(WalkHash, false);
        isMoving = false;

        yield return new WaitForSeconds(idleTime);

        // 🔁 Gira hacia el otro lado
        facingRight = !facingRight;
        transform.rotation = Quaternion.Euler(0f, facingRight ? 0f : 180f, 0f);
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        animator.SetBool(WalkHash, false);

        int attackCount = (Random.value < comboChance) ? Random.Range(2, 4) : 1;

        for (int i = 0; i < attackCount; i++)
        {
            int state = Random.Range(0, 3); // 0–2 → tres tipos de ataque
            animator.SetInteger(AttackStateHash, state);
            animator.SetTrigger(AttackHash);

            yield return new WaitForSeconds(comboPause);
        }

        isAttacking = false;
    }

    // ───────────────────────────────────────────────
    // RAGE
    private void OnHealthChanged_CheckRage(HealthType type, float current, float max)
    {
        if (type != HealthType.Physical || IsEnraged) return;

        float pct = max > 0f ? current / max : 0f;
        if (pct <= rageThresholdPercent)
        {
            SetEnraged(true);
            animator.SetTrigger(RageHash);
        }
    }

    // ───────────────────────────────────────────────
    // Muerte
    private void OnBossDie()
    {
        animator.SetBool(DieHash, true);
        SetHitboxesActive(false);
        StopAllCoroutines();
        isAttacking = false;
        isDead = true;
        enabled = false;
    }

    // ───────────────────────────────────────────────
    // Animation Events
    public void AE_AttackStart() => SetHitboxesActive(true);
    public void AE_HitOn()       => SetHitboxesActive(true);
    public void AE_HitOff()      => SetHitboxesActive(false);
    public void AE_AttackEnd()   => SetHitboxesActive(false);

    private void SetHitboxesActive(bool value)
    {
        if (hitboxes == null) return;
        foreach (var h in hitboxes)
            if (h != null) h.gameObject.SetActive(value);
    }
}
