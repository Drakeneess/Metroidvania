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

        yield return new WaitForSeconds(dieTime);

        FadeController.Instance.FadeIn(fadeToBlackTime);
        backgroundFader.FadeToMenu();

        yield return new WaitForSeconds(fadeToBlackTime);

        gameOverMenu.Show();

        EnemyManager.Instance.ResetAllEnemies();
        GameMenuController.CurrentMode = GameMode.Menu;

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

        gameOverMenu.Hide();

        player.Respawn();
        yield return new WaitForSeconds(gameOverMenu.FadeDuration);

        backgroundFader.FadeToGame();
        FadeController.Instance.FadeOut(fadeToGameTime);

        yield return new WaitForSeconds(fadeToGameTime);

        GameMenuController.CurrentMode = GameMode.Game;
    }
}
