using System.Collections;
using UnityEngine;
using TMPro;

public abstract class TypewriterPanelBase : MonoBehaviour
{
    [Header("UI Base")]
    [SerializeField] protected TextMeshProUGUI textUI;
    [SerializeField, Min(0f)] protected float typingSpeed = 0.03f;

    // Estado común
    protected Coroutine typingCo;
    protected Coroutine autoHideCo;
    protected string currentFullText;
    protected bool isShowing;

    protected virtual void Awake()
    {
    }

    protected virtual void OnEnable()
    {
        LanguageController.OnLanguageChanged += HandleLanguageChangedBase;
    }

    protected virtual void OnDisable()
    {
        LanguageController.OnLanguageChanged -= HandleLanguageChangedBase;
    }

    // -------- Ciclo base --------
    protected void ShowPanel()
    {
        isShowing = true;
    }

    protected void HideImmediate()
    {
        if (typingCo != null) StopCoroutine(typingCo);
        if (autoHideCo != null) StopCoroutine(autoHideCo);
        typingCo = null;
        autoHideCo = null;

        if (textUI) textUI.text = "";
        isShowing = false;
        currentFullText = null;
    }

    protected void TypeText(string full, System.Action onCompleted = null)
    {
        currentFullText = full ?? "";
        if (!isShowing) ShowPanel();

        if (typingCo != null) StopCoroutine(typingCo);
        if (typingSpeed <= 0f)
        {
            if (textUI) textUI.text = currentFullText;
            onCompleted?.Invoke();
        }
        else
        {
            typingCo = StartCoroutine(TypewriterCo(currentFullText, onCompleted));
        }
    }

    protected IEnumerator TypewriterCo(string full, System.Action onCompleted)
    {
        if (textUI) textUI.text = "";
        foreach (char c in full)
        {
            if (textUI) textUI.text += c;
            OnCharTyped(c);
            yield return new WaitForSeconds(typingSpeed);
        }
        typingCo = null;
        onCompleted?.Invoke();
    }

    public virtual void ForceCompleteOrHide()
    {
        if (!isShowing || currentFullText == null) return;

        if (typingCo != null)
        {
            StopCoroutine(typingCo);
            typingCo = null;
            if (textUI) textUI.text = currentFullText;
        }
        else
        {
            HideImmediate();
        }
    }

    protected virtual void OnCharTyped(char c) { }

    private void HandleLanguageChangedBase()
    {
        OnLanguageChanged();
    }

    protected abstract void OnLanguageChanged();

    protected void StartAutoHide(float seconds, System.Action after = null)
    {
        if (autoHideCo != null) StopCoroutine(autoHideCo);
        autoHideCo = StartCoroutine(AutoHideCo(seconds, after));
    }

    private IEnumerator AutoHideCo(float t, System.Action after)
    {
        yield return new WaitForSeconds(t);
        autoHideCo = null;
        after?.Invoke();
    }
}
