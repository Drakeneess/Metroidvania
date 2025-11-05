using UnityEngine;

public class LedgePoint : MonoBehaviour
{
    [Header("Spots")]
    public Transform grabSpot;   // Donde Emil cuelga (manos al borde)
    public Transform climbSpot;  // Donde termina tras escalar (sobre la cornisa)

    [Header("Orientación")]
    public bool alignToNormal = true;   // Si true, mira en la dirección opuesta a la normal del borde
    public Vector3 forwardOverride;     // Úsalo si quieres forzar una orientación específica (opcional)

    // Para visualización en el editor
    private void OnDrawGizmos()
    {
        if (grabSpot)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(grabSpot.position, 0.06f);
        }
        if (climbSpot)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(climbSpot.position, 0.06f);
            if (grabSpot) Gizmos.DrawLine(grabSpot.position, climbSpot.position);
        }
    }
}
