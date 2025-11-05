// SettingsCarousel.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SettingsCarousel : SettingsControlBase
{
    [Header("Opciones")]
    public List<string> options = new();
    public int currentIndex = 0;

    [Header("UI")]
    public TMP_Text label;
    public Button nextButton;
    public Button prevButton;
    public Graphic highlightTarget; // qué pintamos al seleccionar (si null, usa label)

    [Header("Colors")]
    public Color normalColor   = new Color(0.5f, 0.5f, 0.5f, 1f);
    public Color selectedColor = Color.white;

    private Graphic Target => highlightTarget ? highlightTarget : (label ? (Graphic)label : GetComponentInChildren<Graphic>());

    public string CurrentValue => (options.Count > 0) ? options[currentIndex] : "";

    private void Start()
    {
        if (nextButton) nextButton.onClick.AddListener(Next);
        if (prevButton) prevButton.onClick.AddListener(Previous);
        Refresh();
        Highlight(false);
    }

    public override void Highlight(bool active)
    {
        if (Target) Target.color = active ? selectedColor : normalColor;
    }

    public override bool OnSelect()
    {
        // Carrusel también entra en edición (navega con ←/→)
        return true;
    }

    public override bool OnNavigate(float x)
    {
        if (Mathf.Abs(x) < 0.1f || options.Count == 0) return false;
        if (x > 0) Next(); else Previous();
        return true;
    }

    public override string GetValue() => CurrentValue;

    public void SetIndex(int index)
    {
        if (options.Count == 0) return;
        int clamped = Mathf.Clamp(index, 0, options.Count - 1);
        if (clamped == currentIndex) return;

        currentIndex = clamped;
        Refresh();

        // 🔔 Notificar cambio
        NotifyValueChanged();
        NotifyChanged();
    }

    public void Next()
    {
        if (options.Count == 0) return;
        currentIndex = (currentIndex + 1) % options.Count;
        Refresh();
        NotifyValueChanged();
    }

    public void Previous()
    {
        if (options.Count == 0) return;
        currentIndex = (currentIndex - 1 + options.Count) % options.Count;
        Refresh();
        NotifyValueChanged();
    }


    private void Refresh()
    {
        if (label) label.text = CurrentValue;
    }
}
