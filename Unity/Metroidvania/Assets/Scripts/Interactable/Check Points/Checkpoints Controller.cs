using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckpointsController : MonoBehaviour
{
    public Player player;
    public static CheckpointsController Instance { get; set; }
    private List<Checkpoint> Checkpoints = new List<Checkpoint>();
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
    public static void SetNewScene(){
        Instance.SetCheckpoints();   
    }
    private void SetCheckpoints(){
        if(Checkpoints.Count > 0){
            Checkpoints.Clear();
        }
        Checkpoints = FindObjectsOfType<Checkpoint>().ToList();
        player = FindObjectOfType<Player>();
        foreach(Checkpoint checkpoint in Checkpoints){
            checkpoint.SetPlayer(player);
        }
        if(SaveDataController.AreSavedData()){
            player.LastCheckpoint = GetLastCheckpoint();
        }
    }
    private Checkpoint GetLastCheckpoint()
    {
        // Get the last checkpoint from the save data
        foreach(Checkpoint checkpoint in Checkpoints){
            if(checkpoint.CheckpointID == SaveDataController.Instance.saveData.lastCheckpointIndex){
                return checkpoint;
            }
        }
        return null;
    }
}
