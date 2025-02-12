using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputActionController : MonoBehaviour
{
    public static InputActionController Instance { get; private set; }
    private Input inputActions;

    // Buffer de inputs
    private Dictionary<string, float> inputBuffer = new Dictionary<string, float>();
    private float bufferTime = 0.1f; // Tiempo máximo que el input queda en buffer

    // Eventos
    public event Action OnInteractPressed;
    public event Action<float> OnInteractHold;
    public event Action<float> OnMovement;
    public event Action<float> OnRunning;
    public event Action OnDash;
    public event Action OnJump;
    public event Action OnPressAnyButton;
    public event Action OnLightAttack;
    public event Action<float> OnHeavyAttack;
    public event Action<Vector2> OnChangeWeapon;

    public event Action OnSelect;
    public event Action OnBack;
    public event Action<Vector2> OnNavigateMenu;

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

    private void SubscribeToInput()
    {
        if (inputActions == null) return;

        // Juego
        inputActions.Game.Movement.performed += ctx => OnMovement?.Invoke(ctx.ReadValue<float>());
        inputActions.Game.Movement.canceled += ctx => OnMovement?.Invoke(0f);
        inputActions.Game.Jump.performed += _ => AddToBuffer("Jump");
        inputActions.Game.Run.performed += ctx => OnRunning?.Invoke(ctx.ReadValue<float>());
        inputActions.Game.Run.canceled += _ => OnRunning?.Invoke(0f);
        inputActions.Game.Dash.performed += _ => AddToBuffer("Dash");
        inputActions.Game.LightAttack.performed += _ => AddToBuffer("LightAttack");
        inputActions.Game.HeavyAttack.performed += ctx => OnHeavyAttack?.Invoke(ctx.ReadValue<float>());
        inputActions.Game.HeavyAttack.canceled += _ => OnHeavyAttack?.Invoke(0f);
        inputActions.Game.ChangeWeapon.performed += ctx => OnChangeWeapon?.Invoke(ctx.ReadValue<Vector2>());

        // Menú
        inputActions.Menu.Select.performed += _ => AddToBuffer("Select");
        inputActions.Menu.Back.performed += _ => AddToBuffer("Back");
        inputActions.Menu.PAButton.performed += _ => AddToBuffer("PAButton");
        inputActions.Menu.Navigation.performed += ctx => OnNavigateMenu?.Invoke(ctx.ReadValue<Vector2>());

        // Interacción
        inputActions.Game.Interact.performed += _ => AddToBuffer("InteractPressed");
        inputActions.Game.Interact.started += ctx => OnInteractHold?.Invoke(ctx.ReadValue<float>());
        inputActions.Game.Interact.canceled += _ => OnInteractHold?.Invoke(0f);
    }

    private void Update()
    {
        ProcessInputBuffer();
    }

    private void AddToBuffer(string action)
    {
        inputBuffer[action] = Time.time + bufferTime;
    }

    private void ProcessInputBuffer()
    {
        List<string> keysToRemove = new List<string>();

        foreach (var input in inputBuffer)
        {
            if (Time.time >= input.Value)
            {
                switch (input.Key)
                {
                    case "Jump":
                        OnJump?.Invoke();
                        break;
                    case "Dash":
                        OnDash?.Invoke();
                        break;
                    case "Select":
                        OnSelect?.Invoke();
                        break;
                    case "Back":
                        OnBack?.Invoke();
                        break;
                    case "PAButton":
                        OnPressAnyButton?.Invoke();
                        break;
                    case "InteractPressed":
                        OnInteractPressed?.Invoke();
                        break;
                    case "LightAttack":
                        OnLightAttack?.Invoke();
                        break;
                }

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

        inputActions.Game.Movement.performed -= ctx => OnMovement?.Invoke(ctx.ReadValue<float>());
        inputActions.Game.Movement.canceled -= ctx => OnMovement?.Invoke(0f);
        inputActions.Game.Jump.performed -= _ => AddToBuffer("Jump");
        inputActions.Game.Dash.performed -= _ => AddToBuffer("Dash");
        inputActions.Game.Run.performed -= ctx => OnRunning?.Invoke(ctx.ReadValue<float>());
        inputActions.Game.Run.canceled -= _ => OnRunning?.Invoke(0f);
        inputActions.Game.LightAttack.performed -= _ => AddToBuffer("LightAttack");
        inputActions.Game.HeavyAttack.performed -= ctx => OnHeavyAttack?.Invoke(ctx.ReadValue<float>());
        inputActions.Game.HeavyAttack.canceled -= ctx => OnHeavyAttack?.Invoke(0f);
        inputActions.Game.ChangeWeapon.performed -= ctx => OnChangeWeapon?.Invoke(ctx.ReadValue<Vector2>());

        inputActions.Menu.Select.performed -= _ => AddToBuffer("Select");
        inputActions.Menu.Back.performed -= _ => AddToBuffer("Back");
        inputActions.Menu.PAButton.performed -= _ => AddToBuffer("PAButton");
        inputActions.Menu.Navigation.performed -= ctx => OnNavigateMenu?.Invoke(ctx.ReadValue<Vector2>());

        inputActions.Game.Interact.performed -= _ => AddToBuffer("InteractPressed");
        inputActions.Game.Interact.started -= ctx => OnInteractHold?.Invoke(ctx.ReadValue<float>());
        inputActions.Game.Interact.canceled -= ctx => OnInteractHold?.Invoke(0f);
    }
}
