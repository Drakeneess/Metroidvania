using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BossMeleeHitbox : MonoBehaviour
{
    [Tooltip("Daño adicional sobre el daño base del jefe.")]
    public float extraDamage = 0f;

    [Tooltip("Tiempo entre cada aplicación de daño mientras el jugador está dentro del área.")]
    public float tickInterval = 1.0f;

    [Tooltip("Capas que pueden recibir daño (debe incluir Player).")]
    public LayerMask targetMask;

    private Boss owner;
    private Collider col;

    private void Awake()
    {
        owner = GetComponentInParent<Boss>();
        col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnEnable()
    {
        StartCoroutine(DamageLoop());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator DamageLoop()
    {
        var hits = new Collider[8];

        while (true)
        {
            // Verificamos quién está dentro del volumen actual
            int count = Physics.OverlapBoxNonAlloc(
                col.bounds.center,
                col.bounds.extents,
                hits,
                Quaternion.identity,
                targetMask
            );

            for (int i = 0; i < count; i++)
            {
                var other = hits[i];
                if (other == null) continue;

                if (other.TryGetComponent<Player>(out var player))
                {
                    float dmg = Mathf.Max(0f, owner.CurrentAttackDamage + extraDamage);
                    player.TakePhysicalDamage(dmg, owner);
                    // feedback opcional: cámara, sonido, etc.
                }
            }

            yield return new WaitForSeconds(tickInterval);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        var c = GetComponent<Collider>() as BoxCollider;
        if (c == null) return;

        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
        Matrix4x4 old = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(c.center, c.size);
        Gizmos.matrix = old;
    }
#endif
}
