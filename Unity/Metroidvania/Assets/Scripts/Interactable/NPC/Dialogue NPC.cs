using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DialogueNPC : Interactable
{
    [SerializeField]
    public NPCname characterDialogue;
    public int dialogueIndex = 0;

    private string characterName;

    protected override void Start()
    {
        base.Start();
        characterName = characterDialogue.ToString();
    }
    
    protected override void Action()
    {
        base.Action();
        string[] dialogues = DialogueLoader.GetDialogue(characterName,dialogueIndex);

        DialogueSystem.Instance.StartDialogue(characterName,dialogues);  // Usamos el Singleton
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

