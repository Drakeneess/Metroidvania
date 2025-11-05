using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShardItemInteractor : Interactable
{
    public int shardId;
    private Shard shard;
    public Shard Shard { 
        set{
            if(value!=null){
                shard = value;
                isInteractable=true;
            }
            else{
                isInteractable=false;
            }
        } 
        get=>shard;
    }
    protected override void Start()
    {
        base.Start();
        if(shard == null){ isInteractable = false;}
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
        if(shard==null) return;
        base.Action();
        ToolMenuController.Instance.ShowMenu(shard.ShardTool);
        shard.ShardTool.UnlockTool();
        shard.SetInMirror();
        if (WeaponController.Instance.CurrentWeapon == null && shard.ShardTool is Weapon)
        {
            WeaponController.Instance.EquipWeapon(shard.ShardTool as Weapon);
        }
    }
}
