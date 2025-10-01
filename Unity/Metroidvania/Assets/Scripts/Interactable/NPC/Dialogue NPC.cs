using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DialogueNPC : Interactable
{
    [SerializeField]
    [Header("NPC")]
    public NPCname characterDialogue;
    public int dialogueIndex = 0;

    private string characterName;
    private string idCharacter;
    private DialogueBlip dialogueBlip;

    protected override void Start()
    {
        base.Start();
        dialogueBlip = GetComponent<DialogueBlip>();
        characterName = characterDialogue.ToString();
        idCharacter = ((int)characterDialogue).ToString(); // Obtener el índice como string
    }

    protected override void Action()
    {
        base.Action();

        var extras = new List<string>
        {
            $"NPC: {characterName}",
            $"DialogueIndex: {dialogueIndex}"
        };
        LogBegin(extras);
        BehaviorManager.Instance.AddSocial();
        // Obtener el objeto Dialogue
        Dialogue dialogue = DialogueLoader.GetDialogue(idCharacter, dialogueIndex);

        if (dialogue != null)
        {
            // Enviar el diálogo al sistema de diálogos
            DialogueSystem.Instance.StartDialogue(characterName, dialogue);// Mostrar decisiones si existen
            EmotionType emotion = dialogue.GetEmotionForLine(dialogueIndex);
            dialogueBlip.SetActive(true);
            dialogueBlip.SetEmotion(emotion);
            if (dialogueIndex == 1)
            {
                dialogueIndex++;
            }
        }
        else
        {
            Debug.LogWarning($"No se encontró un diálogo para el personaje '{characterName}' con el índice {dialogueIndex}.");
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
    }
}

public enum NPCname{
    NPC1,
    NPC2,
}


