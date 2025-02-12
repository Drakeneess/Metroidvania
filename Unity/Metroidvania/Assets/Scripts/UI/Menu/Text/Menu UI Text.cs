using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIText : MonoBehaviour
{
    public string textKey; // Clave para identificar el texto en el CSV

    private TextMeshProUGUI uiText;

    private void Start()
    {
        uiText = GetComponent<TextMeshProUGUI>();
        UpdateText();
    }

    // Actualiza el texto basado en la clave y el idioma actual
    public void UpdateText()
    {
        if (uiText != null)
        {
            uiText.text = LanguageMenu.GetTranslate(textKey);
        }
    }
}
