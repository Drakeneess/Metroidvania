using UnityEngine;

/// <summary>
/// Compatibility shell for the thesis playthrough identifier.
/// Progression is now owned by local SaveData instead of a remote record.
/// </summary>
public class PlaythroughManager : MonoBehaviour
{
    public static PlaythroughManager Instance { get; private set; }

    public static event System.Action<int, int> OnPlaythroughReady;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void FindPlaythroughID(int studentID)
    {
        Debug.LogWarning("[PlaythroughManager] Remote playthrough IDs are disabled; local save data is used instead.");
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
