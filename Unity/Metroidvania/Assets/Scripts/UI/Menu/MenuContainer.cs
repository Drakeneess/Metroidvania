using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class MenuContainer : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    public CanvasGroup CanvasGroup
    {
        get
        {
            if (!canvasGroup)
                canvasGroup = GetComponent<CanvasGroup>();
            return canvasGroup;
        }
    }

    public bool IsActive => gameObject.activeInHierarchy;

    private void Awake()
    {
        // aseguramos valores iniciales
        CanvasGroup.interactable = false;
        CanvasGroup.blocksRaycasts = false;
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
        CanvasGroup.interactable = visible;
        CanvasGroup.blocksRaycasts = visible;
    }
}
