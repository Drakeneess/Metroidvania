// SettingsSlider.cs (refactor anclado + mouse drag)
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[DisallowMultipleComponent]
public class SettingsSlider : SettingsControlBase
{
    public enum Orientation { Horizontal, Vertical }

    [Header("Config")]
    public float minValue = 0f;
    public float maxValue = 1f;
    public bool  wholeNumbers = false;
    [Range(0.01f, 0.5f)] public float stepPercent = 0.05f;

    [Header("Layout")]
    public Orientation orientation = Orientation.Horizontal;
    public bool reverse = false; // H: derecha->izquierda, V: arriba->abajo

    [Header("UI")]
    public Image         fillImage;   // si lo asignas, usaremos su rect como fillRect
    public RectTransform trackRect;   // contenedor/pista completa
    public RectTransform fillRect;    // rect del fill (mejor que fillAmount)
    public RectTransform handleRect;  // knob
    public TMP_Text      valueLabel;  // opcional
    public Graphic       highlightTarget; // si null usa fillImage o primer Graphic

    [Header("Mouse")]
    public bool dragWithHandle = true; // arrastrar desde el handle
    public bool dragOnTrack    = true; // arrastrar/clickear sobre el track
    public bool jumpOnClick    = true; // al hacer click en el track, saltar al punto

    [Header("Colors")]
    public Color normalColor   = new(0.5f, 0.5f, 0.5f, 1f);
    public Color selectedColor = Color.white;

    [Header("State")]
    [SerializeField] private float currentValue = 0f;
    public float Value => currentValue;

    private bool _dragging = false;

    private Graphic Target
    {
        get
        {
            if (highlightTarget) return highlightTarget;
            if (fillImage) return fillImage;
            return GetComponentInChildren<Graphic>(true);
        }
    }

    private void Reset()      { AutoWire(); }
    private void Awake()      { AutoWire(); }
    private void OnValidate() { ClampRanges(); AutoWire(); }

    private void Start()
    {
        ClampRanges();
        SetValue(currentValue, true);
        Highlight(false);
        SetupPointerReceivers();
    }

    // ===== SettingsControlBase =====
    public override void Highlight(bool active)
    {
        var t = Target;
        if (t) t.color = active ? selectedColor : normalColor;
    }

    public override bool OnSelect() => true; // edición con gamepad/teclado

    public override bool OnNavigate(float x)
    {
        if (Mathf.Abs(x) < 0.1f) return false;

        float step = wholeNumbers ? 1f :
            Mathf.Max(0.001f, (maxValue - minValue) * stepPercent);

        float dir = reverse ? -Mathf.Sign(x) : Mathf.Sign(x);
        SetValue(currentValue + dir * step);
        return true;
    }

    public override string GetValue()
    {
        // porcentaje entero 0..100
        return Mathf.RoundToInt(Mathf.InverseLerp(minValue, maxValue, currentValue) * 100f).ToString();
    }

    // ===== API =====
    public void SetValue(float value, bool force = false)
    {
        float clamped = Mathf.Clamp(value, minValue, maxValue);
        if (wholeNumbers) clamped = Mathf.Round(clamped);

        if (!force && Mathf.Approximately(clamped, currentValue)) return;

        currentValue = clamped;
        UpdateVisuals();

        // 🔔 Notificar cambio
        NotifyValueChanged();
    }


    public void Increment() => SetValue(currentValue + Step());
    public void Decrement() => SetValue(currentValue - Step());

    private float Step() => wholeNumbers ? 1f : Mathf.Max(0.001f, (maxValue - minValue) * stepPercent);

    // ===== Internos =====
    private void UpdateVisuals()
    {
        float t = Mathf.InverseLerp(minValue, maxValue, currentValue); // 0..1

        // 1) Fill con anchors
        if (!fillRect && fillImage) fillRect = fillImage.rectTransform;
        if (fillRect && trackRect)
        {
            if (orientation == Orientation.Horizontal)
            {
                if (!reverse)
                {
                    fillRect.anchorMin = new Vector2(0f,   fillRect.anchorMin.y);
                    fillRect.anchorMax = new Vector2(t,    fillRect.anchorMax.y);
                }
                else
                {
                    fillRect.anchorMin = new Vector2(1f - t, fillRect.anchorMin.y);
                    fillRect.anchorMax = new Vector2(1f,     fillRect.anchorMax.y);
                }
                fillRect.offsetMin = new Vector2(0f, fillRect.offsetMin.y);
                fillRect.offsetMax = new Vector2(0f, fillRect.offsetMax.y);
            }
            else // Vertical
            {
                if (!reverse) // bottom -> top
                {
                    fillRect.anchorMin = new Vector2(fillRect.anchorMin.x, 0f);
                    fillRect.anchorMax = new Vector2(fillRect.anchorMax.x, t );
                }
                else // top -> bottom
                {
                    fillRect.anchorMin = new Vector2(fillRect.anchorMin.x, 1f - t);
                    fillRect.anchorMax = new Vector2(fillRect.anchorMax.x, 1f);
                }
                fillRect.offsetMin = new Vector2(fillRect.offsetMin.x, 0f);
                fillRect.offsetMax = new Vector2(fillRect.offsetMax.x, 0f);
            }
        }

        // 2) Handle con anchors
        if (handleRect && trackRect)
        {
            if (handleRect.parent != trackRect)
                handleRect.SetParent(trackRect, worldPositionStays: false);

            if (orientation == Orientation.Horizontal)
            {
                float u = reverse ? (1f - t) : t;
                handleRect.anchorMin = new Vector2(u, 0.5f);
                handleRect.anchorMax = new Vector2(u, 0.5f);
            }
            else // Vertical
            {
                float u = reverse ? (1f - t) : t;
                handleRect.anchorMin = new Vector2(0.5f, u);
                handleRect.anchorMax = new Vector2(0.5f, u);
            }
            handleRect.anchoredPosition = Vector2.zero; // centrado en el anchor
        }

        if (valueLabel)
            valueLabel.text = GetValue();
    }

    private void ClampRanges()
    {
        if (maxValue < minValue) maxValue = minValue + 1f;
    }

    private void AutoWire()
    {
        if (!fillRect && fillImage) fillRect = fillImage.rectTransform;

        if (!trackRect)
        {
            if (fillRect && fillRect.parent is RectTransform p) trackRect = p;
            else trackRect = GetComponent<RectTransform>();
        }

        if (!valueLabel) valueLabel = GetComponentInChildren<TMP_Text>(true);
    }

    // ===== Mouse / Pointer =====
    private void SetupPointerReceivers()
    {
        // Necesitas un GraphicRaycaster en el Canvas y que los Images tengan Raycast Target activo.
        if (dragWithHandle && handleRect)
            EnsureTriggers(handleRect.gameObject, onDown: OnHandlePointerDown, onDrag: OnAnyDrag, onUp: OnAnyUp);

        if (dragOnTrack && trackRect)
            EnsureTriggers(trackRect.gameObject, onDown: OnTrackPointerDown, onDrag: OnAnyDrag, onUp: OnAnyUp);
    }

    private void EnsureTriggers(GameObject go,
    System.Action<PointerEventData> onDown,
    System.Action<PointerEventData> onDrag,
    System.Action<PointerEventData> onUp)
    {
        var trig = go.GetComponent<EventTrigger>();
        if (!trig) trig = go.AddComponent<EventTrigger>();

        void Add(EventTriggerType type, System.Action<PointerEventData> cb)
        {
            var e = new EventTrigger.Entry { eventID = type };
            e.callback.AddListener((data) => cb?.Invoke((PointerEventData)data));
            trig.triggers.Add(e);
        }

        // Asegura que el objeto tenga un Graphic raycasteable
        var g = go.GetComponent<Graphic>();
        if (!g)
        {
            g = go.AddComponent<Image>();
            ((Image)g).color = new Color(0,0,0,0); // invisible
        }
        g.raycastTarget = true;

        Add(EventTriggerType.PointerDown, onDown);
        Add(EventTriggerType.Drag,        onDrag);
        Add(EventTriggerType.PointerUp,   onUp);
    }


    private void OnHandlePointerDown(PointerEventData e)
    {
        _dragging = true;
        UpdateFromPointer(e); // empieza en la posición actual del puntero
    }

    private void OnTrackPointerDown(PointerEventData e)
    {
        _dragging = true;
        if (jumpOnClick) UpdateFromPointer(e);
    }

    private void OnAnyDrag(PointerEventData e)
    {
        if (_dragging) UpdateFromPointer(e);
    }

    private void OnAnyUp(PointerEventData e)
    {
        _dragging = false;
    }

    private void UpdateFromPointer(PointerEventData e)
    {
        if (!trackRect) return;

        Vector2 local;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(trackRect, e.position, e.pressEventCamera, out local))
            return;

        Rect r = trackRect.rect;

        float u; // 0..1 a lo largo del track
        if (orientation == Orientation.Horizontal)
        {
            u = Mathf.InverseLerp(r.xMin, r.xMax, local.x);
            if (reverse) u = 1f - u;
        }
        else
        {
            u = Mathf.InverseLerp(r.yMin, r.yMax, local.y);
            if (reverse) u = 1f - u;
        }

        float value = Mathf.Lerp(minValue, maxValue, Mathf.Clamp01(u));
        SetValue(value, force: true);
    }
}
