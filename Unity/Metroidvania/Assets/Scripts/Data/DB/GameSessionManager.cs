using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Compatibility shell for the former research session lifecycle.
/// The standalone game no longer creates or closes remote sessions.
/// </summary>
public class GameSessionManager : MonoBehaviour
{
    public static GameSessionManager Instance { get; private set; }

    public int currentSessionId = -1;

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

    public void StartGameSession()
    {
        currentSessionId = -1;
    }

    public void EndGameSession()
    {
        currentSessionId = -1;
    }
}

[System.Serializable]
public class SessionResponse
{
    public string id_session;
}

[System.Serializable]
public class PlayerActionList
{
    public List<PlayerAction> actions;
}

[System.Serializable]
public class CreateSessionPayload
{
    public int id_playthrough;
    public string device_type;
}

[System.Serializable]
public class EndSessionPayload
{
    public int id_session;
    public string log_data;
}
