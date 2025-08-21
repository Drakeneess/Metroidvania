using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ProceduralBlip : MonoBehaviour
{
    [Header("Simulación 8-bit")]
    [Range(2, 256)] public int amplitudeSteps = 16;
    public bool addNoise = false;
    [Range(0f, 0.1f)] public float noiseAmount = 0.01f;

    [Header("Vibrato")]
    public bool useVibrato = false;
    public float vibratoFrequency = 6f;
    public float vibratoIntensity = 10f;

    [Header("Distorsión")]
    public bool useDistortion = false;
    [Range(1f, 10f)] public float distortionAmount = 2f;

    [Header("Filtro Low-Pass")]
    public bool useLowPass = false;
    [Range(0f, 1f)] public float lowPassAmount = 0.2f;

    [Header("Forma de Onda")]
    public WaveType waveType = WaveType.Sine;

    private System.Random threadSafeRNG = new System.Random();
    private float previousSample = 0f;

    private float frequency = 440f;
    private float duration = 0.05f;
    private float sampleRate = 48000f;
    private float phase;
    private int samplesPlayed = 0;
    private int totalSamplesToPlay = 0;
    private bool isPlaying = false;

    public enum WaveType { Sine, Square, Triangle, Saw, Noise }

    public void Play(float freq, float dur)
    {
        frequency = freq;
        duration = dur;
        samplesPlayed = 0;
        totalSamplesToPlay = Mathf.CeilToInt(sampleRate * duration);
        isPlaying = true;
    }

    public void SetEmotion(EmotionType emotion)
    {
        // Presets emocionales
        switch (emotion)
        {
            case EmotionType.Joy:
                waveType = WaveType.Sine;
                useVibrato = false;
                useDistortion = false;
                useLowPass = false;
                break;

            case EmotionType.Sadness:
                waveType = WaveType.Triangle;
                useVibrato = true;
                vibratoIntensity = 5f;
                useDistortion = false;
                useLowPass = true;
                lowPassAmount = 0.4f;
                break;

            case EmotionType.Anger:
                waveType = WaveType.Square;
                useVibrato = false;
                useDistortion = true;
                distortionAmount = 5f;
                useLowPass = false;
                break;

            case EmotionType.Fear:
                waveType = WaveType.Sine;
                useVibrato = true;
                vibratoIntensity = 15f;
                vibratoFrequency = 10f;
                useDistortion = false;
                useLowPass = true;
                lowPassAmount = 0.3f;
                break;

            case EmotionType.Calm:
                waveType = WaveType.Sine;
                useVibrato = false;
                useDistortion = false;
                useLowPass = true;
                lowPassAmount = 0.5f;
                break;

            case EmotionType.Contempt:
                waveType = WaveType.Saw;
                useVibrato = false;
                useDistortion = true;
                distortionAmount = 2f;
                useLowPass = false;
                break;

            case EmotionType.Confidence:
                waveType = WaveType.Square;
                useVibrato = false;
                useDistortion = false;
                useLowPass = false;
                break;
        }
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (!isPlaying) return;

        for (int i = 0; i < data.Length; i += channels)
        {
            float t = phase / sampleRate;

            // Vibrato
            float vibrato = useVibrato ? Mathf.Sin(2 * Mathf.PI * vibratoFrequency * t) * vibratoIntensity : 0f;
            float rawPhase = (frequency + vibrato) * t;

            // Onda base
            float sample = 0f;
            switch (waveType)
            {
                case WaveType.Sine:
                    sample = Mathf.Sin(2 * Mathf.PI * rawPhase);
                    break;
                case WaveType.Square:
                    sample = Mathf.Sign(Mathf.Sin(2 * Mathf.PI * rawPhase));
                    break;
                case WaveType.Triangle:
                    sample = Mathf.PingPong(2 * rawPhase, 1f) * 2f - 1f;
                    break;
                case WaveType.Saw:
                    sample = 2f * (rawPhase - Mathf.Floor(rawPhase + 0.5f));
                    break;
                case WaveType.Noise:
                    sample = (float)(threadSafeRNG.NextDouble() * 2.0 - 1.0);
                    break;
            }

            // Cuantización
            sample = Mathf.Round(sample * amplitudeSteps) / amplitudeSteps;

            // Ruido emocional
            if (addNoise)
            {
                float noise = (float)(threadSafeRNG.NextDouble() * 2.0 - 1.0) * noiseAmount;
                sample += noise;
            }

            // Filtro low-pass
            if (useLowPass)
            {
                sample = Mathf.Lerp(previousSample, sample, 1f - lowPassAmount);
                previousSample = sample;
            }

            // Distorsión suave
            if (useDistortion)
                sample = (float)System.Math.Tanh(sample * distortionAmount);

            // Fade-out para evitar clics
            float fadeOut = Mathf.Clamp01((float)(totalSamplesToPlay - samplesPlayed) / 50f);
            sample *= fadeOut;

            for (int c = 0; c < channels; c++)
                data[i + c] = sample;

            phase++;
            samplesPlayed++;

            if (samplesPlayed >= totalSamplesToPlay)
            {
                isPlaying = false;
                break;
            }
        }
    }
}
