using UnityEngine;

public class MenuUINavigator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform mapContainer;
    [SerializeField] private GameObject mapCanvas;

    [Header("Pan Settings")]
    [SerializeField] private float panSpeed = 200f;
    [SerializeField, Range(0.01f, 1f)] private float panSensitivity = 0.1f;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 0.05f;
    [SerializeField] private Vector2 zoomLimits = new Vector2(0.5f, 3f);

    [Header("Invoke Settings")]
    [SerializeField] private float repeatRate = 0.02f; // ~50 FPS

    private Vector3 originalPosition;
    private Vector3 originalScale;

    private Vector2 currentMovement;
    private float currentZoomInput;
    private bool isPanning;
    private bool isZooming;
    private bool subscribed;

    void Awake()
    {
        if (mapContainer != null)
        {
            originalPosition = mapContainer.anchoredPosition;
            originalScale = mapContainer.localScale;
        }
    }

    void OnEnable()
    {
        if (!subscribed && InputActionController.Instance != null)
        {
            InputActionController.Instance.OnFloatInput += HandleZoomInput;
            InputActionController.Instance.OnVector2Input += HandleNavigationInput;
            InputActionController.Instance.OnActionTriggered += HandleCloseInput;
            subscribed = true;
        }

        currentMovement = Vector2.zero;
        currentZoomInput = 0f;
    }

    void OnDisable()
    {
        Unsubscribe();
        StopPan();
        StopZoom();
        currentMovement = Vector2.zero;
        currentZoomInput = 0f;
    }

    void OnDestroy()
    {
        // Importante si el objeto se destruye sin pasar por OnDisable
        Unsubscribe();
        StopPan();
        StopZoom();
    }

    private void Unsubscribe()
    {
        if (subscribed && InputActionController.Instance != null)
        {
            InputActionController.Instance.OnFloatInput -= HandleZoomInput;
            InputActionController.Instance.OnVector2Input -= HandleNavigationInput;
            InputActionController.Instance.OnActionTriggered -= HandleCloseInput;
            subscribed = false;
        }
    }

    private void HandleCloseInput(string actionName)
    {
        if (actionName != "CloseMap") return;

        // Protecciones contra referencias destruidas (null de Unity)
        if (mapContainer != null)
        {
            ResetMapView();
            PlayerActionLogger.Instance.Log("CloseMap");
        }

        if (mapCanvas) // usa la null de Unity
        {
            // Extra: evita llamar SetActive si ya está desactivado
            if (mapCanvas.activeSelf)
                mapCanvas.SetActive(false);
        }
        else
        {
            // Si llegó aquí destruido, sencillamente ignoramos
            // (o loguea para rastrear quién lo destruye)
            Debug.LogWarning("MenuUINavigator: mapCanvas ya no existe al cerrar.");
        }
    }

    // ------------------ Pan ------------------
    private void HandleNavigationInput(string actionName, Vector2 value)
    {
        if (actionName != "MapNavigation" || mapContainer == null) return;

        currentMovement = -value;

        if (value != Vector2.zero && !isPanning)
        {
            InvokeRepeating(nameof(ApplyPan), 0f, repeatRate);
            isPanning = true;
        }
        else if (value == Vector2.zero && isPanning)
        {
            StopPan();
        }
    }

    private void ApplyPan()
    {
        if (mapContainer == null || currentMovement == Vector2.zero)
        {
            StopPan();
            return;
        }

        mapContainer.anchoredPosition += currentMovement * panSpeed * panSensitivity * Time.unscaledDeltaTime;
    }

    private void StopPan()
    {
        CancelInvoke(nameof(ApplyPan));
        isPanning = false;
    }

    // ------------------ Zoom ------------------
    private void HandleZoomInput(string actionName, float value)
    {
        if (actionName != "MapZoom" || mapContainer == null) return;

        currentZoomInput = value;

        if (Mathf.Abs(value) > 0.01f && !isZooming)
        {
            InvokeRepeating(nameof(ApplyZoom), 0f, repeatRate);
            isZooming = true;
        }
        else if (Mathf.Abs(value) <= 0.01f && isZooming)
        {
            StopZoom();
        }
    }

    private void ApplyZoom()
    {
        if (mapContainer == null || Mathf.Abs(currentZoomInput) <= 0.01f)
        {
            StopZoom();
            return;
        }

        float currentZoom = mapContainer.localScale.x;
        float targetZoom = Mathf.Clamp(currentZoom + currentZoomInput * zoomSpeed, zoomLimits.x, zoomLimits.y);
        float smoothZoom = Mathf.Lerp(currentZoom, targetZoom, 0.3f);
        mapContainer.localScale = new Vector3(smoothZoom, smoothZoom, 1f);
    }

    private void StopZoom()
    {
        CancelInvoke(nameof(ApplyZoom));
        isZooming = false;
    }

    // ------------------ Reset ------------------
    public void ResetMapView()
    {
        if (mapContainer != null)
        {
            mapContainer.anchoredPosition = originalPosition;
            mapContainer.localScale = originalScale;
        }
    }
}
