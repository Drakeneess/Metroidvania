using System;
using System.Collections;
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

    void OnEnable()
    {
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnActionTriggered += OnOptionDialogueSelect;
            InputActionController.Instance.OnFloatInput += OnOptionDialogueNavigate;
        }
    }

    private void OnOptionDialogueSelect(string action)
    {
        if (action == "OptionSelect")
        {
            // Lógica para seleccionar una opción
            if (activeOption != null)
            {
                activeOption.PressButton();
            }
        }
    }

    private void OnOptionDialogueNavigate(string action, float value)
    {
        if (action == "OptionMovement")
        {
            if(value!=0){
                int direction = value > 0 ? 1 : -1;
                ChangeOptionSelect(direction);
            }
        }
    }

    void OnDisable()
    {
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnActionTriggered -= OnOptionDialogueSelect;
            InputActionController.Instance.OnFloatInput -= OnOptionDialogueNavigate;
        }
        if (options != null)
        {
            options.Clear();
        }
    }

    void Awake()
    {
        options = new List<OptionDialogueElement>();

        // Corrección del registro de eventos
        buttons[0].onClick.AddListener(() => ChangeOptionSelect(-1)); // Botón para moverse hacia arriba/izquierda
        buttons[1].onClick.AddListener(() => ChangeOptionSelect(1));  // Botón para moverse hacia abajo/derecha
    }

    /// <summary>
    /// Instancia las opciones de diálogo y oculta todas excepto la primera.
    /// </summary>
    public void SetOptions(List<LocalizedDecision> decisions)
    {
        DialogueSystem.IsOptionActive = true;
        foreach (var decision in decisions)
        {
            GameObject optionDialogue = Instantiate(optionDialogueElementPrefab, panel);
            optionDialogue.transform.localScale = optionDialogueElementPrefab.transform.localScale;

            OptionDialogueElement optionDialogueElement = optionDialogue.GetComponent<OptionDialogueElement>();
            optionDialogueElement.SetButton(decision, gameObject);

            options.Add(optionDialogueElement);
        }

        // Mostrar solo la primera opción
        currentIndex = 0;
        activeOption = options[currentIndex];
        UpdateActiveOption();

        for(int i = 1; i < options.Count; i++){
            options[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Cambia la opción activa basada en la dirección de navegación.
    /// </summary>
    private void ChangeOptionSelect(int direction)
    {
        if (options.Count == 0) return;

        // Ocultar la opción actual
        options[currentIndex].gameObject.SetActive(false);

        // Cambiar el índice
        currentIndex += direction;

        // Asegurar que el índice no salga de los límites
        currentIndex = currentIndex<0? options.Count-1: currentIndex;
        currentIndex = currentIndex>=options.Count? 0: currentIndex;

        // Activar la nueva opción
        UpdateActiveOption();
    }

    /// <summary>
    /// Activa la opción actual y actualiza la referencia de `activeOption`.
    /// </summary>
    private void UpdateActiveOption()
    {
        if (options.Count > 0)
        {
            options[currentIndex].gameObject.SetActive(true);
            activeOption = options[currentIndex];

            activeOption.Button.Select();
        }
    }
}
