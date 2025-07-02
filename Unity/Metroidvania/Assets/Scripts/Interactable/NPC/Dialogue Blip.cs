using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    private void OnDisable()
    {
        DialogueSystem.OnLetterTyped -= OnLetterTyped;
    }

    public void SetActive(bool value) => isActive = value;
    public void SetEmotion(EmotionType emotion) => currentEmotion = emotion;

    private void OnLetterTyped(char c)
    {
        if (!isActive || !char.IsLetterOrDigit(c)) return;

        float baseFreq = GetBaseFrequencyForCharacter();
        float pitchOffset = GetEmotionPitchOffset(currentEmotion);
        float duration = 0.05f;

        blipGenerator.Play(baseFreq + pitchOffset, duration);
    }

    private float GetEmotionVolumeOffset(EmotionType emotion)
    {
        switch (emotion)
        {
            case EmotionType.Joy: return +profile.volumeVariationPerEmotion;
            case EmotionType.Sadness: return -profile.volumeVariationPerEmotion * 0.5f;
            case EmotionType.Anger: return +profile.volumeVariationPerEmotion * 1.5f;
            case EmotionType.Fear: return -profile.volumeVariationPerEmotion * 0.3f;
            case EmotionType.Calm: return 0f;
            case EmotionType.Contempt: return -profile.volumeVariationPerEmotion;
            case EmotionType.Confidence: return +profile.volumeVariationPerEmotion * 0.8f;
            default: return 0f;
        }
    }
    private float GetEmotionPitchOffset(EmotionType emotion)
    {
        switch (emotion)
        {
            case EmotionType.Joy: return 100f;
            case EmotionType.Sadness: return -50f;
            case EmotionType.Anger: return 150f;
            case EmotionType.Fear: return 75f;
            case EmotionType.Calm: return 0f;
            case EmotionType.Contempt: return -25f;
            case EmotionType.Confidence: return 50f;
            default: return 0f;
        }
    }
    private float GetBaseFrequencyForCharacter()
    {
        return profile != null ? profile.baseFrequency : 440f; // fallback: A4
    }
}



public enum EmotionType {
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


