using System.Collections;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public enum InteractionType { Hold, Press }

    public ButtonUI interactableButton;
    public InteractionType interactionType;
    public bool isInteractable = true;
    public float distanceToPlayer = 1;
    public float timeToAction = 1;

    private float holdTime = 0f;
    private bool hasInteracted = false;
    private bool canInteract = false;

    protected virtual void Start()
    {
        if (interactableButton == null)
        {
            interactableButton = GetComponentInChildren<ButtonUI>();
        }

        PlayerProximityDetector.Instance.RegisterInteractable(this);
    }

    protected virtual void OnDestroy()
    {
        if (PlayerProximityDetector.Instance != null)
            PlayerProximityDetector.Instance.UnregisterInteractable(this);
    }

    public void UpdateProximity(bool inRange)
    {
        if (inRange && isInteractable)
        {
            if (!hasInteracted)
            {
                interactableButton.Activate();
                canInteract = true;
            }
        }
        else
        {
            interactableButton.Deactivate();
            hasInteracted = false;
            canInteract = false;
        }
    }

    protected virtual void OnEnable()
    {        
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnActionTriggered += InteractPressed;
            InputActionController.Instance.OnFloatInput += InteractHold;
        }
        else
        {
            Debug.LogWarning($"[{name}] InputActionController.Instance es NULL");
        }
    }


    protected virtual void OnDisable()
    {
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnActionTriggered -= InteractPressed;
            InputActionController.Instance.OnFloatInput -= InteractHold;
        }
    }

    private void InteractPressed(string actionName)
    {
        if (actionName == "InteractPressed")
        {
            if (interactionType == InteractionType.Press && isInteractable && !hasInteracted && canInteract)
            {
                Action();
                hasInteracted = true;
            }
        }
    }

    private void InteractHold(string actionName, float holdDuration)
    {
        if (actionName == "InteractHold")
        {
            if (interactionType == InteractionType.Hold && isInteractable && !hasInteracted && canInteract)
            {
                holdTime += holdDuration;

                if (holdTime >= timeToAction)
                {
                    Action();
                    hasInteracted = true;
                }
            }
            else
            {
                holdTime = 0f;
            }
        }
    }

    protected virtual void Action()
    {
        interactableButton.Deactivate();
    }
}

