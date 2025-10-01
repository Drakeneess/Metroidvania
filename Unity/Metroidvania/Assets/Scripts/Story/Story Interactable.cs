using UnityEngine;
using UnityEngine.Events;

public class StoryInteractable : Interactable
{
    [Header("Historia")]
    public UnityEvent onInteract;

    protected override void Action()
    {
        base.Action();
        onInteract?.Invoke();
    }
}
