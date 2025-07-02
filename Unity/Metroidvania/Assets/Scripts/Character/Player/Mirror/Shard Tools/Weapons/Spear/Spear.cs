using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : Weapon
{
    protected override void Awake()
    {
        shardToolName = "Spear";
        shardToolDescription = shardToolName;
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
    }
}
