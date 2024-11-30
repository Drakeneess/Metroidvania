using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance { get; private set; }

    [Header("Player Settings")]
    public Transform player; // Referencia al jugador

    [Header("Follow Settings")]
    public float followSpeed = 2.0f; // Velocidad con la que la cámara sigue al jugador
    public Vector3 offset; // Desplazamiento de la cámara respecto al jugador

    private bool isFollowing = true; // Bandera para habilitar/deshabilitar el seguimiento
    public static bool IsFollowingPlayer
    {
        get { return instance.isFollowing; }
        set { instance.isFollowing = value; }
    }

    private void Awake()
    {
        // Configuración de la instancia
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LateUpdate()
    {
        if (isFollowing)
        {
            FollowPlayer();
        }
    }

    /// <summary>
    /// Sigue al jugador con un retraso suave.
    /// </summary>
    private void FollowPlayer()
    {
        if (player == null) return;

        // Posición objetivo con el desplazamiento aplicado
        Vector3 targetPosition = player.position + offset;

        // Interpolación suave hacia la posición objetivo
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Sacudir la cámara.
    /// </summary>
    public static void ShakeCamera()
    {
        // Implementación futura para agregar sacudidas a la cámara
        Debug.Log("ShakeCamera not implemented yet.");
    }
}
