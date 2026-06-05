using System;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_STANDALONE || UNITY_EDITOR // Solo en PC/Editor
using UnityEngine.InputSystem.DualShock;
#endif

public class InputController : MonoBehaviour
{
    public static InputController instance { get; private set; }
    public static event Action<int> OnControlSchemeChanged;

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
            DontDestroyOnLoad(gameObject);
            inputActions = new Input();
        }

        InputSystem.onActionChange += OnInputActionChange;
    }


    private void OnDestroy()
    {
        InputSystem.onActionChange -= OnInputActionChange;
    }

    private void OnInputActionChange(object obj, InputActionChange change)
    {
        if (obj is InputAction action && change == InputActionChange.ActionPerformed)
        {
            var device = action.activeControl?.device;
            if (device == null) return;

            currentDevice = device;
            int previousScheme = currentScheme;

            UpdateControlScheme(device);

            if (previousScheme != currentScheme)
            {
                OnControlSchemeChanged?.Invoke(currentScheme);
            }
        }
    }

    private void UpdateControlScheme(InputDevice device)
    {
        // --- TECLADO + MOUSE ---
        if (device is Keyboard || device is Mouse)
        {
            currentScheme = 0;
            SetCursorState(false);
        }

        // --- DUALSENSE (SOLO EN PC/EDITOR) ---
#if UNITY_STANDALONE || UNITY_EDITOR
        else if (device is DualSenseGamepadHID)
        {
            currentScheme = 1;
            SetCursorState(true);
        }
#endif

        // --- GAMEPADS GENÉRICOS (Android incluye algunos aquí) ---
        else if (device is Gamepad)
        {
            currentScheme = 2;
            SetCursorState(true);
        }

        // --- OTROS DISPOSITIVOS ---
        else
        {
            currentScheme = -1;
        }
    }

    private void SetCursorState(bool lockCursor)
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !lockCursor;
#endif
        // 🔸 Android NO usa cursor → no hacemos nada allí.
    }

    public static string GetCurrentDeviceName() 
        => instance.currentDevice?.displayName ?? "Dispositivo desconocido";
}
