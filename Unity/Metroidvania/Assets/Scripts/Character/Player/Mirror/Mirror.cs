using System.Collections;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    [SerializeField] private CharacterMovement player;
    [SerializeField] private float levitationHeight = 0.5f;
    [SerializeField] private float levitationSpeed = 2f;
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float rotationSpeed = 3f;

    [SerializeField] private float distanceX = 2f;
    [SerializeField] private float distanceY = 0.5f;
    [SerializeField] private float distanceZ = 1f;

    private float levitationOffset;
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
    }

    void LateUpdate()
    {
        UpdateLevitation();
        UpdatePosition();
        SmoothRotation();
    }

    // Controla la levitación normal y reducida en combate
    private void UpdateLevitation()
    {
        float levitationFactor = isAttacking ? 0.2f : 1f;
        levitationOffset = Mathf.Sin(Time.time * levitationSpeed * levitationFactor) * (levitationHeight * levitationFactor);
        transform.position = new Vector3(transform.position.x, player.transform.position.y + distanceY + levitationOffset, transform.position.z);
    }

    // Decide si seguir al jugador o moverse frente a él
    private void UpdatePosition()
    {
        Vector3 targetPosition = isAttacking ? GetAttackPosition() : GetFollowPosition();
        if ((targetPosition - transform.position).sqrMagnitude > 0.001f) // Evita cálculos innecesarios
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed * (isAttacking ? 1.5f : 1f));
        }
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
            0f
        );
    }

    // Mantiene la rotación oscilante del espejo
    private void SmoothRotation()
    {
        float rotationAmplitude = isAttacking ? 10f : 2f;
        float rotationFactor = Time.time * rotationSpeed;
        
        Quaternion targetRotation = Quaternion.Euler(
            Mathf.Sin(rotationFactor) * rotationAmplitude,
            Mathf.Cos(rotationFactor) * rotationAmplitude,
            Mathf.Sin(rotationFactor) * rotationAmplitude
        );

        float slerpSpeed = isAttacking ? rotationSpeed : 20f;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * slerpSpeed);
    }

    public void SetAttackingState(bool state)
    {
        isAttacking = state;
    }
}
