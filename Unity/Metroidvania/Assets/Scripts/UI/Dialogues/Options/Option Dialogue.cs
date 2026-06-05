using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionDialogue : MonoBehaviour
{
    public Button[] buttons;
    public Transform panel;
    public GameObject optionDialogueElementPrefab;

    private List<OptionDialogueElement> options;
    private OptionDialogueElement activeOption;
    private int currentIndex;

    void Awake()
    {
        options = new List<OptionDialogueElement>();

        if (buttons != null && buttons.Length >= 2)
        {
            buttons[0].onClick.AddListener(() => ChangeOptionSelect(-1)); // Arriba/Izquierda
            buttons[1].onClick.AddListener(() => ChangeOptionSelect(1));  // Abajo/Derecha
        }
    }

    void OnEnable()
    {
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnActionTriggered += OnOptionDialogueSelect;
            InputActionController.Instance.OnFloatInput += OnOptionDialogueNavigate;
        }
    }

    void OnDisable()
    {
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnActionTriggered -= OnOptionDialogueSelect;
            InputActionController.Instance.OnFloatInput -= OnOptionDialogueNavigate;
        }

        // 🧹 Limpieza segura sin afectar layout
        ClearOptions();
    }

    private void OnOptionDialogueSelect(InputActionType action)
    {
        if (action == InputActionType.OptionSelect && activeOption != null)
            activeOption.PressButton();
    }

    private void OnOptionDialogueNavigate(InputActionType action, float value)
    {
        if (action == InputActionType.OptionMovement && value != 0)
        {
            int direction = value > 0 ? 1 : -1;
            ChangeOptionSelect(direction);
        }
    }

    /// <summary>
    /// Instancia nuevas opciones limpiando las previas sin tocar el layout.
    /// </summary>
    public void SetOptions(List<LocalizedDecision> decisions)
    {
        DialogueSystem.IsOptionActive = true;

        ClearOptions();

        currentIndex = 0;
        activeOption = null;

        foreach (var decision in decisions)
        {
            GameObject optionObj = Instantiate(optionDialogueElementPrefab, panel);

            // ✅ NO cambiamos scale ni forzamos layout
            // optionObj.transform.localScale = Vector3.one;  // ❌ No se usa para evitar alterar diseño

            var element = optionObj.GetComponent<OptionDialogueElement>();
            element.SetButton(decision, gameObject);

            options.Add(element);
        }

        if (options.Count > 0)
        {
            activeOption = options[0];
            UpdateActiveOption();

            for (int i = 1; i < options.Count; i++)
                options[i].gameObject.SetActive(false);
        }
    }

    private void ChangeOptionSelect(int direction)
    {
        if (options.Count == 0) return;

        options[currentIndex].gameObject.SetActive(false);

        currentIndex += direction;

        if (currentIndex < 0) currentIndex = options.Count - 1;
        if (currentIndex >= options.Count) currentIndex = 0;

        UpdateActiveOption();
    }

    private void UpdateActiveOption()
    {
        if (options.Count > 0)
        {
            options[currentIndex].gameObject.SetActive(true);
            activeOption = options[currentIndex];

            activeOption.Button.Select();
        }
    }

    /// <summary>
    /// Limpia los botones sin afectar layout original.
    /// </summary>
    private void ClearOptions()
    {
        if (options != null && options.Count > 0)
        {
            foreach (var opt in options)
            {
                if (opt != null && opt.gameObject != null)
                    Destroy(opt.gameObject);
            }
            options.Clear();
        }
    }
}
