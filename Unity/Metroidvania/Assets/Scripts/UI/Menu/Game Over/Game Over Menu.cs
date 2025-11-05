using UnityEngine;
using System.Collections;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.6f;
    public float FadeDuration => fadeDuration;

    private bool isListening = false;
    private bool allowInput = false;
    private bool isRespawning = false;   // ✅ Candado anti-spam
    private Coroutine fadeRoutine;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        allowInput = false;
        isRespawning = false;
    }

    public void Show()
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        isRespawning = false; // ✅ Reset por si acaso
        FadeTo(1f);
    }

    public void Hide()
    {
        allowInput = false;
        FadeTo(0f);
    }

    private void FadeTo(float target)
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeRoutine(target));
    }

    private IEnumerator FadeRoutine(float target)
    {
        float start = canvasGroup.alpha;
        float t = 0f;
        allowInput = false;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = target;
        bool visible = target > 0.01f;

        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;

        // ✅ Solo permitir input cuando el menú YA terminó de aparecer
        allowInput = visible;
    }

    private void OnEnable()
    {
        if (isListening) return;

        InputActionController.Instance.OnActionTriggered += OnRespawn;
        isListening = true;
    }

    private void OnDisable()
    {
        if (!isListening) return;

        InputActionController.Instance.OnActionTriggered -= OnRespawn;
        isListening = false;
        allowInput = false;
        isRespawning = false;
    }

    private void OnRespawn(string actionName)
    {
        if (!allowInput) return;
        if (actionName != "Select") return;
        if (isRespawning) return; // 🚫 Ya se está respawneando → ignorar inputs extra

        // ✅ Cerrar la puerta de inmediato
        allowInput = false;
        isRespawning = true;

        GameOverManager.Instance.TryRespawn();
    }
}
