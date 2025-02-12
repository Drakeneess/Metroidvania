using System;
using UnityEngine;
using System.Collections.Generic;

public class LanguageController : MonoBehaviour
{
    public static LanguageController Instance { get; private set; }
    private Language currentLanguage;
    private Dictionary<Language, string> languageDict = new Dictionary<Language, string>();

    // Evento para notificar sobre el cambio de idioma
    public static event Action OnLanguageChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        InitializeLanguages();
    }

    private void Start()
    {
        currentLanguage = SettingsValue.Instance.Settings.language;
        SetLanguage(currentLanguage);
    }

    // Inicializa los idiomas disponibles
    private static void InitializeLanguages()
    {
        Instance.languageDict.Add(Language.English, "en");
        Instance.languageDict.Add(Language.Spanish, "es");
    }

    // Cambiar el idioma
    public static void SetLanguage(Language language)
    {
        Instance.currentLanguage = language;
        // Disparar el evento cuando el idioma cambia
        OnLanguageChanged?.Invoke();
    }

    // Obtener el identificador del idioma
    public static string GetLanguageString()
    {
        if (Instance.languageDict.ContainsKey(Instance.currentLanguage))
        {
            return Instance.languageDict[Instance.currentLanguage];
        }

        return "en"; // Valor por defecto
    }
}

public enum Language
{
    English,
    Spanish,
}
