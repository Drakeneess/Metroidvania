using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Compatibility shim for the thesis-era backend.
/// Runtime database access is intentionally disabled in the standalone game.
/// </summary>
public class ApiClient : MonoBehaviour
{
    private const string DisabledMessage = "Research backend is disabled in this build.";

    public IEnumerator PostRequest(string endpoint, string jsonData, Action<string> onSuccess, Action<string> onError)
    {
        Debug.LogWarning($"[ApiClient] {DisabledMessage} Ignored POST: {endpoint}");
        onError?.Invoke(DisabledMessage);
        yield break;
    }

    public IEnumerator GetRequest(string endpoint, Action<string> onSuccess, Action<string> onError)
    {
        Debug.LogWarning($"[ApiClient] {DisabledMessage} Ignored GET: {endpoint}");
        onError?.Invoke(DisabledMessage);
        yield break;
    }
}

[Serializable]
public class GenericResponse<T>
{
    public bool success;
    public string message;
    public T data;
}
