using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public static MusicController Instance { get; private set; }

    [Header("Lista de temas con sus managers")]
    [SerializeField] private List<MusicLayerManager> themes;

    private MusicLayerManager currentTheme;
    private float globalMusicVolume = 50f; // 50 = normal
    private bool volumeReady = false;
    private int? pendingThemeIndex = null;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (var theme in themes)
            theme?.StopThemeImmediate();
    }

    private void Start()
    {
        StartCoroutine(InitializeVolume());
    }

    private IEnumerator InitializeVolume()
    {
        // Espera 1 frame para garantizar que SettingsValue haya hecho Awake()/LoadSettings()
        yield return null;

        if (SettingsValue.Instance != null)
        {
            globalMusicVolume = Mathf.Clamp(SettingsValue.Instance.Settings.music, 0f, 100f);
        }
        else
        {
            globalMusicVolume = 50f;
        }

        // marca listo y aplica volumen global
        volumeReady = true;
        ApplyVolumeToAll(globalMusicVolume);

        // si había un tema pendiente antes de que el volumen estuviera listo, lánzalo ahora
        if (pendingThemeIndex.HasValue)
        {
            int idx = pendingThemeIndex.Value;
            pendingThemeIndex = null;
            PlayTheme(idx); // ahora sí hará FadeIn con el volumen correcto
        }
        else if (currentTheme != null)
        {
            // si por alguna razón ya había un tema activo, asegúrate de aplicarle el volumen correcto
            currentTheme.SetMasterVolume(globalMusicVolume);
        }
    }

    /// <summary>
    /// Cambia el tema activo (si el volumen no está listo, lo difiere).
    /// </summary>
    public void PlayTheme(int index)
    {
        if (index < 0 || index >= themes.Count) return;

        if (!volumeReady)
        {
            // aún no tenemos volumen definitivo → difiere la reproducción
            pendingThemeIndex = index;
            return;
        }

        if (currentTheme != null)
            currentTheme.FadeOutTheme();

        currentTheme = themes[index];
        currentTheme.FadeInTheme(globalMusicVolume); // ya inicializado
    }

    /// <summary>
    /// Cambia el volumen global de la música en tiempo real (0..100)
    /// </summary>
    public void SetGlobalMusicVolume(float volume01to100)
    {
        globalMusicVolume = Mathf.Clamp(volume01to100, 0f, 100f);
        ApplyVolumeToAll(globalMusicVolume);
    }

    private void ApplyVolumeToAll(float slider0to100)
    {
        foreach (var theme in themes)
        {
            if (theme != null)
                theme.SetMasterVolume(slider0to100);
        }
    }

    public MusicLayerManager GetCurrentTheme() => currentTheme;
}
