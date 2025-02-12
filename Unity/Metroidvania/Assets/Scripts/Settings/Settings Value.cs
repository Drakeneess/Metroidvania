using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsValue : MonoBehaviour
{
    [Header("Settings")]
    public GameSettings Settings;

    public static SettingsValue Instance { get; private set; }
    private string settingsFilePath;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        settingsFilePath = Application.persistentDataPath + "/settings.json";
        LoadSettings();
    }


    public void LoadSettings()
    {
        if (!System.IO.File.Exists(settingsFilePath))
        {
            // Copiar un archivo predeterminado desde streamingAssetsPath (si existe)
            string defaultFilePath = Application.streamingAssetsPath + "/settings.json";
            if (System.IO.File.Exists(defaultFilePath))
            {
                System.IO.File.Copy(defaultFilePath, settingsFilePath);
            }
            else
            {
                // Crear un archivo de configuración nuevo si no existe el predeterminado
                Settings = new GameSettings();
                SaveSettings();
            }
        }
        else
        {
            string json = System.IO.File.ReadAllText(settingsFilePath);
            Settings = JsonUtility.FromJson<GameSettings>(json);
        }
    }

    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(Settings, true);
        System.IO.File.WriteAllText(settingsFilePath, json);
    }
}

[System.Serializable]
public class GameSettings
{
    public float rumbleValue = 1f;
    public float volume = 0.5f;
    public int resolutionIndex = 0;
    public Language language = Language.Spanish;
}