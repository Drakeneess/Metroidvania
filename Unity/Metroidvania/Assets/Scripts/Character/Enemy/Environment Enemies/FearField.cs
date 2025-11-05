using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Sistema global invisible que almacena fuentes de miedo generadas por enemigos en conflicto.
/// </summary>
public static class FearField
{
    private class FearSource
    {
        public Vector3 position;
        public float radius;
        public float lifetime;
    }

    private static readonly List<FearSource> sources = new List<FearSource>();

    /// <summary>
    /// Agrega o refresca una fuente de miedo en el campo global.
    /// </summary>
    public static void AddOrRefresh(Vector3 position, float radius, float duration)
    {
        for (int i = 0; i < sources.Count; i++)
        {
            if (Vector3.Distance(sources[i].position, position) < radius * 0.5f)
            {
                sources[i].lifetime = duration;
                return;
            }
        }

        sources.Add(new FearSource
        {
            position = position,
            radius = radius,
            lifetime = duration
        });
    }

    /// <summary>
    /// Llamar una vez por frame para actualizar el estado del campo.
    /// </summary>
    public static void UpdateField(float deltaTime)
    {
        for (int i = sources.Count - 1; i >= 0; i--)
        {
            sources[i].lifetime -= deltaTime;
            if (sources[i].lifetime <= 0)
                sources.RemoveAt(i);
        }
    }

    /// <summary>
    /// Determina si una posición está dentro del campo de miedo.
    /// </summary>
    public static bool IsWithinFear(Vector3 position)
    {
        foreach (var s in sources)
        {
            if (Vector3.Distance(position, s.position) <= s.radius)
                return true;
        }
        return false;
    }

#if UNITY_EDITOR
    // Solo para depuración en el editor
    public static void DrawGizmos()
    {
        foreach (var s in sources)
        {
            Gizmos.color = new Color(0.6f, 0.2f, 1f, 0.08f);
            Gizmos.DrawSphere(s.position, s.radius);
        }
    }
#endif
}
