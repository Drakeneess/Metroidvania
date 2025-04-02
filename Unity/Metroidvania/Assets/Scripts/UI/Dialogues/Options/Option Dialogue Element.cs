using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionDialogueElement : MonoBehaviour
{
    private Button button;
    public Button Button { get => button; }
    private TextMeshProUGUI textMesh;
    private GameObject optionDialogueObject;

    public void SetButton(LocalizedDecision decisionText, GameObject optionDialogue)
    {
        button = GetComponent<Button>();
        textMesh = GetComponentInChildren<TextMeshProUGUI>();

        textMesh.text = decisionText.text;
        button.onClick.AddListener(OnPressedAction);

        optionDialogueObject = optionDialogue;

        // Configurar el botón para usar navegación automática
        Navigation nav = new Navigation { mode = Navigation.Mode.Automatic };
        button.navigation = nav;
    }

    public void PressButton()
    {
        if (button == null)
        {
            Debug.LogError("Button no ha sido inicializado en OptionDialogueElement.");
            return;
        }

        // Asegurar que el EventSystem está activo
        if (EventSystem.current == null)
        {
            Debug.LogError("No hay un EventSystem activo en la escena.");
            return;
        }

        // Forzar selección en el EventSystem
        EventSystem.current.SetSelectedGameObject(button.gameObject);

        // Forzar transición al estado "Highlighted"
        button.Select();
        button.OnSelect(null);

        // Simular un clic en el botón
        ExecuteEvents.Execute(button.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
        ExecuteEvents.Execute(button.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerUpHandler);

        button.onClick.Invoke();
    }

    private void OnPressedAction()
    {
        StartCoroutine(ShowNextDialogue());   
    }

    private IEnumerator ShowNextDialogue(){
        yield return new WaitForSeconds(0.2f);
        DialogueSystem.IsOptionActive = false;
        DialogueSystem.Instance.ShowNextDialogue();
        
        optionDialogueObject.SetActive(false);
    }
}
