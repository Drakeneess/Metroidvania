using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System;

public class ApiClient : MonoBehaviour
{
    private static readonly string baseUrl = "http://localhost/shdow_of_souls_backend/api/";

    public IEnumerator PostRequest(string endpoint, string jsonData, Action<string> onSuccess, Action<string> onError)
    {
        using (UnityWebRequest request = new UnityWebRequest(baseUrl + endpoint, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                onError?.Invoke($"❌ Error: {request.error} - {request.downloadHandler.text}");
            }
        }
    }

    public IEnumerator GetRequest(string endpoint, Action<string> onSuccess, Action<string> onError)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(baseUrl + endpoint))
        {
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                onError?.Invoke($"❌ Error: {request.error} - {request.downloadHandler.text}");
            }
        }
    }
}

[System.Serializable]
public class GenericResponse<T>
{
    public bool success;
    public string message;
    public T data;
}

