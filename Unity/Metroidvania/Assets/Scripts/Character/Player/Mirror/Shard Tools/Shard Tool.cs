using UnityEngine;

public class ShardTool : MonoBehaviour
{
    [Header("Shard Tool Base")]
    [SerializeField] protected ShardToolData shardToolData;
    
    protected string shardToolName = "";
    protected string shardToolDescription = "";
    protected CharacterMovement characterMovement;

    protected virtual void Awake(){
        shardToolDescription += "_Desc";
    }

    protected virtual void Start()
    {
        characterMovement = FindObjectOfType<CharacterMovement>();
        SetToolActive(false);
    }

    public virtual void SetName()
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        shardToolData.toolName = LanguageMenu.GetTranslate(shardToolName);
        shardToolData.toolDescription = LanguageMenu.GetTranslate(shardToolDescription);
        gameObject.SetActive(false);
    }

    public void SetToolActive(bool state)
    {
        if (GetUnlocked())
            gameObject.SetActive(state);
        else
            gameObject.SetActive(false);
    }

    // Métodos comunes
    public virtual string GetToolName() => shardToolData.toolName;
    public virtual string GetToolDescription() => shardToolData.toolDescription;
    public virtual float GetMentalHealthUsage() => shardToolData.mentalHealthUsage;
    public virtual Sprite GetToolImage() => shardToolData.toolImageUI;
    public virtual bool GetUnlocked() => shardToolData.unlocked;
}
