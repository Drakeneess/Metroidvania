using System.Collections;
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
    }

    /// <summary>
    /// Cambia el ícono del botón.
    /// </summary>
    /// <param name="icon">El nuevo sprite para el ícono.</param>
    public void SetButtonIcon(Sprite icon)
    {
        if (icon == null)
        {
            Debug.LogWarning($"Null icon provided for {gameObject.name}. No changes made.");
            return;
        }

        if (buttonIcon.sprite != icon)
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
}
