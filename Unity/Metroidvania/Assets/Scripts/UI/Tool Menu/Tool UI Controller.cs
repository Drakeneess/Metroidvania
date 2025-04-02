using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolUIController : MonoBehaviour
{
    public GameObject toolUI;
    public LayerMask UILayer;
    private Vector3 toolUIPosition;

    void Start()
    {
        toolUIPosition= toolUI.transform.localPosition;
    }

    public void SetNewObject(GameObject obj){
        toolUI = obj;

        toolUI.SetActive(true);
        
        toolUI.transform.SetParent(transform);
        toolUI.transform.localPosition = toolUIPosition;
        toolUI.transform.localScale = Vector3.one;
        toolUI.transform.rotation = Quaternion.identity;

        int uiLayer = Mathf.RoundToInt(Mathf.Log(UILayer.value, 2));
        SetLayerRecursively(toolUI, uiLayer);

        StandardizeScale();
    }
    private void StandardizeScale(float targetHeight = 1.8f)
    {
        Renderer[] renderers = toolUI.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        // Crear un bounds combinado de todos los renderers
        Bounds combinedBounds = renderers[0].bounds;
        foreach (Renderer rend in renderers)
        {
            combinedBounds.Encapsulate(rend.bounds);
        }

        float currentHeight = combinedBounds.size.y;
        if (currentHeight > 0)
        {
            float scaleFactor = targetHeight / currentHeight;
            toolUI.transform.localScale *= scaleFactor;
        }
    }
    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}
