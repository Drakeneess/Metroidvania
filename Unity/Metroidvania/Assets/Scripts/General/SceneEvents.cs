using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneEvents : MonoBehaviour
{
    public static SceneEvents Instance { get; private set; }

    /// <summary>
    /// Evento global que se dispara cada vez que una escena termina de cargar
    /// </summary>
    public static event Action<Scene, LoadSceneMode> OnSceneLoadedGlobal;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Suscribir al evento nativo de Unity
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        OnSceneLoadedGlobal?.Invoke(scene, mode);
    }
}
