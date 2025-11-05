using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class MeleeTriggerZone : MonoBehaviour
{
    private MeleeEnemy owner;
    private BoxCollider boxCollider;
    private bool isActive = false;

    private void Awake()
    {
        owner = GetComponentInParent<MeleeEnemy>();
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        boxCollider.enabled = false; // apagado por defecto
    }

    public void Activate()
    {
        isActive = true;
        boxCollider.enabled = true;
    }

    public void Deactivate()
    {
        isActive = false;
        boxCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;
        if (owner == null || owner.isDead) return;

        if (other.TryGetComponent<Player>(out var player))
        {
            player.TakePhysicalDamage(owner.meleeDamage, owner);
            owner.OnSuccessfulHit(player);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = isActive ? Color.green : new Color(1f, 0f, 0f, 0.3f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
#endif
}
