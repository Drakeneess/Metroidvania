using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controlador para cargar y administrar diálogos y decisiones.
/// </summary>
public class DialogueLoader : MonoBehaviour
{
    public static DialogueLoader Instance { get; private set; }
    public string currentLanguage = "en";

    // Diccionario para almacenar diálogos organizados por personaje e índice
    private Dictionary<string, Dictionary<int, Dialogue>> dialogues = new Dictionary<string, Dictionary<int, Dialogue>>();

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
            LoadDecisions();
        }
    }

    private void Start()
    {
        currentLanguage = LanguageController.GetLanguageString();
    }

    /// <summary>
    /// Carga los diálogos desde el archivo CSV.
    /// </summary>
    private void LoadDialogues()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("dialogues");
        if (csvFile == null)
        {
            Debug.LogError("No se encontró el archivo de diálogos.");
            return;
        }

        string[] lines = csvFile.text.Split('\n');
        string[] headers = lines[0].Split(','); // ["character", "index", "id_conversation", "es", "en"]

        for (int i = 1; i < lines.Length; i++) // Saltar encabezados
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');
            if (values.Length < 4) continue;

            string character = values[0].Trim();
            int index = int.Parse(values[1].Trim());
            string idConversation = values[2].Trim();

            // Si el personaje no está en el diccionario, lo agregamos
            if (!dialogues.ContainsKey(character))
                dialogues[character] = new Dictionary<int, Dialogue>();

            // Creamos el diálogo si no existe
            if (!dialogues[character].ContainsKey(index))
            {
                dialogues[character][index] = new Dialogue(character, index, idConversation);
            }

            Dialogue dialogue = dialogues[character][index];

            // Cargamos los textos en distintos idiomas
            for (int j = 3; j < values.Length; j++)
            {
                string language = headers[j].Trim();
                string[] sentences = values[j].Trim().Split('|');

                if (!dialogue.texts.ContainsKey(language))
                    dialogue.texts[language] = sentences;
            }
        }
    }

    /// <summary>
    /// Carga las decisiones y las asocia con los diálogos correctos.
    /// </summary>
    private void LoadDecisions()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("decisions");
        if (csvFile == null)
        {
            Debug.LogError("No se encontró el archivo de decisiones.");
            return;
        }

        string[] lines = csvFile.text.Split('\n');
        string[] headers = lines[0].Split(','); // ["id_conversation", "index", "weight", "es", "en"]

        for (int i = 1; i < lines.Length; i++) // Saltar encabezados
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');
            if (values.Length < 4) continue;

            string idConversation = values[0].Trim();
            int index = int.Parse(values[1].Trim());
            float weight = float.Parse(values[2].Trim());

            Decision decision = new Decision(idConversation, index, weight);

            // Cargar textos en distintos idiomas
            for (int j = 3; j < values.Length; j++)
            {
                string language = headers[j].Trim();
                string text = values[j].Trim();

                if (!decision.texts.ContainsKey(language))
                    decision.texts[language] = text;
            }

            // Asociamos la decisión con el diálogo correcto
            foreach (var character in dialogues.Keys)
            {
                foreach (var dialogue in dialogues[character].Values)
                {
                    if (dialogue.idConversation == idConversation)
                    {
                        dialogue.decisions.Add(decision);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Obtiene un diálogo con sus decisiones asociadas.
    /// </summary>
    public static Dialogue GetDialogue(string character, int index)
    {
        if (Instance.dialogues.ContainsKey(character) &&
            Instance.dialogues[character].ContainsKey(index))
        {
            return Instance.dialogues[character][index];
        }

        Debug.LogWarning($"No se encontró el diálogo para '{character}' en el índice {index}.");
        return null; // Retorna null si no se encuentra el diálogo
    }
}

/// <summary>
/// Representa un diálogo individual con sus decisiones asociadas.
/// </summary>
public class Dialogue
{
    public string character;
    public int index;
    public string idConversation;
    public Dictionary<string, string[]> texts = new Dictionary<string, string[]>(); // idioma -> frases
    public List<Decision> decisions = new List<Decision>(); // Lista de decisiones asociadas

    public Dialogue(string character, int index, string idConversation)
    {
        this.character = character;
        this.index = index;
        this.idConversation = idConversation;
    }

    /// <summary>
    /// Obtiene el texto del diálogo en el idioma actual.
    /// </summary>
    public string[] GetLocalizedText(string language)
    {
        if (texts.TryGetValue(language, out string[] lines))
        {
            return lines;
        }
        return new string[0]; // Retornar un arreglo vacío si no hay diálogos en el idioma solicitado.
    }

    /// <summary>
    /// Obtiene las decisiones con índice, peso y texto en el idioma actual.
    /// </summary>
    public List<LocalizedDecision> GetLocalizedDecisions(string language)
    {
        List<LocalizedDecision> localizedDecisions = new List<LocalizedDecision>();

        foreach (var decision in decisions)
        {
            if (decision.texts.ContainsKey(language))
            {
                localizedDecisions.Add(new LocalizedDecision(decision.index, decision.weight, decision.texts[language]));
            }
        }
        return localizedDecisions;
    }
}


/// <summary>
/// Representa una decisión dentro del diálogo.
/// </summary>
public class Decision
{
    public string idConversation;
    public int index;
    public float weight;
    public Dictionary<string, string> texts = new Dictionary<string, string>(); // idioma -> texto de decisión

    public Decision(string idConversation, int index, float weight)
    {
        this.idConversation = idConversation;
        this.index = index;
        this.weight = weight;
    }
}

/// <summary>
/// Representa una decisión localizada con su índice, peso y texto.
/// </summary>
public class LocalizedDecision
{
    public int index;
    public float weight;
    public string text;

    public LocalizedDecision(int index, float weight, string text)
    {
        this.index = index;
        this.weight = weight;
        this.text = text;
    }
}

