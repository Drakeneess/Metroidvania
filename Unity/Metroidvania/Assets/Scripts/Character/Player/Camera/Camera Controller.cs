using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance { get; private set; }

    [Header("Player Settings")]
    public Player player;

    [Header("Follow Settings")]
    public float followSpeed = 2.0f;

    [Tooltip("Offset cuando el jugador está a máxima salud")]
    public Vector3 offsetMin = new Vector3(0, 5, -10);

    [Tooltip("Offset cuando el jugador está a mínima salud")]
    public Vector3 offsetMax = new Vector3(0, 8, -16);
    private bool isFollowing = true;

    public static bool IsFollowingPlayer
    {
        get { return instance.isFollowing; }
        set { instance.isFollowing = value; }
    }

    private void Awake()
    {
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

    private void FollowPlayer()
    {
        if (player == null) return;
        float healthPercent = Mathf.Clamp01(player.GetPercentageHealth(HealthType.Physical));

        // Interpolamos el offset entre el cercano (offsetMin) y el lejano (offsetMax)
        Vector3 dynamicOffset = Vector3.Lerp(offsetMax, offsetMin, healthPercent);

        Vector3 targetPosition = player.transform.position + dynamicOffset;

        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    public static void ShakeCamera()
    {
        Debug.Log("ShakeCamera not implemented yet.");
    }
}
