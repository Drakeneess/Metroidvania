using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProximityDetector : MonoBehaviour
{
    public static PlayerProximityDetector Instance;

    private Transform player;
    private List<Interactable> interactables = new List<Interactable>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        player = FindObjectOfType<Player>().transform;
    }

    public void RegisterInteractable(Interactable interactable)
    {
        if (!interactables.Contains(interactable))
            interactables.Add(interactable);
    }

    public void UnregisterInteractable(Interactable interactable)
    {
        if (interactables.Contains(interactable))
            interactables.Remove(interactable);
    }

    private void Update()
    {
        foreach (var interactable in interactables)
        {
            float distance = Vector3.Distance(player.position, interactable.transform.position);
            bool isInRange = distance <= interactable.distanceToPlayer;

            interactable.UpdateProximity(isInRange);
        }
    }
}
