using UnityEngine;
using UnityEngine.UI;

public class ButtonUI : MonoBehaviour
{
    [SerializeField]
    private Image buttonIcon; // Imagen asociada al botón

    [SerializeField]
    private ActionType actionType; // Tipo de acción asociada al botón (opcional)

    private void Awake()
    {
        // Asegurar que la referencia al ícono está asignada
        if (buttonIcon == null)
        {
            buttonIcon = GetComponent<Image>();
            if (buttonIcon == null)
            {
                Debug.LogError($"No Image component found for {gameObject.name}. Please assign it in the inspector.");
            }
        }
        HandleControlSchemeChanged(0);
    }

    /// <summary>
    /// Cambia el ícono del botón.
    /// </summary>
    /// <param name="icon">El nuevo sprite para el ícono.</param>
    public void SetButtonIcon(Sprite icon)
    {
        if (icon != null && buttonIcon.sprite != icon)
        {
            buttonIcon.sprite = icon;
        }
    }

    /// <summary>
    /// Activa el botón y realiza configuraciones visuales opcionales.
    /// </summary>
    public void Activate()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            SubscribeToControlSchemeChange();
        }
    }

    /// <summary>
    /// Desactiva el botón y realiza configuraciones visuales opcionales.
    /// </summary>
    public void Deactivate()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            UnsubscribeFromControlSchemeChange();
        }
    }

    /// <summary>
    /// Obtiene el tipo de acción asociado al botón (opcional).
    /// </summary>
    /// <returns>El tipo de acción como enum.</returns>
    public ActionType GetActionType()
    {
        return actionType;
    }

    private void OnEnable()
    {
        // Registrar al ButtonUIController solo cuando el botón está activo
        ButtonUIController.Register(this);
    }

    private void OnDisable()
    {
        // Desregistrar cuando se desactive
        ButtonUIController.Unregister(this);
        UnsubscribeFromControlSchemeChange();
    }

    private void SubscribeToControlSchemeChange()
    {
        InputController.OnControlSchemeChanged += HandleControlSchemeChanged;
    }

    private void UnsubscribeFromControlSchemeChange()
    {
        InputController.OnControlSchemeChanged -= HandleControlSchemeChanged;
    }

    private void HandleControlSchemeChanged(int newScheme)
    {
        ButtonUIController.UpdateButtonIcon(this, newScheme);
    }
}
