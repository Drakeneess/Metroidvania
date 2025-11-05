using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BossFightTrigger : MonoBehaviour
{
    [Header("Boss a activar")]
    [Tooltip("Referencia al jefe que se activará al entrar el jugador.")]
    public Boss boss;

    [Header("Configuración")]
    [Tooltip("Desactiva el trigger tras activarse una vez.")]
    public bool oneTimeTrigger = true;

    [Tooltip("Retraso antes de activar la pelea (para animación de entrada o fundido).")]
    public float activationDelay = 1.5f;

    private bool triggered = false;


    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;
        MusicController.Instance.GetCurrentTheme().SetLayerActive(1, true);
        MusicController.Instance.GetCurrentTheme().SetLayerActive(2, true);
        StartCoroutine(ActivateBossSequence());
    }

    private System.Collections.IEnumerator ActivateBossSequence()
    {
        // Pequeño retraso por efecto dramático
        yield return new WaitForSeconds(activationDelay);

        // Activar jefe
        if (boss != null)
        {
            boss.ActivateBoss();
            Debug.Log($"🔥 Boss Fight iniciada contra {boss.bossName}");
        }
        else
        {
            Debug.LogWarning("[BossFightTrigger] No se asignó ningún Boss.");
        }

        // Si solo se usa una vez
        if (oneTimeTrigger)
            gameObject.SetActive(false);
    }
}
