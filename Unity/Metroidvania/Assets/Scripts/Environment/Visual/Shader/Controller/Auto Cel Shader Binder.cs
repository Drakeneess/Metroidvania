#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class AutoCelShaderBinderEditor
{
    private const string celShaderName = "Custom/DarkCelWithLineArt";

    static AutoCelShaderBinderEditor()
    {
        // Se llama cada vez que se selecciona algo en el editor
        Selection.selectionChanged += CheckSelection;
    }

    private static void CheckSelection()
    {
        foreach (var obj in Selection.gameObjects)
        {
            Renderer rend = obj.GetComponent<Renderer>();
            if (rend != null && rend.sharedMaterial != null)
            {
                Shader shader = rend.sharedMaterial.shader;
                if (shader != null && shader.name == celShaderName)
                {
                    if (obj.GetComponent<CelShaderController>() == null)
                    {
                        Undo.AddComponent<CelShaderController>(obj);
                        Debug.Log($"[AutoCelShaderBinderEditor] Controlador agregado a {obj.name}");
                    }
                }
            }
        }
    }
}
#endif
