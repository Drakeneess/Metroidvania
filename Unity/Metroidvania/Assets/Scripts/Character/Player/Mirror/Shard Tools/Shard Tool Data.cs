using UnityEngine;

public class ShardToolData : ScriptableObject
{
    [Header("General Tool Info")]
    public string toolName = "";
    public string toolDescription = "";
    public float mentalHealthUsage = 1f;
    public bool unlocked;
    public Sprite toolImageUI;
}
