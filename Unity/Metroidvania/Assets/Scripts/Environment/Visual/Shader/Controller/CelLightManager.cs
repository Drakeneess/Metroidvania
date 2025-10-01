using System.Collections.Generic;
using UnityEngine;

public class CelLightManager : MonoBehaviour
{
    public static CelLightManager Instance;

    public List<ExteriorAmbientalLight> exteriorLights = new();
    public List<InteriorAmbientalLight> interiorLights = new();

    private void Awake()
    {
        Instance = this;
    }

    // Registro de luces
    public void RegisterLight(AmbientalLight light)
    {
        if (light is ExteriorAmbientalLight ext && !exteriorLights.Contains(ext))
            exteriorLights.Add(ext);

        if (light is InteriorAmbientalLight intl && !interiorLights.Contains(intl))
            interiorLights.Add(intl);
    }

    public void UnregisterLight(AmbientalLight light)
    {
        if (light is ExteriorAmbientalLight ext)
            exteriorLights.Remove(ext);

        if (light is InteriorAmbientalLight intl)
            interiorLights.Remove(intl);
    }

    // Registro de controladores
    private readonly List<CelShaderController> controllers = new();

    public void Register(CelShaderController controller)
    {
        if (!controllers.Contains(controller))
            controllers.Add(controller);
    }

    public void Unregister(CelShaderController controller)
    {
        controllers.Remove(controller);
    }

    // Actualizar todos los controladores
    public void RefreshAll()
    {
        foreach (var c in controllers)
            c.UpdateLight();
    }
}
