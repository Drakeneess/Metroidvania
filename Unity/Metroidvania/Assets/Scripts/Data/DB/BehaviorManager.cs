using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class BehaviorManager : MonoBehaviour
{
    public static BehaviorManager Instance { get; private set; }

    [Header("Backend endpoints")]
    private string createBehaviorEndpoint = "behavior/create.php";
    private string updateBehaviorEndpoint = "behavior/update.php";

    public int currentSummaryId = -1;

    // 🔹 Valores acumulados en memoria
    private int inactivityCount = 0;
    private int actionCount = 0;
    private int socialCount = 0;

    private ApiClient apiClient;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        apiClient = GetComponent<ApiClient>();
    }

    // ────────────────────────────────
    // CREAR SUMMARY
    // ────────────────────────────────
    public void CreateBehaviorSummary(int idPlaythrough)
    {
        var payload = new CreateBehaviorPayload
        {
            id_playthrough = idPlaythrough
        };

        string jsonData = JsonUtility.ToJson(payload);

        StartCoroutine(apiClient.PostRequest(
            createBehaviorEndpoint,
            jsonData,
            response =>
            {
                var parsed = JsonUtility.FromJson<GenericResponse<CreateBehaviorResponse>>(response);
                if (parsed.success)
                {
                    currentSummaryId = parsed.data.id_summary;
                    Debug.Log($"✅ Behavior summary creado/obtenido con ID {currentSummaryId}");
                }
                else Debug.LogError("❌ No se pudo crear/obtener summary: " + parsed.message);
            },
            error => Debug.LogError(error)
        ));
    }

    // ────────────────────────────────
    // ACTUALIZAR ESTADO LOCAL
    // ────────────────────────────────
    public void AddInactivity(int amount = 1) => inactivityCount += amount;
    public void AddAction(int amount = 1) => actionCount += amount;
    public void AddSocial(int amount = 1) => socialCount += amount;

    public int GetInactivity() => inactivityCount;
    public int GetActions() => actionCount;
    public int GetSocial() => socialCount;

    // ────────────────────────────────
    // ENVIAR DATOS AL BACKEND (confirmación normal)
    // ────────────────────────────────
    public void SyncBehaviorSummary()
    {
        if (currentSummaryId <= 0)
        {
            Debug.LogWarning("⚠ No hay summary activo para sincronizar.");
            return;
        }

        var payload = new UpdateBehaviorPayload
        {
            id_summary = currentSummaryId,
            inactivity_periods = inactivityCount,
            total_actions = actionCount,
            social_interactions = socialCount
        };

        string jsonData = JsonUtility.ToJson(payload);

        Debug.Log($"📤 Subiendo behavior summary (normal): {jsonData}");

        StartCoroutine(apiClient.PostRequest(
            updateBehaviorEndpoint,
            jsonData,
            response =>
            {
                var parsed = JsonUtility.FromJson<GenericResponse<UpdateBehaviorResponse>>(response);
                if (parsed.success)
                {
                    Debug.Log("✅ Behavior summary sincronizado al backend.");
                }
                else Debug.LogError("❌ No se pudo sincronizar summary: " + parsed.message);
            },
            error => Debug.LogError(error)
        ));
    }

    // ────────────────────────────────
    // ENVIAR DATOS AL BACKEND (fire-and-forget)
    // ────────────────────────────────
    public void SyncBehaviorSummaryFireAndForget()
    {
        if (currentSummaryId <= 0) return;

        var payload = new UpdateBehaviorPayload
        {
            id_summary = currentSummaryId,
            inactivity_periods = inactivityCount,
            total_actions = actionCount,
            social_interactions = socialCount
        };

        string jsonData = JsonUtility.ToJson(payload);

        var request = new UnityWebRequest("https://tuservidor.com/" + updateBehaviorEndpoint, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // 🚀 Fire-and-forget (no yield, no espera de respuesta)
        request.SendWebRequest();

        Debug.Log("📤 Behavior enviado (fire-and-forget).");
    }

    // ────────────────────────────────
    // RESET AL INICIAR UNA NUEVA PARTIDA
    // ────────────────────────────────
    public void ResetBehavior()
    {
        inactivityCount = 0;
        actionCount = 0;
        socialCount = 0;
        currentSummaryId = -1;
    }

    // ────────────────────────────────
    // CIERRE FORZADO (Alt+F4, cerrar app)
    // ────────────────────────────────
    private void OnApplicationQuit()
    {
        SyncBehaviorSummaryFireAndForget();
    }
}

// ────────────────────────────────
// PAYLOADS Y RESPUESTAS
// ────────────────────────────────
[System.Serializable]
public class CreateBehaviorPayload
{
    public int id_playthrough;
}

[System.Serializable]
public class CreateBehaviorResponse
{
    public int id_summary;
}

[System.Serializable]
public class UpdateBehaviorPayload
{
    public int id_summary;
    public int inactivity_periods;
    public int total_actions;
    public int social_interactions;
}

[System.Serializable]
public class UpdateBehaviorResponse
{
    public int id_summary;
    public int inactivity_periods;
    public int total_actions;
    public int social_interactions;
}
