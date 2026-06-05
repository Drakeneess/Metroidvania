using UnityEngine;

public class PauseMenuButtons : MenuButtons
{
    [SerializeField] private MenuContainer settingsContainer;

    protected override void Start()
    {
        base.Start();

        if (buttons.Length > 0) buttons[0].onClick.AddListener(ContinueGame);
        if (buttons.Length > 1) buttons[1].onClick.AddListener(OpenSettings);
        if (buttons.Length > 2) buttons[2].onClick.AddListener(ExitGame);
    }

    protected override void Select(InputActionType actionName)
    {
        if (!menu || !menu.AreOptionsDeployed || MenuInputLock.Blocked)
            return;

        base.Select(actionName);
    }

    protected override void NavigateVertical(Vector2 direction)
    {
        if (!menu || !menu.AreOptionsDeployed || MenuInputLock.Blocked)
            return;

        base.NavigateVertical(direction);
    }

    private void ContinueGame() => PauseManager.Instance.ResumeGame();

    private void OpenSettings()
    {
        MenuInputLock.SetBlocked(true);

        // Igual que en Main Menu:
        MenuTransition.Instance.SwitchTo(settingsContainer);
    }

    private void ExitGame() => PauseManager.Instance.ExitGame();
}
