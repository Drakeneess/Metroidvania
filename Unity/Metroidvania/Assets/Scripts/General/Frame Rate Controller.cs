using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRateController : MonoBehaviour
{
    public int minFPS = 15;  // Cuando está casi muerto
    public int maxFPS = 30;  // Cuando está al 100%

    private Player player;

    void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    void Update()
    {
        float porcentajeVida = Mathf.Clamp01(player.GetCurrentHealth(HealthType.Physical) / player.physicalHealth);
        int fpsActual = Mathf.RoundToInt(Mathf.Lerp(minFPS, maxFPS, porcentajeVida));

        Application.targetFrameRate = fpsActual;
    }
}
