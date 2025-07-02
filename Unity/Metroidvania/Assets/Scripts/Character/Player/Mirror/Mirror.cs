using System.Collections;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    [SerializeField] private CharacterMovement player;
    public CharacterMovement Player { get { return player; } }

    [Header("Levitación")]
    [SerializeField] private float levitationHeight = 0.5f;
    [SerializeField] private float levitationSpeed = 2f;

    [Header("Movimiento")]
    [SerializeField] private float rotationSpeed = 3f;

    [Header("Distancias")]
    [SerializeField] private float distanceX = 2f;
    [SerializeField] private float distanceY = 0.5f;
    [SerializeField] private float distanceZ = 1f;

    private float levitationOffset;
    private Vector3 basePosition;
    private Vector3 velocity = Vector3.zero;
    private Quaternion currentRotation;

    private bool isAttacking = false;
    public float attackRange { get; set; }

    void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<CharacterMovement>();
            if (player == null)
            {
                Debug.LogWarning("No se encontró un CharacterMovement en la escena.");
            }
        }

        basePosition = transform.position;
        currentRotation = transform.rotation;
    }

    void LateUpdate()
    {
        UpdateBasePosition();
        UpdateLevitation();
        SmoothRotation();
    }

    // Movimiento base hacia el objetivo, suavizado
    private void UpdateBasePosition()
    {
        Vector3 targetPosition = isAttacking ? GetAttackPosition() : GetFollowPosition();
        float smoothTime = isAttacking ? 0.1f : 0.2f;

        basePosition = Vector3.SmoothDamp(basePosition, targetPosition, ref velocity, smoothTime);
    }

    // Levitación suave encima de la basePosition
    private void UpdateLevitation()
    {
        float levitationFactor = isAttacking ? 0.2f : 1f;
        levitationOffset = Mathf.Sin(Time.time * levitationSpeed * levitationFactor) * (levitationHeight * levitationFactor);

        Vector3 levitatedPosition = basePosition + new Vector3(0f, distanceY + levitationOffset, 0f);
        transform.position = levitatedPosition;
    }

    private Vector3 GetFollowPosition()
    {
        return new Vector3(
            player.transform.position.x + distanceX * -player.Direction,
            player.transform.position.y,
            player.transform.position.z + distanceZ
        );
    }

    private Vector3 GetAttackPosition()
    {
        return new Vector3(
            player.transform.position.x + (distanceX + attackRange) * player.Direction,
            player.transform.position.y,
            player.transform.position.z
        );
    }

    // Rotación suave y oscilante del espejo
    private void SmoothRotation()
    {
        float rotationAmplitude = isAttacking ? 10f : 2f;
        float rotationFactor = Time.time * rotationSpeed;

        Quaternion targetRotation = Quaternion.Euler(
            Mathf.Sin(rotationFactor) * rotationAmplitude,
            Mathf.Cos(rotationFactor) * rotationAmplitude,
            Mathf.Sin(rotationFactor) * rotationAmplitude
        );

        float slerpSpeed = isAttacking ? rotationSpeed : 5f;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * slerpSpeed);
    }

    public void SetAttackingState(bool state)
    {
        isAttacking = state;
    }
}
