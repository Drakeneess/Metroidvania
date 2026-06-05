using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Interactable : MonoBehaviour
{
    public enum InteractionType { Hold, Press }

    [Header("UI / Tipo")]
    public ButtonUI interactableButton;
    public InteractionType interactionType = InteractionType.Press;

    [Header("Estado")]
    public bool isInteractable = true;

    [Header("Rango / Timings")]
    [Min(0.01f)] public float distanceToPlayer = 1f; // mapea a SphereCollider.radius
    [Min(0f)] public float timeToAction = 1f;        // para Hold

    private float holdTime = 0f;
    protected bool hasInteracted = false;
    protected bool canInteract = false;

    private SphereCollider triggerCol;
    private Rigidbody rb; // opcional para asegurar eventos de trigger

    protected virtual void Reset()
    {
        triggerCol = GetComponent<SphereCollider>();
        triggerCol.isTrigger = true;
        triggerCol.radius = distanceToPlayer;

        // Asegurar un Rigidbody kinemático si no hay en Player ni aquí (fallback práctico)
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
            Debug.Log($"[{name}] Rigidbody kinemático agregado para asegurar eventos de trigger.");
        }
    }

    protected virtual void OnValidate()
    {
        if (triggerCol == null) triggerCol = GetComponent<SphereCollider>();
        if (triggerCol != null)
        {
            triggerCol.isTrigger = true;
            triggerCol.radius = Mathf.Max(0.01f, distanceToPlayer);
        }
    }

    protected virtual void Start()
    {
        if (interactableButton == null)
            interactableButton = GetComponentInChildren<ButtonUI>(includeInactive: true);

        if (interactableButton != null) interactableButton.Deactivate();
    }

    protected virtual void OnEnable()
    {
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnActionTriggered += InteractPressed;
            InputActionController.Instance.OnFloatInput += InteractHold;
//            Debug.Log($"[{name}] Suscrito a InputActionController {InputController.instance.name} ✅");
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

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!isInteractable) { Debug.Log($"[{name}] OnTriggerEnter ignorado: !isInteractable"); return; }
        if (!IsPlayer(other))
        {
            // Debug fino: ver quién entra por si estás chocando con hijos sin tag
            // Debug.Log($"[{name}] OnTriggerEnter por {other.name}, no es Player.");
            return;
        }

        if (!hasInteracted)
        {
            canInteract = true;
            if (interactableButton != null) interactableButton.Activate();
//            Debug.Log($"[{name}] Player en rango → canInteract=TRUE");
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (!IsPlayer(other)) return;

        canInteract = false;
        hasInteracted = false;
        holdTime = 0f;
        if (interactableButton != null) interactableButton.Deactivate();
       // Debug.Log($"[{name}] Player salió del rango → canInteract=FALSE, reset hold");
    }

    private bool IsPlayer(Collider other)
    {
        // Acepta:
        // 1) Tag en el objeto o sus padres
        if (other.CompareTag("Player") || (other.attachedRigidbody && other.attachedRigidbody.CompareTag("Player")))
            return true;

        // 2) Componente Player en el collider o en sus padres
        if (other.GetComponentInParent<Player>() != null)
            return true;

        // 3) Layer opcional (descomenta si usas layer específica)
        // if (other.gameObject.layer == LayerMask.NameToLayer("Player")) return true;

        return false;
    }

    private void InteractPressed(InputActionType actionName)
    {
        if (actionName != InputActionType.InteractPressed) return;
//        Debug.Log($"[{name}] InteractPressed recibido. "
  //              + $"type={interactionType}, isInteractable={isInteractable}, hasInteracted={hasInteracted}, canInteract={canInteract}");

        if (interactionType != InteractionType.Press) return;
        if (!isInteractable || hasInteracted || !canInteract) return;

        //Debug.Log($"[{name}] Ejecutando Action() por Press");
        Action();
        hasInteracted = true;
        if (interactableButton != null) interactableButton.Deactivate();
    }

    private void InteractHold(InputActionType actionName, float deltaHold)
    {
        if (actionName != InputActionType.OnInteractHold) return;

        if (interactionType != InteractionType.Hold)
        {
            // Si llega por error un Hold cuando es Press, ignora
            return;
        }

        if (isInteractable && !hasInteracted && canInteract)
        {
            holdTime += deltaHold;
            // Debug.Log($"[{name}] Hold: {holdTime:0.00}/{timeToAction}");

            if (holdTime >= timeToAction)
            {
                //Debug.Log($"[{name}] Ejecutando Action() por Hold");
                Action();
                hasInteracted = true;
                holdTime = 0f;
                if (interactableButton != null) interactableButton.Deactivate();
            }
        }
        else
        {
            if (holdTime != 0f) Debug.Log($"[{name}] Hold reseteado (isInteractable={isInteractable}, hasInteracted={hasInteracted}, canInteract={canInteract})");
            holdTime = 0f;
        }
    }

    protected virtual void Action()
    {
        if (interactableButton != null) interactableButton.Deactivate();
        BehaviorManager.Instance.AddAction();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, distanceToPlayer);
    }
    protected void LogBegin(List<string> extras)
    {
        PlayerActionLogger.Instance.Log("BeginInteraction", extras);
    }

    protected void LogEnd(List<string> extras)
    {
        PlayerActionLogger.Instance.Log("EndInteraction", extras);
    }
}
