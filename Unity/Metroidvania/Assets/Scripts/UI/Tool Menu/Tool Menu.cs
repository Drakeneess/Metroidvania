using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToolMenu : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI[] texts;
    public ToolUIController toolUIController;
    public Camera UICamera;

    [Header("Canvas a ocultar mientras este menú está activo")]
    public Canvas[] canvasToHide;

    private List<Canvas> canvasesToRestore = new();
    private Canvas ownCanvas;
    private string toolName;
    private string toolDescription;

    void Awake()
    {
        ownCanvas = GetComponent<Canvas>();
    }

    void OnEnable()
    {
        if (InputActionController.Instance != null)
            InputActionController.Instance.OnActionTriggered += OnSelectPressed;

        UICamera.gameObject.SetActive(true);

        if (GameMenuController.Instance != null)
            GameMenuController.CurrentMode = GameMode.ToolMenu;

        // ✅ Ocultar solo los canvases que tú definiste
        canvasesToRestore.Clear();
        foreach (Canvas c in canvasToHide)
        {
            if (c != null && c.gameObject.activeSelf)
            {
                canvasesToRestore.Add(c);
                c.gameObject.SetActive(false);
            }
        }

        SetValues();
    }

    private void OnSelectPressed(string actionName)
    {
        if (actionName != "ToolSelect") return;

        // ✅ Reactivar exactamente los que se desactivaron
        foreach (Canvas c in canvasesToRestore)
        {
            if (c != null) c.gameObject.SetActive(true);
        }

        if (GameMenuController.Instance != null)
            GameMenuController.CurrentMode = GameMode.Game;

        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        if (InputActionController.Instance != null)
            InputActionController.Instance.OnActionTriggered -= OnSelectPressed;

        UICamera.gameObject.SetActive(false);
    }

    public void Initialize(ShardTool shardTool)
    {
        toolName = shardTool.GetToolName();
        toolDescription = shardTool.GetToolDescription();

        GameObject shardToolObject = Instantiate(shardTool.gameObject);
        RemoveAllExcept(shardToolObject, typeof(Transform), typeof(Renderer), typeof(MeshFilter), typeof(MeshRenderer));
        shardToolObject.AddComponent<ToolUI>();
        toolUIController.SetNewObject(shardToolObject);
    }

    private void SetValues()
    {
        if (texts.Length >= 2)
        {
            texts[0].text = toolName;
            texts[1].text = toolDescription;
        }
    }

    void RemoveAllExcept(GameObject obj, params Type[] allowedTypes)
    {
        foreach (var comp in obj.GetComponents<Component>())
        {
            if (Array.IndexOf(allowedTypes, comp.GetType()) == -1 && !(comp is Transform))
                Destroy(comp);
        }

        foreach (Transform child in obj.transform)
            RemoveAllExcept(child.gameObject, allowedTypes);
    }
}
