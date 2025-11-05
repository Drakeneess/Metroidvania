using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonUIEffects : MonoBehaviour
{
    public ButtonUIEffectsType[] effectTypes;

    [Header("Scale Effect (S-B / Hold)")]
    public Vector3 scaleMultiplier = new Vector3(1.2f, 1.2f, 1f);
    public float scaleDuration = 0.25f;

    [Header("Fade Effects")]
    public float fadeDuration = 0.5f;

    [Header("Shake Effect")]
    public float shakeIntensity = 8f;
    public float shakeDuration = 0.4f;

    [Header("Blink Effect")]
    public float blinkSpeed = 0.25f;
    public bool continuousBlink = false;
    public float blinkDuration = 1f;

    [Header("Pressed Effect (P2 – Sink)")]
    public float pressScale = 0.85f;
    public float pressDuration = 0.12f;

    private Image image;
    private CanvasGroup canvasGroup;

    private Vector3 originalScale;
    private Color originalColor;

    private Dictionary<ButtonUIEffectsType, Coroutine> activeCoroutines = new();

    private void Awake()
    {
        image = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        originalScale = transform.localScale;
        originalColor = image ? image.color : Color.white;
    }

    private void OnEnable()
    {
        ApplyAllEffects();
    }

    public void ApplyAllEffects()
    {
        foreach (var type in effectTypes)
        {
            StartEffect(type);
        }
    }

    public void StartEffect(ButtonUIEffectsType type)
    {
        if (activeCoroutines.ContainsKey(type) && activeCoroutines[type] != null)
            StopCoroutine(activeCoroutines[type]);

        Coroutine c = type switch
        {
            ButtonUIEffectsType.Scale    => StartCoroutine(ScaleEffect()),
            ButtonUIEffectsType.FadeIn   => StartCoroutine(FadeEffect(1f)),
            ButtonUIEffectsType.FadeOut  => StartCoroutine(FadeEffect(0f)),
            ButtonUIEffectsType.Shake    => StartCoroutine(ShakeEffect()),
            ButtonUIEffectsType.Blink    => StartCoroutine(BlinkEffect()),
            ButtonUIEffectsType.Pressed  => StartCoroutine(PressedEffect()),
            _ => null
        };

        if (c != null)
            activeCoroutines[type] = c;
    }

    private IEnumerator ScaleEffect()
    {
        Vector3 targetScale = Vector3.Scale(originalScale, scaleMultiplier);
        float elapsed = 0f;

        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / scaleDuration;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        transform.localScale = targetScale; // stays scaled (S-B)
    }

    private IEnumerator FadeEffect(float target)
    {
        float start = canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = target;
    }

    private IEnumerator ShakeEffect()
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float x = Random.Range(-shakeIntensity, shakeIntensity);
            float y = Random.Range(-shakeIntensity, shakeIntensity);
            transform.localPosition = originalPos + new Vector3(x, y, 0);
            yield return null;
        }

        transform.localPosition = originalPos;
    }

    private IEnumerator BlinkEffect()
    {
        float elapsed = 0f;
        bool visible = true;

        while (continuousBlink || elapsed < blinkDuration)
        {
            if (!continuousBlink) elapsed += blinkSpeed;

            yield return StartCoroutine(BlinkStep(visible));
            visible = !visible;
            yield return new WaitForSeconds(blinkSpeed);
        }

        canvasGroup.alpha = 1f; // ensure visible back
    }

    private IEnumerator BlinkStep(bool visible)
    {
        float time = 0f;
        float half = blinkSpeed * 0.5f;

        float start = canvasGroup.alpha;
        float end = visible ? 1f : 0f;

        while (time < half)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, end, time / half);
            yield return null;
        }

        canvasGroup.alpha = end;
    }

    private IEnumerator PressedEffect()
    {
        Vector3 pressedScale = originalScale * pressScale;
        float time = 0f;

        // Scale down (sink)
        while (time < pressDuration)
        {
            time += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, pressedScale, time / pressDuration);
            yield return null;
        }

        // Return to original
        time = 0f;
        while (time < pressDuration)
        {
            time += Time.deltaTime;
            transform.localScale = Vector3.Lerp(pressedScale, originalScale, time / pressDuration);
            yield return null;
        }

        transform.localScale = originalScale;
    }

    private void OnDisable()
    {
        foreach (var kvp in activeCoroutines)
        {
            if (kvp.Value != null)
                StopCoroutine(kvp.Value);
        }

        activeCoroutines.Clear();

        // Reset to original (R1)
        transform.localScale = originalScale;
        canvasGroup.alpha = 1f;
        if (image != null) image.color = originalColor;
    }
}

public enum ButtonUIEffectsType
{
    None,
    Scale,
    FadeIn,
    FadeOut,
    Shake,
    Blink,
    Pressed
}
