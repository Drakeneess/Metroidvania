using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }
    public MenuContainer menuPauseContainer;

    [SerializeField] private PauseMenu pauseMenu;

    private bool isPaused = false;
    private bool isExiting = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnEnable()
    {
        InputActionController.Instance.OnActionTriggered += Pause;
    }

    private void OnDisable()
    {
        if (InputActionController.Instance != null)
            InputActionController.Instance.OnActionTriggered -= Pause;
    }

    private void Pause(string actionName)
    {
        if (actionName != "Pause") return;
        TogglePause();
    }

    public void TogglePause()
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        if (isPaused) return;
        isPaused = true;

        MenuTransition.Instance.SetInitial(menuPauseContainer);
        MenuInputLock.SetBlocked(true);
        GameMenuController.CurrentMode = GameMode.Menu;
        pauseMenu.Show();
    }

    public void ResumeGame()
    {
        if (!isPaused) return;
        isPaused = false;

        pauseMenu.Hide();
        MenuInputLock.SetBlocked(false);
        GameMenuController.CurrentMode = GameMode.Game;
    }

    public void ExitGame()
    {
        if (!isExiting)
            StartCoroutine(ExitRoutine());
    }

    private System.Collections.IEnumerator ExitRoutine()
    {
        isExiting = true;

        Debug.Log("[PauseManager] ExitRoutine started.");

        // 1️⃣ Cerrar sesión en el servidor
        if (GameSessionManager.Instance != null)
        {
            Debug.Log("[PauseManager] Calling EndGameSession...");
            GameSessionManager.Instance.EndGameSession();
        }

        // 2️⃣ Esperar 3 segundos (o el tiempo que decidas)
        yield return new WaitForSeconds(3f);

        Debug.Log("[PauseManager] Quitting application...");

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}
