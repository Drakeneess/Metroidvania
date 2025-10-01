using UnityEngine;

public class BdiTestResultManager : MonoBehaviour
{
    public static BdiTestResultManager Instance { get; private set; }

    private ApiClient apiClient;
    private string insertBdiEndpoint = "bdi/insert_bdi_test_result.php";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // opcional si quieres persistir entre escenas
    }

    private void Start()
    {
        apiClient = GetComponent<ApiClient>();
        if (apiClient == null)
        {
            Debug.LogError("❌ ApiClient no está en el mismo GameObject que BdiTestResultManager.");
        }
    }

    /// <summary>
    /// Envía una decisión del BDI al backend.
    /// </summary>
    public void SendDecision(int idResponse)
    {
        if (SaveDataController.Instance == null)
        {
            Debug.LogError("❌ SaveDataController.Instance es null, no se puede obtener el id_playthrough.");
            return;
        }

        var payload = new BdiDecisionPayload
        {
            id_playthrough = SaveDataController.Instance.saveData.playthroughID,
            id_response = idResponse
        };

        string jsonData = JsonUtility.ToJson(payload);
        Debug.Log("📤 Enviando decisión BDI: " + jsonData);

        StartCoroutine(apiClient.PostRequest(
            insertBdiEndpoint,
            jsonData,
            response =>
            {
                var parsed = JsonUtility.FromJson<GenericResponse<BdiDecisionResponse>>(response);
                if (parsed.success)
                {
                    Debug.Log($"✅ Decisión guardada (ResponseID={parsed.data.id_response}).");
                }
                else
                {
                    Debug.LogError("❌ No se pudo guardar la decisión: " + parsed.message);
                }
            },
            error => Debug.LogError(error)
        ));
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
    public string id_test; // opcional si lo devuelves del SP
}
