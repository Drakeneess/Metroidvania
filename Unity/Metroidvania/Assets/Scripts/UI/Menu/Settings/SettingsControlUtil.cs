using UnityEngine;
using UnityEngine.UI;
#if TMP_PRESENT
using TMPro;
#endif

public static class SettingsControlUtil
{
    // Ejecuta "Select" según el tipo de control
    public static bool TryHandleSelect(Selectable sel)
    {
        if (!sel) return false;

        // Toggle → alternar
        var toggle = sel.GetComponent<Toggle>();
        if (toggle) { toggle.isOn = !toggle.isOn; return true; }

        // Dropdown nativo → abrir
        var dd = sel.GetComponent<Dropdown>();
        if (dd) { dd.Show(); return true; }

#if TMP_PRESENT
        // TMP_Dropdown → abrir
        var tdd = sel.GetComponent<TMP_Dropdown>();
        if (tdd) { tdd.Show(); return true; }
#endif

        // Button → click
        var btn = sel.GetComponent<Button>();
        if (btn) { btn.onClick.Invoke(); return true; }

        // Slider → se maneja en modo edición con ←/→
        return false;
    }

    // Modifica Slider o Dropdown con input horizontal (cuando estás en edición)
    public static bool TryHandleHorizontal(Selectable sel, float x, float sliderStepPercent)
    {
        if (!sel) return false;

        // Slider
        var slider = sel.GetComponent<Slider>();
        if (slider)
        {
            if (slider.wholeNumbers)
                slider.value = Mathf.Clamp(slider.value + Mathf.Sign(x), slider.minValue, slider.maxValue);
            else
            {
                float step = Mathf.Max(0.001f, (slider.maxValue - slider.minValue) * sliderStepPercent);
                slider.value = Mathf.Clamp(slider.value + Mathf.Sign(x) * step, slider.minValue, slider.maxValue);
            }
            return true;
        }

        // Dropdown clásico
        var dd = sel.GetComponent<Dropdown>();
        if (dd && dd.options.Count > 0)
        {
            int next = (dd.value + (x > 0 ? 1 : -1) + dd.options.Count) % dd.options.Count;
            dd.value = next;
            dd.RefreshShownValue();
            return true;
        }

#if TMP_PRESENT
        // TMP_Dropdown
        var tdd = sel.GetComponent<TMP_Dropdown>();
        if (tdd && tdd.options.Count > 0)
        {
            int next = (tdd.value + (x > 0 ? 1 : -1) + tdd.options.Count) % tdd.options.Count;
            tdd.value = next;
            tdd.RefreshShownValue();
            return true;
        }
#endif

        return false;
    }

    // Devuelve un Graphic válido para aplicar colores (incluye búsqueda en hijos)
    public static Graphic GetGraphic(Selectable s)
    {
        if (!s) return null;
        if (s.targetGraphic) return s.targetGraphic;

        var g = s.GetComponent<Graphic>();
        if (g) return g;

        return s.GetComponentInChildren<Graphic>(true);
    }
}
