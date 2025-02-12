using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueLoader : MonoBehaviour
{
    public static DialogueLoader Instance { get; private set; }
    public string currentLanguage = "en";
    // Diccionario para almacenar diálogos: idioma -> personaje -> lista de diálogos indexados
    private Dictionary<string, Dictionary<string, Dictionary<int, string[]>>> dialogues;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            LoadDialogues();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentLanguage = LanguageController.GetLanguageString();
    }

    // Cargar los diálogos desde el archivo CSV
    private void LoadDialogues()
    {
        dialogues = new Dictionary<string, Dictionary<string, Dictionary<int, string[]>>>();

        TextAsset csvFile = Resources.Load<TextAsset>("dialogues"); // Cargar el archivo CSV
        if (csvFile == null)
        {
            Debug.LogError("No se encontró el archivo CSV en Resources.");
            return;
        }

        string[] lines = csvFile.text.Split('\n');
        string[] headers = lines[0].Split(','); // Encabezados: character, index, es, en

        // Procesar cada línea de diálogo
        for (int i = 1; i < lines.Length; i++) // Saltar la primera línea (encabezados)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');
            if (values.Length < 4) continue; // Asegurarse de que la línea tenga todos los campos

            string character = values[0].Trim();
            int index = int.Parse(values[1].Trim());
            for (int j = 2; j < values.Length; j++) // Procesar idiomas
            {
                string language = headers[j].Trim();
                string dialogue = values[j].Trim();

                // Separar el diálogo por "|", dividiendo en frases
                string[] sentences = dialogue.Split('|');

                if (!dialogues.ContainsKey(language))
                {
                    dialogues[language] = new Dictionary<string, Dictionary<int, string[]>>();
                }

                if (!dialogues[language].ContainsKey(character))
                {
                    dialogues[language][character] = new Dictionary<int, string[]>();
                }

                dialogues[language][character][index] = sentences;
            }
        }
    }

    // Obtener el diálogo por idioma, personaje e índice
    public static string[] GetDialogue(string character, int index)
    {
        if (Instance.dialogues.ContainsKey(Instance.currentLanguage) &&
            Instance.dialogues[Instance.currentLanguage].ContainsKey(character) &&
            Instance.dialogues[Instance.currentLanguage][character].ContainsKey(index))
        {
            return Instance.dialogues[Instance.currentLanguage][character][index];
        }

        Debug.LogWarning($"No se encontró el diálogo para idioma '{Instance.currentLanguage}', personaje '{character}', índice {index}.");
        return new string[0]; // Retornar un arreglo vacío si no se encuentra el diálogo
    }
}
