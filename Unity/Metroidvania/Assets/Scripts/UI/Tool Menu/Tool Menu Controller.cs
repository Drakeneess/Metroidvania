using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolMenuController : MonoBehaviour
{
    public static ToolMenuController Instance { get; set; }
    public ToolMenu toolMenu;

    void Awake()
    {
        Instance = this;
    }
    public void ShowMenu(ShardTool shardTool){
        toolMenu.Initialize(shardTool);
        toolMenu.gameObject.SetActive(true);
    }
}
