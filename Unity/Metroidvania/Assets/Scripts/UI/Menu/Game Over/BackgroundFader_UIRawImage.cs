using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class BackgroundFader_UIRawImage : MonoBehaviour
{
    [Header("Fade Settings")]
    [Range(0f, 1f)] public float menuAlpha = 0.3f;
    [Range(0f, 2f)] public float fadeDuration = 0.6f;

    [Header("Revive Pulse (RA4)")]
    public float pulsePeak = 1.12f;     // P-Med
    public float pulseDuration = 0.35f; // time to go peak and return

    private RawImage rawImage;
    private Material instanceMat;
    private Coroutine fadeRoutine;

    private const string AlphaID = "_GlobalAlpha";

    private void Awake()
    {
        rawImage = GetComponent<RawImage>();

        // Important: clone material once to avoid affecting shared material
        instanceMat = Instantiate(rawImage.material);
        rawImage.material = instanceMat;

        SetAlpha(1f);
    }

    /// <summary>
    /// Fade background toward Game Over state (alpha lowered).
    /// </summary>
    public void FadeToMenu()
    {
        StartFade(menuAlpha);
    }

    /// <summary>
    /// Fade background to full gameplay, with pulse effect (RA4).
    /// </summary>
    public void FadeToGame()
    {
        StartFade(1f, doPulse: true);
    }

    private void StartFade(float target, bool doPulse = false)
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeRoutine(target, doPulse));
    }

    private IEnumerator FadeRoutine(float target, bool doPulse)
    {
        float start = GetAlpha();
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(start, target, t / fadeDuration);
            SetAlpha(a);
            yield return null;
        }

        SetAlpha(target);

        if (doPulse)
            yield return PulseRoutine();
    }

    private IEnumerator PulseRoutine()
    {
        float original = GetAlpha();
        float peak = pulsePeak;

        // Go up to peak
        float t = 0f;
        while (t < pulseDuration * 0.5f)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(original, peak, t / (pulseDuration * 0.5f));
            SetAlpha(a);
            yield return null;
        }

        // Return to 1.0
        t = 0f;
        while (t < pulseDuration * 0.5f)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(peak, 1f, t / (pulseDuration * 0.5f));
            SetAlpha(a);
            yield return null;
        }

        SetAlpha(1f);
    }

    private float GetAlpha()
    {
        return instanceMat.HasProperty(AlphaID) ? instanceMat.GetFloat(AlphaID) : 1f;
    }

    private void SetAlpha(float a)
    {
        if (instanceMat.HasProperty(AlphaID))
            instanceMat.SetFloat(AlphaID, a);
    }
}
