using System.Collections.Generic;
using UnityEngine;

public class CelShaderController : MonoBehaviour
{
    [Header("Light Type")]
    public bool isOut = true; // true = exterior, false = interior

    [Header("Update Settings")]
    public bool isStatic = true; // si el objeto no se mueve
    public bool updateOnMove = false; // si debe actualizar solo al moverse
    public int maxLightsToCheck = 8; // limitar cantidad de luces analizadas

    private Material targetMaterial;
    private Vector3 lastPos;

    void Start()
    {
        targetMaterial = GetComponent<Renderer>().material;

        // Registro en el LightManager
        CelLightManager.Instance?.Register(this);

        if (updateOnMove)
            lastPos = transform.position;

        if (isStatic)
            UpdateLight();
    }

    void LateUpdate()
    {
        if (updateOnMove && transform.position != lastPos)
        {
            lastPos = transform.position;
            UpdateLight();
        }
    }

    /// <summary>
    /// Cambia entre luz exterior e interior
    /// </summary>
    public void SetLightMode(bool useExterior)
    {
        isOut = useExterior;
        UpdateLight();
    }

    /// <summary>
    /// Actualiza la luz más cercana y la pasa al shader
    /// </summary>
    public void UpdateLight()
    {
        if (!targetMaterial || CelLightManager.Instance == null) return;

        IList<AmbientalLight> lights = isOut
            ? new List<AmbientalLight>(CelLightManager.Instance.exteriorLights)
            : new List<AmbientalLight>(CelLightManager.Instance.interiorLights);

        if (lights.Count == 0) return;

        AmbientalLight closest = null;
        float minSqrDist = float.MaxValue;

        int lightChecks = Mathf.Min(maxLightsToCheck, lights.Count);

        for (int i = 0; i < lightChecks; i++)
        {
            var light = lights[i];
            float sqrDist = (transform.position - light.transform.position).sqrMagnitude;
            if (sqrDist < minSqrDist)
            {
                minSqrDist = sqrDist;
                closest = light;
            }
        }

        if (!closest) return;

        float dist = Mathf.Sqrt(minSqrDist);
        targetMaterial.SetVector("_LightPos", closest.transform.position);
        targetMaterial.SetFloat("_LightIntensity", closest.intensity / Mathf.Max(dist, 0.01f));
    }

}
