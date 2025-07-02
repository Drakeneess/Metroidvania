using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : Interactable
{
    public int checkpointID;
    private Player player;

    public Vector2 CheckpointPosition { get{
        return new Vector2(transform.position.x,transform.position.y);
        }
    }
    protected override void Start()
    {
        base.Start();
    }
    protected override void Action()
    {
        base.Action();
        player.LastCheckpoint = this;
        SaveDataController.Instance.saveData.lastCheckpointIndex = checkpointID;
        SaveDataController.SaveData();
    }
    public void SetPlayer(Player newPlayer){
        player = newPlayer;
    }
}
