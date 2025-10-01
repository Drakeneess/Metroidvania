using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueSystem : TypewriterPanelBase
{
    public static DialogueSystem Instance { get; private set; } 
    public static bool IsDialogueActive { get; private set; } 

    public delegate void OnLetterTypedDelegate(char c);
    public static event OnLetterTypedDelegate OnLetterTyped;

    [Header("UI extra")]
    public TextMeshProUGUI nameText;
    public OptionDialogue optionDialogue;

    private DialogueBlip[] allBlips;
    private Dialogue currentDialogue; 
    private string[] dialogues; 
    private int currentDialogueIndex = 0;

    private static bool isOptionActive = false;
    public static bool IsOptionActive { get => isOptionActive; set => isOptionActive = value; } 

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

    private void Start()
    {
        allBlips = FindObjectsOfType<DialogueBlip>();
    }

    public void StartDialogue(string name, Dialogue dialogue)
    {
        currentDialogue = dialogue;
        dialogues = dialogue.GetLocalizedText();
        currentDialogueIndex = 0;

        if (nameText) nameText.text = name;

        PanelManager.Instance?.ShowDialogue(); // 🔹 usa el manager
        GameMenuController.CurrentMode = GameMode.Selection;
        IsDialogueActive = true;

        ShowNextDialogue();
    }

    public void ShowNextDialogue()
    {
        if (IsOptionActive) return; 

        if (currentDialogueIndex < (dialogues?.Length ?? 0))
        {
            string sentence = dialogues[currentDialogueIndex];

            if (sentence.StartsWith("[") && sentence.EndsWith("]"))
            {
                sentence = sentence.Substring(1, sentence.Length - 2);
                TypeText(sentence);

                List<LocalizedDecision> decisions = currentDialogue.GetLocalizedDecisions();
                if (decisions.Count > 0)
                {
                    ShowDecisions(decisions);
                    currentDialogueIndex++;
                    return; 
                }
            }
            else
            {
                TypeText(sentence);
                currentDialogueIndex++;
            }
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        if (allBlips != null)
        {
            foreach (var blip in allBlips)
                blip.SetActive(false);
        }

        PanelManager.Instance?.HideAll(); // 🔹 apaga todo
        GameMenuController.CurrentMode = GameMode.Game;
        IsDialogueActive = false;

        var extras = new List<string> { $"Dialogue: {currentDialogue?.idConversation ?? "Unknown"}" };
        PlayerActionLogger.Instance.Log("EndInteraction", extras);
    }

    public void ShowDecisions(List<LocalizedDecision> decisions)
    {
            optionDialogue.gameObject.SetActive(true);
            optionDialogue.SetOptions(decisions);
        }
    

    protected override void OnLanguageChanged()
    {
        if (currentDialogue != null && isShowing)
        {
            dialogues = currentDialogue.GetLocalizedText();

            if (currentDialogueIndex < dialogues.Length)
            {
                string line = dialogues[currentDialogueIndex];
                TypeText(line);
            }
        }
    }

    protected override void OnCharTyped(char c)
    {
        OnLetterTyped?.Invoke(c);
    }
}
