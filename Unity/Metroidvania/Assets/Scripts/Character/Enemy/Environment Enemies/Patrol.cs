using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyPatrol : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 2f;
    public bool movingRight = true;

    [Header("Raycasts")]
    public float wallCheckDistance = 0.6f;
    public float groundCheckDistance = 1.2f;
    public LayerMask groundLayer;
    public LayerMask obstacleLayer;

    [Header("Detección de aliados (mismo layer Character)")]
    public float allyCheckDistance = 0.8f;
    public LayerMask characterLayer;

    [Header("Comportamiento")]
    [Tooltip("Tiempo que espera antes de girar tras detectar una pared o vacío")]
    public float flipDelay = 0.3f;

    private Enemy enemy;
    private Transform enemyTransform;

    // 🔁 Control de giro diferido
    private bool shouldFlip = false;
    private float flipTimer = 0f;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        enemyTransform = transform;
    }

    private void Update()
    {
        if (enemy.isDead) return;
        PatrolMove();
    }

    private void PatrolMove()
    {
        Vector3 moveDir = movingRight ? Vector3.right : Vector3.left;
        Vector3 rayOrigin = enemyTransform.position + Vector3.up * 0.5f;

        // Raycast pared
        bool hitWall = Physics.Raycast(rayOrigin, moveDir, wallCheckDistance, obstacleLayer);

        // Raycast suelo
        bool hasGroundAhead = Physics.Raycast(
            enemyTransform.position + moveDir * 0.5f,
            Vector3.down,
            groundCheckDistance,
            groundLayer
        );

        // Raycast aliado (mismo layer Character)
        RaycastHit hitCharacter;
        bool allyAhead = Physics.Raycast(rayOrigin, moveDir, out hitCharacter, allyCheckDistance, characterLayer);
        if (allyAhead)
        {
            var otherEnemy = hitCharacter.collider.GetComponent<Enemy>();
            var isSelf = hitCharacter.collider.gameObject == gameObject;
            var isPlayer = hitCharacter.collider.CompareTag("Player");

            if (otherEnemy != null && !isSelf && !isPlayer)
                hitWall = true; // tratar aliado como obstáculo
        }

        // --- Manejo del retardo de giro ---
        if ((hitWall || !hasGroundAhead) && !shouldFlip)
        {
            shouldFlip = true;
            flipTimer = 0f;
        }

        if (shouldFlip)
        {
            flipTimer += Time.deltaTime;
            if (flipTimer >= flipDelay)
            {
                Flip();
                shouldFlip = false;
            }
        }

        // Movimiento continuo
        enemyTransform.position += moveDir * enemy.CurrentSpeed * Time.deltaTime;
    }

    public void Flip()
    {
        movingRight = !movingRight;
        enemyTransform.Rotate(0f, 180f, 0f);
    }

    private void OnDrawGizmos()
    {
        if (enemyTransform == null)
            enemyTransform = transform;

        Vector3 moveDir = movingRight ? Vector3.right : Vector3.left;
        Vector3 origin = enemyTransform.position + Vector3.up * 0.5f;

        // Gizmo: pared
        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, origin + moveDir * wallCheckDistance);

        // Gizmo: suelo
        Gizmos.color = Color.yellow;
        Vector3 groundOrigin = enemyTransform.position + moveDir * 0.5f;
        Gizmos.DrawLine(groundOrigin, groundOrigin + Vector3.down * groundCheckDistance);

        // Gizmo: aliado
        Gizmos.color = Color.green;
        Gizmos.DrawLine(origin, origin + moveDir * allyCheckDistance);
    }
}
