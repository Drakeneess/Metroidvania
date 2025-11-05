using UnityEngine;
using System.Collections;

public class LedgeGrab : MonoBehaviour
{
    [Header("Timings")]
    public float hangTime = 0.55f;
    public float climbDuration = 0.6f;

    [Header("Movimiento suave")]
    public AnimationCurve climbEasing = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [Range(0f, 720f)] public float rotateDegreesPerSecond = 540f;

    [Header("Capas / Animación")]
    public string actionsLayerName = "Actions";
    public string climbTriggerName = "ClimbUp";
    public string hangingBoolName = "IsHanging";

    [Header("Refs (asigna o se autollenan)")]
    [SerializeField] private CharacterMovement movement;
    [SerializeField] private MovementControl movementControl;
    [SerializeField] private Animator animator;

    private bool isGrabbing = false;
    private int actionsLayerIndex = -1;
    private PlayerAnimationController anim;

    private void Start()
    {
        GameMenuController.CurrentMode = GameMode.Game;
        if (!movement) movement = GetComponentInParent<CharacterMovement>();
        if (!movementControl) movementControl = GetComponentInParent<MovementControl>();
        if (!animator) animator = GetComponentInParent<Animator>();
        anim = PlayerAnimationController.Instance;

        if (animator && !string.IsNullOrEmpty(actionsLayerName))
            actionsLayerIndex = animator.GetLayerIndex(actionsLayerName);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!movement || !movement.IsOnAir) return;
        if (!other.CompareTag("Ledge")) return;
        if (isGrabbing || (movementControl && movementControl.IsLocked)) return;
        if (!other.TryGetComponent(out LedgePoint ledge)) return;

        StartCoroutine(ClimbRoutine(ledge));
    }

    private IEnumerator ClimbRoutine(LedgePoint ledge)
    {
        isGrabbing = true;

        float totalLock = hangTime + climbDuration + 0.1f;
        if (movementControl) movementControl.LockMovement(totalLock);

        anim.Climb(); // ✅ reemplazo del SetClimbing()

        Transform player = movement.transform;
        player.position = ledge.grabSpot.position;
        AlignFacingToLedge(ledge);

        if (animator && !string.IsNullOrEmpty(hangingBoolName))
            animator.SetBool(hangingBoolName, true);

        yield return new WaitForSeconds(hangTime);

        if (animator)
        {
            if (actionsLayerIndex >= 0) animator.SetLayerWeight(actionsLayerIndex, 1f);
            if (!string.IsNullOrEmpty(climbTriggerName)) animator.SetTrigger(climbTriggerName);
        }

        Vector3 start = player.position;
        Vector3 end = ledge.climbSpot.position;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.0001f, climbDuration);
            float k = climbEasing.Evaluate(Mathf.Clamp01(t));
            player.position = Vector3.LerpUnclamped(start, end, k);
            MaintainFacingDuringClimb(ledge, player);
            yield return null;
        }

        if (animator && !string.IsNullOrEmpty(hangingBoolName))
            animator.SetBool(hangingBoolName, false);
        if (animator && actionsLayerIndex >= 0)
            animator.SetLayerWeight(actionsLayerIndex, 0f);

        movement.IsOnAir = false;
        yield return new WaitForEndOfFrame();
        isGrabbing = false;
    }

    private void AlignFacingToLedge(LedgePoint ledge)
    {
        if (ledge.forwardOverride != Vector3.zero)
        {
            Vector3 fwd = ledge.forwardOverride.normalized;
            fwd.y = 0f;
        }
        if (ledge.alignToNormal)
        {
            Vector3 approxNormal = (ledge.grabSpot.position - ledge.climbSpot.position).normalized;
            approxNormal.y = 0f;
        }
    }

    private void MaintainFacingDuringClimb(LedgePoint ledge, Transform player)
    {
        AlignFacingToLedge(ledge);
    }
}
