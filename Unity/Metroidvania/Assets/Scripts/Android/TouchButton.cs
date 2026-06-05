using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class VirtualActionButtonUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Acciones que este botón representa")]
    public List<InputActionType> actions = new();

    [Header("¿Es botón HOLD?")]
    public bool isHoldButton = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (InputActionController.Instance == null) return;

        foreach (var action in actions)
        {
            if (action == InputActionType.None) continue;
            if (!IsActionAllowed(action)) continue;

            if (isHoldButton)
            {
                InputActionController.Instance.InvokeFloatInput(action, 1f);
            }
            else
            {
                InputActionController.Instance.InvokeBufferedAction(action);
            }

            Debug.Log($"[VirtualAction] DOWN → {action}");
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isHoldButton) return;
        if (InputActionController.Instance == null) return;

        foreach (var action in actions)
        {
            if (action == InputActionType.None) continue;
            if (!IsActionAllowed(action)) continue;

            InputActionController.Instance.InvokeFloatInput(action, 0f);
            Debug.Log($"[VirtualAction] UP → {action}");
        }
    }

    private bool IsActionAllowed(InputActionType action)
    {
        return GameMenuController.CurrentMode switch
        {
            GameMode.Game => action.IsGameAction(),
            GameMode.Menu => action.IsMenuAction(),
            GameMode.Selection => action.IsDialogueAction(),
            GameMode.ToolMenu => action.IsToolAction(),
            GameMode.MapMenu => action.IsMapAction(),
            _ => false
        };
    }
}
