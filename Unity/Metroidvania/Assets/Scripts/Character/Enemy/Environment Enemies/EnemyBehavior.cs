using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Enemy))]
[RequireComponent(typeof(EnemyPatrol))]
public class EnemyBehavior : MonoBehaviour
{
    public enum EnemyState { Patrol, Alert, Conflict, Attack }

    [Header("Estado actual (debug)")]
    public EnemyState currentState = EnemyState.Patrol;

    [Header("Detección")]
    public float alertRange = 5f;
    public float attackRange = 1.5f;
    [Range(30f, 180f)] public float viewAngle = 100f; // mantenemos por si quieres “cono”, pero usamos frontal simple
    public float alertDuration = 2.0f;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;

    [Header("Comunicación de Enemigos")]
    public float conflictRadius = 6f;
    public float conflictMemoryTime = 5f;
    public float conflictExitRange = 15f;
    public float conflictPulseRate = 1.5f;

    [Header("Debug")]
    public bool debugPulses = false;

    private Enemy enemy;
    private EnemyPatrol patrol;
    private Transform enemyTransform;
    private Animator animator;
    private Transform player;

    private float alertTimer;
    private float timeSinceLastSeen;
    private float pulseTimer;
    private bool hasConflictStarted = false;

    private static readonly Collider[] _allyBuffer = new Collider[64];

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        patrol = GetComponent<EnemyPatrol>();
        enemyTransform = transform;
        animator = GetComponentInChildren<Animator>();

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj) player = playerObj.transform;
    }

    private void Update()
    {
        if (enemy.isDead) return;

        switch (currentState)
        {
            case EnemyState.Patrol: PatrolState(); break;
            case EnemyState.Alert: AlertState(); break;
            case EnemyState.Conflict: ConflictState(); break;
            case EnemyState.Attack: AttackState(); break;
        }
    }
    
    private void OnDisable()
    {
        // Si este enemigo estaba registrado en conflicto, darlo de baja
        if (PlayerConflictStateController.Instance != null)
            PlayerConflictStateController.Instance.UnregisterEnemy(enemy);
    }

    // ────────────────────────────────────────────────
    private void PatrolState()
    {
        patrol.enabled = true;
        animator?.SetBool("Alert", false);

        if (PlayerInSight(alertRange))
        {
            patrol.enabled = false;
            currentState = EnemyState.Alert;
            alertTimer = 0f;
            animator?.SetBool("Alert", true);
        }
    }

    private void AlertState()
    {
        alertTimer += Time.deltaTime;
        FacePlayerHorizontally();

        if (PlayerInSight(alertRange))
        {
            currentState = EnemyState.Conflict;
            hasConflictStarted = false;
            return;
        }

        if (alertTimer >= alertDuration)
        {
            currentState = EnemyState.Patrol;
            animator?.SetBool("Alert", false);
        }
    }

    private void ConflictState()
    {
        patrol.enabled = false;

        if (!hasConflictStarted)
        {
            hasConflictStarted = true;
            pulseTimer = 0f;
            AlertNearbyEnemies();
            animator?.SetBool("Alert", true);
        }

        // Pulsos de aviso
        pulseTimer += Time.deltaTime;
        if (pulseTimer >= conflictPulseRate)
        {
            pulseTimer = 0f;
            AlertNearbyEnemies();
        }

        animator.SetBool("Alert", false);
        FacePlayerHorizontally();

        // 📌 Avance SOLO en X hacia el jugador
        if (player != null)
        {
            float step = enemy.CurrentSpeed * 0.6f * Time.deltaTime;
            float targetX = Mathf.MoveTowards(enemyTransform.position.x, player.position.x, step);
            enemyTransform.position = new Vector3(targetX, enemyTransform.position.y, enemyTransform.position.z);
        }

        // Chequeo de visión + rango de ataque (horizontal)
        if (PlayerInSight(alertRange))
        {
            timeSinceLastSeen = 0f;

            float distX = HorizontalDistanceToPlayer();
            if (distX <= attackRange * 1.1f) // margen
            {
                currentState = EnemyState.Attack;
                return;
            }
        }
        else
        {
            timeSinceLastSeen += Time.deltaTime;
            float distXZ = Vector3.Distance(player.position, enemyTransform.position);

            if (timeSinceLastSeen >= conflictMemoryTime || distXZ > conflictExitRange)
            {
                currentState = EnemyState.Patrol;
                patrol.enabled = true;
                animator?.SetBool("Alert", false);
                hasConflictStarted = false;
            }
        }
    }

    private void AttackState()
    {
        if (player == null)
        {
            // 🔹 Sin jugador → salir de conflicto
            if (PlayerConflictStateController.Instance != null)
                PlayerConflictStateController.Instance.UnregisterEnemy(enemy);

            currentState = EnemyState.Patrol;
            return;
        }

        float distX = HorizontalDistanceToPlayer();
        if (distX > attackRange * 1.5f)
        {
            currentState = EnemyState.Conflict;
            return;
        }

        FacePlayerHorizontally();

        enemy.nextAttackTime = 0f;
        enemy.PerformAttack();
    }

    // ────────────────────────────────────────────────
    // 🧠 UTILIDADES

    // Visión 2.5D: requiere estar mirando hacia el jugador (izq/der), tener distancia horizontal dentro del rango
    // y sin obstáculo entre ambos. El raycast se hace en el plano del enemigo para evitar offsets de Z.
    private bool PlayerInSight(float range)
    {
        if (player == null) return false;

        // 1) Distancia horizontal
        float distX = HorizontalDistanceToPlayer();
        if (distX > range) return false;

        // 2) Debe estar al frente (izq/der), no “ojos en la espalda”
        bool playerRight = player.position.x > enemyTransform.position.x;
        if (playerRight != patrol.movingRight) return false;

        // 3) Línea de visión (proyectamos el objetivo al plano Z del enemigo para 2.5D)
        Vector3 origin = enemyTransform.position + Vector3.up * 0.5f;
        Vector3 target = new Vector3(player.position.x, player.position.y + 0.5f, origin.z);

        #if UNITY_EDITOR
        Debug.DrawLine(origin, target, Color.green);
        #endif

        if (Physics.Linecast(origin, target, obstacleLayer))
            return false;

        return true;
    }

    private float HorizontalDistanceToPlayer()
    {
        if (player == null) return float.MaxValue;
        return Mathf.Abs(player.position.x - enemyTransform.position.x);
    }

    private void FacePlayerHorizontally()
    {
        if (player == null) return;
        bool playerRight = player.position.x > enemyTransform.position.x;
        if (playerRight != patrol.movingRight)
            patrol.Flip();
    }

    private void AlertNearbyEnemies()
    {
        int count = Physics.OverlapSphereNonAlloc(enemyTransform.position, conflictRadius, _allyBuffer, ~0);
        for (int i = 0; i < count; i++)
        {
            var col = _allyBuffer[i];
            if (col == null || col.transform == transform) continue;

            var other = col.GetComponent<EnemyBehavior>();
            if (other == null || !other.isActiveAndEnabled) continue;
            if (other.enemy != null && other.enemy.isDead) continue;

            if (other.currentState == EnemyState.Patrol || other.currentState == EnemyState.Alert)
                other.ReceiveConflictSignal();
        }
    }

    public void ReceiveConflictSignal()
    {
        if (currentState == EnemyState.Patrol || currentState == EnemyState.Alert)
        {
            patrol.enabled = false;
            currentState = EnemyState.Conflict;
            hasConflictStarted = true;
            timeSinceLastSeen = 0f;
            animator?.SetBool("Alert", true);

            // 🔹 Si entra por señal, también registrar
            if (PlayerConflictStateController.Instance != null)
            {
                print("Si");
                PlayerConflictStateController.Instance.RegisterEnemy(enemy);
            }
            else
            {
                print("No");
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (patrol == null) return;

        Vector3 origin = transform.position;
        // Cono visual (decorativo si usas solo frontal)
        Vector3 forward = patrol.movingRight ? Vector3.right : Vector3.left;
        float halfAngle = viewAngle * 0.5f;

        Handles.color = new Color(1f, 1f, 0f, 0.15f);
        Handles.DrawSolidArc(origin, Vector3.forward, Quaternion.Euler(0, 0, -halfAngle) * forward, viewAngle, alertRange);

        Handles.color = new Color(1f, 0f, 0f, 0.1f);
        Handles.DrawSolidDisc(origin, Vector3.forward, attackRange);

        Handles.color = new Color(0f, 0.5f, 1f, 0.15f);
        Handles.DrawWireDisc(origin, Vector3.forward, conflictRadius);
    }
#endif
}
