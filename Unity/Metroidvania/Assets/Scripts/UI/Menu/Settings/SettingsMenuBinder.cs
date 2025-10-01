using System;
using UnityEngine;

public class SettingsMenuBinder : MonoBehaviour
{
    public SettingsButtons buttons;

    private void Start()
    {
        LoadFromSettings();

        // suscribir a cambios
        foreach (var ctrl in buttons.controls)
        {
            if (ctrl == null) continue;
            ctrl.OnValueChanged += HandleValueChanged;
        }
    }

    private void OnDestroy()
    {
        foreach (var ctrl in buttons.controls)
        {
            if (ctrl == null) continue;
            ctrl.OnValueChanged -= HandleValueChanged;
        }
    }

    private void HandleValueChanged(SettingsControlBase control)
    {
        if (SettingsValue.Instance == null) return;
        var gs = SettingsValue.Instance.Settings;

        int idx = Array.IndexOf(buttons.controls, control);
        if (idx < 0) return;

        switch (idx)
        {
            case 0 when control is SettingsSlider s: gs.rumbleValue = s.Value; break;
            case 1 when control is SettingsSlider s: gs.music = s.Value; break;
            case 2 when control is SettingsSlider s: gs.fxSound = s.Value; break;
            case 3 when control is SettingsSlider s: gs.brightness = s.Value; break;
            case 4 when control is SettingsCarousel c: gs.resolutionIndex = c.currentIndex; break;
            case 5 when control is SettingsCarousel c:
                gs.language = (Language)c.currentIndex;
                LanguageController.SetLanguage(gs.language); // 🔥 tiempo real
                break;
        }

        SettingsValue.Instance.SaveSettings();
    }

    public void LoadFromSettings()
    {
        if (SettingsValue.Instance == null) return;
        var gs = SettingsValue.Instance.Settings;

        // Idiomas primero
        if (buttons.controls.Length > 5 && buttons.controls[5] is SettingsCarousel langCarousel)
            LoadLanguagesIntoCarousel(langCarousel);

        Apply(0, gs.rumbleValue);
        Apply(1, gs.music);
        Apply(2, gs.fxSound);
        Apply(3, gs.brightness);
        Apply(4, gs.resolutionIndex);
        Apply(5, (int)gs.language);
    }

    private void Apply(int index, float value)
    {
        if (index >= buttons.controls.Length) return;
        if (buttons.controls[index] is SettingsSlider slider)
            slider.SetValue(value, true);
        else if (buttons.controls[index] is SettingsCarousel carousel)
            carousel.SetIndex(Mathf.RoundToInt(value));
    }

    private void Apply(int index, int value)
    {
        if (index >= buttons.controls.Length) return;
        if (buttons.controls[index] is SettingsSlider slider)
            slider.SetValue(value, true);
        else if (buttons.controls[index] is SettingsCarousel carousel)
            carousel.SetIndex(value);
    }

    private void LoadLanguagesIntoCarousel(SettingsCarousel carousel)
    {
        carousel.options.Clear();
        foreach (Language lang in Enum.GetValues(typeof(Language)))
        {
            carousel.options.Add(lang.ToString()); // Si quieres "English", "Espanol"
            // o podrías mapearlos con LanguageController.GetLanguageString()
        }
        carousel.SetIndex((int)SettingsValue.Instance.Settings.language);
    }
}


    