using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class FallRecoveryZone : MonoBehaviour
{
    [Header("Recuperación")]
    [Tooltip("Punto al que volverá el jugador")]
    public Transform returnPoint;

    [Tooltip("Daño al caer")]
    public float fallDamage = 10f;

    [Tooltip("Tiempo que la cámara queda bloqueada")]
    public float cameraFreezeTime = 0.8f;

    private bool isRecovering = false;

    private void Reset()
    {
        // Para que siempre sea trigger
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isRecovering) return;
        if (!other.CompareTag("Player")) return;

        if (!other.TryGetComponent<Player>(out var player)) return;

        StartCoroutine(RecoverPlayer(player));
    }

    private IEnumerator RecoverPlayer(Player player)
    {
        isRecovering = true;

        // ❌ Apagar seguimiento momentáneamente
        CameraController.IsFollowingPlayer = false;

        // 💥 Aplicar daño
        player.TakePhysicalDamage(fallDamage, null);
        FadeController.Instance.FadeIn(0.1f);

        // 🧊 Mini freeze / impacto
        if (FeedbackManager.Instance != null)
            FeedbackManager.Instance.TriggerHitStop(0.1f);

        // 🙈 Esperar para que se vea el “oh shit I fell”
        yield return new WaitForSeconds(cameraFreezeTime);

        // ♻️ Reposicionar
        if (returnPoint != null)
        {
            player.transform.position = returnPoint.position;
        }

        // 🔁 Pequeño delay para no marear a la cámara
        yield return new WaitForSeconds(0.15f);

        // ✅ Restaurar cámara
        CameraController.IsFollowingPlayer = true;
        isRecovering = false;
        FadeController.Instance.FadeOut(0.1f);
    }
}
