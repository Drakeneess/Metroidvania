using System;

public static class MainMenuEvents
{
    public static event Action OnTitleStart;
    public static event Action OnTitleComplete;
    public static event Action OnButtonsReady;
    public static event Action OnMenuActivated;

    public static void TriggerTitleStart() {
        OnTitleStart?.Invoke();
    }

    public static void TriggerTitleComplete() {
        OnTitleComplete?.Invoke();
    }

    public static void TriggerButtonsReady() {
        OnButtonsReady?.Invoke();
    }

    public static void TriggerMenuActivated() {
        OnMenuActivated?.Invoke();
    }
}
