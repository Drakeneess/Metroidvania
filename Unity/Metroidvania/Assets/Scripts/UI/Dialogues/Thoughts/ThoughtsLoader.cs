using System.Collections.Generic;
using UnityEngine;

public class ThoughtsLoader : MonoBehaviour
{
    public static ThoughtsLoader Instance { get; private set; }

    // id -> (lang -> text)
    private readonly Dictionary<string, Dictionary<string, string>> thoughts = new();

    // Idioma actual (se mantiene actualizado)
    public string CurrentLanguage { get; private set; } = "en";

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        LoadCsv();
        LanguageController.OnLanguageChanged += OnLanguageChanged;
    }

    private void Start()
    {
        CurrentLanguage = LanguageController.GetLanguageString();
    }

    private void OnDestroy()
    {
        LanguageController.OnLanguageChanged -= OnLanguageChanged;
    }

    private void OnLanguageChanged()
    {
        CurrentLanguage = LanguageController.GetLanguageString();
    }

    /// <summary>
    /// Estructura de CSV esperada (en Resources/thoughts.csv):
    /// id,en,es,pt
    /// no_mirror,"I can't leave without the mirror","No puedo salir sin el espejo","Não posso sair sem o espelho"
    /// </summary>
    private void LoadCsv()
    {
        TextAsset csv = Resources.Load<TextAsset>("Thoughts");
        if (!csv)
        {
            Debug.LogError("[ThoughtsLoader] No se encontró Resources/thoughts.csv");
            return;
        }

        string[] lines = csv.text.Split('\n');
        if (lines.Length <= 1) return;

        // Parse headers (idiomas)
        var headers = SplitCsvLine(lines[0]);
        if (headers.Count < 2)
        {
            Debug.LogError("[ThoughtsLoader] Encabezados insuficientes. Se espera: id,en,es,...");
            return;
        }

        // headers[0] = "id"; headers[1..] = idiomas
        List<string> langs = new();
        for (int i = 1; i < headers.Count; i++)
        {
            string lang = headers[i].Trim();
            if (!string.IsNullOrEmpty(lang)) langs.Add(lang);
        }

        // Filas
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            var cols = SplitCsvLine(lines[i]);
            if (cols.Count < 1) continue;

            string id = cols[0].Trim();
            if (string.IsNullOrEmpty(id)) continue;

            if (!thoughts.ContainsKey(id))
                thoughts[id] = new Dictionary<string, string>();

            for (int c = 1; c < cols.Count && (c - 1) < langs.Count; c++)
            {
                string lang = langs[c - 1];
                string text = cols[c].Trim();
                if (!thoughts[id].ContainsKey(lang))
                    thoughts[id][lang] = text;
                else
                    thoughts[id][lang] = text; // override si se repite
            }
        }

        // Debug opcional:
        // Debug.Log($"[ThoughtsLoader] Cargados {thoughts.Count} thought IDs.");
    }

    /// <summary> Devuelve texto por id y lang, con fallback (lang actual → "en" → primer idioma disponible → id). </summary>
    public string GetText(string id, string lang = null)
    {
        lang ??= CurrentLanguage;

        if (!thoughts.TryGetValue(id, out var perLang))
            return id; // fallback extremo: muestra id

        // 1) idioma solicitado
        if (!string.IsNullOrEmpty(lang) && perLang.TryGetValue(lang, out var t1) && !string.IsNullOrEmpty(t1))
            return t1;

        // 2) fallback "en"
        if (perLang.TryGetValue("en", out var t2) && !string.IsNullOrEmpty(t2))
            return t2;

        // 3) primer idioma disponible
        foreach (var kv in perLang)
        {
            if (!string.IsNullOrEmpty(kv.Value)) return kv.Value;
        }

        // 4) id
        return id;
    }

    // Parser CSV básico que respeta comillas y comas dentro de comillas
    private List<string> SplitCsvLine(string line)
    {
        List<string> result = new();
        if (line == null) return result;

        bool inQuotes = false;
        var cur = new System.Text.StringBuilder();

        for (int i = 0; i < line.Length; i++)
        {
            char ch = line[i];

            if (ch == '\"')
            {
                // Doble comilla "" -> se interpreta como comilla literal
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '\"')
                {
                    cur.Append('\"');
                    i++; // saltar la segunda
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (ch == ',' && !inQuotes)
            {
                result.Add(cur.ToString());
                cur.Clear();
            }
            else
            {
                cur.Append(ch);
            }
        }
        result.Add(cur.ToString());
        return result;
    }
}
