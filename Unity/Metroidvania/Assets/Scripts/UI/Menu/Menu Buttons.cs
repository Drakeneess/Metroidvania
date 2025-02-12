using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{
    public enum MenuType
    {
        vertical,
        horizontal,
        matrix
    }
    public MenuType menuType;

    public Button[] buttons;
    protected int currentSelection = 0; // Índice del botón seleccionado
    protected Input inputs;
    protected Vector2 cameraDirection;
    protected Menu menu;

    public float navigationDelay = 0.2f; // Tiempo de delay entre navegaciones
    private float navigationCooldown = 0f; // Temporizador para controlar el delay
    private int columns = 3; // Número de columnas en el menú de tipo matriz
    private Coroutine[] animationCoroutines;

    public Color normalColor = Color.white; // Color por defecto
    public Color selectedColor = Color.yellow; // Color de selección

    protected virtual void Awake() {
        
    }

    protected virtual void Start()
    {
        inputs = new Input();
        inputs.Menu.Enable();

        // Inicializar corrutinas para cada botón
        animationCoroutines = new Coroutine[buttons.Length];

        // Asignar eventos para el mouse
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            EventTrigger trigger = buttons[i].gameObject.AddComponent<EventTrigger>();

            // Crear la entrada de evento OnPointerEnter
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((data) => { OnMouseHover(index); });
            trigger.triggers.Add(entry);
        }
        menu = GetComponent<Menu>();

        // Inicializar la selección
        UpdateButtonSelection();

        // Asignar las acciones de entrada
        inputs.Menu.Navigation.performed += ctx => Navigate(ctx.ReadValue<Vector2>());
        inputs.Menu.Select.performed += ctx => Select();
    }

    protected void UpdateButtonSelection()
    {
        if (menu.AreOptionsDeployed)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (i == currentSelection)
                {
                    // Si hay una animación en progreso, detenerla antes de iniciar otra
                    if (animationCoroutines[i] != null) StopCoroutine(animationCoroutines[i]);
                    animationCoroutines[i] = StartCoroutine(AnimateButtonColor(buttons[i], selectedColor));
                }
                else
                {
                    // Detener animación existente si se aplica
                    if (animationCoroutines[i] != null) StopCoroutine(animationCoroutines[i]);
                    animationCoroutines[i] = StartCoroutine(AnimateButtonColor(buttons[i], normalColor));
                }
            }
        }
    }

    private IEnumerator AnimateButtonColor(Button button, Color targetColor)
    {
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage == null) yield break;

        Color startColor = buttonImage.color;
        float elapsedTime = 0f;
        float transitionDuration = 0.15f; // Tiempo de transición ajustado

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            buttonImage.color = Color.Lerp(startColor, targetColor, elapsedTime / transitionDuration);
            yield return null;
        }

        buttonImage.color = targetColor; // Asegurar el color final
    }

    private void Navigate(Vector2 direction)
    {
        if (navigationCooldown <= 0f)
        {
            switch (menuType)
            {
                case MenuType.vertical:
                    NavigateVertical(direction);
                    break;

                case MenuType.horizontal:
                    NavigateHorizontal(direction);
                    break;

                case MenuType.matrix:
                    NavigateMatrix(direction);
                    break;
            }

            navigationCooldown = navigationDelay;
        }
    }

    protected virtual void NavigateVertical(Vector2 direction)
    {
        if (direction.y > 0) currentSelection--;
        else if (direction.y < 0) currentSelection++;

        if (currentSelection < 0)
            currentSelection = buttons.Length - 1;
        else if (currentSelection >= buttons.Length)
            currentSelection = 0;

        UpdateButtonSelection();
    }

    protected virtual void NavigateHorizontal(Vector2 direction)
    {
        if (direction.x > 0) currentSelection++;
        else if (direction.x < 0) currentSelection--;

        if (currentSelection < 0)
            currentSelection = buttons.Length - 1;
        else if (currentSelection >= buttons.Length)
            currentSelection = 0;

        UpdateButtonSelection();
    }

    protected virtual void NavigateMatrix(Vector2 direction)
    {
        int rows = Mathf.CeilToInt((float)buttons.Length / columns);

        if (direction.y > 0) currentSelection -= columns;
        else if (direction.y < 0) currentSelection += columns;
        else if (direction.x > 0) currentSelection++;
        else if (direction.x < 0) currentSelection--;

        if (currentSelection < 0)
            currentSelection = buttons.Length - 1;
        else if (currentSelection >= buttons.Length)
            currentSelection = 0;

        UpdateButtonSelection();
    }

    private void Select()
    {
        if (currentSelection >= 0 && currentSelection < buttons.Length && menu.AreOptionsDeployed)
        {
            buttons[currentSelection].onClick.Invoke();
        }
    }

    public void OnMouseHover(int buttonIndex)
    {
        currentSelection = buttonIndex;
        UpdateButtonSelection();
    }

    protected virtual void Update()
    {
        UpdateButtonSelection();
        if (navigationCooldown > 0f)
        {
            navigationCooldown -= Time.deltaTime;
        }
    }

    private void OnDisable()
    {
        inputs.Menu.Disable();
    }
}
