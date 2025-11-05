using System.Collections;
using UnityEngine;

public class SettingsScrollManualContainer : MonoBehaviour
{
    [Header("Refs")]
    public RectTransform container;       // 🔹 El contenedor que se moverá
    public RectTransform viewport;        // 🔹 El área visible donde se centra
    public int itemCount = 0;             // 🔹 Número total de elementos
    public float itemSpacing = 120f;      // 🔹 Distancia entre elementos (ajusta según tu layout)

    [Header("Motion")]
    public float smoothSpeed = 8f;        // 🔹 Velocidad de desplazamiento
    public float centerOffset = 0f;       // 🔹 Ajuste vertical extra
    public bool wrapAround = true;        // 🔹 Scroll infinito (del último al primero)

    private int currentIndex = 0;
    private Coroutine scrollRoutine;

    public void Initialize(int totalItems)
    {
        itemCount = totalItems;
    }

    public void MoveTo(int index)
    {
        if (itemCount <= 0 || !container || !viewport) return;

        if (wrapAround)
        {
            if (index < 0) index = itemCount - 1;
            if (index >= itemCount) index = 0;
        }
        else
        {
            index = Mathf.Clamp(index, 0, itemCount - 1);
        }

        currentIndex = index;

        float targetY = (index * itemSpacing) - (viewport.rect.height / 2f) + centerOffset;
        Vector2 targetPos = new Vector2(container.anchoredPosition.x, targetY);

        if (scrollRoutine != null)
            StopCoroutine(scrollRoutine);

        scrollRoutine = StartCoroutine(SmoothScroll(targetPos));
    }

    private IEnumerator SmoothScroll(Vector2 target)
    {
        while (Vector2.Distance(container.anchoredPosition, target) > 0.1f)
        {
            container.anchoredPosition = Vector2.Lerp(
                container.anchoredPosition,
                target,
                Time.unscaledDeltaTime * smoothSpeed
            );
            yield return null;
        }

        container.anchoredPosition = target;
    }

    public void ScrollNext()
    {
        MoveTo(currentIndex + 1);
    }

    public void ScrollPrevious()
    {
        MoveTo(currentIndex - 1);
    }

    public int GetCurrentIndex() => currentIndex;
}
