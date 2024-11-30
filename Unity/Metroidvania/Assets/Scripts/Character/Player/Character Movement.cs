using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5.0f;
    public float jumpForce = 5.0f;
    public int maxJump = 2;

    private Rigidbody rb;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    private bool isGrounded;
    private int jumpCount;
    private float direction = 1;

    private float zThreshold = 0.01f; // Umbral para determinar si el personaje está fuera del eje Z permitido.

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jumpCount = 0;
    }

    void Update()
    {
        HandleJump();
        HandleRotation();
        CheckZPosition(); // Verificamos si el personaje necesita reacomodarse en el eje Z.
    }

    private void HandleMovement(float directionMove)
    {
        Vector3 move = transform.right * speed * directionMove * Time.deltaTime;
        transform.Translate(move,Space.Self);
    }

    private void HandleJump()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded)
        {
            jumpCount = 0;
        }

        if (jumpCount < maxJump && InputController.GetJump())
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpCount++;
        }
    }

    private void HandleRotation()
    {
        float horizontalInput = InputController.GetMovement();

        if (horizontalInput != 0)
        {
            direction = Mathf.Sign(horizontalInput);
            float targetRotation = direction == 1 ? 0 : 180;
            transform.rotation = Quaternion.Euler(0, targetRotation, 0);
            HandleMovement(horizontalInput);
        }
    }

    /// <summary>
    /// Verifica si el personaje está desalineado en el eje Z y lo reacomoda si es necesario.
    /// </summary>
    private void CheckZPosition()
    {
        // Si la posición en Z se desvía del valor permitido, reacomodamos.
        if (Mathf.Abs(transform.position.z) > zThreshold)
        {
            Vector3 correctedPosition = new Vector3(transform.position.x, transform.position.y, 0);
            transform.localPosition = correctedPosition;
        }
    }
}
