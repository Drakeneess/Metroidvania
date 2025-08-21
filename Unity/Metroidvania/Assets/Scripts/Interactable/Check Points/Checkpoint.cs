using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : Interactable
{
    [SerializeField] public CheckpointActivationController ActivationController;
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
    protected override void Action()
    {
        base.Action();
        player.RestOnRefugee(this);
        SaveDataController.Instance.saveData.lastCheckpointIndex = checkpointID;
        SaveDataController.SaveData();
        if (!isActivated)
        {
            ActivationController.Activate(this);
        }
        isActivated = true;
    }
    public void SetPlayer(Player newPlayer)
    {
        player = newPlayer;
    }   
}
