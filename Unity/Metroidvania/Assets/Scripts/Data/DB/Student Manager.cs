using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StudentManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField ciInputField;
    [SerializeField] private Button submitButton;
    [SerializeField] private ApiClient apiClient;
    [SerializeField] private TextMeshProUGUI errorText;

    private void Awake()
    {
        ciInputField.onValueChanged.AddListener(_ => errorText.gameObject.SetActive(false));
    }


    private void Start()
    {
        apiClient = apiClient ?? GetComponent<ApiClient>();

        if (SaveDataController.AreSavedData() && SaveDataController.Instance.saveData.studentID != -1)
        {
            gameObject.SetActive(false);
        }
        else
        {
            submitButton.onClick.AddListener(OnConfirmCI);
            StartCoroutine(FadeInElement(gameObject, 0.5f)); // Aparece con Fade In
        }
    }


    public void OnConfirmCI()
    {
        string ci = ciInputField.text.Trim();

        if (!IsCIValid(ci)) return;

        GetStudentIdByCI(ci);
    }

    private bool IsCIValid(string ci)
    {
        if (string.IsNullOrEmpty(ci))
        {
            ShowError("Por favor ingresa tu CI.");
            return false;
        }

        if (!ulong.TryParse(ci, out _)) // Solo números, sin signo
        {
            ShowError("El CI solo debe contener números.");
            return false;
        }

        if (ci.Length < 6 || ci.Length > 10)
        {
            ShowError("El CI debe tener entre 6 y 10 dígitos.");
            return false;
        }

        return true;
    }

    private void GetStudentIdByCI(string ci)
    {
        string endpoint = $"students/get_id_by_ci.php?ci={ci}";

        StartCoroutine(apiClient.GetRequest(
            endpoint,
            response =>
            {
                StudentIdResponseWrapper parsed = JsonUtility.FromJson<StudentIdResponseWrapper>(response);

                if (int.TryParse(parsed.data.id_student.Trim(), out int studentId))
                {
                    SaveDataController.Instance.saveData.studentID = studentId;
                    PlaythroughManager.Instance.FindPlaythroughID(studentId);
                    MainMenuFlowController.Instance.StartMenuFlow();
                    StartCoroutine(FadeOutAndDisable(gameObject, 0.75f));
                }
                else
                {
                    ShowError("El ID recibido no es válido.");
                    Debug.LogError($"❌ Invalid student ID format: {parsed.data.id_student}");
                }
            },
            error =>
            {
                Debug.LogError(error);
                ShowError("Conexión fallida. Intenta de nuevo.");
            }
        ));
    }
    private IEnumerator FadeElement(GameObject element, float duration, float targetAlpha)
    {
        CanvasGroup canvasGroup = GetOrAddCanvasGroup(element);
        float startAlpha = canvasGroup.alpha; // Almacena el valor de opacidad inicial
        float elapsedTime = 0f;

        // Aplica la interpolación de fade durante la duración especificada
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha; // Asegura que el valor final se aplique
    }
    private CanvasGroup GetOrAddCanvasGroup(GameObject obj)
    {
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = obj.AddComponent<CanvasGroup>(); // Si no tiene un CanvasGroup, lo agrega
        }
        return canvasGroup;
    }

    private IEnumerator FadeInElement(GameObject element, float duration)
    {
        CanvasGroup canvasGroup = GetOrAddCanvasGroup(element);
        canvasGroup.alpha = 0f;
        element.SetActive(true);

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }
    private IEnumerator FadeOutAndDisable(GameObject element, float duration)
    {
        yield return StartCoroutine(FadeElement(element, duration, 0f));
        element.SetActive(false);
        gameObject.SetActive(false);
    }
    private void ShowError(string message)
    {
        if (errorText != null)
        {
            errorText.text = message;
            errorText.gameObject.SetActive(true);
            StartCoroutine(HideErrorAfterDelay(10));
        }
    }

    private IEnumerator HideErrorAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        errorText.gameObject.SetActive(false);
    }

}

[System.Serializable]
public class StudentIdData
{
    public string id_student; // viene como string del backend
}

[System.Serializable]
public class StudentIdResponseWrapper : GenericResponse<StudentIdData> { }
