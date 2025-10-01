using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ThoughtSystem : TypewriterPanelBase
{
    public static ThoughtSystem Instance { get; private set; }

    private string currentThoughtId;

    protected override void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        base.Awake();
    }

    public void Show(string thoughtId, bool withTypewriter = true)
    {
        currentThoughtId = thoughtId;
        currentFullText = ThoughtsLoader.Instance.GetText(currentThoughtId);

        if (!string.IsNullOrEmpty(currentFullText))
        {
            PanelManager.Instance?.ShowThought(); // 🔹 usa el manager
            GameMenuController.CurrentMode = GameMode.Selection;

            if (withTypewriter && typingSpeed > 0f)
            {
                if (typingCo != null) StopCoroutine(typingCo);
                typingCo = StartCoroutine(TypewriterCo(currentFullText, null));
            }
            else
            {
                if (textUI) textUI.text = currentFullText;
            }
        }
        else
        {
            Debug.LogWarning($"[ThoughtSystem] No se encontró texto para id '{thoughtId}'");
        }
    }

    public override void ForceCompleteOrHide()
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
            PanelManager.Instance?.HideAll(); // 🔹 apaga todo
            currentThoughtId = null;
            GameMenuController.CurrentMode = GameMode.Game;
            var extras = new List<string> { $"ThoughtId: {currentThoughtId}" };
            PlayerActionLogger.Instance.Log("EndInteraction", extras);
        }
    }

    protected override void OnCharTyped(char c) { }

    protected override void OnLanguageChanged()
    {
        if (!isShowing || string.IsNullOrEmpty(currentThoughtId)) return;

        currentFullText = ThoughtsLoader.Instance.GetText(currentThoughtId);

        if (typingCo != null)
        {
            StopCoroutine(typingCo);
            typingCo = null;
        }

        if (textUI) textUI.text = currentFullText;
    }
}
