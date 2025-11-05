using UnityEngine;

public class PlaythroughManager : MonoBehaviour
{
    public static PlaythroughManager Instance { get; private set; }
    private ApiClient apiClient;

    /// <summary>
    /// Evento que se dispara cuando ya tenemos studentID y playthroughID listos.
    /// Param1 = studentID, Param2 = playthroughID
    /// </summary>
    public static event System.Action<int, int> OnPlaythroughReady;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        apiClient = apiClient ?? GetComponent<ApiClient>();

        // Caso: Ya existen datos guardados (segunda ejecución y posteriores)
        if (SaveDataController.AreSavedData())
        {
            int sid = SaveDataController.Instance.saveData.studentID;
            int pid = SaveDataController.Instance.saveData.playthroughID;

            if (sid != -1 && pid != -1)
            {
                Debug.Log("▶️ PlaythroughManager: Datos existentes detectados. Disparando OnPlaythroughReady.");
                OnPlaythroughReady?.Invoke(sid, pid);
            }
            else if (sid != -1 && pid == -1)
            {
                // Tenemos studentID pero nunca creamos playthrough
                Debug.Log("ℹ️ PlaythroughManager: Hay studentID pero no playthroughID. Creando...");
                FindPlaythroughID(sid);
            }
        }
    }

    /// <summary>
    /// Llamado por StudentManager cuando se obtiene por primera vez el studentID
    /// </summary>
    public void FindPlaythroughID(int studentID)
    {
        CreateOrGetPlaythrough(studentID, "v1.0");
    }

    private void CreateOrGetPlaythrough(int studentId, string version)
    {
        PlaythroughData data = new PlaythroughData
        {
            id_student = studentId,
            version = version
        };

        string jsonData = JsonUtility.ToJson(data);

        StartCoroutine(apiClient.PostRequest(
            "playthrough/create_unique.php",
            jsonData,
            response =>
            {
                var parsed = JsonUtility.FromJson<UniquePlaythroughResponseWrapper>(response);

                if (parsed.success && parsed.data != null)
                {
                    int playthroughId = int.Parse(parsed.data.id_playthrough);
                    SaveDataController.Instance.saveData.playthroughID = playthroughId;

                    Debug.Log($"🎮 Playthrough listo: {playthroughId} (existing: {parsed.data.existing})");

                    // ✅ Ahora sí tenemos ambos IDs → Disparar evento
                    OnPlaythroughReady?.Invoke(studentId, playthroughId);
                }
                else
                {
                    Debug.LogError("❌ Error: respuesta inválida al crear/obtener playthrough.");
                }
            },
            error =>
            {
                Debug.LogError($"❌ Error PlaythroughManager: {error}");
            }
        ));
    }
}

[System.Serializable]
public class PlaythroughData
{
    public int id_student;
    public string version;
}

[System.Serializable]
public class UniquePlaythroughData
{
    public string id_playthrough;
    public bool existing;
}

[System.Serializable]
public class UniquePlaythroughResponseWrapper : GenericResponse<UniquePlaythroughData> { }
