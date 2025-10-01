using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    public static CameraShaker Instance;

    [Header("Params")]
    [SerializeField, Min(0f)] private float frequency = 60f; // “refresco” del offset por segundo (0 = cada frame)
    [SerializeField] private bool useUnscaledTime = false;

    private bool isShaking = false;
    private float magnitude = 0f;
    private float timeLeft = 0f; // <= 0 => indefinido
    private Vector3 currentOffset = Vector3.zero;
    private float accum = 0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    /// <summary> Shake por duración (segundos). </summary>
    public void Shake(float duration, float mag)
    {
        // quita cualquier offset previo antes de arrancar uno nuevo
        if (isShaking) RemoveOffset();

        isShaking = true;
        magnitude = mag;
        timeLeft = duration;
        accum = 0f;
        currentOffset = Vector3.zero; // se calculará en el primer LateUpdate
    }

    /// <summary> Shake indefinido hasta StopShake(). </summary>
    public void StartShake(float mag)
    {
        if (isShaking) RemoveOffset();

        isShaking = true;
        magnitude = mag;
        timeLeft = -1f; // indefinido
        accum = 0f;
        currentOffset = Vector3.zero;
    }

    /// <summary> Detiene el shake y deja la cámara exactamente donde debería estar. </summary>
    public void StopShake()
    {
        if (!isShaking) return;
        RemoveOffset();
        isShaking = false;
        magnitude = 0f;
        timeLeft = 0f;
    }

    private void LateUpdate()
    {
        if (!isShaking) return;

        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        // quitar el offset previo para partir del baseline que dejó el resto de scripts
        if (currentOffset != Vector3.zero)
        {
            transform.localPosition -= currentOffset;
            currentOffset = Vector3.zero;
        }

        // duración finita
        if (timeLeft > 0f)
        {
            timeLeft -= dt;
            if (timeLeft <= 0f)
            {
                isShaking = false;
                return; // ya removimos el offset arriba, quedamos limpios
            }
        }

        // actualizar “frecuencia”
        bool shouldUpdateThisFrame = frequency <= 0f;
        if (!shouldUpdateThisFrame)
        {
            accum += dt;
            if (accum >= (1f / frequency))
            {
                accum = 0f;
                shouldUpdateThisFrame = true;
            }
        }

        if (shouldUpdateThisFrame)
        {
            // nuevo offset aleatorio (podés cambiar por Perlin si querés suavidad)
            float ox = Random.Range(-1f, 1f) * magnitude;
            float oy = Random.Range(-1f, 1f) * magnitude;
            currentOffset = new Vector3(ox, oy, 0f);
        }

        // aplicar el offset al final del frame
        transform.localPosition += currentOffset;
    }

    private void RemoveOffset()
    {
        if (currentOffset != Vector3.zero)
        {
            transform.localPosition -= currentOffset;
            currentOffset = Vector3.zero;
        }
    }
}
