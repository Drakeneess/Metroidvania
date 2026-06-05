using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class VirtualGamepadManager : MonoBehaviour
{
    public static VirtualGamepadManager Instance;

    private Gamepad virtualPad;
    private GamepadState state;

    private void Awake()
    {
        Instance = this;

        // Crear device virtual
        virtualPad = InputSystem.AddDevice<Gamepad>("VirtualGamepad");

        // Inicializamos el estado vacío
        state = new GamepadState();
    }

    public void Press(GamepadButton button)
    {
        // Activar bit correspondiente
        state.buttons |= (uint)button;

        // Enviar el nuevo estado entero
        InputSystem.QueueStateEvent(virtualPad, state);
        InputSystem.Update();

        // Soltar en siguiente frame
        StartCoroutine(ReleaseNextFrame(button));
    }

    public void HoldDown(GamepadButton button)
    {
        state.buttons |= (uint)button;
        InputSystem.QueueStateEvent(virtualPad, state);
        InputSystem.Update();
    }

    public void HoldUp(GamepadButton button)
    {
        state.buttons &= ~(uint)button; // Apagar bit
        InputSystem.QueueStateEvent(virtualPad, state);
        InputSystem.Update();
    }

    private System.Collections.IEnumerator ReleaseNextFrame(GamepadButton button)
    {
        yield return null;
        state.buttons &= ~(uint)button; // apagar bi
        InputSystem.QueueStateEvent(virtualPad, state);
        InputSystem.Update();
    }
}
