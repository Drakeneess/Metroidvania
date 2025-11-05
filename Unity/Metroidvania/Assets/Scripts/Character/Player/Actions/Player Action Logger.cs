using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionLogger : MonoBehaviour
{
    private List<PlayerAction> actions = new List<PlayerAction>();
    public static PlayerActionLogger Instance { get; private set; }

    [SerializeField] private Player player;
    [SerializeField] private float mergeWindowSeconds = 0.75f;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    /// <summary>
    /// Registra una acción. Si mergeWithLastOfSameType = true,
    /// intenta fusionar con la última acción del mismo tipo dentro de la ventana de tiempo.
    /// </summary>
    public void Log(string type, List<string> extras = null, bool mergeWithLastOfSameType = false)
    {
        var nowIso = DateTime.UtcNow.ToString("o");
        float x = (float)Math.Round(transform.position.x, 6);
        float y = (float)Math.Round(transform.position.y, 6);
        ActionContextType actionName = PlayerConflictStateController.Instance.IsInConflict ? ActionContextType.Fight : ActionContextType.Explore;
        float currentHealth = player.Health.GetPercent(HealthType.Physical);

        if (extras == null) extras = new List<string>();

        if (mergeWithLastOfSameType && actions.Count > 0)
        {
            var last = actions[actions.Count - 1];
            if (last.type == type)
            {
                if (DateTime.TryParse(last.timestamp, out var lastTimeUtc))
                {
                    if ((DateTime.UtcNow - lastTimeUtc).TotalSeconds <= mergeWindowSeconds)
                    {
                        // Fusionar: agregamos extras a la lista existente
                        last.actionName = actionName;
                        last.extras.AddRange(extras);
                        last.timestamp = nowIso;
                        last.posX = x;
                        last.posY = y;

                        Debug.Log($"[ACTION LOG][MERGED] {type} | {actionName} | Extras: {string.Join(",", last.extras)}");
                        return;
                    }
                }
            }
        }

        var entry = new PlayerAction(type, actionName, nowIso, x, y, currentHealth, extras);
        actions.Add(entry);
        Debug.Log($"[ACTION LOG] {type} | {actionName} | Extras: {string.Join(",", extras)}");
    }

    public IReadOnlyList<PlayerAction> GetActions() => actions;
}

[System.Serializable]
public class PlayerAction
{
    public string type;
    public ActionContextType actionName;
    public string timestamp; // ISO 8601 (UTC)
    public float posX;
    public float posY;
    public float currentHealth;
    public List<string> extras;

    public PlayerAction(string type, ActionContextType actionName, string timestamp, float x, float y, float currentHealth, List<string> extras = null)
    {
        this.type = type;
        this.actionName = actionName;
        this.timestamp = timestamp;
        this.posX = x;
        this.posY = y;
        this.currentHealth = currentHealth;
        this.extras = extras ?? new List<string>();
    }
}

public enum ActionContextType
{
    Neutral,
    Fight,
    Explore
}
