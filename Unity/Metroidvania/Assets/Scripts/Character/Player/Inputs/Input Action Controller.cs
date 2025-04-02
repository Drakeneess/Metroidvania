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
    private Dictionary<string, float> inputBuffer = new Dictionary<string, float>();
    private float bufferTime = 0.1f; // Tiempo máximo en el buffer

    // Evento único para manejar todas las acciones
    public event Action<string> OnActionTriggered;
    public event Action<string, Vector2> OnVector2Input;
    public event Action<string, float> OnFloatInput;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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
        inputActions.Game.Movement.performed += ctx => OnFloatInput?.Invoke("Movement", ctx.ReadValue<float>());
        inputActions.Game.Movement.canceled += _ => OnFloatInput?.Invoke("Movement", 0f);
        inputActions.Game.Run.performed += ctx => OnFloatInput?.Invoke("Run", ctx.ReadValue<float>());
        inputActions.Game.Run.canceled += _ => OnFloatInput?.Invoke("Run", 0f);
        inputActions.Game.Interact.performed += ctx => OnFloatInput?.Invoke("OnInteractHold", ctx.ReadValue<float>());
        inputActions.Game.Interact.canceled += _ => OnFloatInput?.Invoke("OnInteractHold",0f);
        inputActions.Game.HeavyAttack.performed += ctx => OnFloatInput?.Invoke("HeavyAttack", ctx.ReadValue<float>());
        inputActions.Game.HeavyAttack.canceled += _ => OnFloatInput?.Invoke("HeavyAttack", 0f);
        inputActions.Game.ChangeWeapon.performed += ctx => OnVector2Input?.Invoke("ChangeWeapon",ctx.ReadValue<Vector2>());
        
        inputActions.Dialogue.Navigate.performed += ctx => OnFloatInput?.Invoke("OptionMovement", ctx.ReadValue<float>());
        inputActions.Dialogue.Navigate.canceled += _ => OnFloatInput?.Invoke("OptionMovement", 0f);

        inputActions.Menu.Navigation.performed += ctx => OnVector2Input?.Invoke("Navigation",ctx.ReadValue<Vector2>());

        inputActions.ToolMenu.Rotate.performed += ctx => OnVector2Input?.Invoke("Rotate", ctx.ReadValue<Vector2>());

        // Acciones con buffer
        RegisterBufferedAction(inputActions.Game.Jump, "Jump");
        RegisterBufferedAction(inputActions.Game.Dash, "Dash");
        RegisterBufferedAction(inputActions.Game.LightAttack, "LightAttack");
        RegisterBufferedAction(inputActions.Game.Interact, "InteractPressed");
        
        RegisterBufferedAction(inputActions.Menu.Select, "Select");
        RegisterBufferedAction(inputActions.Menu.Back, "Back");
        RegisterBufferedAction(inputActions.Menu.PAButton, "PAButton");
        
        RegisterBufferedAction(inputActions.Dialogue.Select, "OptionSelect");

        RegisterBufferedAction(inputActions.ToolMenu.Select, "ToolSelect");
    }

    /// <summary>
    /// Agrega una acción al buffer cuando se presiona.
    /// </summary>
    private void RegisterBufferedAction(InputAction action, string actionName)
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
    private void AddToBuffer(string action)
    {
        inputBuffer[action] = Time.time + bufferTime;
    }

    /// <summary>
    /// Procesa el buffer de acciones y las ejecuta si han transcurrido.
    /// </summary>
    private void ProcessInputBuffer()
    {
        List<string> keysToRemove = new List<string>();

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

        inputActions.Game.Movement.performed -= ctx => OnFloatInput?.Invoke("Movement", ctx.ReadValue<float>());
        inputActions.Game.Movement.canceled -= _ => OnFloatInput?.Invoke("Movement", 0f);
        inputActions.Game.Run.performed -= ctx => OnFloatInput?.Invoke("Run", ctx.ReadValue<float>());
        inputActions.Game.Run.canceled -= _ => OnFloatInput?.Invoke("Run", 0f);
        inputActions.Game.Interact.performed -= ctx => OnFloatInput?.Invoke("OnInteractHold", ctx.ReadValue<float>());
        inputActions.Game.Interact.canceled -= _ => OnFloatInput?.Invoke("OnInteractHold",0f);
        inputActions.Game.HeavyAttack.performed -= ctx => OnFloatInput?.Invoke("HeavyAttack", ctx.ReadValue<float>());
        inputActions.Game.HeavyAttack.canceled -= _ => OnFloatInput?.Invoke("HeavyAttack", 0f);
        
        inputActions.Dialogue.Navigate.performed -= ctx => OnFloatInput?.Invoke("OptionMovement", ctx.ReadValue<float>());
        inputActions.Dialogue.Navigate.canceled -= _ => OnFloatInput?.Invoke("OptionMovement", 0f);

        inputActions.Menu.Navigation.performed -= ctx => OnVector2Input?.Invoke("Navigation",ctx.ReadValue<Vector2>());

        inputActions.ToolMenu.Rotate.performed -= ctx => OnVector2Input?.Invoke("Rotate", ctx.ReadValue<Vector2>());

        // Desuscribimos todas las acciones bufferizadas
        UnregisterBufferedAction(inputActions.Game.Jump, "Jump");
        UnregisterBufferedAction(inputActions.Game.Dash, "Dash");
        UnregisterBufferedAction(inputActions.Game.LightAttack, "LightAttack");
        UnregisterBufferedAction(inputActions.Game.Interact, "InteractPressed");
        
        UnregisterBufferedAction(inputActions.Menu.Select, "Select");
        UnregisterBufferedAction(inputActions.Menu.Back, "Back");
        UnregisterBufferedAction(inputActions.Menu.PAButton, "PAButton");
        
        UnregisterBufferedAction(inputActions.Dialogue.Select, "OptionSelect");

        UnregisterBufferedAction(inputActions.ToolMenu.Select, "ToolSelect");
    }

    /// <summary>
    /// Desuscribe una acción registrada en el buffer.
    /// </summary>
    private void UnregisterBufferedAction(InputAction action, string actionName)
    {
        action.performed -= _ => AddToBuffer(actionName);
    }
}
