using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckpointsController : MonoBehaviour
{
    public Player player;
    public static CheckpointsController Instance { get; set; }
    private List<Checkpoint> checkpoints = new List<Checkpoint>();
    public List<Checkpoint> Checkpoints => checkpoints;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        SetCheckpoints();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static void SetNewScene()
    {
        Instance.SetCheckpoints();   
    }
    private void SetCheckpoints(){
        if(checkpoints.Count > 0){
            checkpoints.Clear();
        }
        checkpoints = FindObjectsOfType<Checkpoint>().ToList();
        player = FindObjectOfType<Player>();
        FastTravelMenu fastTravelMenu = FindObjectOfType<FastTravelMenu>(true);
        foreach (Checkpoint checkpoint in checkpoints)
        {
            if (SaveDataController.Instance.saveData.checkpointsUnlocked.Contains(checkpoint.CheckpointID.ToString()))
            {
                checkpoint.ActivateRefugee(true);
                fastTravelMenu.UnlockCheckpoint(checkpoint);
            }
            checkpoint.SetPlayer(player);
        }
        if (SaveDataController.AreSavedData())
        {
            player.LastCheckpoint = GetLastCheckpoint();
            player.SetOnCheckpointPosition();
        }
    }
    private Checkpoint GetLastCheckpoint()
    {
        // Get the last checkpoint from the save data
        foreach(Checkpoint checkpoint in checkpoints){
            if(checkpoint.CheckpointID == SaveDataController.Instance.saveData.lastCheckpointIndex){
                return checkpoint;
            }
        }
        return null;
    }
}
