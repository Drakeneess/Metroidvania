using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : Interactable
{
    [SerializeField] public CheckpointActivationController ActivationController;
    [SerializeField] public string checkpointName;
    private int checkpointID;
    public int CheckpointID { get { return checkpointID; } }
    private Player player;
    private bool isActivated = false;

    private void GetCheckpointID()
    {
        int posX = Mathf.RoundToInt(transform.position.x * 1000);
        int posY = Mathf.RoundToInt(transform.position.y * 1000);
        int scene = gameObject.scene.buildIndex;

        checkpointID = ((scene & 0xFF) << 24) | ((posX & 0xFFF) << 12) | (posY & 0xFFF);
    }

    public Vector2 CheckpointPosition
    {
        get
        {
            return new Vector2(transform.position.x, transform.position.y);
        }
    }
    protected override void Start()
    {
        base.Start();

        GetCheckpointID();
        
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }
    protected override void Action()
    {
        base.Action();

        var extras = new List<string>
        {
            $"InteractionType: {interactionType}",
            $"CheckpointID: {checkpointID}",
            $"CheckpointName: {checkpointName}"
        };
        LogBegin(extras);

        player.SetOnRefugee(this);
        SaveDataController.Instance.saveData.lastCheckpointIndex = checkpointID;
        if (!SaveDataController.Instance.saveData.checkpointsUnlocked.Contains(checkpointID.ToString()))
        {
            SaveDataController.Instance.saveData.checkpointsUnlocked.Add(checkpointID.ToString());
        }
        SaveDataController.SaveData();
        ActivateRefugee();
        CheckpointMenuController.Instance.Open(player);
    }
    public void ActivateRefugee(bool auto=false)
    {
        if (!isActivated)
        {
            if (auto)
            {
                ActivationController.Activate(this, 0);
            }
            else
            {
                ActivationController.Activate(this);
            }
            isActivated = true;
        }
    }
    public void SetPlayer(Player newPlayer)
    {
        player = newPlayer;
    }   
}
