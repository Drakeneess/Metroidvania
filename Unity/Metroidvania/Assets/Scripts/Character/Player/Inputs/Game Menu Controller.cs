using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controlador del menú del juego. Gestiona el estado del juego (en juego o en menú)
/// y actualiza el esquema de control en consecuencia.
/// </summary>
public class GameMenuController : MonoBehaviour
{
    /// <summary>
    /// Instancia única del GameMenuController (Singleton).
    /// </summary>
    public static GameMenuController Instance { get; private set; }

    /// <summary>
    /// Estado actual del juego.
    /// </summary>
    private GameMode currentMode = GameMode.None;

    private GameMode previousMode = GameMode.None;

    /// <summary>
    /// Propiedad estática para acceder y modificar el estado del juego.
    /// Cambia automáticamente el esquema de control según el nuevo estado.
    /// </summary>
    public static GameMode CurrentMode
    {
        get => Instance.currentMode;
        set
        {
            Instance.currentMode = value;
            Instance.UpdateInputScheme(value);
        }
    }

    /// <summary>
    /// Asegura que solo haya una instancia de GameMenuController.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destruye este objeto si ya existe una instancia
        }
        else
        {
            Instance = this; // Asigna la instancia
        }
    }

    /// <summary>
    /// Actualiza el esquema de control según el modo actual del juego.
    /// </summary>
    /// <param name="mode">El nuevo modo de juego.</param>
    private void UpdateInputScheme(GameMode mode)
    {
        if (InputController.instance == null) return; // Evita errores si InputController no está disponible

        previousMode = currentMode;
        switch (mode)
        {
            case GameMode.Menu:
                InputController.instance.InputActions.Game.Disable();
                InputController.instance.InputActions.Dialogue.Disable();
                InputController.instance.InputActions.ToolMenu.Disable();
                InputController.instance.InputActions.Menu.Enable();
                break;

            case GameMode.Game:
                InputController.instance.InputActions.Menu.Disable();
                InputController.instance.InputActions.Dialogue.Disable();
                InputController.instance.InputActions.ToolMenu.Disable();
                InputController.instance.InputActions.Game.Enable();
                break;

            case GameMode.Selection:
                InputController.instance.InputActions.Game.Disable();
                InputController.instance.InputActions.Menu.Disable();
                InputController.instance.InputActions.ToolMenu.Disable();
                InputController.instance.InputActions.Dialogue.Enable();
                break;
            case GameMode.ToolMenu:
                InputController.instance.InputActions.Game.Disable();
                InputController.instance.InputActions.Menu.Disable();
                InputController.instance.InputActions.Dialogue.Disable();
                InputController.instance.InputActions.ToolMenu.Enable();
                break;
            }
    }
    public void SetPreviousMode(){
        if(previousMode != GameMode.None){
            currentMode = previousMode;
        }
    }
}

/// <summary>
/// Enum para representar los distintos modos de juego.
/// </summary>
public enum GameMode
{
    None,
    Menu,      // Modo de menú principal o pausa
    Game,      // Modo de juego activo
    Selection,  // Modo de selección (ej. selección de personaje o nivel)
    ToolMenu
}
