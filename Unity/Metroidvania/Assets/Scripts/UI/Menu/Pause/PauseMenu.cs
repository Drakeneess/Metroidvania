using UnityEngine;
using System.Collections;

public class PauseMenu : Menu
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.35f;

    private Coroutine fadeRoutine;

    protected override void Start()
    {
        base.Start();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        Debug.Log($"[PauseMenu:Start] GO={name} | Alpha={canvasGroup.alpha} | Interactable={canvasGroup.interactable} | Blocks={canvasGroup.blocksRaycasts}");
    }

    public void Show()
    {
        Debug.Log("[PauseMenu] Show()");
        GameMenuController.CurrentMode = GameMode.Menu;
        FadeTo(1f);
    }

    public void Hide()
    {
        Debug.Log("[PauseMenu] Hide()");

        FadeTo(0f);
        GameMenuController.CurrentMode = GameMode.Game;
    }

    private void FadeTo(float target)
    {
        Debug.Log($"[PauseMenu] FadeTo({target})");
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeRoutine(target));
    }

    private IEnumerator FadeRoutine(float target)
    {
        float start = canvasGroup.alpha;
        float t = 0f;

        MenuInputLock.SetBlocked(true);
        canvasGroup.blocksRaycasts = false;

        Debug.Log($"[PauseMenu:FadeRoutine-START] target={target} | startAlpha={start}");

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, t / fadeDuration);
            Debug.Log($"[PauseMenu:Fade] t={t:F2} alpha={canvasGroup.alpha:F2}");
            yield return null;
        }

        canvasGroup.alpha = target;

        bool visible = target > 0.01f;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;

        areOptionsDeployed = visible;
        MenuInputLock.SetBlocked(!visible);

        Debug.Log($"[PauseMenu:FadeRoutine-END] visible={visible} | Alpha={canvasGroup.alpha} | Interactable={canvasGroup.interactable} | Blocks={canvasGroup.blocksRaycasts} | Deployed={areOptionsDeployed} | Blocked={MenuInputLock.Blocked}");
    }
}
