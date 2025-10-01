using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance { get; private set; }
    public static int currentScene { get; private set; }

    public AsyncOperation asyncLoad { get; private set; }

    // ✅ Evento que notifica cuando la escena se activa
    public static event Action OnSceneActivated;

    void Start()
    {
        if (instance != null && instance != this)
        {
            
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        currentScene = SaveDataController.Instance.saveData.lastScene;
        print(currentScene);

        if (currentScene == 0)
        {
            // Escena inicial → Carga controlada
            StartCoroutine(LoadSceneWhenReady(1));
        }
        else
        {
            // Escena de juego → Carga inmediata
            LoadScene(currentScene, false);
        }
    }

    public static void LoadScene(int sceneIndex, bool savingData = true)
    {
        SceneManager.LoadScene(sceneIndex); // Carga directa
        currentScene = sceneIndex;

        SaveDataController.Instance.saveData.lastCheckpointIndex = currentScene;
        if (savingData) SaveDataController.SaveData();

        // ✅ Notificar que la escena ya está activa inmediatamente
        OnSceneActivated?.Invoke();
    }

    private IEnumerator LoadSceneWhenReady(int sceneIndex)
    {
        asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        asyncLoad.allowSceneActivation = false;

        // Espera a que la escena llegue al 90% (precargada)
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }
    }

    public void ActivateSceneManually()
    {
        if (asyncLoad != null && asyncLoad.progress >= 0.9f)
        {
            StartCoroutine(ActivateSceneCoroutine());
        }
    }

    private IEnumerator ActivateSceneCoroutine()
    {
        asyncLoad.allowSceneActivation = true;

        // Espera a que Unity termine de hacer el cambio
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        SaveDataController.Instance.saveData.lastScene = 1;
        
        SaveDataController.SaveData();
        // 🔹 Disparar el evento para notificar a cualquier listener
        OnSceneActivated?.Invoke();
    }
}
