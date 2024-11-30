using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

public class InputController : MonoBehaviour
{
    public static InputController instance { get; private set; }
    public static event Action<int> OnControlSchemeChanged; // Evento para notificar cambios de esquema

    private Input inputActions;
    private static int currentScheme = 0;
    public static int CurrentScheme { get { return currentScheme; } }
    private InputDevice currentDevice;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            inputActions = new Input();
            inputActions.Game.Enable();
        }
        else
        {
            Destroy(gameObject);
        }

        // Suscripción al cambio de acción de entrada
        InputSystem.onActionChange += OnInputActionChange;      
    }

    public static float GetMovement()
    {
        return instance.inputActions?.Game.Movement?.ReadValue<float>() ?? 0f;
    }

    public static bool GetJump()
    {
        return instance.inputActions?.Game.Jump?.triggered ?? false;
    }

    public static bool GetInteractPressed()
    {
        return instance.inputActions?.Game.Interact?.triggered ?? false;
    }

    public static float GetInteractHold()
    {
        return instance.inputActions?.Game.Interact?.ReadValue<float>() ?? 0f;
    }

    private void OnDestroy()
    {
        inputActions?.Game.Disable();
        InputSystem.onActionChange -= OnInputActionChange; // Desuscripción al evento
    }

    private void OnInputActionChange(object obj, InputActionChange change)
    {
        if (obj is InputAction action && change == InputActionChange.ActionPerformed)
        {
            if (action?.activeControl?.device == null)
            {
                Debug.LogWarning("El dispositivo activo es nulo");
                return; // Si el dispositivo es nulo, no hacemos nada
            }

            var device = action.activeControl.device;
            currentDevice = device;

            // Ignorar movimientos pequeños del mouse (opcional)
            if (device is Mouse && action.name == "Aim Mouse")
            {
                return; // Evitar cambios innecesarios
            }

            // Verificar si el esquema de control realmente ha cambiado
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
            SetCursorState(false); // Cursor desbloqueado
        }
        else if (device is DualSenseGamepadHID)
        {
            currentScheme = 1; // Esquema: DualSense
            SetCursorState(true); // Cursor bloqueado
        }
        else if (device is Gamepad)
        {
            currentScheme = 2; // Esquema: Otros Gamepads
            SetCursorState(true); // Cursor bloqueado
        }
        else
        {
            currentScheme = -1; // Esquema desconocido
        }
    }

    // Método para bloquear o desbloquear el cursor según el esquema de control
    private void SetCursorState(bool lockCursor)
    {
        Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !lockCursor;
    }

    // Obtener el dispositivo actual (opcional)
    public static string GetCurrentDeviceName()
    {
        return instance.currentDevice?.displayName ?? "Dispositivo desconocido";
    }
}
