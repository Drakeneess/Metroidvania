using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShardItemInteractor : Interactable
{
    public int shardId;
    private Shard shard;
    public Shard Shard { 
        set{
            shard = value;
            if(shard==null){
                isInteractable=false;
            }
        } 
        get=>shard;
    }
    protected override void Start()
    {
        base.Start();
        
    }
    protected override void OnEnable()
    {
        base.OnEnable();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
    }
    protected override void Action()
    {
        base.Action();
        ToolMenuController.Instance.ShowMenu(shard.ShardTool);
        shard.SetInMirror();
    }
}
