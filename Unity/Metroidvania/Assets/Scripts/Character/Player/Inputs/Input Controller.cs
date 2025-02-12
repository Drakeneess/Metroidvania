using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

public class InputController : MonoBehaviour
{
    public static InputController instance { get; private set; }
    public static event Action<int> OnControlSchemeChanged; // Evento para notificar cambios de esquema

    private Input inputActions;
    public Input InputActions => inputActions;
    private static int currentScheme = 3;
    private InputDevice currentDevice;

    public static int CurrentScheme => currentScheme;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            inputActions = new Input();
            EnableInputActions();  // Llamamos a un método para habilitar las acciones
        }
        else
        {
            Destroy(gameObject);
        }

        // Suscripción al cambio de acción de entrada
        InputSystem.onActionChange += OnInputActionChange;
    }

    private void OnDestroy()
    {
        DisableInputActions();  // Deshabilitamos las acciones al destruir el objeto
        InputSystem.onActionChange -= OnInputActionChange;
    }

    private void EnableInputActions()
    {
        inputActions.Game.Enable();
        inputActions.Menu.Enable();
    }

    private void DisableInputActions()
    {
        inputActions.Game.Disable();
        inputActions.Menu.Disable();
    }

    private void OnInputActionChange(object obj, InputActionChange change)
    {
        if (obj is InputAction action && change == InputActionChange.ActionPerformed)
        {
            var device = action.activeControl?.device;
            if (device == null) return;  // Si el dispositivo es nulo, no hacemos nada

            currentDevice = device;
            int previousScheme = currentScheme;

            UpdateControlScheme(device);

            // Notificar solo si el esquema realmente cambió
            if (previousScheme != currentScheme)
            {
                OnControlSchemeChanged?.Invoke(currentScheme);
            }
        }
    }

    private void UpdateControlScheme(InputDevice device)
    {
        // Detectar el dispositivo activo y actualizar el esquema
        if (device is Keyboard || device is Mouse)
        {
            currentScheme = 0; // Esquema: Teclado y ratón
            SetCursorState(false);
        }
        else if (device is DualSenseGamepadHID)
        {
            currentScheme = 1; // Esquema: DualSense
            SetCursorState(true);
        }
        else if (device is Gamepad)
        {
            currentScheme = 2; // Esquema: Otros Gamepads
            SetCursorState(true);
        }
        else
        {
            currentScheme = -1; // Esquema desconocido
        }
    }

    private void SetCursorState(bool lockCursor)
    {
        Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !lockCursor;
    }

    // Obtener el dispositivo actual (opcional)
    public static string GetCurrentDeviceName() => instance.currentDevice?.displayName ?? "Dispositivo desconocido";
}
