using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class MainMenuFlowController : MonoBehaviour
{
    [Header("Config")]
    public bool autoStart = true;
    public float beginDelay = 0f;
    public MainMenu mainMenu;

    public static MainMenuFlowController Instance { get; private set; }

    private Coroutine titleCoroutine;
    private bool isSkipping = false;
    private bool buttonsReady = false;
    private bool startedBySceneEvent = false;
    private bool startedManuallyFallback = false;
    private bool flowStarted = false; // 🔹 evita arranques múltiples

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnActionTriggered += HandleActionTriggered;
        }

        SceneController.OnSceneActivated += HandleSceneActivated;
        MainMenuEvents.OnTitleComplete += HandleTitleComplete;
        MainMenuEvents.OnButtonsReady += HandleButtonsReady;
        MainMenuEvents.OnMenuActivated += HandleMenuActivated;

        
    }

    private void OnDisable()
    {
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnActionTriggered -= HandleActionTriggered;
        }

        SceneController.OnSceneActivated -= HandleSceneActivated;
        MainMenuEvents.OnTitleComplete -= HandleTitleComplete;
        MainMenuEvents.OnButtonsReady -= HandleButtonsReady;
        MainMenuEvents.OnMenuActivated -= HandleMenuActivated;
    }

    private IEnumerator FallbackKickoff()
    {
        startedManuallyFallback = true;
        yield return null;
        StartMenuFlow();
    }

    void Start()
    {

        if (autoStart && !startedBySceneEvent && !startedManuallyFallback)
        {
            if (!mainMenu) mainMenu = GetComponent<MainMenu>();
            if (mainMenu != null)
            {
                StartCoroutine(FallbackKickoff());
            }
        }
    }

    private void HandleMenuActivated()
    {
        // Podés enganchar lógica extra si querés
    }

    private void HandleSceneActivated()
    {
        startedBySceneEvent = true;
        if (!mainMenu) mainMenu = GetComponent<MainMenu>();
        if (mainMenu != null && autoStart)
        {
            MainMenuEvents.TriggerMenuActivated();
            StartCoroutine(DelayedTitleStart());
        }
    }

    private IEnumerator DelayedTitleStart()
    {
        yield return new WaitForSeconds(beginDelay);
        StartMenuFlow();
    }

    // 🔹 MÉTODO PÚBLICO
    public void StartMenuFlow()
    {
        if (flowStarted) return;
        flowStarted = true;

        if (!mainMenu) mainMenu = GetComponent<MainMenu>();
        if (mainMenu == null) return;

        titleCoroutine = StartCoroutine(mainMenu.StartTittleFadeIn());
    }

    private void HandleActionTriggered(string action)
    {
        switch (action)
        {
            case "PAButton":
                SkipTitleAnimation();
                break;
            case "Select":
                ActivateMenuButtons();
                break;
        }
    }

    private void SkipTitleAnimation()
    {
        if (isSkipping || titleCoroutine == null)
            return;

        isSkipping = true;
        StopCoroutine(titleCoroutine);

        if (mainMenu != null)
            mainMenu.CompleteFadeIn(mainMenu.titleContent);

        RumbleController.RumblePulse(0.1f, 0.2f, 1f);
        MainMenuEvents.TriggerTitleComplete();
    }

    private void HandleTitleComplete()
    {
        StartCoroutine(WaitBeforeEnablingButtons());
    }

    private IEnumerator WaitBeforeEnablingButtons()
    {
        // Durante esta espera el menú NO debe aceptar input
        MenuInputLock.SetBlocked(true);

        yield return new WaitForSeconds(2f);

        MainMenuEvents.TriggerButtonsReady();
    }

    private void HandleButtonsReady()
    {
        buttonsReady = true;
        MenuInputLock.SetBlocked(false);
    }

    private void ActivateMenuButtons()
    {
        bool visible = (mainMenu != null && mainMenu.AreButtonsVisible());

        if (!buttonsReady && !visible)
        {
            RumbleController.RumblePulse(0.1f, 0.1f, 0.2f);
            return;
        }

        if (visible)
        {
            buttonsReady = true;
            return;
        }

        RumbleController.RumblePulse(0.5f, 1f, 0.2f);

        if (mainMenu != null)
            mainMenu.StartCoroutine(mainMenu.ButtonsFadeIn());

        buttonsReady = false;
    }
}
