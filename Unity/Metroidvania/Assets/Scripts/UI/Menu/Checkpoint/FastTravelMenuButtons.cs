using UnityEngine;
using UnityEngine.UI;

public class FastTravelMenuButtons : MenuButtons
{
    [Header("Refs (opcionales)")]
    [SerializeField] private FastTravelMenu fastTravelMenu; // opcional por Inspector
    [SerializeField] private ScrollRect scrollRect;         // opcional por Inspector

    // relleno dinámico
    private RectTransform fillerTop;
    private RectTransform fillerBottom;

    protected override void Start()
    {
        base.Start();

        // Robustez: busca refs si no se asignaron por inspector
        if (!fastTravelMenu) fastTravelMenu = GetComponentInParent<FastTravelMenu>();
        if (!scrollRect)
        {
            scrollRect = GetComponentInChildren<ScrollRect>(includeInactive: true);
            if (!scrollRect) scrollRect = GetComponentInParent<ScrollRect>();
        }

        RefreshButtons();     // primera vez
        //CenterOnSelection();  // centrar inicial
    }

    public void RefreshButtons()
    {
        if (!fastTravelMenu) return;

        buttons = fastTravelMenu.GetComponentsInChildren<Button>(includeInactive: false);
        animationCoroutines = new Coroutine[buttons.Length];

        // Si no hay botones todavía, evita cálculos
        if (buttons.Length == 0) return;

        currentSelection = Mathf.Clamp(currentSelection, 0, buttons.Length - 1);

        AddFillers(); // asegurar espacio extra
        UpdateButtonSelection();
        CenterOnSelection();
    }

    private void AddFillers()
    {
        if (buttons == null || buttons.Length == 0) return;
        if (!scrollRect) return; // sin scroll, nada que rellenar

        RectTransform content = buttons[0].transform.parent as RectTransform;
        if (!content) return;

        // === Asegurar que el layout esté listo antes de medir ===
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);

        // Fallback si el viewport no está asignado
        RectTransform viewport = scrollRect.viewport ? scrollRect.viewport : scrollRect.GetComponent<RectTransform>();
        if (!viewport) return;

        // Crear fillers si no existen
        EnsureFiller(ref fillerTop, content, "FillerTop");
        EnsureFiller(ref fillerBottom, content, "FillerBottom");

        // Calcular padding para centrar un botón en el viewport
        float viewportHeight = viewport.rect.height;
        float buttonHeight = (buttons[0].transform as RectTransform).rect.height;

        // Si el botón aún no tiene tamaño (layout sin aplicar), evita NRE
        if (buttonHeight <= 0f)
        {
            // Forzar layout una vez más e intenta medir
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
            buttonHeight = (buttons[0].transform as RectTransform).rect.height;
            if (buttonHeight <= 0f) return;
        }

        float padding = Mathf.Max(0f, (viewportHeight - buttonHeight) * 0.5f);

        fillerTop.sizeDelta    = new Vector2(1, padding);
        fillerBottom.sizeDelta = new Vector2(1, padding);

        // Colocar correctamente en el orden de hijos
        fillerTop.SetAsFirstSibling();
        fillerBottom.SetAsLastSibling();
    }

    private void EnsureFiller(ref RectTransform filler, RectTransform parent, string name)
    {
        if (filler) return;
        var go = new GameObject(name, typeof(RectTransform), typeof(LayoutElement));
        filler = go.GetComponent<RectTransform>();
        filler.SetParent(parent, false);
        var le = go.GetComponent<LayoutElement>();
        le.minHeight = 0; le.preferredHeight = 0; // solo usaremos sizeDelta para la altura
    }

    protected override void NavigateVertical(Vector2 direction)
    {
        if (buttons == null || buttons.Length == 0) return;

        if (direction.y > 0) currentSelection--;
        else if (direction.y < 0) currentSelection++;

        // navegación infinita
        if (currentSelection < 0) currentSelection = buttons.Length - 1;
        if (currentSelection >= buttons.Length) currentSelection = 0;

        UpdateButtonSelection();
        CenterOnSelection();
    }

    private void CenterOnSelection()
    {
        if (!scrollRect || buttons == null || buttons.Length == 0) return;

        RectTransform target  = buttons[currentSelection].GetComponent<RectTransform>();
        RectTransform content = target.parent as RectTransform;
        if (!content) return;

        // Asegurar layout antes del cálculo
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);

        RectTransform viewport = scrollRect.viewport ? scrollRect.viewport : scrollRect.GetComponent<RectTransform>();
        if (!viewport) return;

        float contentHeight  = content.rect.height;
        float viewportHeight = viewport.rect.height;

        // Si el contenido cabe, no hay scroll que centrar
        if (contentHeight <= viewportHeight)
        {
            scrollRect.normalizedPosition = new Vector2(0, 1f);
            return;
        }

        float targetPos  = -target.localPosition.y; // posición local del botón respecto a content
        float normalized = Mathf.Clamp01((targetPos - viewportHeight * 0.5f) / (contentHeight - viewportHeight));

        // Y = 1-arriba, 0-abajo → invertimos
        scrollRect.normalizedPosition = new Vector2(0, 1f - normalized);
    }

    protected override void Select(string actionName)
    {
        if (actionName == "Select")
        {
            if (currentSelection >= 0 && currentSelection < buttons.Length && menu.AreOptionsDeployed)
                buttons[currentSelection].onClick.Invoke();
        }
        else if (actionName == "Back")
        {
            (menu as FastTravelMenu)?.Close();
        }
    }
}
