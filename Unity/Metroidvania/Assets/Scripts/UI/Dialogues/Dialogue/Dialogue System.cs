using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance { get; private set; } 
    public static bool IsDialogueActive { get; private set; } 
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI nameText;
    public GameObject dialoguePanel;
    public float typingSpeed = 0.05f;
    public OptionDialogue optionDialogue;

    private Dialogue currentDialogue; // Almacena el diálogo actual
    private string[] dialogues; 
    private int currentDialogueIndex = 0;
    
    private static bool isOptionActive = false;
    public static bool IsOptionActive { get => isOptionActive; set => isOptionActive = value; } 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        EndDialogue();
    }

    /// <summary>
    /// Comienza un nuevo diálogo y muestra el primer mensaje.
    /// </summary>
    public void StartDialogue(string name, Dialogue dialogue)
    {
        currentDialogue = dialogue;
        dialogues = dialogue.GetLocalizedText(DialogueLoader.Instance.currentLanguage);
        currentDialogueIndex = 0;
        
        nameText.text = name;
        dialoguePanel.SetActive(true);
        GameMenuController.CurrentMode = GameMode.Selection;
        
        ShowNextDialogue();
    }

    
    /// <summary>
    /// Muestra el siguiente mensaje del diálogo o termina el diálogo si no hay más mensajes.
    /// </summary>
    public void ShowNextDialogue()
    {
        if (IsOptionActive) return; // No avanzar si las opciones están activas

        if (currentDialogueIndex < dialogues.Length)
        {
            string sentence = dialogues[currentDialogueIndex];

            // Verificar si el texto es una pregunta con opciones (marcador [])
            if (sentence.StartsWith("[") && sentence.EndsWith("]"))
            {
                // Mostrar el texto sin los corchetes
                sentence = sentence.Substring(1, sentence.Length - 2);
                StopAllCoroutines();
                StartCoroutine(TypeSentence(sentence));

                // Obtener las decisiones y mostrarlas
                List<LocalizedDecision> decisions = currentDialogue.GetLocalizedDecisions(DialogueLoader.Instance.currentLanguage);
                if (decisions.Count > 0)
                {
                    ShowDecisions(decisions);
                    currentDialogueIndex++;
                    return; // No continuar con el siguiente diálogo hasta que se elija una opción
                }
                print(currentDialogueIndex);
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine(TypeSentence(sentence));
                currentDialogueIndex++;
            }
        }
        else
        {
            EndDialogue();
        }
    }

    /// <summary>
    /// Escribe el texto del diálogo letra por letra.
    /// </summary>
    private IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    /// <summary>
    /// Finaliza el diálogo y oculta el panel.
    /// </summary>
    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        GameMenuController.CurrentMode = GameMode.Game;
    }

    /// <summary>
    /// Muestra las opciones de decisión si están disponibles.
    /// </summary>
    public void ShowDecisions(List<LocalizedDecision> decisions)
    {
        optionDialogue.gameObject.SetActive(true);
        optionDialogue.SetOptions(decisions);
    }
}
