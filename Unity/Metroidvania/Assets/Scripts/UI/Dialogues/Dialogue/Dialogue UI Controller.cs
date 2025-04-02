using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUIController : MonoBehaviour
{
    public Image buttonIcon;
    public Image nextDialogueArrow;
    public Color pressColor = Color.gray;
    public float pressEffectDuration = 0.1f;

    private Color originalButtonColor;
    private Color originalArrowColor;

    private void Awake()
    {
        if (buttonIcon == null)
        {
            buttonIcon = GetComponent<Image>();
            if (buttonIcon == null)
                Debug.LogError("No Image component found on buttonIcon");
        }
        else
        {
            originalButtonColor = buttonIcon.color;
        }

        if (nextDialogueArrow == null)
        {
            nextDialogueArrow = GetComponentInChildren<Image>();
            if (nextDialogueArrow == null)
                Debug.LogError("No Image component found on nextDialogueArrow");
        }
        else
        {
            originalArrowColor = nextDialogueArrow.color;
        }
    }

    private void OnEnable()
    {
        SetOriginalState();
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnActionTriggered += SelectPressed;
        }
    }

    private void OnDisable()
    {
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnActionTriggered -= SelectPressed;
        }
    }

    private void SelectPressed(string actionName)
    {
        if (actionName == "OptionSelect"){
            if (DialogueSystem.Instance.dialoguePanel.activeSelf)
            {
                StartCoroutine(ButtonPressEffect());
                DialogueSystem.Instance.ShowNextDialogue();
            }
        }
    }

    private IEnumerator ButtonPressEffect()
    {
        SetPressState();
        yield return new WaitForSeconds(pressEffectDuration);
        SetOriginalState();
    }

    private void SetPressState()
    {
        if (buttonIcon != null)
            buttonIcon.color = pressColor;
        if (nextDialogueArrow != null)
            nextDialogueArrow.color = pressColor;
    }

    private void SetOriginalState()
    {
        if (buttonIcon != null)
            buttonIcon.color = originalButtonColor;
        if (nextDialogueArrow != null)
            nextDialogueArrow.color = originalArrowColor;
    }
}
