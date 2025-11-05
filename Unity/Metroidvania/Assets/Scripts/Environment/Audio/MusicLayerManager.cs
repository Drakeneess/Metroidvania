using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicLayerManager : MonoBehaviour
{
    [Header("Capas del tema (base + complementos)")]
    [SerializeField] private List<AudioSource> layers;

    [SerializeField] private float fadeTime = 2f;

    private Coroutine fadeCoroutine;
    private float masterVolume = 50f; // escala 0–100
    private bool isActive = false;    // 🔹 nuevo flag de control

    private void Awake()
    {
        foreach (var src in layers)
        {
            if (!src) continue;
            src.loop = true;
            src.volume = 0f;
            src.Stop(); // no arrancamos nada
        }
    }

    /// <summary>
    /// Activa el tema (solo capa base) con fade.
    /// </summary>
    public void FadeInTheme(float globalSlider0to100 = 50f)
    {
        masterVolume = Mathf.Clamp(globalSlider0to100, 0f, 100f);
        isActive = true;

        // Solo la capa base por defecto
        SetLayerActive(0, true);
    }

    /// <summary>
    /// Apaga todas las capas con fade y desactiva el tema.
    /// </summary>
    public void FadeOutTheme()
    {
        if (!isActive) return;
        isActive = false;

        foreach (var src in layers)
        {
            if (src != null && src.isPlaying)
                StartCoroutine(FadeLayer(src, 0f));
        }
    }

    /// <summary>
    /// Detiene completamente el tema (sin fade).
    /// </summary>
    public void StopThemeImmediate()
    {
        isActive = false;
        foreach (var src in layers)
        {
            if (!src) continue;
            src.volume = 0f;
            src.Stop();
        }
    }

    /// <summary>
    /// Enciende o apaga una capa específica del tema.
    /// </summary>
    public void SetLayerActive(int index, bool active)
    {
        if (!isActive || index < 0 || index >= layers.Count) return;
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        AudioSource src = layers[index];
        if (active && !src.isPlaying)
            src.Play();

        float target = active ? SliderToVolume(masterVolume) : 0f;
        fadeCoroutine = StartCoroutine(FadeLayer(src, target));
    }

    /// <summary>
    /// Ajusta el volumen global del tema activo (solo si está activo).
    /// </summary>
    public void SetMasterVolume(float slider0to100)
    {
        masterVolume = Mathf.Clamp(slider0to100, 0f, 100f);
        float perceptual = SliderToVolume(masterVolume);

        // 🚫 no tocar si no está activo
        if (!isActive)
        {
            // aseguramos que todo quede en silencio
            foreach (var src in layers)
            {
                if (src != null)
                {
                    src.volume = 0f;
                    src.Stop();
                }
            }
            return;
        }

        // solo aplicamos volumen si el tema está activo
        foreach (var src in layers)
        {
            if (!src) continue;
            if (perceptual > 0f && !src.isPlaying)
                src.Play();
            src.volume = perceptual;
        }
    }

    private IEnumerator FadeLayer(AudioSource src, float targetVolume)
    {
        if (!src.isPlaying && targetVolume > 0f)
            src.Play();

        float startVolume = src.volume;
        float t = 0f;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            src.volume = Mathf.Lerp(startVolume, targetVolume, t / fadeTime);
            yield return null;
        }

        src.volume = targetVolume;
        if (Mathf.Approximately(targetVolume, 0f))
            src.Stop();
    }

    // 🔸 50 = normal; <50 reduce logarítmico; >50 amplifica ligeramente
    private float SliderToVolume(float slider)
    {
        slider = Mathf.Clamp(slider, 0f, 100f);
        float normalized = (slider - 50f) / 50f;

        if (normalized < 0f)
        {
            float x = Mathf.Abs(normalized);
            return Mathf.Pow(1f - x, 2.2f);
        }
        else
        {
            float boost = 0.5f * Mathf.Pow(normalized, 1.5f);
            return 1f + boost;
        }
    }
}
