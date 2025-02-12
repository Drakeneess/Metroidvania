using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenuController : MonoBehaviour
{
    public static GameMenuController Instance { get; private set; }

    private bool inGame = true;

    public static bool InGame
    {
        get
        {
            // Activa el esquema de control correcto según el estado actual
            if (Instance.inGame)
            {
                InputController.instance.InputActions.Menu.Disable();
                InputController.instance.InputActions.Game.Enable();
            }
            else
            {
                InputController.instance.InputActions.Game.Disable();
                InputController.instance.InputActions.Menu.Enable();
            }

            return Instance.inGame;
        }
        set
        {
            Instance.inGame = value;

            // Cambia automáticamente el esquema de control según el nuevo estado
            if (value)
            {
                InputController.instance.InputActions.Menu.Disable();
                InputController.instance.InputActions.Game.Enable();
            }
            else
            {
                InputController.instance.InputActions.Game.Disable();
                InputController.instance.InputActions.Menu.Enable();
            }
        }
    }

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
}
