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
        Instance = this;
        
        settingsFilePath = Application.persistentDataPath + "/settings.json";
        LoadSettings();
    }

    /// <summary>
    /// Elimina settings.json, save.json y PlayerPrefs para simular instalación nueva
    /// </summary>
    private void ResetForFreshInstall()
    {
        // 🧽 1. Borrar settings.json
        string settingsFilePath = Application.persistentDataPath + "/settings.json";
        if (System.IO.File.Exists(settingsFilePath))
        {
            System.IO.File.Delete(settingsFilePath);
            Debug.Log("🧽 settings.json eliminado.");
        }

        // 💾 2. Borrar save.json (ajusta nombre si tu save usa otro)
        string saveFilePath = Application.persistentDataPath + "/save.json";
        if (System.IO.File.Exists(saveFilePath))
        {
            System.IO.File.Delete(saveFilePath);
            Debug.Log("💾 save.json eliminado.");
        }

        // 🧯 3. Borrar PlayerPrefs
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("🧯 PlayerPrefs eliminados.");

        Debug.Log("✅ Fresh Install ejecutado.");
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
    public float rumbleValue = 0.5f;
    public float fxSound = 0.5f;
    public float music = 50f;
    public float brightness = 0.5f;
    public int resolutionIndex = 0;
    public Language language = Language.Español;
}
