using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonUIController : MonoBehaviour
{
    public Sprite[] buttonIcons;

    private InteractableButtonUI[] buttonsUI;

    private void Awake()
    {
        buttonsUI = FindObjectsOfType<InteractableButtonUI>();
    }

    private void OnEnable()
    {
        // Suscribirse al evento de cambio de esquema
        InputController.OnControlSchemeChanged += UpdateButtonUI;
    }

    private void OnDisable()
    {
        // Desuscribirse del evento al desactivar
        InputController.OnControlSchemeChanged -= UpdateButtonUI;
    }

    private void UpdateButtonUI(int newScheme)
    {
        // Actualizar los íconos de los botones según el nuevo esquema
        foreach (InteractableButtonUI buttonUI in buttonsUI)
        {
            buttonUI.SetButtonIcon(buttonIcons[newScheme]);
        }
    }
}
