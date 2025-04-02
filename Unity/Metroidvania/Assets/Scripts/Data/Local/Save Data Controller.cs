using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public class SaveDataController : MonoBehaviour
{
    public static SaveDataController Instance { get; private set; }

    private string saveFilePath;

    // Referencia al objeto SaveData que contiene los datos que queremos guardar
    public SaveData saveData;

    private void Awake() 
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destruye este objeto si ya existe una instancia
        }
        else
        {
            Instance = this; // Asigna la instancia
        }

        saveFilePath = GetSavePath();
        
        // Comprobamos si los datos guardados existen
        if (AreSavedData())
        {
            LoadData();
        }
        else
        {
            // Es la primera vez que se ejecuta, inicializa con valores predeterminados
            saveData = new SaveData();
        }
    }

    // Método para comprobar si existen datos guardados
    public static bool AreSavedData()
    {
        return File.Exists(Instance.saveFilePath) && new FileInfo(Instance.saveFilePath).Length > 0;
    }

    private static string GetSavePath()
    {
        return Application.dataPath + "/Resources/SaveData.save";
    }

    public static void SaveData()
    {
        string path = GetSavePath();
        try
        {
            using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, Instance.saveData);
            }
        }
        catch (IOException ex)
        {
            Debug.LogError("Error al guardar los datos: " + ex.Message);
        }
    }

    public static SaveData LoadData()
{
    string path = GetSavePath();
    if (File.Exists(path))
    {
        try
        {
            // Verificar si el archivo está vacío
            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Length == 0)
            {
                Debug.LogWarning("El archivo de guardado está vacío.");
                return null;  // Retorna null si el archivo está vacío
            }

            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(stream) as SaveData;
            }
        }
        catch (IOException ex)
        {
            Debug.LogError("Error al cargar los datos: " + ex.Message);
            return null;
        }
        catch (SerializationException ex)
        {
            Debug.LogError("Error de deserialización: " + ex.Message);
            return null;
        }
    }
    else
    {
        Debug.LogWarning("Archivo de guardado no encontrado en " + path);
        return null;
    }
}

}

[System.Serializable]
public class SaveData
{
    public string playerName = "";
    public float timePlayed = 0f;
    public int lastCheckpointIndex = -1;
    public int[] weaponsID= null;
    public int currentWeapon = 0;
    public int[] weaponsLevel = null;
    public int[] toolsID=null;
    public int[] skillID=null;
    public int[] skillLevel=null;
}
