using UnityEngine;

/// <summary>
/// Se asegura de que solo un panel (Dialogue o Thought) esté activo y
/// conmute los componentes comunes para suavizar la UI.
/// </summary>
public class PanelManager : MonoBehaviour
{
    public static PanelManager Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject thoughtPanel;

    [Header("Conjunto UI común (opcional)")]
    [SerializeField] private CommonUISet commonUI;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (dialoguePanel) dialoguePanel.SetActive(false);
        if (thoughtPanel) thoughtPanel.SetActive(false);
        if (commonUI) commonUI.DeactivateAll(); // 🔹 todo apagado al inicio
    }

    void Start()
    {
        HideAll();
    }

    public void ShowDialogue()
    {
        if (thoughtPanel) thoughtPanel.SetActive(false);
        if (dialoguePanel) dialoguePanel.SetActive(true);

        if (commonUI) commonUI.ActivateForDialogue();
    }

    public void ShowThought()
    {
        if (dialoguePanel) dialoguePanel.SetActive(false);
        if (thoughtPanel) thoughtPanel.SetActive(true);

        if (commonUI) commonUI.ActivateForThought();
    }

    public void HideAll()
    {
        if (dialoguePanel) dialoguePanel.SetActive(false);
        if (thoughtPanel) thoughtPanel.SetActive(false);

        if (commonUI) commonUI.DeactivateAll();

        GameMenuController.CurrentMode = GameMode.Game;
    }
}
