using UnityEngine;

public class PlaythroughManager : MonoBehaviour
{
    public static PlaythroughManager Instance { get; private set; }
    private ApiClient apiClient;

    private void Start()
    {
        Instance = this;
        apiClient = GetComponent<ApiClient>();
        if (SaveDataController.AreSavedData() && SaveDataController.Instance.saveData.studentID != -1)
        {
            FindPlaythroughID(SaveDataController.Instance.saveData.studentID);
        }      
    }

    public void FindPlaythroughID(int studentID)
    {
        CreateOrGetPlaythrough(studentID, "v1.0");
    }

    public void CreateOrGetPlaythrough(int studentId, string version)
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
                }
            },
            error => Debug.LogError(error)
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
