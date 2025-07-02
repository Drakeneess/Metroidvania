using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Claws : Weapon
{
    protected override void Awake()
    {
        shardToolName = "Claws";
        shardToolDescription = shardToolName;
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
    }
}
