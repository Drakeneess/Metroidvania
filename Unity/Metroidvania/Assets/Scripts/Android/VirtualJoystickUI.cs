using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystickUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Visual")]
    public RectTransform background;
    public RectTransform handle;

    [Header("Acciones que representa")]
    public List<InputActionType> actions = new();

    [Header("Radio máximo")]
    public float radius = 80f;

    private Vector2 inputVector;

    public void OnPointerDown(PointerEventData eventData) => ProcessDrag(eventData);

    public void OnDrag(PointerEventData eventData) => ProcessDrag(eventData);

    private void ProcessDrag(PointerEventData eventData)
    {
        Vector2 pos;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            eventData.position,
            eventData.pressEventCamera,
            out pos)) return;

        // 👉 Calcula dirección real desde el centro
        Vector2 direction = pos;

        // Limita al radio máximo
        direction = Vector2.ClampMagnitude(direction, radius);

        // Mueve el handle
        handle.anchoredPosition = direction;

        // Normaliza a rango -1..1
        inputVector = direction / radius;

        SendInput(inputVector);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        handle.anchoredPosition = Vector2.zero;
        inputVector = Vector2.zero;
        SendInput(Vector2.zero);
    }

    private void SendInput(Vector2 value)
    {
        if (InputActionController.Instance == null) return;

        foreach (var action in actions)
        {
            if (action == InputActionType.None) continue;
            if (!IsActionAllowed(action)) continue;

            // Vector2 completo
            InputActionController.Instance.InvokeVector2Input(action, value);

            // ✅ Neutral real al centro
            if (value.magnitude < 0.15f)
            {
                InputActionController.Instance.InvokeFloatInput(action, 0f);
                continue;
            }

            float signedValue;

            if (Mathf.Abs(value.x) > Mathf.Abs(value.y))
                signedValue = Mathf.Sign(value.x);
            else
                signedValue = Mathf.Sign(value.y);

            InputActionController.Instance.InvokeFloatInput(action, signedValue);
        }
    }

    private bool IsActionAllowed(InputActionType action)
    {
        return GameMenuController.CurrentMode switch
        {
            GameMode.Game      => action.IsGameAction(),
            GameMode.Menu      => action.IsMenuAction(),
            GameMode.Selection => action.IsDialogueAction(),
            GameMode.ToolMenu  => action.IsToolAction(),
            GameMode.MapMenu   => action.IsMapAction(),
            _ => false
        };
    }
}
