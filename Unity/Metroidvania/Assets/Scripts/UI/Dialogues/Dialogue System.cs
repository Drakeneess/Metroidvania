using System.Collections;
using UnityEngine;
using TMPro; // Usando TextMeshPro

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance { get; private set; } // Propiedad estática para la instancia
    public static bool IsDialogueActive { get; private set; } // Propiedad estática para saber si el di
    public TextMeshProUGUI dialogueText; // Referencia al texto del diálogo
    public TextMeshProUGUI nameText; // Referencia al texto del nombre del personaje
    public GameObject dialoguePanel; // Referencia al panel que contiene el diálogo
    public float typingSpeed = 0.05f; // Velocidad con la que se escribe el texto

    private string[] dialogues; // Array para almacenar las líneas de diálogo
    private int currentDialogueIndex = 0; // Índice actual del diálogo

    // Asegurarse de que solo haya una instancia del sistema de diálogo
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destruye este objeto si ya existe una instancia
        }
        else
        {
            Instance = this; // Asigna la instancia
        }
    }

    private void Start()
    {
        EndDialogue();
    }

    private void Update()
    {
        
    }

    // Comienza el diálogo
    public void StartDialogue(string name, string[] dialogues)
    {
        this.dialogues = dialogues;
        currentDialogueIndex = 0;
        nameText.text = name; // Establece el nombre del personaje
        dialoguePanel.SetActive(true);
        GameMenuController.InGame = false;
        ShowNextDialogue();
    }

    // Muestra el siguiente diálogo
    public void ShowNextDialogue()
    {
        if (currentDialogueIndex < dialogues.Length)
        {
            StopAllCoroutines(); // Detener cualquier corutina anterior
            StartCoroutine(TypeSentence(dialogues[currentDialogueIndex]));
            currentDialogueIndex++;
        }
        else
        {
            EndDialogue();
        }
    }

    // Corutina para escribir el texto lentamente
    private IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    // Finaliza el diálogo
    private void EndDialogue()
    {
        dialoguePanel.SetActive(false); // Cierra el panel de diálogo
        GameMenuController.InGame = true;
    }
}
