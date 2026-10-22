using UnityEngine;

/// <summary>
/// Backwards-compatible adapter for scenes that still reference SettingsUploader.
/// Settings are persisted locally; nothing is uploaded.
/// </summary>
public class SettingsUploader : MonoBehaviour
{
    public static SettingsUploader Instance { get; private set; }

    private bool hasChanges;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SettingsControlBase.OnAnyValueChanged += OnSettingChanged;
    }

    private void OnDisable()
    {
        SettingsControlBase.OnAnyValueChanged -= OnSettingChanged;
    }

    private void OnSettingChanged()
    {
        hasChanges = true;
    }

    public void OnSettingsMenuClosed()
    {
        if (!hasChanges) return;

        SettingsValue.Instance?.SaveSettings();
        hasChanges = false;
    }
}
