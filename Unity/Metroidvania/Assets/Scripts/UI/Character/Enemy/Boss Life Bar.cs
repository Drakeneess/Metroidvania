using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossLifeBar : MonoBehaviour
{
    public static BossLifeBar Instance { get; private set; }

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI bossName;
    [SerializeField] private Image lifebar;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        HideBar();
    }

    public void ShowBar(string name)
    {
        if (bossName) bossName.text = name;
        if (canvasGroup)
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    public void HideBar()
    {
        if (canvasGroup)
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void UpdateHealth(float normalizedValue)
    {
        if (lifebar)
            lifebar.fillAmount = Mathf.Clamp01(normalizedValue);
    }
}
