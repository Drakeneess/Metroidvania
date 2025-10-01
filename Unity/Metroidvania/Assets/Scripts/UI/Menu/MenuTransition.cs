using System.Collections;
using UnityEngine;

public class MenuTransition : MonoBehaviour
{
    public static MenuTransition Instance { get; private set; }

    [SerializeField] private float transitionDuration = 0.5f;

    private MenuContainer current;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // opcional: persistente entre escenas
    }

    /// <summary>
    /// Cambia suavemente entre el contenedor actual y otro.
    /// </summary>
    public void SwitchTo(MenuContainer target)
    {
        if (target == null || target == current) return;
        StopAllCoroutines();
        StartCoroutine(FadeSwitch(current, target));
        current = target;
    }

    public void SetInitial(MenuContainer initial)
    {
        if (!initial) return;
        current = initial;
        initial.SetVisible(true);
        initial.CanvasGroup.alpha = 1f;
    }

    private IEnumerator FadeSwitch(MenuContainer from, MenuContainer to)
    {
        // Fade out del actual
        if (from)
        {
            yield return FadeCanvasGroup(from.CanvasGroup, 1f, 0f, transitionDuration);
            from.SetVisible(false);
        }

        // Fade in del nuevo
        if (to)
        {
            to.SetVisible(true);
            to.CanvasGroup.alpha = 0f;
            yield return FadeCanvasGroup(to.CanvasGroup, 0f, 1f, transitionDuration);
        }
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
    {
        if (!cg) yield break;

        cg.alpha = from;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        cg.alpha = to;
    }
}
