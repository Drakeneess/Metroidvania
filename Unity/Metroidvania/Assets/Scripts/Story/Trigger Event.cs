using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class TriggerEvent : MonoBehaviour
{
    [Header("Filtro")]
    public string requiredTag = "Player";
    public bool oneShot = false;

    [Header("Eventos")]
    public UnityEvent onPlayerEnter;
    public UnityEvent onPlayerExit;

    private Collider col;
    private bool used;

    private void Reset()
    {
        col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (used && oneShot) return;
        if (!Matches(other)) return;

        onPlayerEnter?.Invoke();
        if (oneShot) used = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!Matches(other)) return;
        onPlayerExit?.Invoke();
    }

    private bool Matches(Collider other)
    {
        if (other.CompareTag(requiredTag)) return true;
        if (other.attachedRigidbody && other.attachedRigidbody.CompareTag(requiredTag)) return true;
        if (other.GetComponentInParent<Player>() != null) return true;
        return false;
    }
}
