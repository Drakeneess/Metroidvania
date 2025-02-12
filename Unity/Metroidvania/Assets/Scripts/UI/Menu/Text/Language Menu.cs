using System.Collections.Generic;
using UnityEngine;

public class LanguageMenu : MonoBehaviour
{
    public static LanguageMenu Instance { get; private set; }
    private string language = "en"; // Idioma por defecto
    private Dictionary<string, string> translations = new Dictionary<string, string>();
    private MenuUIText[] menuUITexts;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Si ya hay una instancia, destrúyela
        }
        else
        {
            Instance = this; // Asigna esta instancia
            DontDestroyOnLoad(gameObject); // No destruir entre escenas
        }

        // Subscribir a los cambios de idioma
        LanguageController.OnLanguageChanged += OnLanguageChanged;
    }

    private void Start()
    {
        // Cargar el idioma guardado o establecer por defecto
        language = LanguageController.GetLanguageString();
        LoadTranslations();

        // Actualizar todos los textos del UI
        UpdateAllTexts();
    }

    private void OnDestroy()
    {
        // Des-suscribirse del evento cuando el objeto es destruido
        LanguageController.OnLanguageChanged -= OnLanguageChanged;
    }

    // Método que se llama cuando el idioma cambia
    private void OnLanguageChanged()
    {
        language = LanguageController.GetLanguageString();
        LoadTranslations();
        UpdateAllTexts();
    }

    // Cargar las traducciones desde un archivo CSV
    private void LoadTranslations()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("UI");
        if (csvFile == null)
        {
            Debug.LogError("No se encontró el archivo CSV en Resources.");
            return;
        }

        translations.Clear(); // Limpiar el diccionario antes de cargar

        string[] lines = csvFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++) // Ignorar la cabecera
        {
            string[] columns = lines[i].Split(',');

            if (columns.Length < 3) continue; // Asegurarse de que hay al menos clave e idioma

            string key = columns[0]; // Primera columna es la clave
            int langIndex = language == "es" ? 1 : 2; // 1 para español, 2 para inglés

            if (langIndex < columns.Length)
            {
                translations[key] = columns[langIndex]; // Asignar la traducción al diccionario
            }
        }
    }

    // Obtener una traducción por clave
    public static string GetTranslate(string key)
    {
        if (Instance != null && Instance.translations.ContainsKey(key))
        {
            return Instance.translations[key];
        }

        return key; // Retornar la clave si no se encuentra traducción
    }

    // Actualizar todos los textos del UI
    private void UpdateAllTexts()
    {
        menuUITexts = FindObjectsOfType<MenuUIText>();
        foreach (MenuUIText uiText in menuUITexts)
        {
            uiText.UpdateText();
        }
    }
}
