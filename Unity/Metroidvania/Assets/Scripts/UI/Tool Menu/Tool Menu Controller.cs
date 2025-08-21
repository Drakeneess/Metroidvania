using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolMenuController : MonoBehaviour
{
    public static ToolMenuController Instance { get; set; }
    public ToolMenu toolMenu;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    public void ShowMenu(ShardTool shardTool){
        toolMenu.Initialize(shardTool);
        toolMenu.gameObject.SetActive(true);
    }
}
