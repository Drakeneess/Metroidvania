using UnityEngine;

public abstract class SettingsControlBase : MonoBehaviour
{
    public delegate void ValueChanged(SettingsControlBase control);
    public event ValueChanged OnValueChanged;

    // Llamar esto en el control cuando cambie su valor
    protected void NotifyValueChanged()
    {
        OnValueChanged?.Invoke(this);
    }

    public abstract void Highlight(bool active);
    public abstract bool OnSelect();
    public abstract bool OnNavigate(float x);
    public abstract string GetValue();
}
