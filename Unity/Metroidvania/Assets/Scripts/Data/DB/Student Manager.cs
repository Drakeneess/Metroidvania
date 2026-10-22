using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Compatibility component for the former student-identification gate.
/// The standalone game bypasses research identification entirely.
/// </summary>
public class StudentManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField ciInputField;
    [SerializeField] private Button submitButton;
    [SerializeField] private ApiClient apiClient;
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private LogoController logoController;

    private void Start()
    {
        if (logoController != null)
            logoController.gameObject.SetActive(true);

        // This object represented the thesis participant-identification UI.
        // Keep the component serialized for compatibility, but skip that gate.
        gameObject.SetActive(false);
    }

    public void OnConfirmCI()
    {
        Debug.LogWarning("[StudentManager] Student identification is disabled in the standalone game.");
    }
}

[System.Serializable]
public class StudentIdData
{
    public string id_student;
}

[System.Serializable]
public class StudentIdResponseWrapper : GenericResponse<StudentIdData> { }
