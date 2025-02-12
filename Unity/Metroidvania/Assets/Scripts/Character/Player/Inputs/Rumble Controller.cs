using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class RumbleController : MonoBehaviour
{
    public static RumbleController Instance { get; private set; }
    private static Gamepad pad;
    
    private float currentLowFrequency = 0f;
    private float currentHighFrequency = 0f;
    private bool isContinuousRumble = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destruye este objeto si ya existe una instancia
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Permite que el objeto persista entre escenas
    }
    private void Update() {
        if(isContinuousRumble){

        }
    }

    /// <summary>
    /// Genera un pulso de vibración con las frecuencias y duración especificadas.
    /// </summary>
    public static void RumblePulse(float lowFrequency, float highFrequency, float duration)
    {
        if (!IsValidFrequency(lowFrequency) || !IsValidFrequency(highFrequency) || duration < 0)
        {
            Debug.LogWarning("Rumble parameters are out of range.");
            return;
        }

        pad = GetGamepad();
        if (pad != null)
        {
            
            Instance.StartCoroutine(Instance.RumbleForDuration(lowFrequency, highFrequency, duration));
        }
    }

    /// <summary>
    /// Corutina para manejar una vibración por una duración específica.
    /// </summary>
    private IEnumerator RumbleForDuration(float lowFrequency, float highFrequency, float duration)
    {
        // Sumar las frecuencias actuales sin exceder el rango máximo
        Instance.currentLowFrequency = Mathf.Min(Instance.currentLowFrequency + lowFrequency, 1f);
        Instance.currentHighFrequency = Mathf.Min(Instance.currentHighFrequency + highFrequency, 1f);
            
        pad.SetMotorSpeeds(Instance.currentLowFrequency, Instance.currentHighFrequency);
        yield return new WaitForSeconds(duration);
        // Después de la duración, no se detiene la vibración por sí sola, solo si se hace explícitamente
        Instance.currentLowFrequency = Mathf.Max(Instance.currentLowFrequency - lowFrequency, 0f);
        Instance.currentHighFrequency = Mathf.Max(Instance.currentHighFrequency - highFrequency, 0f);
        pad.SetMotorSpeeds(Instance.currentLowFrequency, Instance.currentHighFrequency);

    }

    /// <summary>
    /// Devuelve el Gamepad actual, si está disponible.
    /// </summary>
    private static Gamepad GetGamepad()
    {
        if (pad == null)
        {
            pad = Gamepad.current; // Actualiza si el gamepad actual se desconectó
        }
        return pad;
    }

    /// <summary>
    /// Valida que la frecuencia esté dentro del rango 0.0 a 1.0.
    /// </summary>
    private static bool IsValidFrequency(float frequency)
    {
        return frequency >= 0f && frequency <= 1f;
    }
}
