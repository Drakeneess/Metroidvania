using System.Collections;
using UnityEngine;

public class LogoController : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private CanvasGroup logoCanvas; // CanvasGroup del logo
    [SerializeField] private float fadeInTime = 2f;
    [SerializeField] private float stayTime = 3f;
    [SerializeField] private float fadeOutTime = 2f;
    [SerializeField] private float musicWaitTimeout = 3f; // tiempo máximo para esperar al MusicController

    private void OnEnable()
    {
        StartCoroutine(StartSequence());
    }

    private IEnumerator StartSequence()
    {
        // 🔹 Asegurar que el logo arranque invisible
        if (logoCanvas != null)
            logoCanvas.alpha = 0f;

        // 🔹 Esperar a que MusicController esté disponible
        float timer = 0f;
        while (MusicController.Instance == null && timer < musicWaitTimeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (MusicController.Instance != null)
        {
            // 🔹 Asegurar que haya un tema inicial y volumen aplicado
            MusicController.Instance.PlayTheme(0);
            var currentTheme = MusicController.Instance.GetCurrentTheme();
            if (currentTheme != null)
                currentTheme.FadeInTheme();
        }
        else
        {
            Debug.LogWarning("[LogoController] MusicController no se inicializó a tiempo — continuando sin música.");
        }

        // 🔹 Fade-in visual
        yield return StartCoroutine(FadeCanvas(logoCanvas, 0f, 1f, fadeInTime));

        // 🔹 Logo visible un tiempo
        yield return new WaitForSeconds(stayTime);

        // 🔹 Fade-out visual
        yield return StartCoroutine(FadeCanvas(logoCanvas, 1f, 0f, fadeOutTime));

        // 🔹 Pequeña pausa opcional antes del cambio
        yield return new WaitForSeconds(0.5f);

        // 🔹 Pasar al menú principal si existe el controlador
        if (MainMenuFlowController.Instance != null)
            MainMenuFlowController.Instance.StartMenuFlow();
        else
            Debug.LogWarning("[LogoController] No se encontró MainMenuFlowController.");

        gameObject.SetActive(false);
    }

    private IEnumerator FadeCanvas(CanvasGroup canvas, float from, float to, float duration)
    {
        if (canvas == null)
        {
            Debug.LogWarning("[LogoController] CanvasGroup no asignado.");
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            canvas.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        canvas.alpha = to;
    }
}
