using System.Collections;
using UnityEngine;

public class MainMenu : Menu
{
    public GameObject[] menuButtons;
    public GameObject[] titleContent;
    public GameObject courtine;
    public float fadeDurationTitle = 1.0f;
    public float fadeDurationButtons = 1.0f;
    public float verticalSpacing = 50f;
    public float horizontalOffset = 30f;


    protected override void Start()
    {
        base.Start();
        InitializeMenu();
    }

    private void OnDestroy()
    {
        // 🔥 Restaurar alpha de la courtine cuando se destruya el menú
        if (courtine != null)
        {
            var raw = courtine.GetComponent<UnityEngine.UI.RawImage>();
            if (raw != null && raw.material.HasProperty("_Alpha"))
            {
                raw.material.SetFloat("_Alpha", 1f);
            }
            else
            {
                var cg = courtine.GetComponent<CanvasGroup>();
                if (!cg) cg = courtine.AddComponent<CanvasGroup>();
                cg.alpha = 1f;
            }
        }
        GameSessionManager.Instance.EndGameSession();
        BehaviorManager.Instance.SyncBehaviorSummaryFireAndForget();
        BehaviorManager.Instance.SyncBehaviorSummary();
    }

    public void InitializeMenu()
    {
        InitializeElements(titleContent, Vector3.zero);
        InitializeElements(menuButtons, new Vector3(horizontalOffset, -verticalSpacing, 0));
    }

    private void InitializeElements(GameObject[] elements, Vector3 offsetStep)
    {
        int position = 0;
        for (int i = 0; i < elements.Length; i++)
        {
            GameObject element = elements[i];
            if (element != null)
            {
                CanvasGroup canvasGroup = GetOrAddCanvasGroup(element);
                canvasGroup.alpha = 0f;

                RectTransform rectTransform = element.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    position = NotDeployButtonContinue(element) ? position - 1 : position;
                    rectTransform.localPosition += offsetStep * position;
                }

                position++;
                element.SetActive(false);
            }
        }
    }

    private CanvasGroup GetOrAddCanvasGroup(GameObject obj)
    {
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = obj.AddComponent<CanvasGroup>();
        }
        return canvasGroup;
    }

    private IEnumerator TitleFadeIn()
    {
        for (int i = 0; i < titleContent.Length; i++)
        {
            GameObject titleItem = titleContent[i];
            titleItem.SetActive(true);
            float duration = (i == titleContent.Length - 1) ? fadeDurationTitle * 1.5f : fadeDurationTitle;

            yield return StartCoroutine(FadeElement(titleItem, duration, 1f));
            RumbleController.RumblePulse(0.05f, 0.1f, 0.7f);
            yield return new WaitForSeconds(0.1f);
        }

        MainMenuEvents.TriggerTitleComplete();
    }

    public IEnumerator StartTittleFadeIn()
    {
        MainMenuEvents.TriggerTitleStart();
        yield return StartCoroutine(TitleFadeIn());
    }

    public IEnumerator ButtonsFadeIn()
    {
        if (titleContent != null && titleContent.Length > 0)
            yield return StartCoroutine(FadeElement(titleContent[^1], 2f, 0f));

        foreach (GameObject button in menuButtons)
        {
            if (NotDeployButtonContinue(button)) continue;

            button.SetActive(true);
            yield return StartCoroutine(FadeElement(button, fadeDurationButtons, 1f));
            RumbleController.RumblePulse(0.01f, 0.07f, 0.4f);
        }

        if (SaveDataController.AreSavedData())
        {
            StartCoroutine(CourtineFadeOut());
        }

        // 'areOptionsDeployed' viene de Menu (base). Si no existe, ignora esta línea.
        areOptionsDeployed = true;
    }

    private IEnumerator CourtineFadeOut()
    {
        yield return StartCoroutine(FadeCourtine(courtine, 2f, 0f));
    }

    private IEnumerator FadeCourtine(GameObject element, float duration, float targetAlpha)
    {
        if (element == null) yield break;

        // si es la courtine, usamos el shader
        var raw = element.GetComponent<UnityEngine.UI.RawImage>();
        if (raw != null && raw.material.HasProperty("_Alpha"))
        {
            float startAlpha = raw.material.GetFloat("_Alpha");
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
                raw.material.SetFloat("_Alpha", newAlpha);
                yield return null;
            }
            raw.material.SetFloat("_Alpha", targetAlpha);
        }
        else
        {
            // fallback normal con CanvasGroup
            CanvasGroup canvasGroup = GetOrAddCanvasGroup(element);
            float startAlpha = canvasGroup.alpha;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
                yield return null;
            }
            canvasGroup.alpha = targetAlpha;
        }
    }

    private IEnumerator FadeElement(GameObject element, float duration, float targetAlpha)
    {
        if (element == null) yield break;

        CanvasGroup canvasGroup = GetOrAddCanvasGroup(element);
        float startAlpha = canvasGroup.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }

    public void CompleteFadeIn(GameObject[] elements)
    {
        StopAllCoroutines();
        foreach (GameObject element in elements)
        {
            if (element != null)
            {
                element.SetActive(true);
                CanvasGroup canvasGroup = GetOrAddCanvasGroup(element);
                canvasGroup.alpha = 1f;
            }
        }
    }

    private bool NotDeployButtonContinue(GameObject element)
    {
        return element == menuButtons[0] && !SaveDataController.AreSavedData();
    }

    // 👇 Nuevo: “ya se ven?” para que el Flow no bloquee si están visibles
    public bool AreButtonsVisible()
    {
        if (menuButtons == null) return false;
        foreach (var b in menuButtons)
        {
            if (!b) continue;
            var cg = b.GetComponent<CanvasGroup>();
            if (b.activeInHierarchy && cg != null && cg.alpha >= 0.95f) return true;
        }
        return false;
    }
}
