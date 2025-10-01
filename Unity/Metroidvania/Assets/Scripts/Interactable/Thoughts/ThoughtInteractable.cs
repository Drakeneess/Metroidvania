using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interactable que lanza un pensamiento usando el ThoughtSystem.
/// </summary>
public class ThoughtInteractable : Interactable
{
    [Header("Thought Config")]
    [SerializeField] private string thoughtId;           // ID del pensamiento en el CSV
    [SerializeField] private bool withTypewriter = true; // mostrar con efecto de tipeo

    protected override void Action()
    {
        base.Action();
        var extras = new List<string>
        {
            $"Object: {name ?? "Unknown"}",
            $"Thought: {thoughtId ?? "Unknown"}",
            $"InteractionType: {interactionType}"
        };
        LogBegin(extras);

        if (ThoughtSystem.Instance != null && !string.IsNullOrEmpty(thoughtId))
        {
            ThoughtSystem.Instance.Show(thoughtId, withTypewriter);
        }
        else
        {
            Debug.LogWarning($"[{name}] No se pudo mostrar Thought. ThoughtSystem.Instance={ThoughtSystem.Instance}, thoughtId='{thoughtId}'");
        }
    }
}
