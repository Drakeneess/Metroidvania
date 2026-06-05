using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controlador de acciones de entrada, manejando eventos de manera optimizada.
/// </summary>
public class InputActionController : MonoBehaviour
{
    public static InputActionController Instance { get; private set; }
    private Input inputActions;

    // Buffer de inputs
    private Dictionary<InputActionType, float> inputBuffer = new Dictionary<InputActionType, float>();
    private float bufferTime = 0.1f; // Tiempo máximo en el buffer

    // Evento único para manejar todas las acciones
    public event Action<InputActionType> OnActionTriggered;
    public event Action<InputActionType, Vector2> OnVector2Input;
    public event Action<InputActionType, float> OnFloatInput;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (InputController.instance == null)
        {
            Debug.LogError("InputController instance is null!");
            return;
        }

        inputActions = InputController.instance.InputActions;
        SubscribeToInput();
    }

    /// <summary>
    /// Suscribe las entradas a los eventos correspondientes.
    /// </summary>
    private void SubscribeToInput()
    {
        if (inputActions == null) return;

        // Asignación genérica de acciones de entrada con nombre
        inputActions.Game.Movement.performed += ctx => OnFloatInput?.Invoke(InputActionType.Movement, ctx.ReadValue<float>());
        inputActions.Game.Movement.canceled += _ => OnFloatInput?.Invoke(InputActionType.Movement, 0f);
        inputActions.Game.Run.performed += ctx => OnFloatInput?.Invoke(InputActionType.Run, ctx.ReadValue<float>());
        inputActions.Game.Run.canceled += _ => OnFloatInput?.Invoke(InputActionType.Run, 0f);
        inputActions.Game.Interact.performed += ctx => OnFloatInput?.Invoke(InputActionType.OnInteractHold, ctx.ReadValue<float>());
        inputActions.Game.Interact.canceled += _ => OnFloatInput?.Invoke(InputActionType.OnInteractHold, 0f);
        inputActions.Game.HeavyAttack.performed += ctx => OnFloatInput?.Invoke(InputActionType.HeavyAttack, ctx.ReadValue<float>());
        inputActions.Game.HeavyAttack.canceled += _ => OnFloatInput?.Invoke(InputActionType.HeavyAttack, 0f);
        inputActions.Game.Block.performed += ctx => OnFloatInput?.Invoke(InputActionType.Block, ctx.ReadValue<float>());
        inputActions.Game.Block.canceled += _ => OnFloatInput?.Invoke(InputActionType.Block, 0f);
        inputActions.Game.ChangeWeapon.performed += ctx => OnVector2Input?.Invoke(InputActionType.ChangeWeapon, ctx.ReadValue<Vector2>());

        inputActions.Dialogue.Navigate.performed += ctx => OnFloatInput?.Invoke(InputActionType.OptionMovement, ctx.ReadValue<float>());
        inputActions.Dialogue.Navigate.canceled += _ => OnFloatInput?.Invoke(InputActionType.OptionMovement, 0f);

        inputActions.Menu.Navigation.performed += ctx => OnVector2Input?.Invoke(InputActionType.Navigation, ctx.ReadValue<Vector2>());
        inputActions.Menu.Navigation.canceled += ctx => OnVector2Input?.Invoke(InputActionType.Navigation, Vector2.zero);

        inputActions.ToolMenu.Rotate.performed += ctx => OnVector2Input?.Invoke(InputActionType.RotateTool, ctx.ReadValue<Vector2>());
        inputActions.ToolMenu.Rotate.canceled += _ => OnVector2Input?.Invoke(InputActionType.RotateTool, Vector2.zero);

        inputActions.Map.Zoom.performed += ctx => OnFloatInput?.Invoke(InputActionType.MapZoom, ctx.ReadValue<float>());
        inputActions.Map.Zoom.canceled += _ => OnFloatInput?.Invoke(InputActionType.MapZoom, 0f);
        inputActions.Map.Navigation.performed += ctx => OnVector2Input?.Invoke(InputActionType.MapNavigation, ctx.ReadValue<Vector2>());
        inputActions.Map.Navigation.canceled += _ => OnVector2Input?.Invoke(InputActionType.MapNavigation, Vector2.zero);

        // Acciones con buffer
        RegisterBufferedAction(inputActions.Game.Jump, InputActionType.Jump);
        RegisterBufferedAction(inputActions.Game.Dash, InputActionType.Dash);
        RegisterBufferedAction(inputActions.Game.LightAttack, InputActionType.LightAttack);
        RegisterBufferedAction(inputActions.Game.Interact, InputActionType.InteractPressed);
        RegisterBufferedAction(inputActions.Game.Map, InputActionType.Map);
        RegisterBufferedAction(inputActions.Game.Cure, InputActionType.Cure);
        RegisterBufferedAction(inputActions.Game.Pause, InputActionType.Pause);

        RegisterBufferedAction(inputActions.Menu.Select, InputActionType.Select);
        RegisterBufferedAction(inputActions.Menu.Back, InputActionType.Back);
        RegisterBufferedAction(inputActions.Menu.PAButton, InputActionType.PAButton);

        RegisterBufferedAction(inputActions.Dialogue.Select, InputActionType.OptionSelect);

        RegisterBufferedAction(inputActions.ToolMenu.Select, InputActionType.ToolSelect);

        RegisterBufferedAction(inputActions.Map.Close, InputActionType.CloseMap);
    }

    /// <summary>
    /// Agrega una acción al buffer cuando se presiona.
    /// </summary>
    private void RegisterBufferedAction(InputAction action, InputActionType actionName)
    {
        action.performed += _ => AddToBuffer(actionName);
    }

    private void Update()
    {
        ProcessInputBuffer();
    }

    /// <summary>
    /// Agrega una acción al buffer.
    /// </summary>
    private void AddToBuffer(InputActionType action)
    {
        inputBuffer[action] = Time.time + bufferTime;
    }

    /// <summary>
    /// Procesa el buffer de acciones y las ejecuta si han transcurrido.
    /// </summary>
    private void ProcessInputBuffer()
    {
        List<InputActionType> keysToRemove = new List<InputActionType>();

        foreach (var input in inputBuffer)
        {
            if (Time.time >= input.Value)
            {
                OnActionTriggered?.Invoke(input.Key);
                keysToRemove.Add(input.Key);
            }
        }

        foreach (var key in keysToRemove)
        {
            inputBuffer.Remove(key);
        }
    }

    private void OnDestroy()
    {
        if (inputActions == null) return;

        inputActions.Game.Movement.performed -= ctx => OnFloatInput?.Invoke(InputActionType.Movement, ctx.ReadValue<float>());
        inputActions.Game.Movement.canceled -= _ => OnFloatInput?.Invoke(InputActionType.Movement, 0f);
        inputActions.Game.Run.performed -= ctx => OnFloatInput?.Invoke(InputActionType.Run, ctx.ReadValue<float>());
        inputActions.Game.Run.canceled -= _ => OnFloatInput?.Invoke(InputActionType.Run, 0f);
        inputActions.Game.Interact.performed -= ctx => OnFloatInput?.Invoke(InputActionType.OnInteractHold, ctx.ReadValue<float>());
        inputActions.Game.Interact.canceled -= _ => OnFloatInput?.Invoke(InputActionType.OnInteractHold, 0f);
        inputActions.Game.HeavyAttack.performed -= ctx => OnFloatInput?.Invoke(InputActionType.HeavyAttack, ctx.ReadValue<float>());
        inputActions.Game.HeavyAttack.canceled -= _ => OnFloatInput?.Invoke(InputActionType.HeavyAttack, 0f);
        inputActions.Game.Block.performed -= ctx => OnFloatInput?.Invoke(InputActionType.Block, ctx.ReadValue<float>());
        inputActions.Game.Block.canceled -= _ => OnFloatInput?.Invoke(InputActionType.Block, 0f);

        inputActions.Dialogue.Navigate.performed -= ctx => OnFloatInput?.Invoke(InputActionType.OptionMovement, ctx.ReadValue<float>());
        inputActions.Dialogue.Navigate.canceled -= _ => OnFloatInput?.Invoke(InputActionType.OptionMovement, 0f);

        inputActions.Menu.Navigation.performed -= ctx => OnVector2Input?.Invoke(InputActionType.Navigation, ctx.ReadValue<Vector2>());
        inputActions.Menu.Navigation.canceled -= ctx => OnVector2Input?.Invoke(InputActionType.Navigation, Vector2.zero);

        inputActions.ToolMenu.Rotate.performed -= ctx => OnVector2Input?.Invoke(InputActionType.RotateTool, ctx.ReadValue<Vector2>());
        inputActions.ToolMenu.Rotate.canceled -= _ => OnVector2Input?.Invoke(InputActionType.RotateTool, Vector2.zero);

        inputActions.Map.Zoom.performed -= ctx => OnFloatInput?.Invoke(InputActionType.MapZoom, ctx.ReadValue<float>());
        inputActions.Map.Zoom.canceled -= _ => OnFloatInput?.Invoke(InputActionType.MapZoom, 0f);
        inputActions.Map.Navigation.performed -= ctx => OnVector2Input?.Invoke(InputActionType.MapNavigation, ctx.ReadValue<Vector2>());
        inputActions.Map.Navigation.canceled -= _ => OnVector2Input?.Invoke(InputActionType.MapNavigation, Vector2.zero);

        // Desuscribimos todas las acciones bufferizadas
        UnregisterBufferedAction(inputActions.Game.Jump, InputActionType.Jump);
        UnregisterBufferedAction(inputActions.Game.Dash, InputActionType.Dash);
        UnregisterBufferedAction(inputActions.Game.LightAttack, InputActionType.LightAttack);
        UnregisterBufferedAction(inputActions.Game.Interact, InputActionType.InteractPressed);
        UnregisterBufferedAction(inputActions.Game.Map, InputActionType.Map);
        UnregisterBufferedAction(inputActions.Game.Cure, InputActionType.Cure);
        UnregisterBufferedAction(inputActions.Game.Pause, InputActionType.Pause);

        UnregisterBufferedAction(inputActions.Menu.Select, InputActionType.Select);
        UnregisterBufferedAction(inputActions.Menu.Back, InputActionType.Back);
        UnregisterBufferedAction(inputActions.Menu.PAButton, InputActionType.PAButton);

        UnregisterBufferedAction(inputActions.Dialogue.Select, InputActionType.OptionSelect);

        UnregisterBufferedAction(inputActions.ToolMenu.Select, InputActionType.ToolSelect);
        
        UnregisterBufferedAction(inputActions.Map.Close, InputActionType.CloseMap);
    }

    // 🔹 Permite que otros (como la UI) agreguen acciones al buffer
    public void EnqueueAction(InputActionType actionName)
    {
        AddToBuffer(actionName);
    }
    public void InvokeFloatInput(InputActionType actionName, float value)
    {
        OnFloatInput?.Invoke(actionName, value);
    }
    public void InvokeVector2Input(InputActionType actionName, Vector2 value)
    {
        OnVector2Input?.Invoke(actionName, value);
    }

    public void InvokeBufferedAction(InputActionType actionName)
    {
        AddToBuffer(actionName);
    }


    /// <summary>
    /// Desuscribe una acción registrada en el buffer.
    /// </summary>
    private void UnregisterBufferedAction(InputAction action, InputActionType actionName)
    {
        action.performed -= _ => AddToBuffer(actionName);
    }
}
