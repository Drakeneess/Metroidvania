using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    [SerializeField]
    private CharacterMovement player;
    [SerializeField]
    private float levitationHeight = 0.5f;  // Altura máxima de levitación
    [SerializeField]
    private float levitationSpeed = 2f;  // Velocidad de levitación
    [SerializeField]
    private float followSpeed = 5f;  // Velocidad con la que el espejo sigue al jugador
    [SerializeField]
    private float rotationSpeed = 3f;  // Velocidad con la que el espejo rota

    // Distancia deseada en los ejes X y Y
    [SerializeField]
    private float distanceX = 2f;  // Distancia en X respecto al jugador
    [SerializeField]
    private float distanceY = 0.5f;  // Distancia en Y respecto al jugador
    [SerializeField]
    private float distanceZ = 1f;  // Distancia en Z respecto al jugador
    

    private float levitationOffset;

    // Start is called before the first frame update
    void Start()
    {
        if(player == null){
            player = FindObjectOfType<CharacterMovement>();  // Buscar al jugador si no está asignado
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Efecto de levitación (movimiento oscilante)
        LevitatingEffect();

        // Seguir al jugador con un suave movimiento y aplicar las distancias en X y Y
        FollowPlayer();

        // Agregar rotaciones suaves
        SmoothRotation();
    }

    // Efecto de levitación
    private void LevitatingEffect()
    {
        levitationOffset = Mathf.Sin(Time.time * levitationSpeed) * levitationHeight;
        transform.position = new Vector3(transform.position.x, player.transform.position.y + distanceY + levitationOffset, transform.position.z);
    }

    // Hacer que el espejo siga al jugador con suavizado y con distancia ajustada
    private void FollowPlayer()
    {
        // Calculamos la posición deseada manteniendo las distancias en X y Z
        Vector3 targetPosition = new Vector3(
            player.transform.position.x + distanceX * -player.Direction,  // Distancia en X
            player.transform.position.y,  
            player.transform.position.z + distanceZ);             // Distancia en Z

        // Movemos el espejo suavemente hacia la posición calculada
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);  
    }

    // Rotaciones suaves en X, Y y Z
    private void SmoothRotation()
    {
        // Calculamos la rotación deseada (puedes ajustar los valores según el efecto que desees)
        Quaternion targetRotation = Quaternion.Euler(
            Mathf.Sin(Time.time * rotationSpeed) * 10f, // Oscilación suave en X
            Mathf.Cos(Time.time * rotationSpeed) * 10f, // Oscilación suave en Y
            Mathf.Sin(Time.time * rotationSpeed) * 10f  // Oscilación suave en Z
        );

        // Aplicamos la rotación suave
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}
