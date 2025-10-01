using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class VolumeManager : MonoBehaviour
{
    public static VolumeManager Instance { get; private set; }

    [Header("Lista de Volumes a controlar (orden importa)")]
    [SerializeField] private List<Volume> volumes = new List<Volume>();

    [Header("Config")]
    [Range(0f, 1f)] public float maxWeight = 0.7f;
    [SerializeField, Min(0f)] private float transitionDuration = 1f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Asegura que todos los Volumes arranquen apagados
        foreach (var v in volumes)
            if (v) v.weight = 0f;
    }

    /// <summary>
    /// Activa un perfil por índice, con fade. Los demás bajan a 0.
    /// </summary>
    public void ActivateProfile(int index)
    {
        if (index < 0 || index >= volumes.Count)
        {
            Debug.LogWarning($"[VolumeManager] Índice {index} fuera de rango.");
            return;
        }

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(TransitionToProfile(index));
    }

    private IEnumerator TransitionToProfile(int index)
    {
        float elapsed = 0f;

        // Guardamos estados iniciales
        float[] startWeights = new float[volumes.Count];
        for (int i = 0; i < volumes.Count; i++)
            startWeights[i] = volumes[i] ? volumes[i].weight : 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);

            for (int i = 0; i < volumes.Count; i++)
            {
                if (!volumes[i]) continue;

                float target = (i == index) ? maxWeight : 0f;
                volumes[i].weight = Mathf.Lerp(startWeights[i], target, t);
            }

            yield return null;
        }

        // Asegura estado final exacto
        for (int i = 0; i < volumes.Count; i++)
        {
            if (!volumes[i]) continue;
            volumes[i].weight = (i == index) ? maxWeight : 0f;
        }

        currentRoutine = null;
    }
}
