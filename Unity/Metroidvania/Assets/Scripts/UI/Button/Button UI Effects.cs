using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ButtonUIEffects : MonoBehaviour
{
    public ButtonUIEffectsType[] effectTypes; // Array de efectos a aplicar

    [Header("Scale Effect")]
    public Vector3 scaleMultiplier = new Vector3(1.2f, 1.2f, 1f);
    public float scaleDuration = 0.5f;

    [Header("Fade Effects")]
    public float fadeDuration = 0.5f;

    [Header("Shake Effect")]
    public float shakeIntensity = 10f;
    public float shakeDuration = 0.5f;

    [Header("Blink Effect")]
    public float blinkSpeed = 0.2f;
    public bool isBlinkContinuous = false;
    public float blinkDuration = 1f;

    private Image image;
    private Vector3 originalScale;
    private Color originalColor;

    private Coroutine activeCoroutine = null; // Variable para trackear la coroutine activa

    private void Start()
    {
        image = GetComponent<Image>();
        originalScale = transform.localScale;
        originalColor = image != null ? image.color : Color.white;
    }

    private void OnEnable() {
        ApplyAllEffects(); // Iniciar efectos al principio
    }

    public void ApplyAllEffects()
    {
        foreach (var effectType in effectTypes)
        {
            ApplyEffect(effectType);
        }
    }

    private void ApplyEffect(ButtonUIEffectsType effectType)
    {
        if (activeCoroutine != null) // Si ya hay una coroutine en ejecución, cancelarla
        {
            StopCoroutine(activeCoroutine);
        }

        switch (effectType)
        {
            case ButtonUIEffectsType.Shake:
                activeCoroutine = StartCoroutine(ShakeEffect());
                break;
            case ButtonUIEffectsType.Blink:
                activeCoroutine = StartCoroutine(BlinkEffect());
                break;
            case ButtonUIEffectsType.FadeIn:
                activeCoroutine = StartCoroutine(FadeEffect(1f));
                break;
            case ButtonUIEffectsType.FadeOut:
                activeCoroutine = StartCoroutine(FadeEffect(0f));
                break;
            case ButtonUIEffectsType.Scale:
                activeCoroutine = StartCoroutine(ScaleEffect());
                break;
            default:
                Debug.LogWarning("No effect selected.");
                break;
        }
    }

    public void ApplyIntroEffect()
    {
        foreach (var effectType in effectTypes)
        {
            if (effectType == ButtonUIEffectsType.FadeIn)
            {
                ApplyEffect(effectType);
            }
        }
    }

    public void ApplyOutEffect()
    {
        foreach (var effectType in effectTypes)
        {
            if (effectType == ButtonUIEffectsType.Scale || effectType == ButtonUIEffectsType.FadeOut)
            {
                ApplyEffect(effectType);
            }
        }
    }

    private IEnumerator ScaleEffect()
    {
        Vector3 targetScale = originalScale * scaleMultiplier.x;
        return AnimateScale(targetScale);
    }

    private IEnumerator FadeEffect(float targetAlpha)
    {
        if (image != null)
        {
            Color startColor = image.color;
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / fadeDuration);
                Color newColor = image.color;
                newColor.a = Mathf.Lerp(startColor.a, targetAlpha, t);
                image.color = newColor;
                yield return null;
            }

            // Ensure the final alpha is set correctly
            Color finalColor = image.color;
            finalColor.a = targetAlpha;
            image.color = finalColor;
        }
    }

    private IEnumerator ShakeEffect()
    {
        Vector3 originalPosition = transform.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            elapsedTime += Time.deltaTime;
            float offsetX = Random.Range(-shakeIntensity, shakeIntensity);
            float offsetY = Random.Range(-shakeIntensity, shakeIntensity);
            transform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0);
            yield return null;
        }

        transform.localPosition = originalPosition;
    }

    private IEnumerator BlinkEffect()
    {
        if (image != null)
        {
            float elapsedTime = 0f;
            bool isVisible = true;

            while (isBlinkContinuous || elapsedTime < blinkDuration)
            {
                if (!isBlinkContinuous)
                    elapsedTime += blinkSpeed;

                // Gradual fade in/out
                yield return StartCoroutine(FadeInOut(isVisible));
                isVisible = !isVisible;
                yield return new WaitForSeconds(blinkSpeed);
            }

            image.color = originalColor;
        }
    }

    private IEnumerator FadeInOut(bool isVisible)
    {
        float fadeElapsed = 0f;
        float fadeDuration = blinkSpeed / 2f;

        while (fadeElapsed < fadeDuration)
        {
            fadeElapsed += Time.deltaTime;
            float alpha = isVisible
                ? Mathf.Lerp(0f, 1f, fadeElapsed / fadeDuration)
                : Mathf.Lerp(1f, 0f, fadeElapsed / fadeDuration);

            Color newColor = image.color;
            newColor.a = alpha;
            image.color = newColor;
            yield return null;
        }
    }

    private IEnumerator AnimateScale(Vector3 targetScale)
    {
        float elapsedTime = 0f;

        while (elapsedTime < scaleDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / scaleDuration;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        transform.localScale = originalScale;
    }

    private void OnDisable()
    {
        // Cancel any active coroutines
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }
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
