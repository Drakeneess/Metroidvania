using System.Collections;
using UnityEngine;

public class SettingsUploader : MonoBehaviour
{
    public static SettingsUploader Instance { get; private set; }

    private ApiClient apiClient;
    private bool hasChanges = false;

    private void Awake()
    {
        // --- Singleton seguro ---
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        // -------------------------

        apiClient = FindObjectOfType<ApiClient>();
    }

    private void OnEnable()
    {
        SettingsControlBase.OnAnyValueChanged += OnSettingChanged;
    }

    private void OnDisable()
    {
        SettingsControlBase.OnAnyValueChanged -= OnSettingChanged;
    }

    private void OnSettingChanged()
    {
        hasChanges = true;
    }

    /// <summary>
    /// Llamar cuando el usuario cierre el menú de opciones
    /// </summary>
    public void OnSettingsMenuClosed()
    {
        if (!hasChanges) return;

        StartCoroutine(UploadSettings());
        hasChanges = false;
    }

    private IEnumerator UploadSettings()
    {
        int playthroughId = SaveDataController.Instance.saveData.playthroughID;
        if (playthroughId <= 0)
        {
            Debug.LogWarning("❌ No hay PlaythroughID, no se puede subir settings.");
            yield break;
        }

        var gs = SettingsValue.Instance.Settings;

        SettingsPayload payload = new SettingsPayload
        {
            id_playthrough = playthroughId,
            Music = Mathf.RoundToInt(gs.music),
            Fx = Mathf.RoundToInt(gs.fxSound * 100f),
            Lang = (int)gs.language,
            Rumbling = Mathf.RoundToInt(gs.rumbleValue * 100f),
            Resolution = gs.resolutionIndex,
            Bright = Mathf.RoundToInt(gs.brightness * 100f)
        };

        string json = JsonUtility.ToJson(payload);

        yield return apiClient.PostRequest(
            "playthrough/insert_playthrough_settings.php",
            json,
            response => Debug.Log($"✅ Settings subidos: {response}"),
            error => Debug.LogError(error)
        );
    }
}

[System.Serializable]
public class SettingsPayload
{
    public int id_playthrough;
    public int Music;
    public int Fx;
    public int Lang;
    public int Rumbling;
    public int Resolution;
    public int Bright;
}
