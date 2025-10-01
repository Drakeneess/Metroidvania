// SettingsHighlightLerp.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsHighlightLerp : MonoBehaviour
{
    private readonly Dictionary<Graphic, Coroutine> _anims = new();

    public void UpdateVisuals(Selectable[] items, int currentIndex, Color normal, Color selected)
    {
        if (items == null) return;
        for (int i = 0; i < items.Length; i++)
        {
            var g = GetGraphic(items[i]);
            if (!g) continue;

            Color target = (i == currentIndex) ? selected : normal;
            if (_anims.TryGetValue(g, out var c) && c != null) StopCoroutine(c);
            _anims[g] = StartCoroutine(LerpColor(g, target, 0.15f));
        }
    }

    private IEnumerator LerpColor(Graphic g, Color target, float dur)
    {
        Color start = g.color; float t = 0f;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            g.color = Color.Lerp(start, target, t / dur);
            yield return null;
        }
        g.color = target;
    }

    private static Graphic GetGraphic(Selectable s)
    {
        if (!s) return null;
        return s.targetGraphic ? s.targetGraphic : s.GetComponent<Graphic>();
    }
}
