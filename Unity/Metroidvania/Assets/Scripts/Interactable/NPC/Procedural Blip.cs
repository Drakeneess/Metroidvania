using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ProceduralBlip : MonoBehaviour
{
    private float frequency = 440f; // Hz
    private float duration = 0.05f; // segundos
    private float sampleRate = 48000f;
    private float phase;
    private int samplesPlayed = 0;
    private int totalSamplesToPlay = 0;
    private bool isPlaying = false;

    public void Play(float freq, float dur)
    {
        frequency = freq;
        duration = dur;
        samplesPlayed = 0;
        totalSamplesToPlay = Mathf.CeilToInt(sampleRate * duration);
        isPlaying = true;
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (!isPlaying) return;

        for (int i = 0; i < data.Length; i += channels)
        {
            float sample = Mathf.Sin(2 * Mathf.PI * frequency * phase / sampleRate);

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
