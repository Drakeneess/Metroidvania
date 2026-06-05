using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class SettingsButtons : MonoBehaviour
{
    public enum Layout { Vertical, Horizontal, Matrix }
    public Layout layout = Layout.Vertical;

    [Tooltip("Controles en orden (Slider/Carousel/Toggle/Button adaptados a SettingsControlBase)")]
    public SettingsControlBase[] controls;

    [Header("Navegación")]
    public float moveDelay = 0.2f;          // tiempo entre movimientos al cambiar selección
    public float holdInitialDelay = 0.3f;   // tiempo de espera tras mantener dirección
    public float holdRepeatRate   = 0.05f;  // intervalo de repetición mientras se mantiene
    public int columns = 2; // para Matrix

    [Header("Ref")]
    public MenuContainer menuButtonsContainer;
    public SettingsScrollManualContainer scrollCenter;


    private int current = 0;
    private bool isEditing = false;

    private float moveCooldown = 0f;

    // --- Auto-repeat ---
    private float holdTimer = 0f;
    private Vector2 lastDir = Vector2.zero;
    private bool holding = false;

    void Start()
    {
        if (controls == null) controls = new SettingsControlBase[0];

        // Hover con mouse
        for (int i = 0; i < controls.Length; i++)
        {
            if (!controls[i]) continue;
            var go = controls[i].gameObject;
            var trig = go.GetComponent<EventTrigger>() ?? go.AddComponent<EventTrigger>();
            int idx = i;

            var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            entry.callback.AddListener(_ => { if (!isEditing) SetSelection(idx); });
            trig.triggers.Add(entry);
        }

        UpdateHighlights();
        FocusCurrent();
        GameMenuController.CurrentMode = GameMode.Menu;
    }

    private void OnEnable()
    {
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnVector2Input += OnNavigate;
            InputActionController.Instance.OnActionTriggered += OnAction;
        }

        // 🔹 Esperar 1 frame y forzar refresh visual de idioma
        StartCoroutine(ForceLanguageRefresh());

        // centrar el elemento actual al abrir
        scrollCenter?.MoveTo(current);
    }

    private IEnumerator ForceLanguageRefresh()
    {
        yield return null; // Esperar un frame a que el idioma se fije
        LanguageMenu.Instance?.OnLanguageChanged(); // Fuerza actualización de textos visibles
    }


    void OnDisable()
    {
        if (InputActionController.Instance == null) return;
        InputActionController.Instance.OnVector2Input    -= OnNavigate;
        InputActionController.Instance.OnActionTriggered -= OnAction;
        isEditing = false;
    }

    void Update()
    {
        if (moveCooldown > 0f)
            moveCooldown -= Time.unscaledDeltaTime;

        // manejar repetición
        if (holding && Mathf.Abs(lastDir.x) > 0.1f && controls.Length > 0)
        {
            holdTimer -= Time.unscaledDeltaTime;
            if (holdTimer <= 0f)
            {
                controls[current]?.OnNavigate(lastDir.x);
                holdTimer = holdRepeatRate;
            }
        }
    }

    private void OnNavigate(InputActionType action, Vector2 dir)
    {
        if (action != InputActionType.Navigation || controls.Length == 0) return;

        // === Input horizontal (ajuste de valores) ===
        if (Mathf.Abs(dir.x) > 0.1f && dir != Vector2.zero)
        {
            // primer input inmediato
            if (!holding || Mathf.Sign(dir.x) != Mathf.Sign(lastDir.x))
            {
                controls[current]?.OnNavigate(dir.x);
                holdTimer = holdInitialDelay;
                holding = true;
                lastDir = dir;
            }
            return;
        }
        else
        {
            // si soltó la tecla
            holding = false;
        }

        // === Input vertical / navegación entre elementos ===
        if (moveCooldown > 0f) return;

        int next = current;
        switch (layout)
        {
            case Layout.Vertical:
                if (dir.y > 0.1f) next--;
                else if (dir.y < -0.1f) next++;
                break;

            case Layout.Horizontal:
                if (dir.x > 0.1f) next++;
                else if (dir.x < -0.1f) next--;
                break;

            case Layout.Matrix:
                if (dir.y > 0.1f) next -= columns;
                else if (dir.y < -0.1f) next += columns;
                else if (dir.x > 0.1f) next++;
                else if (dir.x < -0.1f) next--;
                break;
        }

        if (controls.Length > 0) SetSelection(Wrap(next, controls.Length));
        moveCooldown = moveDelay;
    }

    private void OnAction(InputActionType action)
    {
        if (controls.Length == 0) return;
        var ctrl = controls[current];

        if (action == InputActionType.Select)
        {
            if (!isEditing)
                isEditing = ctrl?.OnSelect() ?? false;
            else
                isEditing = false;
        }
        else if (action == InputActionType.Back)
        {
            if (isEditing) isEditing = false;
            else
            {
                SettingsUploader.Instance?.OnSettingsMenuClosed();
                MenuInputLock.SetBlocked(false);
                MenuTransition.Instance.SwitchTo(menuButtonsContainer);
            }
        }
    }

    private void SetSelection(int index)
    {
        current = Mathf.Clamp(index, 0, Mathf.Max(0, controls.Length - 1));
        UpdateHighlights();
        FocusCurrent();
        scrollCenter?.MoveTo(current);
    }


    private void UpdateHighlights()
    {
        for (int i = 0; i < controls.Length; i++)
            if (controls[i]) controls[i].Highlight(i == current);
    }

    private void FocusCurrent()
    {
        if (EventSystem.current && controls.Length > 0 && controls[current])
        {
            var go = controls[current].gameObject;
            if (EventSystem.current.currentSelectedGameObject != go)
                EventSystem.current.SetSelectedGameObject(go);
        }
    }

    private static int Wrap(int idx, int len)
    {
        if (len <= 0) return 0;
        if (idx < 0) return len - 1;
        if (idx >= len) return 0;
        return idx;
    }
}
