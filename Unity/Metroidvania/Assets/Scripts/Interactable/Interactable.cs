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

    protected Transform player;
    private float holdTime = 0f;
    private bool hasInteracted = false;
    private bool canInteract=false;

    protected virtual void Start()
    {
        player = FindObjectOfType<Player>().transform;
        if (interactableButton == null)
        {
            interactableButton = GetComponentInChildren<ButtonUI>();
        }
    }

    protected virtual void OnEnable()
    {
        if (InputActionController.Instance != null)
        {
            // Suscribir eventos de presionar y mantener
            InputActionController.Instance.OnInteractPressed += InteractPressed;
            InputActionController.Instance.OnInteractHold += InteractHold;
        }
    }

    protected virtual void OnDisable()
    {
        if (InputActionController.Instance != null)
        {
            // Desuscribir eventos de presionar y mantener
            InputActionController.Instance.OnInteractPressed -= InteractPressed;
            InputActionController.Instance.OnInteractHold -= InteractHold;
        }
    }

    private void Update()
    {
        DetectPlayer();
    }

    private void DetectPlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= distanceToPlayer && isInteractable)
        {
            if (!hasInteracted)
            {
                interactableButton.Activate();
                canInteract=true;
            }
        }
        else
        {
            interactableButton.Deactivate();
            hasInteracted = false;
            canInteract = false;
        }
    }

    // Acción para la interacción con "Presionar"
    private void InteractPressed()
    {
        if (interactionType == InteractionType.Press && isInteractable && !hasInteracted && canInteract)
        {
            Action();
            hasInteracted = true;
        }
    }

    // Acción para la interacción con "Mantener"
    private void InteractHold(float holdDuration)
    {
        if (interactionType == InteractionType.Hold && isInteractable && !hasInteracted && !canInteract)
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
            holdTime = 0f;  // Resetear si no se mantiene
        }
    }

    // Acción que realiza el interactuable (puedes personalizarla según sea necesario)
    protected virtual void Action()
    {
        interactableButton.Deactivate();
    }
}
