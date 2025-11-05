using UnityEngine;
using UnityEngine.UI;

public class FastTravelMenuButtons : MenuButtons
{
    [Header("Refs (opcionales)")]
    [SerializeField] private FastTravelMenu fastTravelMenu; 
    [SerializeField] private ScrollRect scrollRect;        

    protected override void Start()
    {
        base.Start();

        if (!fastTravelMenu) fastTravelMenu = GetComponentInParent<FastTravelMenu>();
        RefreshButtons();
    }

    public void RefreshButtons()
    {
        if (!fastTravelMenu) return;

        // ✅ Solo botones del Content. Sin fillers, sin otros botones mezclados
        buttons = fastTravelMenu.Content.GetComponentsInChildren<Button>(includeInactive: false);
        animationCoroutines = new Coroutine[buttons.Length];

        if (buttons.Length == 0) return;

        currentSelection = Mathf.Clamp(currentSelection, 0, buttons.Length - 1);

        UpdateButtonSelection();
    }

    protected override void NavigateVertical(Vector2 dir)
    {
        if (buttons == null || buttons.Length == 0) return;

        if (dir.y > 0) currentSelection--;
        else if (dir.y < 0) currentSelection++;

        if (currentSelection < 0) currentSelection = buttons.Length - 1;
        if (currentSelection >= buttons.Length) currentSelection = 0;

        UpdateButtonSelection();
    }

    protected override void Select(string actionName)
    {
        if (actionName == "Select" || actionName == "OptionSelect" || actionName == "Submit" || actionName == "Interact")
        {
            if (currentSelection >= 0 && currentSelection < buttons.Length && menu.AreOptionsDeployed)
                buttons[currentSelection].onClick.Invoke();
        }
        else if (actionName == "Back" || actionName == "Cancel")
        {
            (menu as FastTravelMenu)?.Close();
        }
    }
}
