using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteriorAmbientalLight : AmbientalLight
{
    void OnEnable() => CelLightManager.Instance?.RegisterLight(this);
    void OnDisable() => CelLightManager.Instance?.UnregisterLight(this);
}
