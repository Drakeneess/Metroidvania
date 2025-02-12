using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Necesario para detectar interacciones con el mouse

public class SelectableItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button button;
    private Image background; // Fondo para visualización
    public Color normalColor = Color.white;
    public Color selectedColor = Color.green;

    private void Awake()
    {
        button = GetComponent<Button>();
        background = GetComponent<Image>();

        if (button == null)
            Debug.LogError("Falta el componente Button en " + gameObject.name);

        if (background == null)
            Debug.LogWarning("Falta el componente Image en " + gameObject.name);

        SetDeselectedVisual();
    }

    public void SetOnPressedClick(UnityAction onSelected)
    {
        if (button != null)
            button.onClick.AddListener(onSelected);
    }

    public void OnSelected()
    {
        SetSelectedVisual();
    }

    public void SetOnDeselected()
    {
        SetDeselectedVisual();
    }

    private void SetSelectedVisual()
    {
        if (background != null)
            background.color = selectedColor;
    }

    private void SetDeselectedVisual()
    {
        if (background != null)
            background.color = normalColor;
    }

    public void OnPressed()
    {
        if (button != null)
            button.onClick.Invoke();
    }

    // Detectar cuando el mouse pasa sobre el botón
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnSelected();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetOnDeselected();
    }
}
