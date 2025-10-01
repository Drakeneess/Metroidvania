using System.Collections.Generic;
using UnityEngine;

public class DialogueLoader : MonoBehaviour
{
    public static DialogueLoader Instance { get; private set; }
    public string currentLanguage = "en";

    private Dictionary<string, Dictionary<int, Dialogue>> dialogues = new();

    private void Awake()
    {
        Instance = this;
        LoadDialogues();
        LoadDecisions();

        // 🔔 escuchar cambios de idioma
        LanguageController.OnLanguageChanged += OnLanguageChanged;
    }

    private void Start()
    {
        currentLanguage = LanguageController.GetLanguageString();
    }

    private void OnDestroy()
    {
        LanguageController.OnLanguageChanged -= OnLanguageChanged;
    }

    private void OnLanguageChanged()
    {
        currentLanguage = LanguageController.GetLanguageString();
    }

    // --- carga de diálogos igual que antes ---
    private void LoadDialogues()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("dialogues");
        if (csvFile == null)
        {
            Debug.LogError("No se encontró el archivo de diálogos.");
            return;
        }

        string[] lines = csvFile.text.Split('\n');
        string[] headers = lines[0].Split(',');

        int emotionColIndex = System.Array.IndexOf(headers, "emotion");

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');
            if (values.Length < 4) continue;

            string character = values[0].Trim();
            int index = int.Parse(values[1].Trim());
            string idConversation = values[2].Trim();

            if (!dialogues.ContainsKey(character))
                dialogues[character] = new Dictionary<int, Dialogue>();

            if (!dialogues[character].ContainsKey(index))
                dialogues[character][index] = new Dialogue(character, index, idConversation);

            Dialogue dialogue = dialogues[character][index];

            // EMOCIONES
            string[] emotionPerLine = values[emotionColIndex].Trim().Split('|');
            EmotionType[] parsedEmotions = new EmotionType[emotionPerLine.Length];
            for (int j = 0; j < emotionPerLine.Length; j++)
            {
                if (int.TryParse(emotionPerLine[j], out int emoValue) &&
                    System.Enum.IsDefined(typeof(EmotionType), emoValue))
                    parsedEmotions[j] = (EmotionType)emoValue;
                else
                    parsedEmotions[j] = EmotionType.Calm;
            }

            // TEXTOS
            for (int j = 4; j < values.Length; j++)
            {
                string language = headers[j].Trim();
                string[] sentences = values[j].Trim().Split('|');

                dialogue.texts[language] = sentences;

                // ajustar emociones
                EmotionType[] adjustedEmotions = new EmotionType[sentences.Length];
                for (int k = 0; k < sentences.Length; k++)
                    adjustedEmotions[k] = k < parsedEmotions.Length ? parsedEmotions[k] : parsedEmotions[^1];

                if (dialogue.emotions == null)
                    dialogue.emotions = adjustedEmotions;
            }
        }
    }

    private void LoadDecisions()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("decisions");
        if (csvFile == null)
        {
            Debug.LogError("No se encontró el archivo de decisiones.");
            return;
        }

        string[] lines = csvFile.text.Split('\n');
        string[] headers = lines[0].Split(',');

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');
            if (values.Length < 4) continue;

            string idConversation = values[0].Trim();
            int index = int.Parse(values[1].Trim());
            int idDecision = int.Parse(values[2].Trim());

            Decision decision = new Decision(idConversation, index, idDecision);

            for (int j = 3; j < values.Length; j++)
            {
                string language = headers[j].Trim();
                string text = values[j].Trim();
                decision.texts[language] = text;
            }

            foreach (var character in dialogues.Keys)
            {
                foreach (var dialogue in dialogues[character].Values)
                {
                    if (dialogue.idConversation == idConversation)
                        dialogue.decisions.Add(decision);
                }
            }
        }
    }

    public static Dialogue GetDialogue(string character, int index)
    {
        if (Instance.dialogues.ContainsKey(character) &&
            Instance.dialogues[character].ContainsKey(index))
            return Instance.dialogues[character][index];

        Debug.LogWarning($"No se encontró el diálogo para '{character}' en el índice {index}.");
        return null;
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
    public Dictionary<string, string[]> texts = new();
    public EmotionType[] emotions;
    public List<Decision> decisions = new();

    public Dialogue(string character, int index, string idConversation)
    {
        this.character = character;
        this.index = index;
        this.idConversation = idConversation;
    }

    public string[] GetLocalizedText()
    {
        string lang = DialogueLoader.Instance.currentLanguage;
        if (texts.TryGetValue(lang, out string[] lines))
            return lines;
        return System.Array.Empty<string>();
    }

    public List<LocalizedDecision> GetLocalizedDecisions()
    {
        string lang = DialogueLoader.Instance.currentLanguage;
        List<LocalizedDecision> localizedDecisions = new();

        foreach (var decision in decisions)
        {
            if (decision.texts.TryGetValue(lang, out string text))
                localizedDecisions.Add(new LocalizedDecision(decision.index, decision.idDecision, text));
        }
        return localizedDecisions;
    }

    public EmotionType GetEmotionForLine(int lineIndex)
    {
        if (emotions != null && lineIndex < emotions.Length)
            return emotions[lineIndex];
        return EmotionType.Calm;
    }
}

/// <summary>
/// Representa una decisión dentro del diálogo.
/// </summary>
public class Decision
{
    public string idConversation;
    public int index;
    public int idDecision;
    public Dictionary<string, string> texts = new Dictionary<string, string>(); // idioma -> texto de decisión

    public Decision(string idConversation, int index, int idDecision)
    {
        this.idConversation = idConversation;
        this.index = index;
        this.idDecision = idDecision;
    }
}

/// <summary>
/// Representa una decisión localizada con su índice, peso y texto.
/// </summary>
public class LocalizedDecision
{
    public int index;
    public int idDecision;
    public string text;

    public LocalizedDecision(int index, int idDecision, string text)
    {
        this.index = index;
        this.idDecision = idDecision;
        this.text = text;
    }
}

