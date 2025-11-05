using UnityEngine;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { private set; get; }

    [SerializeField] private Player player;
    [SerializeField] private GameOverMenu gameOverMenu;
    [SerializeField] private BackgroundFader_UIRawImage backgroundFader;

    [SerializeField] private float dieTime = 1.2f;
    [SerializeField] private float fadeToBlackTime = 1f;
    [SerializeField] private float fadeToGameTime = 1f;

    private bool canRespawn = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void TriggerGameOver()
    {
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        canRespawn = false;

        // Wait for death animation
        yield return new WaitForSeconds(dieTime);

        // Fade to black
        FadeController.Instance.FadeIn(fadeToBlackTime);
        backgroundFader.FadeToMenu();
        yield return new WaitForSeconds(fadeToBlackTime);

        // Show Game Over UI (with its own fade)
        gameOverMenu.Show();
        EnemyManager.Instance.ResetAllEnemies();
        // Switch to menu mode
        GameMenuController.CurrentMode = GameMode.Menu;

        // Allow respawn now
        canRespawn = true;
    }

    public void TryRespawn()
    {
        if (!canRespawn) return;
        StartCoroutine(RespawnSequence());
    }

    private IEnumerator RespawnSequence()
    {
        canRespawn = false;

        // Fade out menu UI (its own fade)
        gameOverMenu.Hide();
        // Respawn player
        player.Respawn();
        yield return new WaitForSeconds(gameOverMenu.FadeDuration); // need public FadeDuration

        // Fade screen back to gameplay
        backgroundFader.FadeToGame();
        FadeController.Instance.FadeOut(fadeToGameTime);
        yield return new WaitForSeconds(fadeToGameTime);

        // Now return to gameplay
        GameMenuController.CurrentMode = GameMode.Game;
    }
}
