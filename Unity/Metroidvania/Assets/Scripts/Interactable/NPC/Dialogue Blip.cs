using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(ProceduralBlip))]
public class DialogueBlip : MonoBehaviour
{
    [SerializeField] public CharacterBlipProfile profile;

    private AudioSource audioSource;
    private bool isActive = false;
    private EmotionType currentEmotion = EmotionType.Calm;
    private ProceduralBlip blipGenerator;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        blipGenerator = GetComponent<ProceduralBlip>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnEnable()
    {
        DialogueSystem.OnLetterTyped += OnLetterTyped;

        // 🔹 suscribir al controlador global de SFX
        if (SFXController.Instance != null)
            SFXController.Instance.RegisterSource(audioSource);
    }

    private void OnDisable()
    {
        DialogueSystem.OnLetterTyped -= OnLetterTyped;

        // 🔹 desuscribir
        if (SFXController.Instance != null)
            SFXController.Instance.UnregisterSource(audioSource);
    }

    public void SetActive(bool value) => isActive = value;
    public void SetEmotion(EmotionType emotion) => currentEmotion = emotion;

    private void OnLetterTyped(char c)
    {
        if (!isActive || !char.IsLetterOrDigit(c)) return;

        float baseFreq = GetBaseFrequencyForCharacter();

        // Semitonos por emoción
        int semitones = GetEmotionSemitoneOffset(currentEmotion);
        float factor = Mathf.Pow(2f, semitones / 12f);
        float finalFreq = Mathf.Clamp(baseFreq * factor, 180f, 850f);
        float duration = 0.05f;

        // 🔹 Ajustar volumen local según perfil y volumen global
        float globalVol = SFXController.Instance != null
            ? SFXController.Instance.GetGlobalSFXVolume() / 100f
            : 0.5f; // fallback
        float emotionVol = GetEmotionVolumeMultiplier(currentEmotion);
        audioSource.volume = profile.baseVolume * emotionVol * globalVol;

        // ✅ aplicar preset emocional antes de sonar
        blipGenerator.SetEmotion(currentEmotion);
        blipGenerator.Play(finalFreq, duration);
    }

    private int GetEmotionSemitoneOffset(EmotionType emotion)
    {
        switch (emotion)
        {
            case EmotionType.Joy: return +3;       // menor tercera ↑
            case EmotionType.Sadness: return -2;   // segunda ↓
            case EmotionType.Anger: return +7;     // quinta justa ↑
            case EmotionType.Fear: return +1;      // semitono ↑
            case EmotionType.Calm: return 0;       // sin cambio
            case EmotionType.Contempt: return -3;  // menor tercera ↓
            case EmotionType.Confidence: return +5;// cuarta justa ↑
            default: return 0;
        }
    }

    private float GetEmotionVolumeMultiplier(EmotionType emotion)
    {
        // 🔸 variaciones de volumen perceptivo por emoción
        switch (emotion)
        {
            case EmotionType.Joy: return 1.1f;
            case EmotionType.Anger: return 1.2f;
            case EmotionType.Fear: return 0.9f;
            case EmotionType.Sadness: return 0.8f;
            case EmotionType.Contempt: return 0.9f;
            case EmotionType.Confidence: return 1.15f;
            case EmotionType.Calm: 
            default: return 1f;
        }
    }

    private float GetBaseFrequencyForCharacter()
    {
        return profile != null ? profile.baseFrequency : 440f; // A4 por defecto
    }
}


public enum EmotionType
{
    Joy = 0,
    Sadness = 1,
    Anger = 2,
    Fear = 3,
    Calm = 4,
    Contempt = 5,
    Confidence = 6
}

[System.Serializable]
public class CharacterBlipProfile
{
    public float baseFrequency = 440f; // Frecuencia central del personaje
    public float basePitch = 1f;
    public float pitchVariationPerEmotion = 0.1f;
    public float baseVolume = 1f;
    public float volumeVariationPerEmotion = 0.1f;
}
