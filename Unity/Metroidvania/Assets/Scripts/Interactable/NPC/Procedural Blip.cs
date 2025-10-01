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
    public float vibratoFrequency = 6f;   // Hz
    public float vibratoIntensity = 10f;  // Hz (±)

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
    private int totalSamplesToPlay = 0;
    private int samplesPlayed = 0;
    private bool isPlaying = false;

    // ✅ usar sample rate real + tiempo en double para precisión
    private int sampleRate;
    private double time;         // segundos acumulados (resetea por blip)
    private double dt;           // 1 / sampleRate

    public enum WaveType { Sine, Square, Triangle, Saw, Noise }

    private void Awake()
    {
        sampleRate = AudioSettings.outputSampleRate;     // ✅ NO hardcodear 48000
        if (sampleRate <= 0) sampleRate = 48000;
        dt = 1.0 / sampleRate;
    }

    public void Play(float freq, float dur)
    {
        // ✅ reset total por blip
        frequency = Mathf.Clamp(freq, 120f, 900f);                  // rango seguro
        duration  = Mathf.Clamp(dur, 0.02f, 0.15f);
        totalSamplesToPlay = Mathf.CeilToInt((float)(sampleRate * duration));
        samplesPlayed = 0;
        time = 0.0;                                                 // ✅ reset tiempo
        previousSample = 0f;
        isPlaying = true;
    }

    public void SetEmotion(EmotionType emotion)
    {
        // Presets emocionales (suaves por defecto)
        switch (emotion)
        {
            case EmotionType.Joy:
                waveType = WaveType.Sine;
                useVibrato = true;  vibratoFrequency = 6f;  vibratoIntensity = 5f;
                useDistortion = false;
                useLowPass = false;
                break;

            case EmotionType.Sadness:
                waveType = WaveType.Triangle;
                useVibrato = true;  vibratoFrequency = 5f;  vibratoIntensity = 3f;
                useDistortion = false;
                useLowPass = true;  lowPassAmount = 0.45f;
                break;

            case EmotionType.Anger:
                waveType = WaveType.Square;
                useVibrato = false;
                useDistortion = true; distortionAmount = 3f;
                useLowPass = true;  lowPassAmount = 0.25f; // ✅ recorta agudos
                break;

            case EmotionType.Fear:
                waveType = WaveType.Sine;
                useVibrato = true;  vibratoFrequency = 9f;  vibratoIntensity = 8f;
                useDistortion = false;
                useLowPass = true;  lowPassAmount = 0.35f;
                break;

            case EmotionType.Calm:
                waveType = WaveType.Sine;
                useVibrato = false;
                useDistortion = false;
                useLowPass = true;  lowPassAmount = 0.55f;
                break;

            case EmotionType.Contempt:
                waveType = WaveType.Saw;
                useVibrato = false;
                useDistortion = true; distortionAmount = 2f;
                useLowPass = true;  lowPassAmount = 0.35f; // ✅ recorta brillo
                break;

            case EmotionType.Confidence:
                waveType = WaveType.Square;
                useVibrato = false;
                useDistortion = false;
                useLowPass = true;  lowPassAmount = 0.2f;
                break;
        }
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (!isPlaying) return;

        for (int i = 0; i < data.Length; i += channels)
        {
            // ✅ vibrato en Hz, estable por tiempo
            double vibHz = useVibrato ? System.Math.Sin(2.0 * System.Math.PI * vibratoFrequency * time) * vibratoIntensity : 0.0;
            double f = System.Math.Max(1.0, frequency + vibHz);
            double phaseAngle = 2.0 * System.Math.PI * f * time;

            float sample;
            switch (waveType)
            {
                case WaveType.Sine:
                    sample = (float)System.Math.Sin(phaseAngle);
                    break;
                case WaveType.Square:
                    sample = System.Math.Sin(phaseAngle) >= 0 ? 1f : -1f;
                    break;
                case WaveType.Triangle:
                    sample = (float)(2.0 * System.Math.Asin(System.Math.Sin(phaseAngle)) / System.Math.PI);
                    break;
                case WaveType.Saw:
                    sample = (float)(2.0 * (phaseAngle / (2.0 * System.Math.PI) - System.Math.Floor(phaseAngle / (2.0 * System.Math.PI) + 0.5)));
                    break;
                default: // Noise
                    sample = (float)(threadSafeRNG.NextDouble() * 2.0 - 1.0);
                    break;
            }

            // Cuantización 8-bit style
            sample = Mathf.Round(sample * amplitudeSteps) / amplitudeSteps;

            if (addNoise)
            {
                float noise = (float)(threadSafeRNG.NextDouble() * 2.0 - 1.0) * noiseAmount;
                sample += noise;
            }

            if (useLowPass)
            {
                sample = Mathf.Lerp(previousSample, sample, 1f - lowPassAmount);
                previousSample = sample;
            }

            if (useDistortion)
                sample = (float)System.Math.Tanh(sample * distortionAmount);

            // Fade-out corto
            float fadeOut = Mathf.Clamp01((float)(totalSamplesToPlay - samplesPlayed) / 64f);
            sample *= fadeOut;

            for (int c = 0; c < channels; c++)
                data[i + c] = sample;

            // ✅ avanzar tiempo con dt estable
            time += dt;
            samplesPlayed++;

            if (samplesPlayed >= totalSamplesToPlay)
            {
                isPlaying = false;
                break;
            }
        }
    }
}
