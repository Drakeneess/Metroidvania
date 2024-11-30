using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public enum InteractionType { Hold, Press } // Tipos de interacción: presionar o mantener.
    public InteractableButtonUI interactableButton;
    public InteractionType interactionType; // Tipo de interacción actual.
    public bool isInteractable = true; // Si el objeto es interactuable.
    public float distanceToPlayer = 1; // Distancia a la que el jugador debe estar para interactuar.
    
    public float timeToAction = 1; // Tiempo requerido para realizar la acción al mantener el botón.

    protected Transform player; // Referencia al jugador.
    private float holdTime = 0f; // Tiempo durante el cual el jugador mantiene el botón presionado.
    private bool hasInteracted = false; // Bloqueo para evitar interacción repetida.

    protected virtual void Start()
    {
        player = FindObjectOfType<Player>().transform; // Obtenemos la referencia al jugador.
        if(interactableButton==null){
            interactableButton = GetComponentInChildren<InteractableButtonUI>();
        }
    }

    // Update es llamado una vez por cuadro (frame).
    protected virtual void Update()
    {
        DetectPlayer(); // Verificamos si el jugador está cerca y si puede interactuar.
    }

    private void DetectPlayer()
    {
        // Calculamos la distancia entre el jugador y el objeto interactuable.
        float distance = Vector3.Distance(transform.position, player.position);

        // Si el jugador está dentro del rango y el objeto es interactuable.
        if (distance <= distanceToPlayer && isInteractable)
        {
            if(!hasInteracted){
                interactableButton.Activate();
            }
            if (interactionType == InteractionType.Press) // Si la interacción es "Presionar"
            {
                if (InputController.GetInteractPressed() && !hasInteracted) // Si el jugador presiona el botón de interactuar y no ha interactuado antes.
                {
                    Action(); // Realizamos la acción.
                    hasInteracted = true; // Bloqueamos la interacción.
                }
            }
            else if (interactionType == InteractionType.Hold) // Si la interacción es "Mantener"
            {
                if (InputController.GetInteractHold() > 0 && !hasInteracted) // Si el jugador mantiene el botón presionado y no ha interactuado antes.
                {
                    // Aumentamos el tiempo de "hold" mientras el botón está siendo presionado.
                    holdTime += Time.deltaTime; 

                    // Puedes definir un tiempo mínimo de mantener el botón, por ejemplo, 1 segundo:
                    if (holdTime >= timeToAction)
                    {
                        Action(); // Realizamos la acción cuando el jugador mantuvo el botón el tiempo suficiente.
                        hasInteracted = true; // Bloqueamos la interacción.
                    }
                }
                else
                {
                    // Si el jugador deja de mantener el botón, reiniciamos el tiempo y desbloqueamos la interacción.
                    holdTime = 0f;
                    if (InputController.GetInteractHold() <= 0)
                    {
                        hasInteracted = false; // Permitimos la interacción nuevamente cuando el jugador suelta el botón.
                    }
                }
            }
        }
        else{
            interactableButton.Deactivate(); // Desactivamos el botón de interacción si el jugador
            hasInteracted = false; // jugador no está cerca o el objeto no es interactuable
        }
    }

    protected virtual void Action()
    {
        interactableButton.Deactivate();
        // Aquí iría la lógica para la acción que se ejecuta cuando el jugador interactúa.
        Debug.Log("Interacción realizada");
    }
}
