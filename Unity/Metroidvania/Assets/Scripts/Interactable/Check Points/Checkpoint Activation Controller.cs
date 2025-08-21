using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CheckpointActivationController
{
    public Material activeMaterial;
    public MeshRenderer checkpointRenderer;

    public void Activate(MonoBehaviour runner, float duration = 9f)
    {
        runner.StartCoroutine(TransitionToActive(duration));
    }
    private IEnumerator TransitionToActive(float duration)
    {
        Material[] mats = checkpointRenderer.materials;

        if (mats.Length > 1)
        {
            Material targetMat = mats[1]; // segundo material

            // Propiedades iniciales
            Color startColor = targetMat.color;
            Color endColor = activeMaterial.color;

            Color startEmission = targetMat.GetColor("_EmissionColor");
            Color endEmission = activeMaterial.GetColor("_EmissionColor");

            float startSmooth = targetMat.GetFloat("_Smoothness");
            float endSmooth = activeMaterial.GetFloat("_Smoothness");

            // Habilitar emisión si no lo está
            targetMat.EnableKeyword("_EMISSION");

            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float lerpValue = t / duration;

                // Color base
                targetMat.color = Color.Lerp(startColor, endColor, lerpValue);

                // Emisión
                Color lerpedEmission = Color.Lerp(startEmission, endEmission, lerpValue);
                targetMat.SetColor("_EmissionColor", lerpedEmission);

                // Smoothness (brillo especular)
                targetMat.SetFloat("_Smoothness", Mathf.Lerp(startSmooth, endSmooth, lerpValue));

                yield return null;
            }

            // Asegurar valores finales exactos
            targetMat.color = endColor;
            targetMat.SetColor("_EmissionColor", endEmission);
            targetMat.SetFloat("_Smoothness", endSmooth);
        }
        else
        {
            Debug.LogWarning("El MeshRenderer no tiene un segundo material.");
        }
    }
}
