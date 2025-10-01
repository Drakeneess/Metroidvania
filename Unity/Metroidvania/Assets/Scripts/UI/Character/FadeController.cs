using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    public static FadeController Instance;

    [SerializeField] private Image fadeImage;
    [SerializeField, Min(0f)] private float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (fadeImage) fadeImage.gameObject.SetActive(true);
        SetAlpha(0f); // empieza transparente
    }

    public void FadeIn(float duration = -1f, System.Action onComplete = null)
    {
        float finalDuration = duration < 0 ? fadeDuration : duration;

        StartCoroutine(FadeRoutine(1f, fadeDuration, onComplete));
    }

    public void FadeOut(float duration = -1f, System.Action onComplete = null)
    {
        float finalDuration = duration < 0 ? fadeDuration : duration;
        StartCoroutine(FadeRoutine(0f, finalDuration, onComplete));
    }

    private IEnumerator FadeRoutine(float target, float duration, System.Action onComplete)
    {
        if (!fadeImage) yield break;

        float start = fadeImage.color.a;
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(start, target, t / duration);
            SetAlpha(a);
            yield return null;
        }

        SetAlpha(target);
        onComplete?.Invoke();
    }


    private void SetAlpha(float a)
    {
        if (!fadeImage) return;
        Color c = fadeImage.color;
        c.a = a;
        fadeImage.color = c;
    }
}
