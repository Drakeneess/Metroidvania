using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scythe : Weapon
{
    protected override void Awake()
    {
        shardToolName = "Scythe";
        shardToolDescription = shardToolName;
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
    }
}
