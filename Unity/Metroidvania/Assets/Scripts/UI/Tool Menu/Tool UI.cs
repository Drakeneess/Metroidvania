using UnityEngine;

public class ToolUI : MonoBehaviour
{
    [Header("Rotación Manual")]
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float verticalMin = -30f;
    [SerializeField] private float verticalMax = 30f;

    [Header("Idle Rotation")]
    [SerializeField] private float idleRotationSpeed = 10f;

    private Vector2 currentRotation;
    private Vector2 rotationInput = Vector2.zero;
    private bool isRotatingManually => rotationInput != Vector2.zero;

    void OnEnable()
    {
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnVector2Input += HandleInput;
        }
    }

    void OnDisable()
    {
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnVector2Input -= HandleInput;
        }
    }

    void Update()
    {
        if (isRotatingManually)
        {
            currentRotation.x -= rotationInput.y * rotationSpeed * Time.deltaTime;
            currentRotation.y += rotationInput.x * rotationSpeed * Time.deltaTime;
        }
        else
        {
            currentRotation.y += idleRotationSpeed * Time.deltaTime;
        }

        currentRotation.x = Mathf.Clamp(currentRotation.x, verticalMin, verticalMax);
        transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0f);
    }

    private void HandleInput(InputActionType actionName, Vector2 value)
    {
        if (actionName != InputActionType.RotateTool) return;

        rotationInput = value != Vector2.zero ? value.normalized : Vector2.zero;
    }
}
