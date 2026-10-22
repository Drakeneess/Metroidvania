using UnityEngine;

/// <summary>
/// Compatibility component for thesis-era BDI result submission.
/// Decisions are no longer sent or stored remotely.
/// </summary>
public class BdiTestResultManager : MonoBehaviour
{
    public static BdiTestResultManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SendDecision(int idResponse)
    {
        Debug.LogWarning($"[BdiTestResultManager] Ignored legacy BDI response {idResponse}; research submission is disabled.");
    }
}

[System.Serializable]
public class BdiDecisionPayload
{
    public int id_playthrough;
    public int id_response;
}

[System.Serializable]
public class BdiDecisionResponse
{
    public string id_playthrough;
    public string id_response;
    public string id_test;
}
