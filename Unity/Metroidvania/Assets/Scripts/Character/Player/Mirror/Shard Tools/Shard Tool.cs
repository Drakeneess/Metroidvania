using UnityEngine;

public class ShardTool : MonoBehaviour
{
    [Header("Shard Tool Base")]
    [SerializeField] protected ShardToolData shardToolData;
    [SerializeField] protected int toolID = 0;
    
    protected string shardToolName = "";
    protected string shardToolDescription = "";
    protected CharacterMovement characterMovement;
    protected Quaternion originalRotation;


    protected virtual void Awake()
    {
        shardToolDescription += "_Desc";
    }

    protected virtual void Start()
    {
        characterMovement = FindObjectOfType<CharacterMovement>();
        if (SaveDataController.Instance.saveData.toolUnlocked.Contains(toolID) && SaveDataController.AreSavedData())
        {
            shardToolData.unlocked = true;
        }
        else
        {
            shardToolData.unlocked = false;
        }
    }

    protected virtual void OnEnable()
    {
        AlignWithFacing();
    }

    protected virtual void OnDisable()
    {

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

    public virtual void UnlockTool()
    {
        shardToolData.unlocked = true;
        SaveDataController.Instance.saveData.toolUnlocked.Add(toolID);
    }
    // ShardTool.cs
    public int GetFacingDirection()
    {
        if (characterMovement != null)
        {
            float h = characterMovement.Direction;
            if (Mathf.Abs(h) > 0.01f) return h >= 0f ? 1 : -1;
        }
        // Fallback (por si no hay input): usa el signo del scale X o asume derecha
        return transform.lossyScale.x >= 0f ? 1 : -1;
    }

    protected void AlignWithFacing()
    {
        int facing = GetFacingDirection(); // +1 derecha, -1 izquierda

        // Orientamos el arma mirando al frente del personaje
        if (facing > 0)
        {
            transform.localRotation = originalRotation;
        }
        else
        {
            // Flip horizontal global (rotar 180° en Y)
            transform.localRotation = Quaternion.Euler(0f, 180f, 0f) * originalRotation;
        }
    }

    // Métodos comunes
    public virtual int GetToolID() => toolID;
    public virtual string GetToolName() => shardToolData.toolName;
    public virtual string GetToolDescription() => shardToolData.toolDescription;
    public virtual float GetMentalHealthUsage() => shardToolData.mentalHealthUsage;
    public virtual Sprite GetToolImage() => shardToolData.toolImageUI;
    public virtual bool GetUnlocked() => shardToolData.unlocked;
}
