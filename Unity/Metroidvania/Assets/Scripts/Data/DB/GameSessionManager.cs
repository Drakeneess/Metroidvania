using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSessionManager : MonoBehaviour
{
    public static GameSessionManager Instance { get; private set; }

    [Header("Backend endpoints")]
    private string createSessionEndpoint = "session/create.php";
    private string endSessionEndpoint = "session/end.php";

    public int currentSessionId = -1;
    private ApiClient apiClient;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        apiClient = GetComponent<ApiClient>();
    }

    // 🔹 Inicia la sesión en el backend
    public void StartGameSession()
    {
        var payload = new CreateSessionPayload
        {
            id_playthrough = SaveDataController.Instance.saveData.playthroughID,
            device_type = SystemInfo.deviceType.ToString()
        };

        string jsonData = JsonUtility.ToJson(payload);
        Debug.Log("📤 Enviando JSON: " + jsonData);

        StartCoroutine(apiClient.PostRequest(
            createSessionEndpoint,
            jsonData,
            response =>
            {
                var parsed = JsonUtility.FromJson<GenericResponse<SessionResponse>>(response);
                if (parsed.success)
                {
                    currentSessionId = int.Parse(parsed.data.id_session);
                    BehaviorManager.Instance.CreateBehaviorSummary(SaveDataController.Instance.saveData.playthroughID);
                    Debug.Log($"✅ Sesión iniciada con ID {currentSessionId}");
                }
                else Debug.LogError("❌ No se pudo iniciar la sesión: " + parsed.message);
            },
            error => Debug.LogError(error)
        ));
    }

    // 🔹 Termina la sesión y manda el log del PlayerActionLogger
    public void EndGameSession()
    {
        if (currentSessionId <= 0)
        {
            Debug.LogWarning("⚠ No hay sesión activa para cerrar.");
            return;
        }

        // ✅ Obtener todas las acciones del logger
        var actions = PlayerActionLogger.Instance.GetActions();
        PlayerActionList wrapper = new PlayerActionList { actions = new List<PlayerAction>(actions) };

        // Si no hay acciones, al menos mandamos un objeto vacío
        string logJson = actions.Count > 0
            ? JsonUtility.ToJson(wrapper)
            : "{}";

        var payload = new EndSessionPayload
        {
            id_session = currentSessionId,
            log_data = logJson
        };

        string jsonData = JsonUtility.ToJson(payload);
        Debug.Log("📤 Enviando JSON de cierre: " + jsonData);

        StartCoroutine(apiClient.PostRequest(
            endSessionEndpoint,
            jsonData,
            response =>
            {
                var parsed = JsonUtility.FromJson<GenericResponse<object>>(response);
                if (parsed.success)
                {
                    Debug.Log("✅ Sesión cerrada correctamente.");
                    currentSessionId = -1;
                }
                else Debug.LogError("❌No se pudo cerrar la sesión: " + parsed.message);
            },
            error => Debug.LogError(error)
        ));
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

