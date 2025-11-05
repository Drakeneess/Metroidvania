using System.Collections.Generic;
using UnityEngine;

public class SFXController : MonoBehaviour
{
    public static SFXController Instance { get; private set; }

    [Range(0f, 100f)]
    [SerializeField] private float globalSFXVolume = 50f; // 50 = normal
    private readonly List<AudioSource> registeredSources = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Inicializa desde SettingsValue si está disponible
        if (SettingsValue.Instance != null)
        {
            globalSFXVolume = Mathf.Clamp(SettingsValue.Instance.Settings.fxSound * 100f, 0f, 100f);
        }
        else
        {
            Debug.LogWarning("[SFXController] SettingsValue not ready — using default 50");
        }
    }

    /// <summary>
    /// Registra un AudioSource para que siga el volumen global de FX.
    /// </summary>
    public void RegisterSource(AudioSource source)
    {
        if (source == null || registeredSources.Contains(source)) return;

        registeredSources.Add(source);
        source.volume = SliderToVolume(globalSFXVolume);
    }

    /// <summary>
    /// Quita un AudioSource del control global.
    /// </summary>
    public void UnregisterSource(AudioSource source)
    {
        if (source == null) return;
        registeredSources.Remove(source);
    }

    /// <summary>
    /// Ajusta el volumen global de efectos (0–100).
    /// </summary>
    public void SetGlobalSFXVolume(float volume01to100)
    {
        globalSFXVolume = Mathf.Clamp(volume01to100, 0f, 100f);

        float perceptual = SliderToVolume(globalSFXVolume);
        foreach (var src in registeredSources)
        {
            if (src == null) continue;
            src.volume = perceptual;
        }
    }

    /// <summary>
    /// Devuelve el volumen actual (en escala 0–100).
    /// </summary>
    public float GetGlobalSFXVolume() => globalSFXVolume;

    /// <summary>
    /// Conversión perceptiva logarítmica (igual que música).
    /// </summary>
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
