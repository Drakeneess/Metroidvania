using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ToolMenu : MonoBehaviour
{
    public TextMeshProUGUI[] texts;
    public ToolUIController toolUIController;
    public Camera UICamera;

    private Canvas[] canvas;
    private Canvas actualCanvas;
    private string toolName;
    private string toolDescription;

    // Start is called before the first frame update
    void Awake()
    {
        canvas = FindObjectsOfType<Canvas>();
        actualCanvas = GetComponent<Canvas>();
    }

    void OnEnable()
    {
        if(InputActionController.Instance != null)
        {
            InputActionController.Instance.OnActionTriggered += OnSelectPressed;
        }
        UICamera.gameObject.SetActive(true);
        if(GameMenuController.Instance != null){
            GameMenuController.CurrentMode = GameMode.ToolMenu;
        }
        foreach(Canvas canva in canvas){
            if(canva != actualCanvas){
                canva.gameObject.SetActive(false);
            }
        }
        SetValues();
    }

    private void OnSelectPressed(string actionName)
    {
        if(actionName == "ToolSelect"){
            foreach(Canvas canva in canvas){
                if(canva.gameObject.CompareTag("GameUI")){
                    canva.gameObject.SetActive(true);
                }
            }
            if(GameMenuController.Instance != null){
                GameMenuController.CurrentMode = GameMode.Game;
            }
            gameObject.SetActive(false);
        }
    }

    void OnDisable()
    {
        UICamera.gameObject.SetActive(false);
    }

    public void Initialize(ShardTool shardTool){
        toolName = shardTool.GetToolName();
        toolDescription = shardTool.GetToolDescription();

        // Elimina scripts o componentes innecesarios

        GameObject shardToolObject = Instantiate(shardTool.gameObject);
        RemoveAllExcept(shardToolObject, typeof(Transform), typeof(Renderer), typeof(MeshFilter), typeof(MeshRenderer));
        shardToolObject.AddComponent<ToolUI>();
        toolUIController.SetNewObject(shardToolObject);
    }
    private void SetValues(){
        texts[0].text = toolName;
        texts[1].text = toolDescription;
    }

    void RemoveAllExcept(GameObject obj, params System.Type[] allowedTypes)
    {
        Component[] components = obj.GetComponents<Component>();
        foreach (Component comp in components)
        {
            if (System.Array.IndexOf(allowedTypes, comp.GetType()) == -1 && !(comp is Transform))
            {
                Destroy(comp); // o Destroy(comp) si no es en el editor
            }
        }

        foreach (Transform child in obj.transform)
        {
            RemoveAllExcept(child.gameObject, allowedTypes);
        }
    }
}
