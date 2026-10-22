using UnityEngine;

/// <summary>
/// Keeps the former research counters available locally without sending telemetry.
/// </summary>
public class BehaviorManager : MonoBehaviour
{
    public static BehaviorManager Instance { get; private set; }

    private int inactivityCount;
    private int actionCount;
    private int socialCount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void CreateBehaviorSummary(int idPlaythrough)
    {
        ResetBehavior();
    }

    public void AddInactivity(int amount = 1) => inactivityCount += Mathf.Max(0, amount);
    public void AddAction(int amount = 1) => actionCount += Mathf.Max(0, amount);
    public void AddSocial(int amount = 1) => socialCount += Mathf.Max(0, amount);

    public int GetInactivity() => inactivityCount;
    public int GetAction() => actionCount;
    public int GetSocial() => socialCount;

    public void SyncBehaviorSummary()
    {
        // Intentionally local-only. Thesis telemetry was removed from the game runtime.
    }

    public void SyncBehaviorSummaryFireAndForget()
    {
        // Intentionally local-only. No network request is performed on shutdown.
    }

    public void ResetBehavior()
    {
        inactivityCount = 0;
        actionCount = 0;
        socialCount = 0;
    }
}
